USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllStructureLevelDatas]    Script Date: 11/20/2023 9:21:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetAllStructureLevelDatas]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'Structure level data list fetched succesfully';

	SELECT StructureDataId, StructureId, StructureLevel, HitPoint, FoodProduction, WoodProduction, OreProduction, 
		PopulationSupport, StructureSupport, TimeToBuild, SafeDeposit, ResourceCapicity, WoundedCapacity, InstantBuildCost, KingEXP
	FROM [dbo].[StructureData];

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END