USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddQuestUpgradeStructureTemplate]    Script Date: 1/31/2024 7:19:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--*********************************************************************** QUEST TEMPLATES **********************************************************************************************************************************

ALTER   PROCEDURE [dbo].[AddQuestUpgradeStructureTemplate]
	@ChapterId INT,
	@MileStoneId INT,
	@StructureId INT,
	@Targetlevel INT
AS
BEGIN
	DECLARE @tChapterId INT = @ChapterId;
	DECLARE @tMileStoneId INT = @MileStoneId;
	DECLARE @tStructureId INT = @StructureId;
	DECLARE @tTargetlevel INT = @Targetlevel;

	DECLARE @ecId INT = NULL;
	SELECT @ecId = [ChapterId] FROM [dbo].[Chapter] WHERE [ChapterId] = @tChapterId;
	IF(@ecId IS NULL) RETURN;

	DECLARE @eqId INT = NULL;
	SELECT @eqId = [QuestId] FROM [dbo].[Quest] WHERE [ChapterId] = @ecId AND [MilestoneId] = @tMileStoneId;
	IF(@eqId IS NOT NULL) RETURN;
	
	DECLARE @sCode VARCHAR(100) = NULL;
	SELECT @sCode = [Code] FROM [dbo].[Structure] WHERE [StructureId] = @tStructureId;
	IF(@sCode IS NULL) RETURN;

	DECLARE @tLvlId INT = NULL;
	SELECT @tLvlId = [StructureDataId] FROM [dbo].[StructureData] WHERE [StructureId] = @tStructureId AND [StructureLevel] = @Targetlevel;
	IF(@tLvlId IS NULL) RETURN;

	DECLARE @qtypeId INT;
	SELECT @qtypeId = [QuestTypeId] FROM [dbo].[QuestType] WHERE [Code] = 'BuildingUpgrade';
	IF(@qtypeId IS NULL) RETURN;

	INSERT INTO [dbo].[Quest] (ChapterId, QuestTypeId, MilestoneId, Data) VALUES (@tChapterId, @qtypeId, @tMileStoneId, '{"StructureType":"' + @sCode + '","Level":' + CONVERT(VARCHAR(100),@tTargetlevel) + '}');
END