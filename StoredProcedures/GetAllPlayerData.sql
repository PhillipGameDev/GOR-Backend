USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllPlayerData]    Script Date: 12/17/2022 7:03:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetAllPlayerData]
	@PlayerId INT,
	@DataCode VARCHAR(100) = NULL,
	@ValueId INT = NULL
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
	DECLARE @tempValueId INT = ISNULL(@ValueId, 0);
	IF (@DataCode IS NOT NULL) SET @tempDataCode = LTRIM(RTRIM(@DataCode));

	BEGIN TRY
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p 
		WHERE p.[PlayerId] = @userId;

		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
		ELSE IF (@DataCode IS NOT NULL)
			BEGIN
				SELECT @dataTypeId = d.[DataTypeId] FROM [dbo].[DataType] AS d 
				WHERE d.[Code] = @tempDataCode;

				IF (@dataTypeId IS NULL)
					BEGIN
						SET @currentId = NULL;
						SET @case = 201;
						SET @message = 'Invalid data type code';
					END
				ELSE IF (@ValueId IS NOT NULL AND (@tempValueId <= 0))
					BEGIN
						SET @currentId = NULL;
						SET @case = 202;
						SET @message = 'Invalid value id';
					END
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
			SET @case = 100;
			IF (@dataTypeId IS NULL)
				BEGIN
					SELECT p.[PlayerDataId], d.[Code] AS 'DataType', p.[ValueId], p.[Value] FROM [dbo].[PlayerData] AS p 
					INNER JOIN [dbo].[DataType] AS d ON d.[DataTypeId] = p.[DataTypeId]
					WHERE p.[PlayerId] = @currentId AND p.[Value] IS NOT NULL;

					SET @message = 'Fetched all player game data';
				END
			ELSE
				BEGIN
					IF (@ValueId IS NOT NULL)
						SELECT p.[PlayerDataId], d.[Code] AS 'DataType', p.[ValueId], p.[Value] FROM [dbo].[PlayerData] AS p 
						INNER JOIN [dbo].[DataType] AS d ON d.[DataTypeId] = p.[DataTypeId]
						WHERE p.[PlayerId] = @currentId AND p.[DataTypeId] = @dataTypeId AND p.[ValueId] = @tempValueId AND p.[Value] IS NOT NULL
					ELSE
						SELECT p.[PlayerDataId], d.[Code] AS 'DataType', p.[ValueId], p.[Value] FROM [dbo].[PlayerData] AS p 
						INNER JOIN [dbo].[DataType] AS d ON d.[DataTypeId] = p.[DataTypeId]
						WHERE p.[PlayerId] = @currentId AND p.[DataTypeId] = @dataTypeId AND p.[Value] IS NOT NULL;

					SET @message = 'Fetched player game data';
				END
		END
	ELSE
		SELECT p.[PlayerDataId], p.[DataTypeId] AS 'DataType', p.[ValueId], p.[Value] FROM [dbo].[PlayerData] AS p 
		WHERE p.[PlayerId] = NULL;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
