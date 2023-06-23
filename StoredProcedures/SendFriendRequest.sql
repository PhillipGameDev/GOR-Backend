USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[SendFriendRequest]    Script Date: 6/21/2023 6:24:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[SendFriendRequest]
	@FromPlayerId INT,
	@ToPlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	DECLARE @userId INT = NULL;
	DECLARE @toUserId INT = NULL;

	BEGIN TRY
		SELECT @userId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @FromPlayerId;
		SELECT @toUserId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @ToPlayerId;
	END TRY
	BEGIN CATCH
	END CATCH

	DECLARE @requestId BIGINT = NULL;
	IF ((@userId IS NULL) OR (@toUserId IS NULL))
		BEGIN
			SET @case = 200;
			SET @message = 'Account does not exist';
		END
	ELSE IF (@userId = @toUserId)
		BEGIN
			SET @case = 201;
			SET @message = 'Invalid ID';
		END
	ELSE IF (EXISTS(SELECT TOP 1 * FROM [dbo].[Friends] WHERE ([PlayerId] = @userId) AND ([FriendPlayerId] = @toUserId)))
		BEGIN
			SET @case = 202;
			SET @message = 'You are already friends';
		END
	ELSE
		BEGIN
			BEGIN TRY
				DECLARE @prevFlag TINYINT;
				SELECT @requestId = [RequestId], @prevFlag = [Flags] FROM [dbo].[FriendRequest] 
				WHERE ([FromPlayerId] = @userId) AND ([ToPlayerId] = @toUserId);

				SET @case = 100;
				SET @message = 'Friend request sent.';
				IF (@requestId IS NULL)
					BEGIN
						INSERT INTO [dbo].[FriendRequest] VALUES (@userId, @toUserId, 0, @time);
						SET @requestId = SCOPE_IDENTITY();
					END
				ELSE IF ((@prevFlag = 0) OR (@prevFlag = 2))
					BEGIN
						SET @case = 101;
						SET @message = 'Friend request already sent';
					END
				ELSE
					BEGIN
						UPDATE [dbo].[FriendRequest] SET [Flags] = 0 WHERE [RequestId] = @requestId;

						SELECT @requestId = [RequestId], @prevFlag = [Flags] FROM [dbo].[FriendRequest] 
						WHERE ([FromPlayerId] = @toUserId) AND ([ToPlayerId] = @userId);
						IF ((@requestId IS NOT NULL) AND (@prevFlag = 2))
							UPDATE [dbo].[FriendRequest] SET [Flags] = 4 WHERE [RequestId] = @requestId;
					END
			END TRY
			BEGIN CATCH
				IF ((ERROR_NUMBER() = 2601) OR (ERROR_NUMBER() = 2627))
					BEGIN
						SET @case = 101;
						SET @message = 'Friend request already sent.';
					END
				ELSE
					BEGIN
						SET @case = 300;
						SET @message = 'An error occurred.';
					END
			END CATCH
		END

	SELECT fr.[RequestId], CAST(NULL AS INT) AS 'FromPlayerId', CAST(NULL AS VARCHAR) AS 'FromPlayerName', fr.[ToPlayerId], p.[Name] AS 'ToPlayerName', fr.[Flags] FROM [dbo].[FriendRequest] AS fr
	INNER JOIN [dbo].[Player] AS p ON p.[PlayerId] = fr.[ToPlayerId] 
	WHERE fr.[RequestId] = @requestId;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END