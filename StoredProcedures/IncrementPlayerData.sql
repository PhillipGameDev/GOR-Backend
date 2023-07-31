USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[IncrementPlayerData]    Script Date: 8/14/2022 3:51:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[IncrementPlayerData]
	@PlayerId INT,
	@PlayerDataId BIGINT = NULL,
	@DataCode VARCHAR(100) = NULL,
	@ValueId INT = NULL,
	@Value INT = NULL,
	@Log BIT = 1
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;

	DECLARE @validUserId INT = NULL;
	DECLARE @validId BIGINT = NULL;
	DECLARE @validTypeId INT = NULL;

	DECLARE @tempDataCode VARCHAR(100) = NULL;
	DECLARE @tempValueId INT = @ValueId;
	DECLARE @tempValue INT = ISNULL(@Value, 0);
	DECLARE @oldValue VARCHAR(MAX) = NULL;
	IF (@DataCode IS NOT NULL) SET @tempDataCode = LTRIM(RTRIM(@DataCode));

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
				IF (@PlayerDataId IS NULL)
					IF (@tempValueId IS NULL)
						BEGIN
							SET @validUserId = NULL;
							SET @case = 202;
							SET @message = 'Invalid value id';
						END
					ELSE
						BEGIN
							SELECT @validTypeId = d.[DataTypeId] FROM [dbo].[DataType] AS d 
							WHERE d.[Code] = @tempDataCode;
					
							IF (@validTypeId IS NULL)
								BEGIN
									SET @validUserId = NULL;
									SET @case = 201;
									SET @message = 'Invalid data type code';
								END
							ELSE
								SELECT TOP 1 @validId = d.[PlayerDataId], @oldValue = d.[Value] FROM [dbo].[PlayerData] AS d 
								WHERE d.[PlayerId] = @validUserId AND d.[DataTypeId] = @validTypeId AND d.[ValueId] = @tempValueId;-- AND d.[Value] IS NOT NULL;
						END
				ELSE
					SELECT @validId = d.[PlayerDataId], @oldValue = d.[Value] FROM [dbo].[PlayerData] AS d 
					WHERE d.[PlayerId] = @validUserId AND d.[PlayerDataId] = @PlayerDataId;-- AND d.[Value] IS NOT NULL;

				IF (@validUserId IS NOT NULL)
					BEGIN
						DECLARE @finalValue BIGINT = CONVERT(BIGINT, ISNULL(@oldValue, '0')) + @tempValue;
						IF ((@tempValue <> 0) AND (@finalValue < 0)) SET @finalValue = 0;

						IF (@validId IS NULL)
							BEGIN
								DECLARE @tempTable table (PlayerDataId BIGINT);

								INSERT INTO [dbo].[PlayerData]
								OUTPUT INSERTED.PlayerDataId INTO @tempTable
								VALUES (@validUserId, @validTypeId, @tempValueId, CAST(@finalValue AS VARCHAR(MAX)));

								SELECT @validId = [PlayerDataId] FROM @tempTable;
							END
						ELSE IF (@tempValue <> 0)
							UPDATE [dbo].[PlayerData] SET [Value] = CAST(@finalValue AS VARCHAR(MAX))
							WHERE [PlayerDataId] = @validId;

						SET @case = 101;
						SET @message = 'Updated player game data';
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
		BEGIN
--			SELECT p.[PlayerDataId], d.[Code] AS 'DataType', p.[ValueId], p.[Value] FROM [dbo].[PlayerData] AS p 
--			INNER JOIN [dbo].[DataType] AS d ON d.[DataTypeId] = p.[DataTypeId]
--			WHERE p.[PlayerDataId] = @validId;

			DECLARE @outputTable table (PlayerDataId BIGINT, DataTypeId INT, 
									ValueId INT, Value VARCHAR(MAX), 
									OldValue VARCHAR(MAX));

			INSERT INTO @outputTable
			SELECT p.[PlayerDataId], p.[DataTypeId], p.[ValueId], p.[Value], @oldValue as 'OldValue' FROM [dbo].[PlayerData] AS p 
			WHERE p.[PlayerDataId] = @validId;

			SELECT p.[PlayerDataId], d.[Code] AS 'DataType', p.[ValueId], p.[Value], p.[OldValue] FROM @outputTable AS p 
			INNER JOIN [dbo].[DataType] AS d ON d.[DataTypeId] = p.[DataTypeId];
		END
	ELSE
		SELECT p.[PlayerDataId], d.[Code] AS 'DataType', p.[ValueId], p.[Value], p.[Value] FROM [dbo].[PlayerData] AS p 
		INNER JOIN [dbo].[DataType] AS d ON d.[DataTypeId] = p.[DataTypeId];

	IF (@Log = 1) EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
