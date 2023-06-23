USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetMarchingTroops]    Script Date: 6/16/2023 1:31:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetMarchingTroops]
	@PlayerId INT = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @userId INT = @PlayerId;
	DECLARE @currentId INT = NULL;

	BEGIN TRY
		IF (@userId IS NULL)
			BEGIN
				SELECT CAST([PlayerId] AS BIGINT) AS 'PlayerId', 'Marching' AS 'DataType', [ValueId], [Value] FROM [dbo].[PlayerData] 
				WHERE [DataTypeId] = 4 AND [Value] IS NOT NULL AND [Value] <> '';

				SET @case = 100;
				SET @message = 'Player game data fetched succesfully';
			END
		ELSE
			BEGIN
				SELECT @currentId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @userId;
				IF (@currentId IS NULL)
					BEGIN
						SELECT CAST(0 AS BIGINT) AS 'PlayerDataId', CAST(NULL AS VARCHAR) AS 'DataType', 1 AS 'ValueId', CAST(NULL AS VARCHAR) AS 'Value';

						SET @case = 200;
						SET @message = 'Player does not exists';
					END
				ELSE 
					BEGIN
						SELECT [PlayerDataId], 'Marching' AS 'DataType', [ValueId], [Value] FROM [dbo].[PlayerData]
						WHERE [PlayerId] = @currentId AND [DataTypeId] = 4 AND [Value] IS NOT NULL AND [Value] <> '';

						SET @case = 100;
						SET @message = 'Player game data fetched succesfully';
					END
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END