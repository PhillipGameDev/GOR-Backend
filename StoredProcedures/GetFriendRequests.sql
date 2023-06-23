USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetFriendRequests]    Script Date: 6/5/2023 3:09:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetFriendRequests]
	@PlayerId INT,
	@Filter BIT = 0
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	DECLARE @userId INT = @PlayerId;
	DECLARE @currentId INT = NULL;
	DECLARE @tFilter BIT = ISNULL(@Filter, 0);

	SELECT @currentId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @userId;

	IF (@currentId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'Account does not exist';
		END
	ELSE
		BEGIN
			BEGIN TRY
				/* flags 1 = accepted, 2 = rejected */
				IF (@tFilter = 0)
					SELECT fr.[RequestId], fr.[FromPlayerId], p.[Name] AS 'FromPlayerName', 0, CAST(NULL AS NVARCHAR), fr.[Flags] FROM [dbo].[FriendRequest] AS fr
					INNER JOIN [dbo].[Player] AS p ON p.[PlayerId] = fr.[FromPlayerId] 
					WHERE fr.[ToPlayerId] = @currentId AND fr.[Flags] = 0;
				ELSE IF (@tFilter = 1)
 					SELECT fr.[RequestId], 0, CAST(NULL AS NVARCHAR), fr.[ToPlayerId], p.[Name] AS 'ToPlayerName', fr.[Flags] FROM [dbo].[FriendRequest] AS fr
					INNER JOIN [dbo].[Player] AS p ON p.[PlayerId] = fr.[ToPlayerId] 
 					WHERE fr.[FromPlayerId] = @currentId;
 				ELSE
 					SELECT fr.[RequestId], fr.[FromPlayerId], p1.[Name] AS 'FromPlayerName', fr.[ToPlayerId], p2.[Name] AS 'ToPlayerName', fr.[Flags] FROM [dbo].[FriendRequest] AS fr
					INNER JOIN [dbo].[Player] AS p1 ON p1.[PlayerId] = fr.[FromPlayerId]
						JOIN [dbo].[Player] AS p2 ON p2.[PlayerId] = fr.[ToPlayerId]
 					WHERE (fr.[FromPlayerId] = @currentId) OR (fr.[ToPlayerId] = @currentId);

				SET @case = 100;
				SET @message = 'Friend requests';
			END TRY
			BEGIN CATCH
				SET @case = 200;
				SET @message = 'Account does not exist';
			END CATCH
		END

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END