USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerData]    Script Date: 10/3/2022 1:35:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetPlayerData]
	@PlayerId INT,
	@DataCode VARCHAR(100),
	@ValueId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @currentId INT = NULL;
	DECLARE @tempValueId INT = @ValueId;
	DECLARE @currentDataId BIGINT = NULL;
	DECLARE @tempDataCode VARCHAR(100) = LTRIM(RTRIM(@DataCode));

	BEGIN TRY
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;
		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
		ELSE 
			BEGIN
				DECLARE @dataTypeId INT = NULL
				SELECT @dataTypeId = d.[DataTypeId] FROM [dbo].[DataType] AS d WHERE d.[Code] = @tempDataCode;
				
				IF (@dataTypeId IS NULL)
					BEGIN
						SET @currentId = NULL;
						SET @case = 201;
						SET @message = 'Invaid data type code';
					END
				ELSE
					BEGIN
						SELECT @currentDataId = d.[PlayerDataId] FROM [dbo].[PlayerData] AS d 
						WHERE d.[PlayerId] = @currentId AND d.[DataTypeId] = @dataTypeId AND d.[ValueId] = @tempValueId AND d.[Value] IS NOT NULL;

						IF (@currentDataId IS NULL)
							BEGIN
								SET @case = 101;
								SET @message = 'Player game data does not exists';
							END
						ELSE
							BEGIN
								SET @case = 100;
								SET @message = 'Player game data fetched succesfully';
							END
					END
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	IF @currentDataId IS NOT NULL
		BEGIN
			SELECT p.[PlayerDataId], d.[Code] AS 'DataType', p.[ValueId], p.[Value] FROM [dbo].[PlayerData] AS p 
			INNER JOIN [dbo].[DataType] AS d ON d.[DataTypeId] = p.[DataTypeId]
			WHERE p.[PlayerDataId] = @currentDataId;
		END

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
