USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpdateTutorialInfo]    Script Date: 3/6/2023 5:34:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[UpdateTutorialInfo]
	@PlayerId VARCHAR(MAX),
	@ProgressData VARCHAR(MAX),
	@IsComplete BIT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @playerTutorialId INT = NULL;
	DECLARE @existingId INT = NULL;

	DECLARE @tPlayerId  VARCHAR(MAX) = LTRIM(RTRIM(@PlayerId));
	DECLARE @tPlayerData VARCHAR(MAX) = LTRIM(RTRIM(@ProgressData));
	DECLARE @tIsComplete BIT = ISNULL(@IsComplete, 0);
	
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
						INSERT INTO [dbo].[PlayerTutorial] VALUES (@tPlayerId, @tPlayerId, @tPlayerData, @tIsComplete, @time);
/*						SELECT @playerTutorialId = [PlayerTutorialId] FROM [dbo].[PlayerTutorial] WHERE [PlayerId] = @tPlayerId;*/
					END
				ELSE 
					BEGIN
						UPDATE [dbo].[PlayerTutorial] 
						SET [ProgressData] = @tPlayerData, [IsCompleted] = @tIsComplete
						WHERE [PlayerTutorialId] = @playerTutorialId;
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