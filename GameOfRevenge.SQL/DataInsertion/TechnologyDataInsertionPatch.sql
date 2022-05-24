--SELECT * FROM [dbo].[Technology]

--SELECT d.[TechnologyDataId], t.[Name], d.[TechnologyLevel] FROM [dbo].[TechnologyData] AS d
--INNER JOIN [dbo].[Technology] AS t ON d.[TechnologyId] = t.[TechnologyId]

--SELECT * FROM [dbo].[TechnologyRequirement]

USE [GameOfRevenge]
GO

DELETE FROM [dbo].[TechnologyRequirement]
GO
DELETE FROM [dbo].[TechnologyData]
GO
DELETE FROM [dbo].[Technology]
GO

DBCC CHECKIDENT ('[TechnologyRequirement]', RESEED, 0);
GO
DBCC CHECKIDENT ('[TechnologyData]', RESEED, 0);
GO
DBCC CHECKIDENT ('[Technology]', RESEED, 0);
GO

-- ******************************************* ADD TECH  *******************************

INSERT INTO [dbo].[Technology] VALUES 
	('Resource Production', 'ResourceProduction'),
	('Construction Speed', 'ConstructionSpeed'),
	('Traning Speed', 'TraningSpeed'),
	('Recovery Speed', 'RecoverySpeed'),
	('Army Attack', 'ArmyAttack'),('Army Defence', 'ArmyDefence')
GO

-- ******************************************* ADD TECH DATA *******************************

DECLARE @techId int;
DECLARE myCursor CURSOR FORWARD_ONLY FOR SELECT [TechnologyId] FROM [dbo].[Technology]
OPEN myCursor;
FETCH NEXT FROM myCursor INTO @techId
WHILE @@FETCH_STATUS = 0 BEGIN
	INSERT INTO [dbo].[TechnologyData] (TechnologyId, TechnologyLevel, TechBonusValue, TimeTaken) VALUES 
	(@techId, 1, 1, 43200), (@techId, 2, 2, 259200), (@techId, 3, 3, 518400), (@techId, 4, 4, 691200), (@techId, 5, 5, 950400)
    FETCH NEXT FROM myCursor INTO @techId
END;
CLOSE myCursor;
DEALLOCATE myCursor;
GO

-- ******************************************* ADD TECH REQ *******************************

DECLARE @structId INT;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Acadamy';

DECLARE @datatypeStructId INT;
SELECT @datatypeStructId = [DataTypeId] FROM [dbo].[DataType] WHERE [Code] = 'Structure';

DECLARE @datatypeResId INT;
SELECT @datatypeResId = [DataTypeId] FROM [dbo].[DataType] WHERE [Code] = 'Resource';

DECLARE @foodId INT, @woodId INT, @oreId INT;
SELECT @foodId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Food';
SELECT @woodId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Wood';
SELECT @oreId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Ore';

DECLARE @techId int, @lvl int;
DECLARE myCursor CURSOR FORWARD_ONLY FOR SELECT [TechnologyDataId], [TechnologyLevel] FROM [dbo].[TechnologyData]
OPEN myCursor;
FETCH NEXT FROM myCursor INTO @techId, @lvl
WHILE @@FETCH_STATUS = 0 BEGIN
	DECLARE @f INT = @lvl * @lvl * @lvl * 1000;
	DECLARE @w INT = @lvl * @lvl * @lvl * 800;
	DECLARE @o INT = @lvl * @lvl * @lvl * 650;
	INSERT INTO [dbo].[TechnologyRequirement] VALUES 
		(@techId, @datatypeResId, @foodId, @f),
		(@techId, @datatypeResId, @woodId, @w),
		(@techId, @datatypeResId, @oreId, @o)
	INSERT INTO [dbo].[TechnologyRequirement] VALUES  (@techId, @datatypeStructId, @structId, @lvl)

    FETCH NEXT FROM myCursor INTO @techId, @lvl
END;
CLOSE myCursor;
DEALLOCATE myCursor;
GO


UPDATE [dbo].[TechnologyRequirement] 
SET [Value] = 30
WHERE [Value] = 5
GO

UPDATE [dbo].[TechnologyRequirement] 
SET [Value] = 24
WHERE [Value] = 4
GO

UPDATE [dbo].[TechnologyRequirement] 
SET [Value] = 15
WHERE [Value] = 3
GO

UPDATE [dbo].[TechnologyRequirement] 
SET [Value] = 5
WHERE [Value] = 2
GO