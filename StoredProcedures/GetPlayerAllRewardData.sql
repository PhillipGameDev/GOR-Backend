USE [GameOfRevenge]
GO

/****** Object:  StoredProcedure [dbo].[GetPlayerAllRewardData]    Script Date: 2/15/2023 11:28:49 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[GetPlayerAllRewardData]
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
			SELECT @dataTypeId = d.[DataTypeId] FROM [dbo].[DataType] AS d WHERE d.[Code] = 'Reward';

			SET @case = 100;
			SELECT p.[PlayerDataId], d.[Code] AS 'DataType', q.[ReqValueId] AS 'ValueId', q.[Value], p.[Value] AS 'Count' FROM [dbo].[PlayerData] AS p 
			INNER JOIN [dbo].[QuestReward] AS q ON q.[QuestRewardId] = p.[ValueId]
			INNER JOIN [dbo].[DataType] AS d ON d.[DataTypeId] = q.[DataTypeId]
			WHERE p.[PlayerId] = @currentId AND p.[DataTypeId] = @dataTypeId AND p.[Value] IS NOT NULL

			SET @message = 'Fetched player game data';
		END
	ELSE
		SELECT p.[PlayerDataId], p.[DataTypeId] AS 'DataType', p.[ValueId] AS 'ValueId', p.[Value], p.[Value] AS 'Count' FROM [dbo].[PlayerData] AS p 
		WHERE p.[PlayerId] = NULL;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO


