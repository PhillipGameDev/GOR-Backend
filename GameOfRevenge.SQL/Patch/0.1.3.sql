USE [GameOfRevenge]
GO

CREATE TABLE [dbo].[PlayerTutorial]
(
	PlayerTutorialId INT IDENTITY(1,1) NOT NULL,
	PlayerId INT NULL,
	PlayerIdentifier VARCHAR(MAX) NOT NULL,
	ProgressData VARCHAR(MAX) NULL,
	IsCompleted BIT NOT NULL,
	StartedOn DATETIME NOT NULL,
	CONSTRAINT [PK_PlayerTutorial_PlayerTutorialId] PRIMARY KEY CLUSTERED (PlayerTutorialId ASC)
)
GO

CREATE OR ALTER PROCEDURE [dbo].[GetPlayerTutorial]
	@PlayerIdentifier VARCHAR(MAX)
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @userId INT = NULL;
	DECLARE @tPlayerIdentifier  VARCHAR(MAX) = LTRIM(RTRIM(@PlayerIdentifier));
	
	SELECT @userId = [PlayerTutorialId] FROM [dbo].[PlayerTutorial] WHERE [PlayerIdentifier] = @tPlayerIdentifier;

	BEGIN TRY
		IF (@userId IS NULL)
			BEGIN
				INSERT INTO [dbo].[PlayerTutorial] VALUES (NULL, @tPlayerIdentifier, NULL, 0, @time);
				SELECT @userId = [PlayerTutorialId] FROM [dbo].[PlayerTutorial] WHERE [PlayerIdentifier] = @tPlayerIdentifier;
			END
		SET @case = 100;
		SET @message = 'Player tutorial data';
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT * FROM [dbo].[PlayerTutorial] WHERE [PlayerIdentifier] = @tPlayerIdentifier;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[UpdateTutorialInfo]
	@PlayerIdentifier VARCHAR(MAX),
	@ProgressData VARCHAR(MAX),
	@IsComplete BIT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @userId INT = NULL;

	DECLARE @tPlayerIdentifier  VARCHAR(MAX) = LTRIM(RTRIM(@PlayerIdentifier));
	DECLARE @tPlayerData VARCHAR(MAX) = LTRIM(RTRIM(@ProgressData));
	DECLARE @tIsComplete BIT = ISNULL(@IsComplete, 0);
	
	SELECT @userId = [PlayerTutorialId] FROM [dbo].[PlayerTutorial] WHERE [PlayerIdentifier] = @tPlayerIdentifier;

	BEGIN TRY
		IF (@userId IS NULL)
			BEGIN
				INSERT INTO [dbo].[PlayerTutorial] VALUES (NULL, @tPlayerIdentifier, @tPlayerData, @tIsComplete, @time);
				SELECT @userId = [PlayerTutorialId] FROM [dbo].[PlayerTutorial] WHERE [PlayerIdentifier] = @tPlayerIdentifier;
			END
		ELSE 
			BEGIN
				UPDATE [dbo].[PlayerTutorial] 
				SET [ProgressData] = @tPlayerData, [IsCompleted] = @tIsComplete
				WHERE [PlayerIdentifier] = @tPlayerIdentifier;
			END
		SET @case = 100;
		SET @message = 'Player tutorial data';
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT * FROM [dbo].[PlayerTutorial] WHERE [PlayerIdentifier] = @tPlayerIdentifier;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO
