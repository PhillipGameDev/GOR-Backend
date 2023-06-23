USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[RespondToFriendRequest]    Script Date: 6/5/2023 6:17:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[RespondToFriendRequest]
	@FromPlayerId INT,
	@ToPlayerId INT,
	@Value TINYINT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	DECLARE @userId INT = NULL;
	DECLARE @toUserId INT = NULL;
	DECLARE @tValue TINYINT = ISNULL(@Value, 0);

	BEGIN TRY
		SELECT @userId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @FromPlayerId;
		SELECT @toUserId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @ToPlayerId;
	END TRY
	BEGIN CATCH
	END CATCH

	IF ((@userId IS NULL) OR (@toUserId IS NULL))
		BEGIN
			SET @case = 200;
			SET @message = 'Account does not exist';
		END
	ELSE IF ((@tValue <> 1) AND (@tValue <> 2))
		BEGIN
			SET @case = 201;
			SET @message = 'Invalid value';
		END
	ELSE
		BEGIN
			BEGIN TRY
				UPDATE [dbo].[FriendRequest] SET [Flags] = @tValue WHERE [FromPlayerId] = @userId AND [ToPlayerId] = @toUserId;

				BEGIN TRY
					UPDATE [dbo].[FriendRequest] SET [Flags] = 4 WHERE [FromPlayerId] = @toUserId AND [ToPlayerId] = @userId;
				END TRY
				BEGIN CATCH
				END CATCH

				IF (@tValue = 1)
				BEGIN
					BEGIN TRY
						INSERT INTO [dbo].[Friends] VALUES (@userId, @toUserId, @time);
					END TRY
					BEGIN CATCH
					END CATCH
					BEGIN TRY
						INSERT INTO [dbo].[Friends] VALUES (@toUserId, @userId, @time);
					END TRY
					BEGIN CATCH
					END CATCH
				END

				SET @case = 100;
				SET @message = 'Friend request responded';
			END TRY
			BEGIN CATCH
				BEGIN
					SET @case = 300;
					SET @message = 'An error occurred'
				END
			END CATCH
		END

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END