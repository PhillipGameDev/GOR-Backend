USE [GameOfRevenge]
GO

DELETE FROM [dbo].[TroopRequirement]
GO
DELETE FROM [dbo].[TroopData]
GO
DELETE FROM [dbo].[Troop]
GO

DBCC CHECKIDENT ('[TroopRequirement]', RESEED, 0);
GO
DBCC CHECKIDENT ('[TroopData]', RESEED, 0);
GO
DBCC CHECKIDENT ('[Troop]', RESEED, 0);
GO

-- ******************************************* ADD TROOP  *******************************

INSERT INTO [dbo].[Troop] VALUES 
	('Swordsmen','Swordsmen', 'Basic troop', 1, 0, 0, 0),
	('Archer','Archer', 'Basic ranged troop', 0, 0, 0, 0),
	('Knight','Knight', 'Best troop', 1, 1, 0, 0),
	('Seige','Seige', 'Basic Seige troop', 0, 0, 0, 1)
GO

-- ******************************************* ADD TROOP DATA *******************************

DECLARE @troopId INT;
SELECT @troopId = [TroopId] FROM [dbo].[Troop] WHERE [Code] = 'Swordsmen';
INSERT INTO [dbo].[TroopData] VALUES 
	(@troopId, 1, 80, 10, 25, 1, 1, 4, 1.4, 6, 6, 1, 1), 
	(@troopId, 2, 85, 10, 28, 1, 1, 6, 1.3, 8, 8, 1, 2), 
	(@troopId, 3, 95, 10, 32, 1, 1, 8, 1.2, 12, 12, 1, 3), 
	(@troopId, 4, 105, 10, 35, 1, 1, 10, 1.1, 13, 14, 1, 4), 
	(@troopId, 5, 120, 10, 40, 1, 1, 12, 1, 15, 18, 1, 5)
GO

DECLARE @troopId INT;
SELECT @troopId = [TroopId] FROM [dbo].[Troop] WHERE [Code] = 'Archer';
INSERT INTO [dbo].[TroopData] VALUES 
	(@troopId, 1, 50, 10, 21, 5, 1, 2, 1.5, 4, 10, 1, 1), 
	(@troopId, 2, 60, 10, 24, 5, 1, 3, 1.5, 6, 12, 1, 2), 
	(@troopId, 3, 70, 10, 28, 6, 1, 5, 1.4, 8, 14, 1, 3), 
	(@troopId, 4, 80, 10, 32, 6, 1, 7, 1.4, 10, 18, 1, 4), 
	(@troopId, 5, 90, 10, 36, 7, 1, 9, 1.3, 12, 20, 1, 5)
GO

DECLARE @troopId INT;
SELECT @troopId = [TroopId] FROM [dbo].[Troop] WHERE [Code] = 'Knight';
INSERT INTO [dbo].[TroopData] VALUES 
	(@troopId, 1, 125, 10, 27, 1, 1, 8, 2.5, 15, 20, 1, 2), 
	(@troopId, 2, 138, 10, 30, 1, 1, 10, 2.3, 20, 22, 1, 3), 
	(@troopId, 3, 142, 10, 34, 1, 1, 12, 2.2, 25, 24, 1, 4), 
	(@troopId, 4, 150, 10, 38, 1, 1, 15, 2.1, 30, 28, 1, 5), 
	(@troopId, 5, 160, 10, 45, 1, 1, 18, 2, 40, 30, 1, 6)
GO

DECLARE @troopId INT;
SELECT @troopId = [TroopId] FROM [dbo].[Troop] WHERE [Code] = 'Seige';
INSERT INTO [dbo].[TroopData] VALUES 
	(@troopId, 1, 250, 10, 45, 7, 1, 0, 0.6, 0, 50, 1, 3), 
	(@troopId, 2, 280, 10, 58, 7, 1, 0, 0.6, 0, 60, 1, 4), 
	(@troopId, 3, 310, 10, 70, 8, 1, 0, 0.6, 0, 70, 1, 5), 
	(@troopId, 4, 350, 10, 88, 9, 1, 0, 0.6, 0, 80, 1, 6), 
	(@troopId, 5, 400, 10, 100, 10, 1, 0, 0.5, 0, 100, 1, 7)
GO

-- ******************************************* ADD TROOP REQ *******************************

CREATE OR ALTER PROCEDURE [dbo].[AddTroopReqStructAsPerTroopType]
	@troopDataId INT,
	@troopId INT,
	@level INT
AS
BEGIN
	DECLARE @tTroopDataId INT = ISNULL(@troopDataId, 0);
	DECLARE @tTroopId INT = ISNULL(@troopId, 0);
	DECLARE @tLvl INT = ISNULL(@level, 0);
	
	IF (@tlvl <= 0) RETURN;
	IF (@tTroopId <= 0) RETURN;
	IF (@tTroopDataId <= 0) RETURN;

	DECLARE @datatypeStructId INT;
	SELECT @datatypeStructId = [DataTypeId] FROM [dbo].[DataType] WHERE [Code] = 'Structure';

	DECLARE @workshopId INT, @barackId INT, @shootingId INT, @stableId INT
	SELECT @workshopId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'WorkShop';
	SELECT @barackId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Barracks';
	SELECT @shootingId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'ShootingRange';
	SELECT @stableId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Stable';

	DECLARE @swordId INT, @calvId INT, @archId INT, @seigId INT
	SELECT @swordId = [TroopId] FROM [dbo].[Troop] WHERE [Code] = 'Swordsmen';
	SELECT @archId = [TroopId] FROM [dbo].[Troop] WHERE [Code] = 'Archer';
	SELECT @calvId = [TroopId] FROM [dbo].[Troop] WHERE [Code] = 'Knight';
	SELECT @seigId = [TroopId] FROM [dbo].[Troop] WHERE [Code] = 'Seige';

	IF (@swordId = @tTroopId) INSERT INTO [dbo].[TroopRequirement] VALUES (@tTroopDataId, @datatypeStructId, @barackId, @tLvl);
	ELSE IF (@archId = @tTroopId) INSERT INTO [dbo].[TroopRequirement] VALUES (@tTroopDataId, @datatypeStructId, @shootingId, @tLvl)
	ELSE IF (@calvId = @tTroopId) INSERT INTO [dbo].[TroopRequirement] VALUES (@tTroopDataId, @datatypeStructId, @stableId, @tLvl)
	ELSE IF (@seigId = @tTroopId) INSERT INTO [dbo].[TroopRequirement] VALUES (@tTroopDataId, @datatypeStructId, @workshopId, @tLvl)

	DECLARE @f FLOAT, @w FLOAT, @o FLOAT

	IF (@swordId = @tTroopId)
		BEGIN
			SET @f = 25;
			SET @w = 10;
			SET @o = 5;
		END
	ELSE IF (@archId = @tTroopId)
		BEGIN
			SET @f = 15;
			SET @w = 25;
			SET @o = 5;
		END
	ELSE IF (@calvId = @tTroopId)
		BEGIN
			SET @f = 25;
			SET @w = 25;
			SET @o = 25;
		END
	ELSE IF (@seigId = @tTroopId)
		BEGIN
			SET @f = 35;
			SET @w = 150;
			SET @o = 45;
		END

		WHILE (@tLvl > 0)
			BEGIN
				SET @f = @f * 1.1;
				SET @w = @w * 1.1;
				SET @o = @o * 1.1;
				SET @tLvl = @tLvl - 1;
			END

	IF (@swordId = @tTroopId AND @tLvl = 1) SET @o = 0;
	IF (@archId = @tTroopId AND @tLvl = 1) SET @o = 0;

	DECLARE @datatypeResId INT;
	SELECT @datatypeResId = [DataTypeId] FROM [dbo].[DataType] WHERE [Code] = 'Resource';

	DECLARE @foodId INT, @woodId INT, @oreId INT;
	SELECT @foodId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Food';
	SELECT @woodId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Wood';
	SELECT @oreId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Ore';

	INSERT INTO [dbo].[TroopRequirement] VALUES 
		(@tTroopDataId, @datatypeResId, @foodId, @f),
		(@tTroopDataId, @datatypeResId, @woodId, @w),
		(@tTroopDataId, @datatypeResId, @oreId, @o)
END
GO

DECLARE @troopId int, @lvl INT, @ttid int;
DECLARE myCursor CURSOR FORWARD_ONLY FOR SELECT [TroopDataId], [TroopLevel], [TroopId] FROM [dbo].[TroopData]
OPEN myCursor;
FETCH NEXT FROM myCursor INTO @troopId, @lvl, @ttid
WHILE @@FETCH_STATUS = 0 BEGIN
	EXEC [dbo].[AddTroopReqStructAsPerTroopType] @troopId, @ttid, @lvl
    FETCH NEXT FROM myCursor INTO @troopId, @lvl, @ttid
END;
CLOSE myCursor;
DEALLOCATE myCursor;
GO

DECLARE @datattypeId INT = 0;
SELECT @datattypeId = [DataTypeId] FROM [dbo].[DataType]
WHERE [Code] = 'Structure'

UPDATE [dbo].[TroopRequirement] 
SET [Value] = 23
WHERE [Value] = 4 AND [DataTypeId] = @datattypeId

UPDATE [dbo].[TroopRequirement] 
SET [Value] = 8
WHERE [Value] = 2 AND [DataTypeId] = @datattypeId

UPDATE [dbo].[TroopRequirement] 
SET [Value] = 13
WHERE [Value] = 3 AND [DataTypeId] = @datattypeId

UPDATE [dbo].[TroopRequirement] 
SET [Value] = 30
WHERE [Value] = 5 AND [DataTypeId] = @datattypeId
GO

DROP PROCEDURE [dbo].[AddTroopReqStructAsPerTroopType]
GO
