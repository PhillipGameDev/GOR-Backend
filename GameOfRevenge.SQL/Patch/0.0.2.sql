USE [GameOfRevenge]
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllStructures]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'Structure list fetched succesfully';

	SELECT s.[StructureId], s.[Name], s.[Code], p.[Code] AS 'PlacementType', s.[Description]
	FROM [dbo].[Structure] AS s
	INNER JOIN [dbo].[StructurePlacementType] AS p ON p.[StructurePlacementTypeId] = s.[StructurePlacementTypeId]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllStructureLevelDatas]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'Structure level data list fetched succesfully';

	SELECT StructureDataId, StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit, ResourceCapicity, WoundedCapacity
	FROM [dbo].[StructureData];

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO


CREATE OR ALTER PROCEDURE [dbo].[GetAllStructureLevelRequirements]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'Structure level data requirement list fetched succesfully';

	SELECT s.[StructureRequirementId], s.[StructureDataId], s.[DataTypeId], s.[ReqValueId], s.[Value] FROM [dbo].[StructureRequirement] AS s
	ORDER BY s.[StructureDataId], s.[DataTypeId] ASC

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllStructureLocation]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'Structure location list fetched succesfully';

	SELECT sl.[StructureLocationId], s.[Code] AS 'Structure', sl.[LocationId] FROM [dbo].[StructureLocation] AS sl
	INNER JOIN [dbo].[Structure] AS s ON s.[StructureId] = sl.[StructureId]
	ORDER BY sl.[StructureLocationId]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllStructureBuildLimit]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'Structure build limit list fetched succesfully';

	SELECT sl.[StructureBuildDataId], sl.[TownHallLevel], s.[Code] AS 'BuildStructure', sl.[MaxBuildCount] FROM [dbo].[StructureBuildData] AS sl
	INNER JOIN [dbo].[Structure] AS s ON s.[StructureId] = sl.[BuildStructureId]
	ORDER BY sl.[StructureBuildDataId]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO


CREATE OR ALTER PROCEDURE [dbo].[GetAllTroops]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'All troop level datas';

	SELECT [TroopId], [Name], [Code], [Description], [IsMelee], [IsMounted], [IsMagic], [IsSeige] FROM [dbo].[Troop]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllTroopDatas]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'All troop level datas';

	SELECT [TroopDataId], [TroopId], [TroopLevel], [Health], [WoundedThreshold], [AttackDamage], [AttackRange], [AttackSpeed], [Defence], [MovementSpeed], [WeightLoad], [TraningTime], [OutputCount], [PowerValue] FROM [dbo].[TroopData]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllTroopDataRequirements]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'All troop level datas';

	SELECT [TroopRequirementId], [TroopDataId], [DataTypeId], [ReqValueId], [Value] FROM [dbo].[TroopRequirement]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO





