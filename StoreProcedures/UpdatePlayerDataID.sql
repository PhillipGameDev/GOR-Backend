USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpdatePlayerDataID]    Script Date: 8/14/2022 8:05:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[UpdatePlayerDataID]
	@PlayerId INT,
	@PlayerDataId BIGINT,
	@Value VARCHAR(MAX)
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;

	DECLARE @validUserId INT = NULL;
	DECLARE @validId BIGINT = NULL;

	DECLARE @tempId BIGINT = @PlayerDataId;
	DECLARE @tempValue VARCHAR(MAX) = NULL;
	DECLARE @oldValue VARCHAR(MAX) = NULL;
	IF (@Value IS NOT NULL) SET @tempValue = LTRIM(RTRIM(@Value));

	BEGIN TRY
		SELECT @validUserId = p.[PlayerId] FROM [dbo].[Player] AS p 
		WHERE p.[PlayerId] = @userId;

		IF (@validUserId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
		ELSE IF (@tempId IS NULL)
			BEGIN
				SET @case = 201;
				SET @message = 'Missing record id';
			END
		ELSE
			BEGIN
				SELECT @validId = d.[PlayerDataId], @oldValue = d.[Value] FROM [dbo].[PlayerData] AS d 
				WHERE d.[PlayerDataId] = @tempId AND d.[PlayerId] = @validUserId;

				IF (@validId IS NULL)
					BEGIN
						SET @case = 202;
						SET @message = 'Invalid record id';
					END
				ELSE
					BEGIN
						UPDATE [dbo].[PlayerData] SET [Value] = @tempValue
						WHERE [PlayerDataId] = @validId;

						SET @case = 101;
						SET @message = 'Updated player game data';
					END
			END
	END TRY
	BEGIN CATCH
		SET @validId = NULL;
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	IF (@validId IS NOT NULL)
		IF (@oldValue IS NULL)
			SELECT p.[PlayerDataId], d.[Code] AS 'DataType', p.[ValueId], p.[Value] FROM [dbo].[PlayerData] AS p 
			INNER JOIN [dbo].[DataType] AS d ON d.[DataTypeId] = p.[DataTypeId]
			WHERE p.[PlayerDataId] = @validId
		ELSE
			BEGIN
				DECLARE @outputTable table (PlayerDataId BIGINT, DataTypeId INT, 
										ValueId INT, Value VARCHAR(MAX), 
										OldValue VARCHAR(MAX));

				INSERT INTO @outputTable
				SELECT p.[PlayerDataId], p.[DataTypeId], p.[ValueId], p.[Value], @oldValue as 'OldValue' FROM [dbo].[PlayerData] AS p 
				WHERE p.[PlayerDataId] = @validId;

				SELECT p.[PlayerDataId], d.[Code] AS 'DataType', p.[ValueId], p.[Value], p.[OldValue] FROM @outputTable AS p 
				INNER JOIN [dbo].[DataType] AS d ON d.[DataTypeId] = p.[DataTypeId];
			END

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
