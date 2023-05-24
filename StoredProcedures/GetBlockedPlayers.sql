USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetBlockedPlayers]    Script Date: 5/1/2023 8:28:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetBlockedPlayers]
	@PlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	DECLARE @userId INT = @PlayerId;
	DECLARE @currentId INT = NULL;

	SELECT @currentId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @userId;

	IF (@currentId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'Account does not exist';
		END
	ELSE
		BEGIN
			BEGIN TRY
				SELECT [BlockedPlayerId] FROM [dbo].[BlockedPlayers] WHERE [PlayerId] = @currentId;

				SET @case = 100;
				SET @message = 'Blocked players';
			END TRY
			BEGIN CATCH
				SET @case = 200;
				SET @message = 'Account does not exist';
			END CATCH
		END

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END