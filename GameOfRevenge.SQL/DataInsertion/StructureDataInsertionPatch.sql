USE [GameOfRevenge]
GO

DELETE FROM [dbo].[StructureRequirement]
GO
DELETE FROM [dbo].[StructureLocation]
GO
DELETE FROM [dbo].[StructureBuildData]
GO
DELETE FROM [dbo].[StructureData]
GO
DELETE FROM [dbo].[Structure]
GO
DBCC CHECKIDENT ('[StructureRequirement]', RESEED, 0);
GO
DBCC CHECKIDENT ('[StructureLocation]', RESEED, 0);
GO
DBCC CHECKIDENT ('[StructureData]', RESEED, 0);
go
DBCC CHECKIDENT ('[StructureBuildData]', RESEED, 0);
GO
DBCC CHECKIDENT ('[Structure]', RESEED, 0);
GO

DECLARE @ruralId INT, @urbanId INT, @fixedId INT;
SELECT @ruralId = [StructurePlacementTypeId] FROM [dbo].[StructurePlacementType] WHERE [Code] = 'Rural';
SELECT @urbanId = [StructurePlacementTypeId] FROM [dbo].[StructurePlacementType] WHERE [Code] = 'Urban';
SELECT @fixedId = [StructurePlacementTypeId] FROM [dbo].[StructurePlacementType] WHERE [Code] = 'Fixed';

--Fixed from 1-25,
--Urban from 26-50,
--Rural from 51-100
INSERT INTO [dbo].[Structure]  (Name, Code, StructurePlacementTypeId, Description) VALUES
	('City Counsel', 'CityCounsel', @fixedId, 'Main building for the city'),
	('Gate', 'Gate', @fixedId, 'The Wall protects your Castle. Upgrade it to increase trapcapacity and city defense.'),
	('Watch Tower', 'WatchTower', @fixedId, 'The Watch Tower can scout enemies and bring you information about approaching troops.'),
	('Embassy', 'Embassy', @fixedId, 'Create or join alliance'),
	('Warehouse', 'Warehouse', @fixedId, 'The Resources stored in the Warehouse will not be plundered. Upgrade the Warehouse level to increase the amount.'),
	('Market', 'Market', @fixedId, 'Provides you different discounts'),
	('Blacksmith', 'Blacksmith', @fixedId, 'To build glorious weapons and armour'),

	('Training Heroes', 'TrainingHeroes', @fixedId, 'Recruit and manage your heros here'),
	('Acadamy', 'Acadamy', @fixedId, 'The Institute is where you can research technologies to strengthen your power'),

	('Farm', 'Farm', @ruralId, 'Need a Farm to supply food to troops'),
	('Sawmill', 'Sawmill', @ruralId, 'The Lumber mill is where Lumber is produced and held. Upgrade it to increase yield and holding capacity.'),
	('Mine', 'Mine', @ruralId, 'Produces ore every minute'),
	('Infantry Camp', 'InfantryCamp', @ruralId, 'Marching Tent increases the number of regular soldiers( Infantry, Archers, Cavalry,Siege Engines) trained at one time and the speed of their training. Upgrade to train more troops at faster rate.'),

	('WorkShop', 'WorkShop', @urbanId, 'Train seige troops'),
	('Barracks', 'Barracks', @urbanId, 'The Barracks is where you train Infantry. Upgrade it to train higher level Infantries'),
	('Shooting Range', 'ShootingRange', @urbanId, 'Train ranged troops here'),
	('Stable', 'Stable', @urbanId, 'The Stable is where your cavalry are trained. Upgrade it to unlock higher level cavalry'),
	('Infirmary', 'Infirmary', @urbanId, 'The First aid Tent is where wounded soldiers are treated. Upgrade it to add more space.')
GO

--	**************************************************** LOC ID *******************************************************

DECLARE @structId INT;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'CityCounsel'
INSERT INTO [dbo].[StructureLocation] VALUES (@structId, 1)
GO

DECLARE @structId INT;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Gate'
INSERT INTO [dbo].[StructureLocation] VALUES (@structId, 2)
GO

DECLARE @structId INT;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'WatchTower'
INSERT INTO [dbo].[StructureLocation] VALUES (@structId, 4)
GO

DECLARE @structId INT;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Warehouse'
INSERT INTO [dbo].[StructureLocation] VALUES (@structId, 5)
GO

DECLARE @structId INT;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Embassy'
INSERT INTO [dbo].[StructureLocation] VALUES (@structId, 5)
GO

DECLARE @structId INT;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Market'
INSERT INTO [dbo].[StructureLocation] VALUES (@structId, 6)
GO

DECLARE @structId INT;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Acadamy'
INSERT INTO [dbo].[StructureLocation] VALUES (@structId, 7)
GO

DECLARE @structId INT;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'TrainingHeroes'
INSERT INTO [dbo].[StructureLocation] VALUES (@structId, 8)
GO

DECLARE @structId INT;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Blacksmith'
INSERT INTO [dbo].[StructureLocation] VALUES (@structId, 9)
GO

DECLARE @structId1 INT;
DECLARE @structId2 INT;
DECLARE @structId3 INT;
DECLARE @structId4 INT;

SELECT @structId1 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Farm'
SELECT @structId2 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Sawmill'
SELECT @structId3 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Mine'
SELECT @structId4 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'InfantryCamp'

DECLARE @id INT = 51;
WHILE ( @id <= 100)
	BEGIN
		INSERT INTO [dbo].[StructureLocation] VALUES (@structId1, @id)
		INSERT INTO [dbo].[StructureLocation] VALUES (@structId2, @id)
		INSERT INTO [dbo].[StructureLocation] VALUES (@structId3, @id)
		INSERT INTO [dbo].[StructureLocation] VALUES (@structId4, @id)
		SET @id = @id + 1;
	END
GO

DECLARE @structId1 INT;
DECLARE @structId2 INT;
DECLARE @structId3 INT;
DECLARE @structId4 INT;
DECLARE @structId5 INT;

SELECT @structId1 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Barracks'
SELECT @structId2 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'ShootingRange'
SELECT @structId3 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Stable'
SELECT @structId4 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Infirmary'
SELECT @structId5 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'WorkShop'

DECLARE @id INT = 26;
WHILE ( @id <= 50)
	BEGIN
		INSERT INTO [dbo].[StructureLocation] VALUES (@structId1, @id)
		INSERT INTO [dbo].[StructureLocation] VALUES (@structId2, @id)
		INSERT INTO [dbo].[StructureLocation] VALUES (@structId3, @id)
		INSERT INTO [dbo].[StructureLocation] VALUES (@structId4, @id)
		INSERT INTO [dbo].[StructureLocation] VALUES (@structId5, @id)
		SET @id = @id + 1;
	END
GO


-- ***************************** BUILD LIMIT AS PER CITY HALL ********************************************


DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Gate'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId, 1); SET @cid = @cid + 1; END
GO


DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'WatchTower'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId, 1) SET @cid = @cid + 1; END
GO

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Embassy'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId, 1) SET @cid = @cid + 1; END
GO

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Warehouse'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId, 1) SET @cid = @cid + 1; END
GO

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Acadamy'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId, 1) SET @cid = @cid + 1; END
GO

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Blacksmith'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId, 1) SET @cid = @cid + 1; END
GO

DECLARE @structId INT, @cid INT = 10;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'TrainingHeroes'
INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId, 1);
GO

DECLARE @structId INT, @cid INT = 8;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Market'
INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId, 1);
GO

DECLARE @structId1 INT;
DECLARE @structId2 INT;
DECLARE @structId3 INT;
DECLARE @structId4 INT;

SELECT @structId1 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Farm'
SELECT @structId2 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Sawmill'
SELECT @structId3 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Mine'
SELECT @structId4 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'InfantryCamp'

DECLARE @cid INT = 1,  @maxVal FLOAT = 1;
WHILE (@cid <= 30)
	BEGIN
		INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId1, FLOOR(@maxVal))
		INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId2, FLOOR(@maxVal))
		INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId3, FLOOR(@maxVal))
		INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId4, FLOOR(@maxVal))
		SET @maxVal = @maxVal + 0.4
		SET @cid = @cid + 1;
	END
GO

DECLARE @structId1 INT;
DECLARE @structId2 INT;
DECLARE @structId3 INT;
DECLARE @structId4 INT;
DECLARE @structId5 INT;

SELECT @structId1 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Barracks'
SELECT @structId2 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'ShootingRange'
SELECT @structId3 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Stable'
SELECT @structId4 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Infirmary'
SELECT @structId5 = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'WorkShop'

DECLARE @cid INT = 1,  @maxVal FLOAT = 1;
WHILE (@cid <= 30)
	BEGIN
		IF(@cid = 30) SET @maxVal = @maxVal + 0.1
		INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId1, FLOOR(@maxVal))
		INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId2, FLOOR(@maxVal))
		INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId3, FLOOR(@maxVal))
		INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId4, FLOOR(@maxVal))
		INSERT INTO [dbo].[StructureBuildData] VALUES (@cid, @structId5, FLOOR(@maxVal))
		SET @maxVal = @maxVal + 0.1
		SET @cid = @cid + 1;
	END

GO

-- ********************************STRUCTURE DATA******************************************************************


DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'CityCounsel'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureData] 
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
VALUES (@structId, @cid, 1000 + 100 * @cid, 0, 0, 0, 0, 4000, @cid * 60, 0); SET @cid = @cid + 1; END
GO

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Gate'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureData] 
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
VALUES (@structId, @cid, 1000 + 100 * @cid, 0, 0, 0, 0, 0, @cid * 60, 0); SET @cid = @cid + 1; END
GO

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'WatchTower'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureData] 
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
VALUES (@structId, @cid, 1000 + 100 * @cid, 0, 0, 0, 0, 0, @cid * 60, 0); SET @cid = @cid + 1; END
GO

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Embassy'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
VALUES (@structId, @cid, 1000 + 100 * @cid, 0, 0, 0, 0, 0, @cid * 60, 0);; SET @cid = @cid + 1; END
GO

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Warehouse'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureData]
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
VALUES (@structId, @cid, 1000 + 100 * @cid, 0, 0, 0, 0, 0, @cid * 60, 10000 * @cid); SET @cid = @cid + 1; END
GO

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Acadamy'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureData]
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
 VALUES (@structId, @cid, 1000 + 100 * @cid, 0, 0, 0, 0, 0, @cid * 60, 0); SET @cid = @cid + 1; END
GO

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'WorkShop'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureData]
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
 VALUES (@structId, @cid, 1000 + 100 * @cid, 0, 0, 0, 0, 0, @cid * 60, 0); SET @cid = @cid + 1; END
GO

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Blacksmith'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureData]
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
 VALUES (@structId, @cid, 1000 + 100 * @cid, 0, 0, 0, 0, 0, @cid * 60, 0); SET @cid = @cid + 1; END
GO


DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Market'
INSERT INTO [dbo].[StructureData] 
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
VALUES (@structId, @cid, 2000, 0, 0, 0, 0, 0, 18000, 0);;
GO

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'TrainingHeroes'
INSERT INTO [dbo].[StructureData] 
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
VALUES (@structId, @cid, 1800, 0, 0, 0, 0, 0, 18000, 0);;
GO



DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Barracks'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureData] 
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
VALUES (@structId, @cid, 1000 + 100 * @cid, 0, 0, 0, 0, 0, @cid * 60, 0);; SET @cid = @cid + 1; END
GO

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'ShootingRange'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureData] 
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
VALUES (@structId, @cid, 1000 + 100 * @cid, 0, 0, 0, 0, 0, @cid * 60, 0);; SET @cid = @cid + 1; END
GO

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Stable'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureData] 
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
VALUES (@structId, @cid, 1000 + 100 * @cid, 0, 0, 0, 0, 0, @cid * 60, 0);; SET @cid = @cid + 1; END
GO

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Infirmary'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureData] 
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
VALUES (@structId, @cid, 1000 + 100 * @cid, 0, 0, 0, 100 + @cid * 5, 0, @cid * 60, 0);; SET @cid = @cid + 1; END
GO

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'InfantryCamp'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureData] 
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
VALUES (@structId, @cid, 1000 + 100 * @cid, 0, 0, 0, 100 + @cid * 25, 0, @cid * 60, 0);; SET @cid = @cid + 1; END
GO


DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Mine'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureData] 
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
VALUES (@structId, @cid, 1000 + 100 * @cid, 0, 0, 100 + @cid * 25, 0, 0, @cid * 60, 0);; SET @cid = @cid + 1; END
GO


DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Sawmill'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureData] 
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
VALUES (@structId, @cid, 1000 + 100 * @cid, 0, 100 + @cid * 25, 0, 0, 0, @cid * 60, 0);; SET @cid = @cid + 1; END
GO


DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Farm'
WHILE (@cid <= 30) BEGIN INSERT INTO [dbo].[StructureData] 
(StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit)
VALUES (@structId, @cid, 1000 + 100 * @cid, 100 + @cid * 25, 0, 0, 0, 0, @cid * 60, 0);; SET @cid = @cid + 1; END
GO


-- ********************************STRUCTURE REQUIREMENTS DATA******************************************************************

CREATE OR ALTER PROCEDURE [dbo].[TempAddReqRes]
	@structDataId INT,
	@food INT, 
	@wood INT,
	@ore INT
AS
BEGIN
	DECLARE @tFood INT = ISNULL(@food, 0);
	DECLARE @tWood INT = ISNULL(@wood, 0);
	DECLARE @tOre INT = ISNULL(@ore, 0);
	DECLARE @tstructDataId INT = ISNULL(@structDataId, 0);

	IF(@tstructDataId <= 0) RETURN;

	DECLARE @datatypeResId INT;
	SELECT @datatypeResId = [DataTypeId] FROM [dbo].[DataType] WHERE [Code] = 'Resource';

	DECLARE @foodId INT, @woodId INT, @oreId INT;
	SELECT @foodId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Food';
	SELECT @woodId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Wood';
	SELECT @oreId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Ore';

	IF (@tFood >= 1) INSERT INTO [dbo].[StructureRequirement] VALUES (@tstructDataId, @datatypeResId, @foodId, @tFood)
	IF (@tWood >= 1) INSERT INTO [dbo].[StructureRequirement] VALUES (@tstructDataId, @datatypeResId, @woodId, @tWood)
	IF (@tOre >= 1) INSERT INTO [dbo].[StructureRequirement] VALUES (@tstructDataId, @datatypeResId, @oreId, @tOre)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[TempAddReqStruct]
	@structDataId INT,
	@structId INT,
	@structLvl INT
AS
BEGIN
	DECLARE @tstructDataId INT = ISNULL(@structDataId, 0);
	DECLARE @tstructId INT = ISNULL(@structId, 0);
	DECLARE @tstructLvl INT = ISNULL(@structLvl, 0);
	
	IF(@tstructLvl <= 0) RETURN;
	IF(@tstructId <= 0) RETURN;
	IF(@tstructDataId <= 0) RETURN;

	DECLARE @datatypeStructId INT;
	SELECT @datatypeStructId = [DataTypeId] FROM [dbo].[DataType] WHERE [Code] = 'Structure';

	INSERT INTO [dbo].[StructureRequirement] VALUES (@tstructDataId, @datatypeStructId, @tstructId, @tstructLvl)
END
GO

DECLARE @structId INT;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'CityCounsel';

DECLARE @structgId INT;
SELECT @structgId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Gate';

DECLARE @id int, @lvl int, @sid INT;
DECLARE myCursor CURSOR FORWARD_ONLY FOR SELECT [StructureDataId], [StructureLevel], [StructureId] FROM [dbo].[StructureData]
OPEN myCursor;
FETCH NEXT FROM myCursor INTO @id, @lvl, @sid
WHILE @@FETCH_STATUS = 0 BEGIN
	DECLARE @f INT = @lvl * @lvl * 1000;
	DECLARE @w INT = @lvl * @lvl * 750;
	DECLARE @o INT = @lvl * @lvl * 500;
    EXECUTE [dbo].[TempAddReqRes] @id, @f, @w, @o;
	IF (@sid != @structId) EXECUTE [dbo].[TempAddReqStruct] @id, @structId, @lvl;
	ELSE IF(@lvl > 1)
		BEGIN
			SET @lvl = @lvl - 1;
			EXEC [dbo].[TempAddReqStruct] @id, @structgId, @lvl
		END
    FETCH NEXT FROM myCursor INTO @id, @lvl, @sid
END;
CLOSE myCursor;
DEALLOCATE myCursor;
GO


DROP PROCEDURE [dbo].[TempAddReqRes]
GO

DROP PROCEDURE [dbo].[TempAddReqStruct]
GO

DECLARE @structId INT;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'CityCounsel';

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 1 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 2 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 3 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 4 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 5 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 6 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 7 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 8 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 9 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 10 AND [StructureId] = @structId
GO

DECLARE @structId INT;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Gate';

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 1 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 2 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 3 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 4 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 5 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 6 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 7 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 8 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 9 AND [StructureId] = @structId

UPDATE [dbo].[StructureData]
SET [InstantBuildCost] = 0
WHERE [StructureLevel] = 10 AND [StructureId] = @structId
GO

--Add ur patch here

GO

--select * from structure

SELECT s.* FROM StructureData as s
inner join  Structure as ss on ss.[StructureId] = s.[StructureId] where code like 'Infirmary'

--OR code = 'Ore' OR code = 'Wood'


GO
DECLARE @structId INT, @level INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Infirmary'

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 1500
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 2250
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 3000
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 3750
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 4500
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 5250
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 6000
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 6750
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 7500
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 8250
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 9000
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 9750
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 10500
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 11250
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 12750
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 13500
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 14250
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 15000
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 15750
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 16500
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 17250
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 18000
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;


UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 18750
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 19500
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 20250
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 21000
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 21750
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 22500
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 23250
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

UPDATE [dbo].[StructureData]
SET [WoundedCapacity] = 24000
WHERE [StructureId] = @structId AND [StructureLevel] = @level;
SET @level = @level + 1;

GO










