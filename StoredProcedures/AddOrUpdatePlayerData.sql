USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddOrUpdatePlayerData]    Script Date: 3/6/2023 2:53:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[AddOrUpdatePlayerData]
	@PlayerId INT,
	@DataCode VARCHAR(100),
	@ValueId INT,
	@Value VARCHAR(MAX) = NULL,
	@Unique BIT = 0
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;

	DECLARE @validUserId INT = NULL;
	DECLARE @validTypeId INT = NULL
	DECLARE @validId BIGINT = NULL;

	DECLARE @tempValueId INT = @ValueId;
	DECLARE @tempDataCode VARCHAR(100) = NULL;
	DECLARE @tempValue VARCHAR(MAX) = NULL;
	DECLARE @tempUnique BIT = @Unique;
	DECLARE @oldValue VARCHAR(MAX) = NULL;
	IF (@DataCode IS NOT NULL) SET @tempDataCode = NULLIF(LTRIM(RTRIM(@DataCode)), '');
	IF (@Value IS NOT NULL) SET @tempValue = LTRIM(RTRIM(@Value));

	BEGIN TRY
		SELECT @validUserId = p.[PlayerId] FROM [dbo].[Player] AS p
		WHERE p.[PlayerId] = @userId;

		IF (@validUserId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
		ELSE 
			BEGIN
				IF (@tempDataCode IS NOT NULL)
					SELECT @validTypeId = d.[DataTypeId] FROM [dbo].[DataType] AS d 
					WHERE d.[Code] = @tempDataCode;
				
				IF (@validTypeId IS NULL)
					BEGIN
						SET @validUserId = NULL;
						SET @case = 201;
						SET @message = 'Invalid data type code';
					END
				ELSE
					BEGIN
						IF (@tempUnique = 1)
							SELECT TOP 1 @validId = d.[PlayerDataId], @oldValue = d.[Value] FROM [dbo].[PlayerData] AS d 
							WHERE d.[PlayerId] = @validUserId AND d.[DataTypeId] = @validTypeId AND d.[ValueId] = @tempValueId;
						
						IF (@validId IS NOT NULL)
							BEGIN
								UPDATE [dbo].[PlayerData] SET [Value] = @tempValue
								WHERE [PlayerDataId] = @validId;

								SET @case = 101;
								SET @message = 'Updated player game data';
							END
						ELSE IF (@tempValue IS NULL)
							BEGIN
								SET @validUserId = NULL;
								SET @case = 202;
								SET @message = 'Cannot create a new record for a null value';
							END
						ELSE
							BEGIN
								DECLARE @tempTable table (PlayerDataId BIGINT);

								INSERT INTO [dbo].[PlayerData]
								OUTPUT INSERTED.PlayerDataId INTO @tempTable
								VALUES (@validUserId, @validTypeId, @tempValueId, @tempValue);

								SELECT @validId = [PlayerDataId] FROM @tempTable;

								SET @case = 100;
								SET @message = 'Added player game data';
							END
					END
			END
	END TRY
	BEGIN CATCH
		SET @validUserId = NULL;
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	IF (@validUserId IS NOT NULL)
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