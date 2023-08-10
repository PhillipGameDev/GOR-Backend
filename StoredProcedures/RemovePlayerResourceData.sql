USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[RemovePlayerResourceData]    Script Date: 4/21/2023 12:59:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



ALTER   PROCEDURE [dbo].[RemovePlayerResourceData]
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

	DECLARE @tempFood INT = ISNULL(@Food, 0);
	DECLARE @tempWood INT = ISNULL(@Wood, 0);
	DECLARE @tempOre INT = ISNULL(@Ore, 0);
	DECLARE @tempGem INT = ISNULL(@Gem, 0);
	DECLARE @tempGold INT = ISNULL(@Gold, 0);

	DECLARE @foodId INT, @woodId INT, @oreId INT, @gemId INT, @goldId INT, @resDataTypeId INT;
	DECLARE @foodDataId INT = NULL, @foodPlayerValue VARCHAR(MAX) = '0';
	DECLARE @woodDataId INT = NULL, @woodPlayerValue VARCHAR(MAX) = '0';
	DECLARE @oreDataId INT = NULL, @orePlayerValue VARCHAR(MAX) = '0';
	DECLARE @gemDataId INT = NULL, @gemPlayerValue VARCHAR(MAX) = '0';
	DECLARE @goldDataId INT = NULL, @goldPlayerValue VARCHAR(MAX) = '0';

	DECLARE @tempPlayerAllDataTable TABLE
	(
		PlayerDataId BIGINT NOT NULL,
		PlayerId INT NOT NULL,
		DataTypeId INT NOT NULL,
		ValueId INT NOT NULL,
		Value VARCHAR(MAX) NULL
	);

	BEGIN TRY
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

		IF (@tempFood = 0 AND @tempWood = 0 AND @tempOre = 0 AND @tempGem = 0 AND @tempGold = 0)
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
				SELECT @resDataTypeId = [DataTypeId] FROM [dbo].[DataType] WHERE [Code] = 'Resource';

				INSERT INTO @tempPlayerAllDataTable
				SELECT p.[PlayerDataId], p.[PlayerId], p.[DataTypeId], p.[ValueId], p.[Value]
				FROM [dbo].[PlayerData] AS p
				WHERE p.[PlayerId] = @currentId AND p.[DataTypeId] = @resDataTypeId;
				
				IF (@tempFood > 0)
					BEGIN
						SELECT @foodId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Food';

						SELECT @foodDataId = p.[PlayerDataId], @foodPlayerValue = p.[Value]
						FROM @tempPlayerAllDataTable AS p WHERE p.[ValueId] = @foodId

						IF (@foodPlayerValue IS NULL OR LTRIM(RTRIM(@foodPlayerValue)) = '') SET @foodPlayerValue = '0'
						DECLARE @valueINTFood BIGINT = CONVERT(BIGINT, @foodPlayerValue);
						SET @valueINTFood = @valueINTFood - @tempFood;
						IF (@valueINTFood < 0) SET @valueINTFood = 0;
						SET @foodPlayerValue = CONVERT(VARCHAR(MAX), @valueINTFood);
						
						IF (@foodDataId IS NULL) 
							INSERT INTO [dbo].[PlayerData] VALUES (@currentId, @resDataTypeId, @foodId, @foodPlayerValue)
						ELSE 
							UPDATE [dbo].[PlayerData] SET [Value] = @foodPlayerValue WHERE [PlayerDataId] = @foodDataId;
					END
				IF (@tempWood > 0)
					BEGIN
						SELECT @woodId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Wood';

						SELECT @woodDataId = p.[PlayerDataId], @woodPlayerValue = p.[Value]
						FROM @tempPlayerAllDataTable AS p WHERE p.[ValueId] = @woodId

						IF (@woodPlayerValue IS NULL OR LTRIM(RTRIM(@woodPlayerValue)) = '') SET @woodPlayerValue = '0'
						DECLARE @valueINTWood BIGINT = CONVERT(BIGINT, @woodPlayerValue);
						SET @valueINTWood = @valueINTWood - @tempWood;
						IF (@valueINTWood < 0) SET @valueINTWood = 0;
						SET @woodPlayerValue = CONVERT(VARCHAR(MAX), @valueINTWood);
						
						IF (@woodDataId IS NULL) 
							INSERT INTO [dbo].[PlayerData] VALUES (@currentId, @resDataTypeId, @woodId, @woodPlayerValue)
						ELSE 
							UPDATE [dbo].[PlayerData] SET [Value] = @woodPlayerValue WHERE [PlayerDataId] = @woodDataId;
					END
				IF (@tempOre > 0)
					BEGIN
						SELECT @oreId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Ore';

						SELECT @oreDataId = p.[PlayerDataId], @orePlayerValue = p.[Value]
						FROM @tempPlayerAllDataTable AS p WHERE p.[ValueId] = @oreId

						IF (@orePlayerValue IS NULL OR LTRIM(RTRIM(@orePlayerValue)) = '') SET @orePlayerValue = '0'
						DECLARE @valueINTOre BIGINT = CONVERT(BIGINT, @orePlayerValue);
						SET @valueINTOre = @valueINTOre - @tempOre;
						IF (@valueINTOre < 0) SET @valueINTOre = 0;
						SET @orePlayerValue = CONVERT(VARCHAR(MAX), @valueINTOre);
						
						IF (@oreDataId IS NULL) 
							INSERT INTO [dbo].[PlayerData] VALUES (@currentId, @resDataTypeId, @oreId, @orePlayerValue)
						ELSE 
							UPDATE [dbo].[PlayerData] SET [Value] = @orePlayerValue WHERE [PlayerDataId] = @oreDataId;
					END
				IF (@tempGem > 0)
					BEGIN
						SELECT @gemId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Gems';

						SELECT @gemDataId = p.[PlayerDataId], @gemPlayerValue = p.[Value]
						FROM @tempPlayerAllDataTable AS p WHERE p.[ValueId] = @gemId

						IF (@gemPlayerValue IS NULL OR LTRIM(RTRIM(@gemPlayerValue)) = '') SET @gemPlayerValue = '0'
						DECLARE @valueINTGem BIGINT = CONVERT(BIGINT, @gemPlayerValue);
						SET @valueINTGem = @valueINTGem - @tempGem;
						IF (@valueINTGem < 0) SET @valueINTGem = 0;
						SET @gemPlayerValue = CONVERT(VARCHAR(MAX), @valueINTGem);
						
						IF (@gemDataId IS NULL) 
							INSERT INTO [dbo].[PlayerData] VALUES (@currentId, @resDataTypeId, @gemId, @gemPlayerValue)
						ELSE 
							UPDATE [dbo].[PlayerData] SET [Value] = @gemPlayerValue WHERE [PlayerDataId] = @gemDataId;
					END
				IF (@tempGold > 0)
					BEGIN
						SELECT @goldId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Gold';

						SELECT @goldDataId = p.[PlayerDataId], @goldPlayerValue = p.[Value]
						FROM @tempPlayerAllDataTable AS p WHERE p.[ValueId] = @goldId

						IF (@goldPlayerValue IS NULL OR LTRIM(RTRIM(@goldPlayerValue)) = '') SET @goldPlayerValue = '0'
						DECLARE @valueINTGold BIGINT = CONVERT(BIGINT, @goldPlayerValue);
						SET @valueINTGold = @valueINTGold - @tempGold;
						IF (@valueINTGold < 0) SET @valueINTGold = 0;
						SET @goldPlayerValue = CONVERT(VARCHAR(MAX), @valueINTGold);
						
						IF (@goldDataId IS NULL) 
							INSERT INTO [dbo].[PlayerData] VALUES (@currentId, @resDataTypeId, @goldId, @goldPlayerValue)
						ELSE 
							UPDATE [dbo].[PlayerData] SET [Value] = @goldPlayerValue WHERE [PlayerDataId] = @goldDataId;
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