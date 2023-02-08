USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpdatePlayerProperties]    Script Date: 2/8/2023 1:47:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[UpdatePlayerProperties]
	@PlayerId INT,
	@Name VARCHAR(1000)
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;

	DECLARE @validUserId INT = NULL;

	BEGIN TRY
		SELECT @validUserId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

		IF (@validUserId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
		ELSE
			BEGIN
				UPDATE [dbo].[Player] SET [Name] = @Name WHERE [PlayerId] = @userId;

				SET @case = 100;
				SET @message = 'Updated player name';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT p.[PlayerId], p.[PlayerIdentifier], p.[RavasAccountId], p.[Name], p.[AcceptedTermAndCondition], p.[IsAdmin], p.[IsDeveloper], p.[WorldId], p.[WorldTileId], 'Info' = NULL
	FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;


	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
