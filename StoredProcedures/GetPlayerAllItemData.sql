USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerAllItemData]    Script Date: 11/4/2023 9:36:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[GetPlayerAllItemData]
	@PlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @currentId INT = NULL;
	DECLARE @tempDataCode VARCHAR(100) = NULL;
	DECLARE @dataTypeId INT = NULL;

	BEGIN TRY
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
	END TRY
	BEGIN CATCH
		SET @currentId = NULL;
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	IF (@currentId IS NOT NULL)
		BEGIN
			SELECT @dataTypeId = d.[DataTypeId] FROM [dbo].[DataType] AS d WHERE d.[Code] = 'Item';

			SET @case = 100;
			SELECT p.[PlayerDataId], i.[Id] AS 'ItemId', d.[Code] AS 'DataType', i.[ValueId] AS 'ValueId', i.[Value], p.[Value] AS 'Count' 
			FROM [dbo].[PlayerData] AS p 
			INNER JOIN [dbo].[Item] as i ON p.[ValueId] = i.[Id]
			INNER JOIN [dbo].[DataType] AS d ON d.[DataTypeId] = i.[DataTypeId]
			WHERE p.[PlayerId] = @currentId AND p.[DataTypeId] = @dataTypeId AND p.[Value] IS NOT NULL

			SET @message = 'Fetched player game data';
		END
	ELSE
		SELECT p.[PlayerDataId], p.[DataTypeId] AS 'DataType', p.[ValueId] AS 'ValueId', p.[Value], p.[Value] AS 'Count' FROM [dbo].[PlayerData] AS p 
		WHERE p.[PlayerId] = NULL;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
