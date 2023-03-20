USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllStructureBuildLimit]    Script Date: 3/15/2023 10:50:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetAllStructureBuildLimit]
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
	ORDER BY s.[Code], sl.[TownHallLevel]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END