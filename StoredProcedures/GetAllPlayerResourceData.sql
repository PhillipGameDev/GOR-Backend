USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllPlayerResourceData]    Script Date: 8/14/2022 2:51:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetAllPlayerResourceData]
	@PlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @currentId INT = NULL;
	BEGIN TRY
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p 
		WHERE p.[PlayerId] = @userId;
		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
		ELSE 
			BEGIN
				SET @case = 100;
				SET @message = 'Fetched all player game data';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH
	
	SELECT p.[PlayerDataId], d.[Code] AS 'DataType', p.[ValueId], p.[Value] FROM [dbo].[PlayerData] AS p 
	INNER JOIN [dbo].[DataType] AS d ON d.[DataTypeId] = p.[DataTypeId]
	WHERE p.[PlayerId] = @currentId AND d.[Code] = 'Resource' AND p.[Value] IS NOT NULL

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
