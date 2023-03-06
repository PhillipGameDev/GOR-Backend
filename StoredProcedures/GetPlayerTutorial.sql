USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerTutorial]    Script Date: 3/6/2023 6:25:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetPlayerTutorial]
	@PlayerId VARCHAR(MAX)
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @playerTutorialId INT = NULL;
	DECLARE @tPlayerId  VARCHAR(MAX) = LTRIM(RTRIM(@PlayerId));
	DECLARE @existingId INT = NULL;

	SELECT @existingId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @tPlayerId;
	
	IF (@existingId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'No existing account found';
		END
	ELSE
		BEGIN

		SELECT @playerTutorialId = [PlayerTutorialId] FROM [dbo].[PlayerTutorial] WHERE [PlayerId] = @tPlayerId;

		BEGIN TRY
			IF (@playerTutorialId IS NULL)
				BEGIN
					INSERT INTO [dbo].[PlayerTutorial] VALUES (@tPlayerId, @tPlayerId, NULL, 0, @time);
/*					SELECT @playerTutorialId = [PlayerTutorialId] FROM [dbo].[PlayerTutorial] WHERE [PlayerId] = @tPlayerId;*/
				END
			SET @case = 100;
			SET @message = 'Player tutorial data';
		END TRY
		BEGIN CATCH
			SET @case = 0;
			SET @error = 1;
			SET @message = ERROR_MESSAGE();
		END CATCH
	END

	SELECT * FROM [dbo].[PlayerTutorial] WHERE [PlayerId] = @tPlayerId;

	EXEC [dbo].[GetMessage] @tPlayerId, @message, @case, @error, @time, 1, 1;
END