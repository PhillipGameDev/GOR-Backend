USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerDataById]    Script Date: 8/14/2022 3:25:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetPlayerDataById]
	@PlayerDataId BIGINT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;
	DECLARE @tempDataId BIGINT = @PlayerDataId;
	DECLARE @currentDataId BIGINT = NULL;

	BEGIN TRY
		SELECT @currentDataId = p.[PlayerDataId] FROM [dbo].[PlayerData] AS p 
		WHERE p.[PlayerDataId] = @tempDataId;

		IF (@currentDataId IS NULL)
			BEGIN
				SET @case = 202;
				SET @message = 'Player game data does not exists';
			END
		ELSE
			BEGIN
				SET @case = 100;
				SET @message = 'Player game data fetched succesfully';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT p.[PlayerDataId], d.[Code] AS 'DataType', p.[ValueId], p.[Value] FROM [dbo].[PlayerData] AS p 
	INNER JOIN [dbo].[DataType] AS d ON d.[DataTypeId] = p.[DataTypeId]
	WHERE p.[PlayerDataId] = @currentDataId AND p.[Value] IS NOT NULL;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
