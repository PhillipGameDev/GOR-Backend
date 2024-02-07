USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddQuestXStructureTemplate]    Script Date: 1/31/2024 7:19:04 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[AddQuestXStructureTemplate]
	@ChapterId INT,
	@MileStoneId INT,
	@StructureId INT,
	@Count INT
AS
BEGIN
	DECLARE @tChapterId INT = @ChapterId;
	DECLARE @tMileStoneId INT = @MileStoneId;
	DECLARE @tStructureId INT = @StructureId;
	DECLARE @tCount INT = @Count;

	DEClARE @ecId INT = NULL;
	SELECT @ecId = [ChapterId] FROM [dbo].[Chapter] WHERE [ChapterId] = @tChapterId;
	IF(@ecId IS NULL) RETURN;

	DEClARE @eqId INT = NULL;
	SELECT @eqId = [QuestId] FROM [dbo].[Quest] WHERE [ChapterId] = @ecId AND [MilestoneId] = @tMileStoneId;
	IF(@eqId IS NOT NULL) RETURN;
	
	DEClARE @sCode VARCHAR(100) = NULL;
	SELECT @sCode = [Code] FROM [dbo].[Structure] WHERE [StructureId] = @tStructureId;
	IF(@sCode IS NULL) RETURN;

	DECLARE @qtypeId INT;
	SELECT @qtypeId = [QuestTypeId] FROM [dbo].[QuestType] WHERE [Code] = 'XBuildingCount';
	IF(@qtypeId IS NULL) RETURN;

	INSERT INTO [dbo].[Quest] (ChapterId, QuestTypeId, MilestoneId, Data) VALUES (@tChapterId, @qtypeId, @tMileStoneId, '{"StructureType":"' + @sCode + '","Count":' + CONVERT(VARCHAR(100),@tCount) + '}');
END