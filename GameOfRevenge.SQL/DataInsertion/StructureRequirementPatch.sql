Use GameOfRevenge
go

CREATE OR ALTER PROCEDURE [dbo].[TempAddReqRes]
	@structCode varchar(100),
	@structLvl INT,
	@food INT, 
	@wood INT,
	@ore INT
AS
BEGIN
	DECLARE @tFood INT = ISNULL(@food, 0);
	DECLARE @tWood INT = ISNULL(@wood, 0);
	DECLARE @tOre INT = ISNULL(@ore, 0);

	DECLARE @tstructCode varchar(100) = LTRIM(RTRIM(@structCode));
	DECLARE @tstructLvl INT = ISNULL(@structLvl, 0);

	DECLARE @tstructDataId INT = 0;
	SELECT @tstructDataId = d.structuredataid from structuredata  as d
	inner join structure as s on s.StructureId = d.StructureId
	where d.StructureLevel = @tstructLvl and s.Code = @tstructCode

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
	@structdCode varchar(100),
	@structdLvl INT,
	@structCode varchar(100),
	@structLvl INT
AS
BEGIN

	DECLARE @tstructdCode varchar(100) = LTRIM(RTRIM(@structdCode));
	DECLARE @tstructdLvl INT = ISNULL(@structdLvl, 0);
	DECLARE @tstructCode varchar(100) = LTRIM(RTRIM(@structCode));
	DECLARE @tstructLvl INT = ISNULL(@structLvl, 0);
	DECLARE @tstructId INT = 0;
	SELECT @tstructId = structureid from structure where code = @tstructCode


	DECLARE @tstructDataId INT = 0;
	SELECT @tstructDataId = d.structuredataid from structuredata  as d
	inner join structure as s on s.StructureId = d.StructureId
	where d.StructureLevel = @tstructdLvl and s.Code = @tstructdCode

	IF(@tstructLvl <= 0) RETURN;
	IF(@tstructId <= 0) RETURN;
	IF(@tstructDataId <= 0) RETURN;

	DECLARE @datatypeStructId INT;
	SELECT @datatypeStructId = [DataTypeId] FROM [dbo].[DataType] WHERE [Code] = 'Structure';

	INSERT INTO [dbo].[StructureRequirement] VALUES (@tstructDataId, @datatypeStructId, @tstructId, @tstructLvl)
END
GO

DELETE FROM [dbo].[StructureRequirement]
GO

DELETE FROM [dbo].[StructureLocation]
GO

DECLARE @CityCounsilCode VARCHAR(100) = 'CityCounsil';
DECLARE @GateCode VARCHAR(100) = 'Gate';
DECLARE @AcadamyCode VARCHAR(100) = 'Acadamy';
DECLARE @BarracksCode VARCHAR(100) = 'Barracks';
DECLARE @BlacksmithCode VARCHAR(100) = 'Blacksmith';
DECLARE @EmbassyCode VARCHAR(100) = 'Embassy';
DECLARE @InfantryCampCode VARCHAR(100) = 'InfantryCamp';
DECLARE @InfirmaryCode VARCHAR(100) = 'Infirmary';
DECLARE @MarketCode VARCHAR(100) = 'Market';
DECLARE @MineCode VARCHAR(100) = 'Mine';
DECLARE @FarmCode VARCHAR(100) = 'Farm';
DECLARE @SawmillCode VARCHAR(100) = 'Sawmill';
DECLARE @ShootingRangeCode VARCHAR(100) = 'ShootingRange';
DECLARE @StableCode VARCHAR(100) = 'Stable';
DECLARE @TrainingHeroesCode VARCHAR(100) = 'TrainingHeroes';
DECLARE @WarehouseCode VARCHAR(100) = 'Warehouse';
DECLARE @WatchTowerCode VARCHAR(100) = 'WatchTower';
DECLARE @WorkShopCode VARCHAR(100) = 'WorkShop';


--*******************************RESOURCE REQ *******************************************

EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 2, 2800, 2800, 0
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 3, 4000, 4000, 0
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 4, 6000, 6000, 0
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 5, 10000, 10000, 0
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 6, 17000, 17000, 0
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 7, 35000, 35000, 0
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 8, 65000, 65000, 0
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 9, 129000, 129000, 0
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 10, 250000, 250000, 0
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 11, 4000000, 4000000, 4000
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 12, 8000000, 8000000, 8000
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 13, 1530000, 1530000, 15300
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 14, 27500000, 2750000, 27500
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 15, 4200000, 4200000, 420000
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 16, 5500000, 5500000, 550000
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 17, 7700000, 7700000, 770000
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 18, 10000000, 10000000, 1000000
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 19, 13000000, 13000000, 1300000
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 20, 16000000, 16000000, 1600000
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 21, 20000000, 20000000, 2000000
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 22, 20000000, 20000000, 2000000
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 23, 30000000, 30000000, 3000000
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 24, 30000000, 30000000, 3000000
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 25, 55000000, 55000000, 5500000
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 26, 75000000, 75000000, 7500000	
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 27, 100000000, 100000000, 10000000
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 28, 130000000, 130000000, 13000000
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 29, 170000000, 170000000, 17000000 
EXEC [dbo].[TempAddReqRes] @CityCounsilCode, 30, 220000000, 220000000, 22000000

--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 2, @SawmillCode, 1
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 2, @FarmCode, 1
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 2, @WareHouseCode, 1

--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 3, @Gate, 2
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 3, @Sawmill, 1
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 3, @c, 3
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 3, @c, 3
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 3, @c, 3
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 3, @c, 3
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 3, @c, 3
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 3, @c, 3


--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 4, @c, 4
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 5, @c, 5
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 6, @c, 6
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 7, @c, 7
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 8, @c, 8
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 9, @c, 9
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 10, @c, 10
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 11, @c, 11
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 12, @c, 12
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 13, @c, 13
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 14, @c, 14
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 15, @c, 15
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 16, @c, 16
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 17, @c, 17
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 18, @c, 18
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 19, @c, 19
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 20, @c, 20
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 21, @c, 21
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 22, @c, 22
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 23, @c, 23
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 24, @c, 24
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 25, @c, 25
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 26, @c, 26
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 27, @c, 27
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 28, @c, 28
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 29, @c, 29
--EXEC [dbo].[TempAddReqStruct] @CityCounsilCode, 30, @c, 30
----***********************************************************************************
----***********************************************************************************
--******************GATE****************************************
----***********************************************************************************
----***********************************************************************************
EXEC [dbo].[TempAddReqRes] @GateCode, 2, 0, 2500, 0
EXEC [dbo].[TempAddReqRes] @GateCode, 3, 0, 3500, 0
EXEC [dbo].[TempAddReqRes] @GateCode, 4, 0, 5000, 0
EXEC [dbo].[TempAddReqRes] @GateCode, 5, 0, 8000, 0
EXEC [dbo].[TempAddReqRes] @GateCode, 6, 0, 28000, 0
EXEC [dbo].[TempAddReqRes] @GateCode, 7, 0, 52000, 0
EXEC [dbo].[TempAddReqRes] @GateCode, 8, 0, 103500, 0
EXEC [dbo].[TempAddReqRes] @GateCode, 9, 0, 200000, 0
EXEC [dbo].[TempAddReqRes] @GateCode, 10, 0, 320000, 12800
EXEC [dbo].[TempAddReqRes] @GateCode, 11, 0, 640000, 25600
EXEC [dbo].[TempAddReqRes] @GateCode, 12, 0, 1224000, 48900
EXEC [dbo].[TempAddReqRes] @GateCode, 13, 0, 1224000, 48900
EXEC [dbo].[TempAddReqRes] @GateCode, 14, 0, 2200000, 88000 
EXEC [dbo].[TempAddReqRes] @GateCode, 15, 0, 3360000, 134400 
EXEC [dbo].[TempAddReqRes] @GateCode, 16, 0, 4400000, 176000 
EXEC [dbo].[TempAddReqRes] @GateCode, 17, 0, 616000, 246400 
EXEC [dbo].[TempAddReqRes] @GateCode, 18, 0, 8000000, 320000
EXEC [dbo].[TempAddReqRes] @GateCode, 19, 0, 10400000, 416000
EXEC [dbo].[TempAddReqRes] @GateCode, 20, 0, 12800000, 512000 
EXEC [dbo].[TempAddReqRes] @GateCode, 21, 0, 16000000, 640000
EXEC [dbo].[TempAddReqRes] @GateCode, 22, 0, 20000000, 800000
EXEC [dbo].[TempAddReqRes] @GateCode, 23, 0, 24000000, 960000
EXEC [dbo].[TempAddReqRes] @GateCode, 24, 0, 32000000, 1280000
EXEC [dbo].[TempAddReqRes] @GateCode, 25, 0, 44000000, 1760000 
EXEC [dbo].[TempAddReqRes] @GateCode, 26, 0, 60000000, 2400000 
EXEC [dbo].[TempAddReqRes] @GateCode, 27, 0, 80000000, 3200000
EXEC [dbo].[TempAddReqRes] @GateCode, 28, 0, 104000000, 4160000
EXEC [dbo].[TempAddReqRes] @GateCode, 29, 0, 136000000, 5440000
EXEC [dbo].[TempAddReqRes] @GateCode, 30, 0, 167000000, 7040000

EXEC [dbo].[TempAddReqStruct] @GateCode, 2, @CityCounsilCode, 2
EXEC [dbo].[TempAddReqStruct] @GateCode, 3, @CityCounsilCode, 3
EXEC [dbo].[TempAddReqStruct] @GateCode, 4, @CityCounsilCode, 4
EXEC [dbo].[TempAddReqStruct] @GateCode, 5, @CityCounsilCode, 5
EXEC [dbo].[TempAddReqStruct] @GateCode, 6, @CityCounsilCode, 6
EXEC [dbo].[TempAddReqStruct] @GateCode, 7, @CityCounsilCode, 7
EXEC [dbo].[TempAddReqStruct] @GateCode, 8, @CityCounsilCode, 8
EXEC [dbo].[TempAddReqStruct] @GateCode, 9, @CityCounsilCode, 9
EXEC [dbo].[TempAddReqStruct] @GateCode, 10, @CityCounsilCode, 10
EXEC [dbo].[TempAddReqStruct] @GateCode, 11, @CityCounsilCode, 11
EXEC [dbo].[TempAddReqStruct] @GateCode, 12, @CityCounsilCode, 12
EXEC [dbo].[TempAddReqStruct] @GateCode, 13, @CityCounsilCode, 13
EXEC [dbo].[TempAddReqStruct] @GateCode, 14, @CityCounsilCode, 14
EXEC [dbo].[TempAddReqStruct] @GateCode, 15, @CityCounsilCode, 15
EXEC [dbo].[TempAddReqStruct] @GateCode, 16, @CityCounsilCode, 16
EXEC [dbo].[TempAddReqStruct] @GateCode, 17, @CityCounsilCode, 17
EXEC [dbo].[TempAddReqStruct] @GateCode, 18, @CityCounsilCode, 18
EXEC [dbo].[TempAddReqStruct] @GateCode, 19, @CityCounsilCode, 19
EXEC [dbo].[TempAddReqStruct] @GateCode, 20, @CityCounsilCode, 20
EXEC [dbo].[TempAddReqStruct] @GateCode, 21, @CityCounsilCode, 21
EXEC [dbo].[TempAddReqStruct] @GateCode, 22, @CityCounsilCode, 22
EXEC [dbo].[TempAddReqStruct] @GateCode, 23, @CityCounsilCode, 23
EXEC [dbo].[TempAddReqStruct] @GateCode, 24, @CityCounsilCode, 24
EXEC [dbo].[TempAddReqStruct] @GateCode, 25, @CityCounsilCode, 25
EXEC [dbo].[TempAddReqStruct] @GateCode, 26, @CityCounsilCode, 26
EXEC [dbo].[TempAddReqStruct] @GateCode, 27, @CityCounsilCode, 27
EXEC [dbo].[TempAddReqStruct] @GateCode, 28, @CityCounsilCode, 28
EXEC [dbo].[TempAddReqStruct] @GateCode, 29, @CityCounsilCode, 29
EXEC [dbo].[TempAddReqStruct] @GateCode, 30, @CityCounsilCode, 30
--***********************************************************************************
--***********************************************************************************
--********************************Sawmill***************************************************
--***********************************************************************************
EXEC [dbo].[TempAddReqRes] @SawmillCode, 1, 100, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 2, 200, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 3, 300, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 4, 400, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 5, 600, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 6, 900, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 7, 1400, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 8, 2000, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 9, 2900, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 10, 4500, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 11, 8700, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 12, 20100, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 13, 44500, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 14, 90300,  0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 15, 188700, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 16, 336900, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 17, 600500, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 18, 1080900, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 19, 1700400, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 20, 2550600, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 21, 3800100, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 22, 5000300, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 23, 6400000, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 24, 8000000, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 25, 1000000, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 26, 1250000, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 27, 1465000, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 28, 1800200, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 29, 2200000, 0, 0
EXEC [dbo].[TempAddReqRes] @SawmillCode, 30, 2500000, 0, 0


EXEC [dbo].[TempAddReqStruct] @SawmillCode, 1, @CityCounsilCode, 1
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 2, @CityCounsilCode, 2
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 3, @CityCounsilCode, 3
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 4, @CityCounsilCode, 4
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 5, @CityCounsilCode, 5
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 6, @CityCounsilCode, 6
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 7, @CityCounsilCode, 7
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 8, @CityCounsilCode, 8
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 9, @CityCounsilCode, 9
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 10, @CityCounsilCode, 10
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 11, @CityCounsilCode, 11
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 12, @CityCounsilCode, 12
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 13, @CityCounsilCode, 13
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 14, @CityCounsilCode, 14
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 15, @CityCounsilCode, 15
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 16, @CityCounsilCode, 16
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 17, @CityCounsilCode, 17
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 18, @CityCounsilCode, 18
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 19, @CityCounsilCode, 19
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 20, @CityCounsilCode, 20
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 21, @CityCounsilCode, 21
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 22, @CityCounsilCode, 22
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 23, @CityCounsilCode, 23
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 24, @CityCounsilCode, 24
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 25, @CityCounsilCode, 25
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 26, @CityCounsilCode, 26
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 27, @CityCounsilCode, 27
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 28, @CityCounsilCode, 28
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 29, @CityCounsilCode, 29
EXEC [dbo].[TempAddReqStruct] @SawmillCode, 30, @CityCounsilCode, 30
----***********************************************************************************
----***********************************************************************************

--**********************************FARM*****************************
--***********************************************************************************
EXEC [dbo].[TempAddReqRes] @FarmCode, 1, 0, 100, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 2, 0, 200, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 3, 0, 300, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 4, 0, 400, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 5, 0, 600, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 6, 0, 900, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 7, 0, 1400, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 8, 0, 2000, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 9, 0, 2900, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 10, 0, 4500, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 11, 0, 8700, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 12, 0, 20100, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 13, 0, 44500, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 14, 0, 90300, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 15, 0, 188700, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 16, 0, 336900, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 17, 0, 600500, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 18, 0, 1080900, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 19, 0, 1700400, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 20, 0, 2550600, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 21, 0, 3800100, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 22, 0, 5000300, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 23, 0, 6400000, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 24, 0, 8000000, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 25, 0, 1000000, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 26, 0, 1250000, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 27, 0, 1465000, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 28, 0, 1800200, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 29, 0, 2200000, 0
EXEC [dbo].[TempAddReqRes] @FarmCode, 30, 0, 2500000, 0

EXEC [dbo].[TempAddReqStruct] @FarmCode, 1, @CityCounsilCode, 1
EXEC [dbo].[TempAddReqStruct] @FarmCode, 2, @CityCounsilCode, 2
EXEC [dbo].[TempAddReqStruct] @FarmCode, 3, @CityCounsilCode, 3
EXEC [dbo].[TempAddReqStruct] @FarmCode, 4, @CityCounsilCode, 4
EXEC [dbo].[TempAddReqStruct] @FarmCode, 5, @CityCounsilCode, 5
EXEC [dbo].[TempAddReqStruct] @FarmCode, 6, @CityCounsilCode, 6
EXEC [dbo].[TempAddReqStruct] @FarmCode, 7, @CityCounsilCode, 7
EXEC [dbo].[TempAddReqStruct] @FarmCode, 8, @CityCounsilCode, 8
EXEC [dbo].[TempAddReqStruct] @FarmCode, 9, @CityCounsilCode, 9
EXEC [dbo].[TempAddReqStruct] @FarmCode, 10, @CityCounsilCode, 10
EXEC [dbo].[TempAddReqStruct] @FarmCode, 11, @CityCounsilCode, 11
EXEC [dbo].[TempAddReqStruct] @FarmCode, 12, @CityCounsilCode, 12
EXEC [dbo].[TempAddReqStruct] @FarmCode, 13, @CityCounsilCode, 13
EXEC [dbo].[TempAddReqStruct] @FarmCode, 14, @CityCounsilCode, 14
EXEC [dbo].[TempAddReqStruct] @FarmCode, 15, @CityCounsilCode, 15
EXEC [dbo].[TempAddReqStruct] @FarmCode, 16, @CityCounsilCode, 16
EXEC [dbo].[TempAddReqStruct] @FarmCode, 17, @CityCounsilCode, 17
EXEC [dbo].[TempAddReqStruct] @FarmCode, 18, @CityCounsilCode, 18
EXEC [dbo].[TempAddReqStruct] @FarmCode, 19, @CityCounsilCode, 19
EXEC [dbo].[TempAddReqStruct] @FarmCode, 20, @CityCounsilCode, 20
EXEC [dbo].[TempAddReqStruct] @FarmCode, 21, @CityCounsilCode, 21
EXEC [dbo].[TempAddReqStruct] @FarmCode, 22, @CityCounsilCode, 22
EXEC [dbo].[TempAddReqStruct] @FarmCode, 23, @CityCounsilCode, 23
EXEC [dbo].[TempAddReqStruct] @FarmCode, 24, @CityCounsilCode, 24
EXEC [dbo].[TempAddReqStruct] @FarmCode, 25, @CityCounsilCode, 25
EXEC [dbo].[TempAddReqStruct] @FarmCode, 26, @CityCounsilCode, 26
EXEC [dbo].[TempAddReqStruct] @FarmCode, 27, @CityCounsilCode, 27
EXEC [dbo].[TempAddReqStruct] @FarmCode, 28, @CityCounsilCode, 28
EXEC [dbo].[TempAddReqStruct] @FarmCode, 29, @CityCounsilCode, 29
EXEC [dbo].[TempAddReqStruct] @FarmCode, 30, @CityCounsilCode, 30
--***********************************************************************************
--***********************************************************************************
--**********************************Mine*****************************
--***********************************************************************************
EXEC [dbo].[TempAddReqRes] @MineCode, 1, 100, 100, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 2, 200, 200, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 3, 300, 300, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 4, 400, 400, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 5, 600, 600, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 6, 900, 900, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 7, 1400, 1400, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 8, 2000, 2000, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 9, 2900, 2900, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 10, 4500, 4500, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 11, 8700, 8700, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 12, 20100, 20100, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 13, 44500, 44500, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 14, 90300, 90300, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 15, 188700, 188700, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 16, 336900, 336900, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 17, 600500, 600500, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 18, 1080900, 1080900, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 19, 1700400, 1700400, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 20, 2550600, 2550600, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 21, 3800100, 3800100, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 22, 5000300, 5000300, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 23, 6400000, 6400000, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 24, 8000000, 8000000, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 25, 1000000, 1000000, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 26, 1250000, 1250000, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 27, 1465000, 1465000, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 28, 1800200, 1800200, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 29, 2200000, 2200000, 0
EXEC [dbo].[TempAddReqRes] @MineCode, 30, 2500000, 2500000, 0

EXEC [dbo].[TempAddReqStruct] @MineCode, 1, @CityCounsilCode, 1
EXEC [dbo].[TempAddReqStruct] @MineCode, 2, @CityCounsilCode, 2
EXEC [dbo].[TempAddReqStruct] @MineCode, 3, @CityCounsilCode, 3
EXEC [dbo].[TempAddReqStruct] @MineCode, 4, @CityCounsilCode, 4
EXEC [dbo].[TempAddReqStruct] @MineCode, 5, @CityCounsilCode, 5
EXEC [dbo].[TempAddReqStruct] @MineCode, 6, @CityCounsilCode, 6
EXEC [dbo].[TempAddReqStruct] @MineCode, 7, @CityCounsilCode, 7
EXEC [dbo].[TempAddReqStruct] @MineCode, 8, @CityCounsilCode, 8
EXEC [dbo].[TempAddReqStruct] @MineCode, 9, @CityCounsilCode, 9
EXEC [dbo].[TempAddReqStruct] @MineCode, 10, @CityCounsilCode, 10
EXEC [dbo].[TempAddReqStruct] @MineCode, 11, @CityCounsilCode, 11
EXEC [dbo].[TempAddReqStruct] @MineCode, 12, @CityCounsilCode, 12
EXEC [dbo].[TempAddReqStruct] @MineCode, 13, @CityCounsilCode, 13
EXEC [dbo].[TempAddReqStruct] @MineCode, 14, @CityCounsilCode, 14
EXEC [dbo].[TempAddReqStruct] @MineCode, 15, @CityCounsilCode, 15
EXEC [dbo].[TempAddReqStruct] @MineCode, 16, @CityCounsilCode, 16
EXEC [dbo].[TempAddReqStruct] @MineCode, 17, @CityCounsilCode, 17
EXEC [dbo].[TempAddReqStruct] @MineCode, 18, @CityCounsilCode, 18
EXEC [dbo].[TempAddReqStruct] @MineCode, 19, @CityCounsilCode, 19
EXEC [dbo].[TempAddReqStruct] @MineCode, 20, @CityCounsilCode, 20
EXEC [dbo].[TempAddReqStruct] @MineCode, 21, @CityCounsilCode, 21
EXEC [dbo].[TempAddReqStruct] @MineCode, 22, @CityCounsilCode, 22
EXEC [dbo].[TempAddReqStruct] @MineCode, 23, @CityCounsilCode, 23
EXEC [dbo].[TempAddReqStruct] @MineCode, 24, @CityCounsilCode, 24
EXEC [dbo].[TempAddReqStruct] @MineCode, 25, @CityCounsilCode, 25
EXEC [dbo].[TempAddReqStruct] @MineCode, 26, @CityCounsilCode, 26
EXEC [dbo].[TempAddReqStruct] @MineCode, 27, @CityCounsilCode, 27
EXEC [dbo].[TempAddReqStruct] @MineCode, 28, @CityCounsilCode, 28
EXEC [dbo].[TempAddReqStruct] @MineCode, 29, @CityCounsilCode, 29
EXEC [dbo].[TempAddReqStruct] @MineCode, 30, @CityCounsilCode, 30
--***********************************************************************************
--***********************************************************************************
--**********************************InfantryCamp*****************************
--***********************************************************************************
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 1, 0, 100, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 2, 0, 300, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 3, 0, 600, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 4, 0, 1000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 5, 0, 5000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 6, 0, 15000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 7, 0, 28000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 8, 0, 41000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 9, 0, 54000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 10, 0, 67000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 11, 0, 80000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 12, 0, 93000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 13, 0, 106000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 14, 0, 119000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 15, 0, 132000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 16, 0, 145000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 17, 0, 158000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 18, 0, 171000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 19, 0, 184000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 20, 0, 197000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 21, 0, 210000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 22, 0, 223000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 23, 0, 236000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 24, 0, 249000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 25, 0, 262000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 26, 0, 275000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 27, 0, 288000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 28, 0, 301000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 29, 0, 314000, 0
EXEC [dbo].[TempAddReqRes] @InfantryCampCode, 30, 0, 327000, 0

EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 1, @CityCounsilCode, 1
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 2, @CityCounsilCode, 2
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 3, @CityCounsilCode, 3
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 4, @CityCounsilCode, 4
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 5, @CityCounsilCode, 5
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 6, @CityCounsilCode, 6
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 7, @CityCounsilCode, 7
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 8, @CityCounsilCode, 8
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 9, @CityCounsilCode, 9
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 10, @CityCounsilCode, 10
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 11, @CityCounsilCode, 11
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 12, @CityCounsilCode, 12
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 13, @CityCounsilCode, 13
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 14, @CityCounsilCode, 14
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 15, @CityCounsilCode, 15
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 16, @CityCounsilCode, 16
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 17, @CityCounsilCode, 17
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 18, @CityCounsilCode, 18
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 19, @CityCounsilCode, 19
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 20, @CityCounsilCode, 20
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 21, @CityCounsilCode, 21
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 22, @CityCounsilCode, 22
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 23, @CityCounsilCode, 23
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 24, @CityCounsilCode, 24
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 25, @CityCounsilCode, 25
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 26, @CityCounsilCode, 26
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 27, @CityCounsilCode, 27
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 28, @CityCounsilCode, 28
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 29, @CityCounsilCode, 29
EXEC [dbo].[TempAddReqStruct] @InfantryCampCode, 30, @CityCounsilCode, 30
--***********************************************************************************
--***********************************************************************************
----**********************************Acadamy*****************************
----***********************************************************************************
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 1, 500, 500, 0
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 2, 1000, 1400, 0
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 3, 1300, 1900, 0
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 4, 1700, 2600, 0
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 5, 2500, 4000, 0
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 6, 3900, 6400, 0
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 7, 7000, 12000, 0
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 8, 13000, 23000, 0
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 9, 26000, 45000, 0
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 10, 50000, 88000, 0
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 11, 80000, 140000, 14000 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 12, 160000, 280000, 28000 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 13, 306000, 536000, 53600 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 14, 550000, 963000, 96300 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 15, 840000, 1470000, 147000 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 16, 1100000, 1925000, 192500 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 17, 1540000, 2695000, 269500 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 18, 2000000, 3500000, 350000 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 19, 2600000, 4550000, 455000 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 20, 3200000, 5600000, 560000 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 21, 4000000, 7000000, 700000 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 22, 5000000, 8750000, 875000 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 23, 6000000, 10500000, 1050000 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 24, 8000000, 14000000, 1400000 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 25, 11000000, 19250000, 1925000 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 26, 15000000, 26250000, 2625000 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 27, 20000000, 35000000, 3500000 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 28, 26000000, 45500000, 4550000 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 29, 34000000, 59500000, 5950000 
EXEC [dbo].[TempAddReqRes] @AcadamyCode, 30, 44000000, 77000000, 7700000

EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 1, @CityCounsilCode, 1
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 2, @CityCounsilCode, 2
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 3, @CityCounsilCode, 3
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 4, @CityCounsilCode, 4
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 5, @CityCounsilCode, 5
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 6, @CityCounsilCode, 6
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 7, @CityCounsilCode, 7
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 8, @CityCounsilCode, 8
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 9, @CityCounsilCode, 9
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 10, @CityCounsilCode, 10
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 11, @CityCounsilCode, 11
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 12, @CityCounsilCode, 12
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 13, @CityCounsilCode, 13
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 14, @CityCounsilCode, 14
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 15, @CityCounsilCode, 15
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 16, @CityCounsilCode, 16
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 17, @CityCounsilCode, 17
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 18, @CityCounsilCode, 18
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 19, @CityCounsilCode, 19
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 20, @CityCounsilCode, 20
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 21, @CityCounsilCode, 21
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 22, @CityCounsilCode, 22
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 23, @CityCounsilCode, 23
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 24, @CityCounsilCode, 24
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 25, @CityCounsilCode, 25
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 26, @CityCounsilCode, 26
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 27, @CityCounsilCode, 27
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 28, @CityCounsilCode, 28
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 29, @CityCounsilCode, 29
EXEC [dbo].[TempAddReqStruct] @AcadamyCode, 30, @CityCounsilCode, 30
----***********************************************************************************
----***********************************************************************************
--**********************************FARM*****************************
--***********************************************************************************
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 2, 800, 800, 0
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 3, 1500, 1500, 0
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 4, 2800, 2800, 0
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 5, 5000, 5000, 0
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 6, 8500, 8500, 1300
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 7, 8500, 8500, 2000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 8, 8500, 8500, 3200
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 9, 29000, 29000, 5200
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 10, 43500, 43500, 8200
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 11, 67500, 67500, 12500
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 12, 102500, 102500, 20000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 13, 155000, 155000, 30000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 14, 232500, 232500, 45000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 15, 350000, 350000, 67500
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 16, 525000, 525000, 102500
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 17, 787500, 787500, 155000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 18, 1200000, 1200000, 250000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 19, 1800000, 1800000, 375000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 20, 2700000, 2700000, 575000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 21, 4100000, 4100000, 875000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 22, 5300000, 5300000, 900000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 23, 6100000, 6100000, 1300000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 24, 7400000, 7400000, 1370000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 25, 8900000, 8900000, 1480000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 26, 9400000, 9400000, 1660000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 27, 10000000, 10000000, 1940000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 28, 12040000, 12040000, 20020000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 29, 13000000, 13000000, 23005000
EXEC [dbo].[TempAddReqRes] @WatchTowerCode, 30, 15000000, 15000000, 25000000

EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 1, @FarmCode, 1
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 2, @FarmCode, 2
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 3, @FarmCode, 3
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 4, @FarmCode, 4
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 5, @FarmCode, 5
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 6, @FarmCode, 6
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 7, @FarmCode, 7
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 8, @FarmCode, 8
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 9, @FarmCode, 9
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 10, @FarmCode, 10
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 11, @FarmCode, 11
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 12, @FarmCode, 12
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 13, @FarmCode, 13
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 14, @FarmCode, 14
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 15, @FarmCode, 15
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 16, @FarmCode, 16
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 17, @FarmCode, 17
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 18, @FarmCode, 18
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 19, @FarmCode, 19
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 20, @FarmCode, 20
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 21, @FarmCode, 21
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 22, @FarmCode, 22
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 23, @FarmCode, 23
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 24, @FarmCode, 24
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 25, @FarmCode, 25
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 26, @FarmCode, 26
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 27, @FarmCode, 27
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 28, @FarmCode, 28
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 29, @FarmCode, 29
EXEC [dbo].[TempAddReqStruct] @WatchTowerCode, 30, @FarmCode, 30
--***********************************************************************************
--***********************************************************************************
--**********************************Warehouse*****************************
--***********************************************************************************
--EXEC [dbo].[TempAddReqRes] @WarehouseCode, 2, 0, 0, 0
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 2, 0, 200, 0
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 3, 0, 400, 0
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 4, 0, 600, 0
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 5, 0, 800, 0
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 6, 0, 1600, 0
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 7, 0, 3200, 0
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 8, 0, 6400, 0
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 9, 0, 10500, 0
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 10, 0, 22100, 2210 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 11, 0, 37000, 3700 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 12, 0, 49000, 4900 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 13, 0, 62500, 6250 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 14, 0, 90300, 9030 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 15, 0, 188700, 18870 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 16, 0, 336900, 33690 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 17, 0, 600500, 60050 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 18, 0, 1080900, 108090 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 19, 0, 1700400, 170040 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 20, 0, 2550600, 255060 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 21, 0, 3800100, 380010 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 22, 0, 5000300, 500030 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 23, 0, 6400000, 640000 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 24, 0, 8000000, 800000 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 25, 0, 10000000, 1000000 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 26, 0, 12500000, 1250000 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 27, 0, 14650000, 1465000 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 28, 0, 18002000, 1800200 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 29, 0, 22000000, 2200000 
EXEC [dbo].[TempAddReqRes] @WarehouseCode, 30, 0, 25000000, 2500000 


--EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 1, @CityCounsilCode, 1
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 2, @CityCounsilCode, 2
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 3, @CityCounsilCode, 3
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 4, @CityCounsilCode, 4
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 5, @CityCounsilCode, 5
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 6, @CityCounsilCode, 6
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 7, @CityCounsilCode, 7
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 8, @CityCounsilCode, 8
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 9, @CityCounsilCode, 9
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 10, @CityCounsilCode, 10
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 11, @CityCounsilCode, 11
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 12, @CityCounsilCode, 12
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 13, @CityCounsilCode, 13
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 14, @CityCounsilCode, 14
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 15, @CityCounsilCode, 15
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 16, @CityCounsilCode, 16
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 17, @CityCounsilCode, 17
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 18, @CityCounsilCode, 18
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 19, @CityCounsilCode, 19
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 20, @CityCounsilCode, 20
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 21, @CityCounsilCode, 21
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 22, @CityCounsilCode, 22
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 23, @CityCounsilCode, 23
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 24, @CityCounsilCode, 24
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 25, @CityCounsilCode, 25
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 26, @CityCounsilCode, 26
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 27, @CityCounsilCode, 27
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 28, @CityCounsilCode, 28
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 29, @CityCounsilCode, 29
EXEC [dbo].[TempAddReqStruct] @WarehouseCode, 30, @CityCounsilCode, 30
--***********************************************************************************
--***********************************************************************************
--**********************************BLACKSMITH*****************************
--***********************************************************************************
EXEC [dbo].[TempAddReqRes] @BlacksmithCode, 1, 80000, 80000, 8000
EXEC [dbo].[TempAddReqStruct] @BlacksmithCode, 1, @CityCounsilCode, 10
--***********************************************************************************
--***********************************************************************************
--**********************************TrainingHeroes*****************************
--***********************************************************************************
EXEC [dbo].[TempAddReqRes] @TrainingHeroesCode, 1, 80000, 80000, 8000
EXEC [dbo].[TempAddReqStruct] @TrainingHeroesCode, 1, @CityCounsilCode, 15
--***********************************************************************************
--***********************************************************************************
--**********************************MarketHeroes*****************************
--***********************************************************************************
EXEC [dbo].[TempAddReqRes] @MarketCode, 1, 80000, 80000, 8000
EXEC [dbo].[TempAddReqStruct] @MarketCode, 1, @CityCounsilCode, 14
--***********************************************************************************
--***********************************************************************************
--**********************************Embassy*****************************
--***********************************************************************************
EXEC [dbo].[TempAddReqRes] @EmbassyCode, 1, 8000, 8000, 0
EXEC [dbo].[TempAddReqStruct] @EmbassyCode, 1, @CityCounsilCode, 5
--***********************************************************************************
--***********************************************************************************
--**************************************InfirmaryCode*********************************************
--***********************************************************************************
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 1, 0, 100, 0
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 2, 0, 200, 0
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 3, 0, 400, 0
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 4, 0, 600, 0
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 5, 0, 800, 0
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 6, 0, 1600, 0
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 7, 0, 3200, 0
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 8, 0, 6400, 0
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 9, 0, 10500, 0
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 10, 0, 22100, 2210  
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 11, 0, 37000, 3700  
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 12, 0, 49000, 4900  
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 13, 0, 62500, 6250  
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 14, 0, 90300, 9030  
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 15, 0, 188700, 18870  
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 16, 0, 336900, 33690  
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 17, 0, 600500, 60050 
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 18, 0, 1080900, 108090  
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 19, 0, 1700400, 170040 
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 20, 0, 2550600, 255060 
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 21, 0, 3800100, 380010 
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 22, 0, 5000300, 500030 
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 23, 0, 6400000, 640000 
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 24, 0, 8000000, 800000 
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 25, 0, 10000000, 1000000  
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 26, 0, 12500000, 1250000 
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 27, 0, 14650000, 1465000 
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 28, 0, 18002000, 1800200 
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 29, 0, 22000000, 2200000 
EXEC [dbo].[TempAddReqRes] @InfirmaryCode, 30, 0, 25000000, 2500000  

EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 1, @InfantryCampCode, 1
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 2, @InfantryCampCode, 2
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 3, @InfantryCampCode, 3
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 4, @InfantryCampCode, 4
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 5, @InfantryCampCode, 5
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 6, @InfantryCampCode, 6
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 7, @InfantryCampCode, 7
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 8, @InfantryCampCode, 8
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 9, @InfantryCampCode, 9
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 10, @InfantryCampCode, 10
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 11, @InfantryCampCode, 11
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 12, @InfantryCampCode, 12
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 13, @InfantryCampCode, 13
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 14, @InfantryCampCode, 14
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 15, @InfantryCampCode, 15
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 16, @InfantryCampCode, 16
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 17, @InfantryCampCode, 17
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 18, @InfantryCampCode, 18
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 19, @InfantryCampCode, 19
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 20, @InfantryCampCode, 20
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 21, @InfantryCampCode, 21
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 22, @InfantryCampCode, 22
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 23, @InfantryCampCode, 23
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 24, @InfantryCampCode, 24
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 25, @InfantryCampCode, 25
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 26, @InfantryCampCode, 26
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 27, @InfantryCampCode, 27
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 28, @InfantryCampCode, 28
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 29, @InfantryCampCode, 29
EXEC [dbo].[TempAddReqStruct] @InfirmaryCode, 30, @InfantryCampCode, 30
--***********************************************************************************
--***********************************************************************************
--**********************************Barracks*****************************
--***********************************************************************************
EXEC [dbo].[TempAddReqRes] @BarracksCode, 1, 500, 500, 0
EXEC [dbo].[TempAddReqRes] @BarracksCode, 2, 1000, 1400, 0
EXEC [dbo].[TempAddReqRes] @BarracksCode, 3, 1300, 1900, 0
EXEC [dbo].[TempAddReqRes] @BarracksCode, 4, 1700, 2600, 0
EXEC [dbo].[TempAddReqRes] @BarracksCode, 5, 2500, 4000, 0
EXEC [dbo].[TempAddReqRes] @BarracksCode, 6, 3900, 6400, 0
EXEC [dbo].[TempAddReqRes] @BarracksCode, 7, 7000, 12000, 0
EXEC [dbo].[TempAddReqRes] @BarracksCode, 8, 13000, 23000, 0
EXEC [dbo].[TempAddReqRes] @BarracksCode, 9, 26000, 45000, 0
EXEC [dbo].[TempAddReqRes] @BarracksCode, 10, 50000, 88000, 0
EXEC [dbo].[TempAddReqRes] @BarracksCode, 11, 80000, 140000, 14000
EXEC [dbo].[TempAddReqRes] @BarracksCode, 12, 160000, 280000, 28000
EXEC [dbo].[TempAddReqRes] @BarracksCode, 13, 306000, 536000, 53600
EXEC [dbo].[TempAddReqRes] @BarracksCode, 14, 550000, 963000, 96300
EXEC [dbo].[TempAddReqRes] @BarracksCode, 15, 840000, 1470000, 147000
EXEC [dbo].[TempAddReqRes] @BarracksCode, 16, 1100000, 1925000, 192500		
EXEC [dbo].[TempAddReqRes] @BarracksCode, 17, 1540000, 2695000, 269500		
EXEC [dbo].[TempAddReqRes] @BarracksCode, 18, 2000000, 3500000, 350000
EXEC [dbo].[TempAddReqRes] @BarracksCode, 19, 2600000, 4550000, 455000
EXEC [dbo].[TempAddReqRes] @BarracksCode, 20, 3200000, 5600000, 560000
EXEC [dbo].[TempAddReqRes] @BarracksCode, 21, 4000000, 7000000, 700000
EXEC [dbo].[TempAddReqRes] @BarracksCode, 22, 5000000, 8750000, 875000		
EXEC [dbo].[TempAddReqRes] @BarracksCode, 23, 6000000, 10500000, 1050000		
EXEC [dbo].[TempAddReqRes] @BarracksCode, 24, 8000000, 14000000, 1400000
EXEC [dbo].[TempAddReqRes] @BarracksCode, 25, 11000000, 19250000, 1925000
EXEC [dbo].[TempAddReqRes] @BarracksCode, 26, 15000000, 26250000, 2625000		
EXEC [dbo].[TempAddReqRes] @BarracksCode, 27, 20000000, 35000000, 3500000		
EXEC [dbo].[TempAddReqRes] @BarracksCode, 28, 26000000, 45500000, 4550000
EXEC [dbo].[TempAddReqRes] @BarracksCode, 29, 34000000, 59500000, 5950000		
EXEC [dbo].[TempAddReqRes] @BarracksCode, 30, 44000000, 77000000, 7700000

EXEC [dbo].[TempAddReqStruct] @BarracksCode, 1, @CityCounsilCode, 1
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 2, @CityCounsilCode, 2
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 3, @CityCounsilCode, 3
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 4, @CityCounsilCode, 4
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 5, @CityCounsilCode, 5
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 6, @CityCounsilCode, 6
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 7, @CityCounsilCode, 7
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 8, @CityCounsilCode, 8
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 9, @CityCounsilCode, 9
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 10, @CityCounsilCode, 10
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 11, @CityCounsilCode, 11
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 12, @CityCounsilCode, 12
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 13, @CityCounsilCode, 13
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 14, @CityCounsilCode, 14
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 15, @CityCounsilCode, 15
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 16, @CityCounsilCode, 16
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 17, @CityCounsilCode, 17
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 18, @CityCounsilCode, 18
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 19, @CityCounsilCode, 19
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 20, @CityCounsilCode, 20
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 21, @CityCounsilCode, 21
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 22, @CityCounsilCode, 22
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 23, @CityCounsilCode, 23
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 24, @CityCounsilCode, 24
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 25, @CityCounsilCode, 25
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 26, @CityCounsilCode, 26
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 27, @CityCounsilCode, 27
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 28, @CityCounsilCode, 28
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 29, @CityCounsilCode, 29
EXEC [dbo].[TempAddReqStruct] @BarracksCode, 30, @CityCounsilCode, 30
--***********************************************************************************
--***********************************************************************************
--**********************************ShootingRange*****************************
--***********************************************************************************
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 1, 500, 500, 0
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 2, 1000, 1400, 0
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 3, 1300, 1900, 0
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 4, 1700, 2600, 0
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 5, 2500, 4000, 0
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 6, 3900, 6400, 0
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 7, 7000, 12000, 0
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 8, 13000, 23000, 0
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 9, 26000, 45000, 0
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 10, 50000, 88000, 0
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 11, 80000, 140000, 14000
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 12, 160000, 280000, 28000
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 13, 306000, 536000, 53600
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 14, 550000, 963000, 96300
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 15, 840000, 1470000, 147000
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 16, 1100000, 1925000, 192500		
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 17, 1540000, 2695000, 269500		
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 18, 2000000, 3500000, 350000
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 19, 2600000, 4550000, 455000
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 20, 3200000, 5600000, 560000
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 21, 4000000, 7000000, 700000
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 22, 5000000, 8750000, 875000		
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 23, 6000000, 10500000, 1050000		
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 24, 8000000, 14000000, 1400000
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 25, 11000000, 19250000, 1925000
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 26, 15000000, 26250000, 2625000		
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 27, 20000000, 35000000, 3500000		
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 28, 26000000, 45500000, 4550000
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 29, 34000000, 59500000, 5950000		
EXEC [dbo].[TempAddReqRes] @ShootingRangeCode, 30, 44000000, 77000000, 7700000

EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 1, @CityCounsilCode, 1
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 2, @CityCounsilCode, 2
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 3, @CityCounsilCode, 3
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 4, @CityCounsilCode, 4
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 5, @CityCounsilCode, 5
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 6, @CityCounsilCode, 6
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 7, @CityCounsilCode, 7
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 8, @CityCounsilCode, 8
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 9, @CityCounsilCode, 9
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 10, @CityCounsilCode, 10
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 11, @CityCounsilCode, 11
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 12, @CityCounsilCode, 12
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 13, @CityCounsilCode, 13
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 14, @CityCounsilCode, 14
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 15, @CityCounsilCode, 15
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 16, @CityCounsilCode, 16
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 17, @CityCounsilCode, 17
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 18, @CityCounsilCode, 18
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 19, @CityCounsilCode, 19
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 20, @CityCounsilCode, 20
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 21, @CityCounsilCode, 21
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 22, @CityCounsilCode, 22
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 23, @CityCounsilCode, 23
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 24, @CityCounsilCode, 24
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 25, @CityCounsilCode, 25
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 26, @CityCounsilCode, 26
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 27, @CityCounsilCode, 27
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 28, @CityCounsilCode, 28
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 29, @CityCounsilCode, 29
EXEC [dbo].[TempAddReqStruct] @ShootingRangeCode, 30, @CityCounsilCode, 30
--***********************************************************************************
--***********************************************************************************
--**********************************Stable*****************************
--***********************************************************************************
EXEC [dbo].[TempAddReqRes] @StableCode, 1, 500, 500, 0
EXEC [dbo].[TempAddReqRes] @StableCode, 2, 1000, 1400, 0
EXEC [dbo].[TempAddReqRes] @StableCode, 3, 1300, 1900, 0
EXEC [dbo].[TempAddReqRes] @StableCode, 4, 1700, 2600, 0
EXEC [dbo].[TempAddReqRes] @StableCode, 5, 2500, 4000, 0
EXEC [dbo].[TempAddReqRes] @StableCode, 6, 3900, 6400, 0
EXEC [dbo].[TempAddReqRes] @StableCode, 7, 7000, 12000, 0
EXEC [dbo].[TempAddReqRes] @StableCode, 8, 13000, 23000, 0
EXEC [dbo].[TempAddReqRes] @StableCode, 9, 26000, 45000, 0
EXEC [dbo].[TempAddReqRes] @StableCode, 10, 50000, 88000, 0
EXEC [dbo].[TempAddReqRes] @StableCode, 11, 80000, 140000, 14000
EXEC [dbo].[TempAddReqRes] @StableCode, 12, 160000, 280000, 28000
EXEC [dbo].[TempAddReqRes] @StableCode, 13, 306000, 536000, 53600
EXEC [dbo].[TempAddReqRes] @StableCode, 14, 550000, 963000, 96300
EXEC [dbo].[TempAddReqRes] @StableCode, 15, 840000, 1470000, 147000
EXEC [dbo].[TempAddReqRes] @StableCode, 16, 1100000, 1925000, 192500		
EXEC [dbo].[TempAddReqRes] @StableCode, 17, 1540000, 2695000, 269500		
EXEC [dbo].[TempAddReqRes] @StableCode, 18, 2000000, 3500000, 350000
EXEC [dbo].[TempAddReqRes] @StableCode, 19, 2600000, 4550000, 455000
EXEC [dbo].[TempAddReqRes] @StableCode, 20, 3200000, 5600000, 560000
EXEC [dbo].[TempAddReqRes] @StableCode, 21, 4000000, 7000000, 700000
EXEC [dbo].[TempAddReqRes] @StableCode, 22, 5000000, 8750000, 875000		
EXEC [dbo].[TempAddReqRes] @StableCode, 23, 6000000, 10500000, 1050000		
EXEC [dbo].[TempAddReqRes] @StableCode, 24, 8000000, 14000000, 1400000
EXEC [dbo].[TempAddReqRes] @StableCode, 25, 11000000, 19250000, 1925000
EXEC [dbo].[TempAddReqRes] @StableCode, 26, 15000000, 26250000, 2625000		
EXEC [dbo].[TempAddReqRes] @StableCode, 27, 20000000, 35000000, 3500000		
EXEC [dbo].[TempAddReqRes] @StableCode, 28, 26000000, 45500000, 4550000
EXEC [dbo].[TempAddReqRes] @StableCode, 29, 34000000, 59500000, 5950000		
EXEC [dbo].[TempAddReqRes] @StableCode, 30, 44000000, 77000000, 7700000

EXEC [dbo].[TempAddReqStruct] @StableCode, 1, @CityCounsilCode, 1
EXEC [dbo].[TempAddReqStruct] @StableCode, 2, @CityCounsilCode, 2
EXEC [dbo].[TempAddReqStruct] @StableCode, 3, @CityCounsilCode, 3
EXEC [dbo].[TempAddReqStruct] @StableCode, 4, @CityCounsilCode, 4
EXEC [dbo].[TempAddReqStruct] @StableCode, 5, @CityCounsilCode, 5
EXEC [dbo].[TempAddReqStruct] @StableCode, 6, @CityCounsilCode, 6
EXEC [dbo].[TempAddReqStruct] @StableCode, 7, @CityCounsilCode, 7
EXEC [dbo].[TempAddReqStruct] @StableCode, 8, @CityCounsilCode, 8
EXEC [dbo].[TempAddReqStruct] @StableCode, 9, @CityCounsilCode, 9
EXEC [dbo].[TempAddReqStruct] @StableCode, 10, @CityCounsilCode, 10
EXEC [dbo].[TempAddReqStruct] @StableCode, 11, @CityCounsilCode, 11
EXEC [dbo].[TempAddReqStruct] @StableCode, 12, @CityCounsilCode, 12
EXEC [dbo].[TempAddReqStruct] @StableCode, 13, @CityCounsilCode, 13
EXEC [dbo].[TempAddReqStruct] @StableCode, 14, @CityCounsilCode, 14
EXEC [dbo].[TempAddReqStruct] @StableCode, 15, @CityCounsilCode, 15
EXEC [dbo].[TempAddReqStruct] @StableCode, 16, @CityCounsilCode, 16
EXEC [dbo].[TempAddReqStruct] @StableCode, 17, @CityCounsilCode, 17
EXEC [dbo].[TempAddReqStruct] @StableCode, 18, @CityCounsilCode, 18
EXEC [dbo].[TempAddReqStruct] @StableCode, 19, @CityCounsilCode, 19
EXEC [dbo].[TempAddReqStruct] @StableCode, 20, @CityCounsilCode, 20
EXEC [dbo].[TempAddReqStruct] @StableCode, 21, @CityCounsilCode, 21
EXEC [dbo].[TempAddReqStruct] @StableCode, 22, @CityCounsilCode, 22
EXEC [dbo].[TempAddReqStruct] @StableCode, 23, @CityCounsilCode, 23
EXEC [dbo].[TempAddReqStruct] @StableCode, 24, @CityCounsilCode, 24
EXEC [dbo].[TempAddReqStruct] @StableCode, 25, @CityCounsilCode, 25
EXEC [dbo].[TempAddReqStruct] @StableCode, 26, @CityCounsilCode, 26
EXEC [dbo].[TempAddReqStruct] @StableCode, 27, @CityCounsilCode, 27
EXEC [dbo].[TempAddReqStruct] @StableCode, 28, @CityCounsilCode, 28
EXEC [dbo].[TempAddReqStruct] @StableCode, 29, @CityCounsilCode, 29
EXEC [dbo].[TempAddReqStruct] @StableCode, 30, @CityCounsilCode, 30
--***********************************************************************************
--***********************************************************************************
--**********************************WorkShop*****************************
--***********************************************************************************
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 1, 500, 500, 0
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 2, 1000, 1400, 0
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 3, 1300, 1900, 0
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 4, 1700, 2600, 0
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 5, 2500, 4000, 0
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 6, 3900, 6400, 0
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 7, 7000, 12000, 0
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 8, 13000, 23000, 0
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 9, 26000, 45000, 0
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 10, 50000, 88000, 0
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 11, 80000, 140000, 14000
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 12, 160000, 280000, 28000
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 13, 306000, 536000, 53600
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 14, 550000, 963000, 96300
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 15, 840000, 1470000, 147000
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 16, 1100000, 1925000, 192500		
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 17, 1540000, 2695000, 269500		
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 18, 2000000, 3500000, 350000
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 19, 2600000, 4550000, 455000
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 20, 3200000, 5600000, 560000
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 21, 4000000, 7000000, 700000
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 22, 5000000, 8750000, 875000		
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 23, 6000000, 10500000, 1050000		
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 24, 8000000, 14000000, 1400000
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 25, 11000000, 19250000, 1925000
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 26, 15000000, 26250000, 2625000		
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 27, 20000000, 35000000, 3500000		
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 28, 26000000, 45500000, 4550000
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 29, 34000000, 59500000, 5950000		
EXEC [dbo].[TempAddReqRes] @WorkShopCode, 30, 44000000, 77000000, 7700000


EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 1, @CityCounsilCode, 1
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 2, @CityCounsilCode, 2
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 3, @CityCounsilCode, 3
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 4, @CityCounsilCode, 4
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 5, @CityCounsilCode, 5
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 6, @CityCounsilCode, 6
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 7, @CityCounsilCode, 7
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 8, @CityCounsilCode, 8
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 9, @CityCounsilCode, 9
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 10, @CityCounsilCode, 10
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 11, @CityCounsilCode, 11
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 12, @CityCounsilCode, 12
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 13, @CityCounsilCode, 13
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 14, @CityCounsilCode, 14
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 15, @CityCounsilCode, 15
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 16, @CityCounsilCode, 16
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 17, @CityCounsilCode, 17
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 18, @CityCounsilCode, 18
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 19, @CityCounsilCode, 19
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 20, @CityCounsilCode, 20
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 21, @CityCounsilCode, 21
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 22, @CityCounsilCode, 22
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 23, @CityCounsilCode, 23
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 24, @CityCounsilCode, 24
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 25, @CityCounsilCode, 25
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 26, @CityCounsilCode, 26
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 27, @CityCounsilCode, 27
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 28, @CityCounsilCode, 28
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 29, @CityCounsilCode, 29
EXEC [dbo].[TempAddReqStruct] @WorkShopCode, 30, @CityCounsilCode, 30
--***********************************************************************************
--***********************************************************************************



























----**********************************FARM*****************************
----***********************************************************************************
--EXEC [dbo].[TempAddReqRes] @FarmCode, 2, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 3, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 4, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 5, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 6, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 7, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 8, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 9, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 10, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 11, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 12, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 13, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 14, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 15, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 16, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 17, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 18, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 19, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 20, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 21, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 22, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 23, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 24, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 25, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 26, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 27, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 28, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 29, 0, 0, 0
--EXEC [dbo].[TempAddReqRes] @FarmCode, 30, 0, 0, 0

--EXEC [dbo].[TempAddReqStruct] @FarmCode, 1, @CityCounsilCode, 1
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 2, @CityCounsilCode, 2
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 3, @CityCounsilCode, 3
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 4, @CityCounsilCode, 4
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 5, @CityCounsilCode, 5
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 6, @CityCounsilCode, 6
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 7, @CityCounsilCode, 7
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 8, @CityCounsilCode, 8
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 9, @CityCounsilCode, 9
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 10, @CityCounsilCode, 10
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 11, @CityCounsilCode, 11
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 12, @CityCounsilCode, 12
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 13, @CityCounsilCode, 13
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 14, @CityCounsilCode, 14
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 15, @CityCounsilCode, 15
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 16, @CityCounsilCode, 16
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 17, @CityCounsilCode, 17
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 18, @CityCounsilCode, 18
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 19, @CityCounsilCode, 19
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 20, @CityCounsilCode, 20
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 21, @CityCounsilCode, 21
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 22, @CityCounsilCode, 22
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 23, @CityCounsilCode, 23
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 24, @CityCounsilCode, 24
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 25, @CityCounsilCode, 25
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 26, @CityCounsilCode, 26
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 27, @CityCounsilCode, 27
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 28, @CityCounsilCode, 28
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 29, @CityCounsilCode, 29
--EXEC [dbo].[TempAddReqStruct] @FarmCode, 30, @CityCounsilCode, 30
----***********************************************************************************
----***********************************************************************************


GO
DROP PROCEDURE [dbo].[TempAddReqRes]

GO
DROP PROCEDURE  [dbo].[TempAddReqStruct]

GO