USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddQuestResourceTrackingTemplate]    Script Date: 1/31/2024 7:22:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[AddQuestResourceTrackingTemplate]
	@ChapterId INT,
	@MileStoneId INT,
	@ResourceId INT,
	@Count INT
AS
BEGIN
	DECLARE @tChapterId INT = @ChapterId;
	DECLARE @tMileStoneId INT = @MileStoneId;
	DECLARE @tResourceId INT = @ResourceId;
	DECLARE @tCount INT = @Count;

	DEClARE @ecId INT = NULL;
	SELECT @ecId = [ChapterId] FROM [dbo].[Chapter] WHERE [ChapterId] = @tChapterId;
	IF(@ecId IS NULL) RETURN;

	DEClARE @eqId INT = NULL;
	SELECT @eqId = [QuestId] FROM [dbo].[Quest] WHERE [ChapterId] = @ecId AND [MilestoneId] = @tMileStoneId;
	IF(@eqId IS NOT NULL) RETURN;
	
	DEClARE @sCode VARCHAR(100) = NULL;
	SELECT @sCode = [Code] FROM [dbo].[Resource] WHERE [ResourceId] = @tResourceId;
	IF(@sCode IS NULL) RETURN;

	DECLARE @qtypeId INT;
	SELECT @qtypeId = [QuestTypeId] FROM [dbo].[QuestType] WHERE [Code] = 'ResourceCollection';
	IF(@qtypeId IS NULL) RETURN;

	INSERT INTO [dbo].[Quest] (ChapterId, QuestTypeId, MilestoneId, Data) VALUES (@tChapterId, @qtypeId, @tMileStoneId, '{"ResourceType":"' + @sCode + '","Count":' + CONVERT(VARCHAR(100),@tCount) + '}');
END