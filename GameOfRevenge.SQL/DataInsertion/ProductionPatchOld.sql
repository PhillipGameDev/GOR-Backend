USE [GameOfRevenge1]
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


DECLARE @structId INT, @lvl INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'CityCounsel'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 0);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 280);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 600);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 2400);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 4800);
SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 9300);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 12900);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 18000);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 25200);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 35400);
SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 45300);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 63180);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 84600);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 115200);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 154800);
SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 208800);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 284400);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 324000);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 367200);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 417600);
SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 478800);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 547200);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 630000);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 723600);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 831600);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 954000);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 1083600);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 1238400);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 1400400);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 0, 0, 0, 0, 4000, 0, 0, 1569600);
SET @lvl = @lvl + 1;
GO


DECLARE @structId INT, @lvl INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Gate'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 8000, 0, 0, 0, 0, 0, 0, 224);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 8000, 0, 0, 0, 0, 0, 0, 480);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 8100, 0, 0, 0, 0, 0, 0, 1920);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 8200, 0, 0, 0, 0, 0, 0, 3840);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 8300, 0, 0, 0, 0, 0, 0, 6600);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 8400, 0, 0, 0, 0, 0, 0, 10800);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl,8500, 0, 0, 0, 0, 0, 0, 14220);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 8600, 0, 0, 0, 0, 0, 0, 19800);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 8700, 0, 0, 0, 0, 0, 0, 27000);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 8800, 0, 0, 0, 0, 0, 0, 36720);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 8900, 0, 0, 0, 0, 0, 0, 51120);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 9000, 0, 0, 0, 0, 0, 0, 65520);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 9100, 0, 0, 0, 0, 0, 0, 87120);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 9200, 0, 0, 0, 0, 0, 0, 125280);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 9300, 0, 0, 0, 0, 0, 0, 171360);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 9400, 0, 0, 0, 0, 0, 0, 227520);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 9500, 0, 0, 0, 0, 0, 0, 262080);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 9600, 0, 0, 0, 0, 0, 0, 293040);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 9700, 0, 0, 0, 0, 0, 0, 332640);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 9800, 0, 0, 0, 0, 0, 0, 384480);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 9900, 0, 0, 0, 0, 0, 0, 441360);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 10000, 0, 0, 0, 0, 0, 0, 503280);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 10100, 0, 0, 0, 0, 0, 0, 580320);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 10200, 0, 0, 0, 0, 0, 0, 668880);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 10300, 0, 0, 0, 0, 0, 0, 765360);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 10400, 0, 0, 0, 0, 0, 0, 869760);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 10500, 0, 0, 0, 0, 0, 0, 975600);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 10600, 0, 0, 0, 0, 0, 0, 1120320);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 10700, 0, 0, 0, 0, 0, 0, 1282320);
SET @lvl = @lvl + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)
VALUES (@structId, @lvl, 10700, 0, 0, 0, 0, 0, 0, 1282320);
SET @lvl = @lvl + 1;
GO

DECLARE @structId INT, @lvl INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'WatchTower'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 0); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 30); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 100); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 300); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 780); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 1620); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 3600); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 5400); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 8400); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 11640); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 16800); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 22260); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 33840); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 41280); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 43200); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 53700); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 58020); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 69660); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 83640); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 108000); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 144000); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 172800); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 230400); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 266400); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 327600); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 453600); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 518400); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 712800); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 856800); SET @lvl = @lvl + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, SafeDeposit, TimeToBuild)VALUES (@structId, @lvl, 0, 0, 0, 0, 0, 0, 0, 1144800); SET @lvl = @lvl + 1;