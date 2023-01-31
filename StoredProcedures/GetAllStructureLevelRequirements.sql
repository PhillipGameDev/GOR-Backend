USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllStructureLevelRequirements]    Script Date: 8/17/2022 5:08:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[GetAllStructureLevelRequirements]
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
