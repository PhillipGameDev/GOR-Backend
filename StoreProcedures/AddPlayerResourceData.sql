USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddPlayerResourceData]    Script Date: 8/14/2022 4:23:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



ALTER   PROCEDURE [dbo].[AddPlayerResourceData]
	@PlayerId INT,
	@Food INT = NULL,
	@Wood INT = NULL,
	@Ore INT = NULL,
	@Gem INT = NULL,
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

	DECLARE @tempFood INT = ISNULL(@Food, 0);
	DECLARE @tempWood INT = ISNULL(@Wood, 0);
	DECLARE @tempOre INT = ISNULL(@Ore, 0);
	DECLARE @tempGem INT = ISNULL(@Gem, 0);

	DECLARE @foodId INT, @woodId INT, @oreId INT, @gemId INT, @resDataTypeId INT;
	DECLARE @foodPlayerId INT = NULL, @foodPlayerValue VARCHAR(MAX) = '0';
	DECLARE @woodPlayerId INT = NULL, @woodPlayerValue VARCHAR(MAX) = '0';
	DECLARE @orePlayerId INT = NULL, @orePlayerValue VARCHAR(MAX) = '0';
	DECLARE @gemPlayerId INT = NULL, @gemPlayerValue VARCHAR(MAX) = '0';

	DECLARE @tempPlayerAllDataTable TABLE
	(
		PlayerDataId BIGINT NOT NULL,
		PlayerId INT NOT NULL,
		DataTypeId INT NOT NULL,
		ValueId INT NOT NULL,
		Value VARCHAR(MAX) NOT NULL
	);

	BEGIN TRY
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;
		IF (@tempFood = 0 AND @tempWood = 0 AND @tempOre = 0 AND @tempGem = 0)
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
				SELECT p.[PlayerDataId], p.[PlayerId], p.[DataTypeId], p.[ValueId], p.[Value] FROM [dbo].[PlayerData] AS p;
				

				SELECT @resDataTypeId = [DataTypeId] FROM [dbo].[DataType] WHERE [Code] = 'Resource';
				SELECT @foodId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Food';
				SELECT @woodId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Wood';
				SELECT @oreId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Ore';
				SELECT @gemId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Gems';

				DECLARE @foodUpdate BIT = 0, @woodUpdate BIT = 0, @oreUpdate BIT = 0, @gemUpdate BIT = 0;
				IF(@tempFood > 0) SET @foodUpdate = 1;
				IF(@tempWood > 0) SET @woodUpdate = 1;
				IF(@tempOre > 0) SET @oreUpdate = 1;
				IF(@tempGem > 0) SET @gemUpdate = 1;

				IF (@foodUpdate = 1)
					BEGIN
						SELECT @foodPlayerId = p.[PlayerDataId], @foodPlayerValue = p.[Value]
						FROM @tempPlayerAllDataTable AS p 
						WHERE p.[PlayerId] = @currentId AND p.[DataTypeId] = @resDataTypeId AND p.[ValueId] = @foodId

						IF (@foodPlayerValue IS NULL OR LTRIM(RTRIM(@foodPlayerValue)) = '') SET @foodPlayerValue = '0'
						DECLARE @valueINTFood INT = CONVERT(INT, @foodPlayerValue);
						SET @valueINTFood = @valueINTFood + @tempFood;
						SET @foodPlayerValue = CONVERT(VARCHAR(MAX), @valueINTFood);
						
						IF (@foodPlayerId IS NULL)
							INSERT INTO [dbo].[PlayerData] VALUES (@currentId, @resDataTypeId, @foodPlayerId, @foodPlayerValue)
						ELSE
							UPDATE [dbo].[PlayerData] SET [Value] = @foodPlayerValue WHERE [PlayerDataId] = @foodPlayerId;
					END
				IF (@woodUpdate = 1)
					BEGIN
						SELECT @woodPlayerId = p.[PlayerDataId], @woodPlayerValue = p.[Value]
						FROM @tempPlayerAllDataTable AS p 
						WHERE p.[PlayerId] = @currentId AND p.[DataTypeId] = @resDataTypeId AND p.[ValueId] = @woodId

						IF (@woodPlayerValue IS NULL OR LTRIM(RTRIM(@woodPlayerValue)) = '') SET @woodPlayerValue = '0'
						DECLARE @valueINTWood INT = CONVERT(INT, @woodPlayerValue);
						SET @valueINTWood = @valueINTWood + @tempWood;
						SET @woodPlayerValue = CONVERT(VARCHAR(MAX), @valueINTWood);
						
						IF (@woodPlayerId IS NULL) 
							INSERT INTO [dbo].[PlayerData] VALUES (@currentId, @resDataTypeId, @woodPlayerId, @woodPlayerValue)
						ELSE 
							UPDATE [dbo].[PlayerData] SET [Value] = @woodPlayerValue WHERE [PlayerDataId] = @woodPlayerId;
					END
				IF (@oreUpdate = 1)
					BEGIN
						SELECT @orePlayerId = p.[PlayerDataId], @orePlayerValue = p.[Value]
						FROM @tempPlayerAllDataTable AS p 
						WHERE p.[PlayerId] = @currentId AND p.[DataTypeId] = @resDataTypeId AND p.[ValueId] = @oreId

						IF (@orePlayerValue IS NULL OR LTRIM(RTRIM(@orePlayerValue)) = '') SET @orePlayerValue = '0'
						DECLARE @valueINTOre INT = CONVERT(INT, @orePlayerValue);
						SET @valueINTOre = @valueINTOre + @tempOre;
						SET @orePlayerValue = CONVERT(VARCHAR(MAX), @valueINTOre);
						
						IF (@orePlayerId IS NULL) 
							INSERT INTO [dbo].[PlayerData] VALUES (@currentId, @resDataTypeId, @orePlayerId, @orePlayerValue)
						ELSE 
							UPDATE [dbo].[PlayerData] SET [Value] = @orePlayerValue WHERE [PlayerDataId] = @orePlayerId;
					END
				IF (@gemUpdate = 1)
					BEGIN
						SELECT @gemPlayerId = p.[PlayerDataId], @gemPlayerValue = p.[Value]
						FROM @tempPlayerAllDataTable AS p 
						WHERE p.[PlayerId] = @currentId AND p.[DataTypeId] = @resDataTypeId AND p.[ValueId] = @gemId

						IF (@gemPlayerValue IS NULL OR LTRIM(RTRIM(@gemPlayerValue)) = '') SET @gemPlayerValue = '0'
						DECLARE @valueINTGem INT = CONVERT(INT, @gemPlayerValue);
						SET @valueINTGem = @valueINTGem + @tempGem;
						SET @gemPlayerValue = CONVERT(VARCHAR(MAX), @valueINTGem);
						
						IF (@gemPlayerId IS NULL) 
							INSERT INTO [dbo].[PlayerData] VALUES (@currentId, @resDataTypeId, @gemPlayerId, @gemPlayerValue)
						ELSE 
							UPDATE [dbo].[PlayerData] SET [Value] = @gemPlayerValue WHERE [PlayerDataId] = @gemPlayerId;
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
