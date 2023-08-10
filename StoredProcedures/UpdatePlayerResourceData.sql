USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpdatePlayerResourceData]    Script Date: 8/14/2022 3:31:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[UpdatePlayerResourceData]
	@PlayerId INT,
	@Food INT = NULL,
	@Wood INT = NULL,
	@Ore INT = NULL,
	@Gem INT = NULL,
	@Gold INT = NULL,
	@ShowResult BIT = 1
AS
BEGIN
	DECLARE @tsh INT = ISNULL(@ShowResult, 0);
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @currentId INT = NULL;

	DECLARE @tempFood INT = @Food;
	DECLARE @tempWood INT = @Wood;
	DECLARE @tempOre INT = @Ore;
	DECLARE @tempGem INT = @Gem;
	DECLARE @tempGold INT = @Gold;

	DECLARE @goldPlayerId INT = NULL, @goldPlayerValue VARCHAR(MAX) = CONVERT(VARCHAR(MAX), @tempGold);
	DECLARE @gemPlayerId INT = NULL, @gemPlayerValue VARCHAR(MAX) = CONVERT(VARCHAR(MAX), @tempGem);
	DECLARE @orePlayerId INT = NULL, @orePlayerValue VARCHAR(MAX) = CONVERT(VARCHAR(MAX), @tempOre);
	DECLARE @woodPlayerId INT = NULL, @woodPlayerValue VARCHAR(MAX) = CONVERT(VARCHAR(MAX), @tempWood);
	DECLARE @foodPlayerId INT = NULL, @foodPlayerValue VARCHAR(MAX) = CONVERT(VARCHAR(MAX), @tempFood);

	DECLARE @tempPlayerAllDataTable TABLE
	(
		PlayerDataId BIGINT NOT NULL,
		PlayerId INT NOT NULL,
		DataTypeId INT NOT NULL,
		ValueId INT NOT NULL,
		Value VARCHAR(MAX) NOT NULL
	);

	DECLARE @foodId INT, @woodId INT, @oreId INT, @gemId INT, @goldId INT, @resDataTypeId INT;

	BEGIN TRY
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;
		IF (@tempFood IS NULL AND @tempWood IS NULL AND @tempOre IS NULL AND @tempGem IS NULL AND @tempGold IS NULL)
			BEGIN
				SET @case = 101;
				SET @message = 'No updates was needed';
			END
		ELSE IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
		ELSE 
			BEGIN
				INSERT INTO @tempPlayerAllDataTable
				SELECT p.[PlayerDataId], p.[PlayerId], p.[DataTypeId], p.[ValueId], p.[Value] FROM [dbo].[PlayerData] AS p 
				
				SELECT @resDataTypeId = [DataTypeId] FROM [dbo].[DataType] WHERE [Code] = 'Resource';
				SELECT @foodId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Food';
				SELECT @woodId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Wood';
				SELECT @oreId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Ore';
				SELECT @gemId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Gems';
				SELECT @goldId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Gold';

				DECLARE @foodUpdate BIT = 0, @woodUpdate BIT = 0, @oreUpdate BIT = 0, @gemUpdate BIT = 0, @goldUpdate BIT = 0;
				IF(@tempFood IS NOT NULL AND @tempFood >= 0) SET @foodUpdate = 1;
				IF(@tempWood IS NOT NULL AND @tempWood >= 0) SET @woodUpdate = 1;
				IF(@tempOre IS NOT NULL AND @tempOre >= 0) SET @oreUpdate = 1;
				IF(@tempGem IS NOT NULL AND @tempGem >= 0) SET @gemUpdate = 1;
				IF(@tempGold IS NOT NULL AND @tempGold >= 0) SET @goldUpdate = 1;

				IF (@foodUpdate = 1)
					BEGIN
						SELECT @foodPlayerId = p.[PlayerDataId] FROM @tempPlayerAllDataTable AS p 
						WHERE p.[PlayerId] = @currentId AND p.[DataTypeId] = @resDataTypeId AND p.[ValueId] = @foodId
						
						IF (@foodPlayerId IS NULL) 
							INSERT INTO [dbo].[PlayerData] VALUES (@currentId, @resDataTypeId, @foodPlayerId, @foodPlayerValue)
						ELSE 
							UPDATE [dbo].[PlayerData] SET [Value] = @foodPlayerValue WHERE [PlayerDataId] = @foodPlayerId
					END
				IF (@woodUpdate = 1)
					BEGIN
						SELECT @woodPlayerId = p.[PlayerDataId] FROM @tempPlayerAllDataTable AS p 
						WHERE p.[PlayerId] = @currentId AND p.[DataTypeId] = @resDataTypeId AND p.[ValueId] = @woodId
						
						IF (@woodPlayerId IS NULL) 
							INSERT INTO [dbo].[PlayerData] VALUES (@currentId, @resDataTypeId, @woodPlayerId, @woodPlayerValue)
						ELSE 
							UPDATE [dbo].[PlayerData] SET [Value] = @woodPlayerValue WHERE [PlayerDataId] = @woodPlayerId
					END
				IF (@oreUpdate = 1)
					BEGIN
						SELECT @orePlayerId = p.[PlayerDataId] FROM @tempPlayerAllDataTable AS p 
						WHERE p.[PlayerId] = @currentId AND p.[DataTypeId] = @resDataTypeId AND p.[ValueId] = @oreId

						IF (@orePlayerId IS NULL) 
							INSERT INTO [dbo].[PlayerData] VALUES (@currentId, @resDataTypeId, @orePlayerId, @orePlayerValue)
						ELSE 
							UPDATE [dbo].[PlayerData] SET [Value] = @orePlayerValue WHERE [PlayerDataId] = @orePlayerId
					END
				IF (@gemUpdate = 1)
					BEGIN
						SELECT @gemPlayerId = p.[PlayerDataId] FROM @tempPlayerAllDataTable AS p 
						WHERE p.[PlayerId] = @currentId AND p.[DataTypeId] = @resDataTypeId AND p.[ValueId] = @gemId

						IF (@gemPlayerId IS NULL)
							INSERT INTO [dbo].[PlayerData] VALUES (@currentId, @resDataTypeId, @gemPlayerId, @gemPlayerValue)
						ELSE
							UPDATE [dbo].[PlayerData] SET [Value] = @gemPlayerValue WHERE [PlayerDataId] = @gemPlayerId
					END
				IF (@goldUpdate = 1)
					BEGIN
						SELECT @goldPlayerId = p.[PlayerDataId] FROM @tempPlayerAllDataTable AS p 
						WHERE p.[PlayerId] = @currentId AND p.[DataTypeId] = @resDataTypeId AND p.[ValueId] = @goldId

						IF (@goldPlayerId IS NULL)
							INSERT INTO [dbo].[PlayerData] VALUES (@currentId, @resDataTypeId, @goldPlayerId, @goldPlayerValue)
						ELSE
							UPDATE [dbo].[PlayerData] SET [Value] = @goldPlayerValue WHERE [PlayerDataId] = @goldPlayerId
					END
				SET @case = 100;
				SET @message = 'Fetched all player game data';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	IF (@tsh = 1)
		SELECT p.[PlayerDataId], d.[Code] AS 'DataType', p.[ValueId], p.[Value] FROM [dbo].[PlayerData] AS p 
		INNER JOIN [dbo].[DataType] AS d ON d.[DataTypeId] = p.[DataTypeId]
		WHERE p.[PlayerId] = @currentId AND p.[DataTypeId] = @resDataTypeId AND p.[Value] IS NOT NULL;
	
	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, @tsh, 1;
END
