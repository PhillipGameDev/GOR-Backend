USE [GameOfRevenge]
GO

DELETE FROM [dbo].[HeroRequirement]
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

DBCC CHECKIDENT ('[HeroRequirement]', RESEED, 0);
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



--*****************************************
--*****************************************
--*****************************************
--*****************************************
--*****************************************
--*****************************************
--*****************************************
--*****************************************
--*****************************************
--*****************************************





DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'CityCounsel'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 0); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 280); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 600); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 2400); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 4800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 9300); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 12900); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 18000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 25200); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 35400); 
SET @cid = @cid + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 45300); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 63180); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 84600); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 115200); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 154800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 208800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 284400); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 324000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 367200); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 417600); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 478800); 
SET @cid = @cid + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 547200); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 630000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 723600); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 831600); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 954000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 1083600); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 1238400); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 1400400); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 1605600); 
SET @cid = @cid + 1;


GO


--GATE

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Gate'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 0); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 224); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 480); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 1920); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 3840); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 6600); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 10800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 14220); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 19800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 27000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 36720); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 51120); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 65520); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 87120); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 125280); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 171360); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 227520); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 262080); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 293040); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 332640); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 384480); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 441360); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 503280); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 580320); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 668880); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 765360); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 869760); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 975600); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 1120320); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, HitPoint, TimeToBuild) 
VALUES (@structId, @cid, 1000, 1282320); 
SET @cid = @cid + 1;
GO


--WATCHTOWER

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'WatchTower'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 0); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 30); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 100); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 300); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 780); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 1620); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 3600); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 5400); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 8400); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 11700); 
SET @cid = @cid + 1;
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 16800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 22260); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 33840); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 41280); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 43200); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 53700); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 58020); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 69660); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 83640); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 108000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 144000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 172800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 230400); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 255600); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 327600); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 453600); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 518400); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 712800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 856800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 1144800); 
SET @cid = @cid + 1;


GO


-- acedemy *************************

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Acadamy'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 30); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 182); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 390); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 1560); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 2940); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 5040); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 7800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 11520); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 15915); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 22395); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 30000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 39885); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 54900); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 75960); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 100440); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 137880); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 189360); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 217440); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 245520); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 273420); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 315540); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 357480); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 404280); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 471960); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 539640); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 616680); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 703080); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 798840); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 899640); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 1029960); 
SET @cid = @cid + 1;

GO

-- WORKSHOP *************************

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'WorkShop'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 60); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 182); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 390); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 1560); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 2940); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 5040); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 7800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 11520); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 15915); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 22395); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 30000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 39885); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 54900); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 75960); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 100440); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 137880); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 189360); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 217440); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 245520); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 273420); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 315540); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 357480); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 404280); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 471960); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 539640); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 616680); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 703080); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 798840); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 899640); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 1029960); 
SET @cid = @cid + 1;

GO


-- Warehouse *************************

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Warehouse'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 0, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 55, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 82, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 147, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 328, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 535, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 993, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 1961, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 3552, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 6269, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 11334, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 17335, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 23911, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 30163, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 40304, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 57476, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 65705, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 73978, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 86204, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 102962, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 116667, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 132856, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 148156, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 158400, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 177790, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 196660, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 212821, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 230447, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 243979, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, SafeDeposit) 
VALUES (@structId, @cid, 259200, 500); 
SET @cid = @cid + 1;

GO

-- FARMS *********************************************



-- Farm *************************

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Farm'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 2, 180); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 30, 300); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 45, 440); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 110, 580); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 265, 740); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 520, 900); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 835, 1080); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 995, 1300); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 1200, 1540); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 1515, 1790); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 1832, 2040); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 2700, 2310); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 3600, 2590); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 5782, 2890); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 7888, 3190); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 12625, 3520); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 17700, 3900); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 21944, 4300); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 26499, 4700); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 33240, 5100); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 38659, 5500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 45096, 6000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 54522, 6500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 64794, 7000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 70424, 7600); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 78741, 8200); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 84240, 8800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 91361, 9400); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 100361, 10200); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, FoodProduction) 
VALUES (@structId, @cid, 107879, 11200); 
SET @cid = @cid + 1;
GO



-- Sawmill *************************

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Sawmill'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 2, 180); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 30, 300); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 45, 440); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 110, 580); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 265, 740); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 520, 900); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 835, 1080); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 995, 1300); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 1200, 1540); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 1515, 1790); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 1832, 2040); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 2700, 2310); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 3600, 2590); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 5782, 2890); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 7888, 3190); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 12625, 3520); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 17700, 3900); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 21944, 4300); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 26499, 4700); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 33240, 5100); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 38659, 5500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 45096, 6000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 54522, 6500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 64794, 7000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 70424, 7600); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 78741, 8200); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 84240, 8800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 91361, 9400); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 100361, 10200); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoodProduction) 
VALUES (@structId, @cid, 107879, 11200); 
SET @cid = @cid + 1;
GO





-- Mine *************************

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Mine'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 2, 180); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 30, 300); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 45, 440); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 110, 580); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 265, 740); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 520, 900); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 835, 1080); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 995, 1300); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 1200, 1540); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 1515, 1790); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 1832, 2040); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 2700, 2310); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 3600, 2590); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 5782, 2890); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 7888, 3190); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 12625, 3520); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 17700, 3900); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 21944, 4300); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 26499, 4700); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 33240, 5100); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 38659, 5500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 45096, 6000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 54522, 6500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 64794, 7000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 70424, 7600); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 78741, 8200); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 84240, 8800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 91361, 9400); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 100361, 10200); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, OreProduction) 
VALUES (@structId, @cid, 107879, 11200); 
SET @cid = @cid + 1;
GO



-- Infirmary *************************

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Infirmary'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 45, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 150, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 285, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 530, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 985, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 1840, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 2755, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 3575, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 4500, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 5115, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 6632, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 9180, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 10800, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 14182, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 18208, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 22405, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 28631, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 32744, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 37299, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 42216, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 46579, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 52296, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 58122, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 64794, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 70424, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 78741, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 84240, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 94281, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 106434, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, WoundedCapacity) 
VALUES (@structId, @cid, 115929, 500); 
SET @cid = @cid + 1;

GO



-- InfantryCamp *************************

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'InfantryCamp'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 30, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 60, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 300, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 870, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 962, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 2040, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 2969, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 5760, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 13560, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 25740, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 47940, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 61440, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 73800, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 86400, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 104400, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 154800, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 183600, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 244800, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 270000, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 324000, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 370800, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 428400, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 464400, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 496800, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 604800, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 777600, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 849600, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 705600, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 1036800, 500); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, PopulationSupport) 
VALUES (@structId, @cid, 1368000, 500); 
SET @cid = @cid + 1;
GO




-- Barracks *************************

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Barracks'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 30); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 182); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 390); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 1560); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 2940); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 5040); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 7800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 11520); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 15915); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 22395); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 30000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 39885); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 54900); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 75960); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 100440); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 137880); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 189360); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 217440); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 245520); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 273420); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 315540); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 357480); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 404280); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 471960); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 539640); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 616680); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 703080); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 798840); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 899640); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 1029960); 
SET @cid = @cid + 1;

GO


-- ShootingRange *************************

DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'ShootingRange'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 30); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 182); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 390); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 1560); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 2940); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 5040); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 7800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 11520); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 15915); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 22395); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 30000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 39885); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 54900); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 75960); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 100440); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 137880); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 189360); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 217440); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 245520); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 273420); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 315540); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 357480); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 404280); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 471960); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 539640); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 616680); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 703080); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 798840); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 899640); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 1029960); 
SET @cid = @cid + 1;

GO



-- Stable *************************
DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Stable'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 30); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 182); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 390); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 1560); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 2940); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 5040); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 7800); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 11520); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 15915); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 22395); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 30000); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 39885); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 54900); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 75960); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 100440); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 137880); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 189360); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 217440); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 245520); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 273420); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 315540); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 357480); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 404280); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 471960); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 539640); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 616680); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 703080); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 798840); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 899640); 
SET @cid = @cid + 1;

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, @cid, 1029960); 
SET @cid = @cid + 1;

GO


--MARKET**************************
DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Market'
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, 1, 1282320);;
GO


-- TrainingHeroes ***********************
DECLARE @structId INT, @cid INT = 1;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'TrainingHeroes'
INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel,TimeToBuild)
VALUES (@structId, 1, 1282320);;
GO


-- Blacksmith *************************
DECLARE @structId INT;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Blacksmith'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, 1, 1282320); 
GO


-- EMBASSY *************************
DECLARE @structId INT;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Embassy'

INSERT INTO [dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild) 
VALUES (@structId, 1, 128232); 