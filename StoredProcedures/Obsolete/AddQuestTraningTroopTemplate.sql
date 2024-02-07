USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddQuestTraningTroopTemplate]    Script Date: 1/31/2024 7:19:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[AddQuestTraningTroopTemplate]
	@ChapterId INT,
	@MileStoneId INT,
	@TroopId INT,
	@Targetlevel INT,
	@Count INT
AS
BEGIN
	DECLARE @tChapterId INT = @ChapterId;
	DECLARE @tMileStoneId INT = @MileStoneId;
	DECLARE @tTroopId INT = @TroopId;
	DECLARE @tCount INT = @Count;
	DECLARE @tTargetlevel INT = @Targetlevel;

	DEClARE @ecId INT = NULL;
	SELECT @ecId = [ChapterId] FROM [dbo].[Chapter] WHERE [ChapterId] = @tChapterId;
	IF(@ecId IS NULL) RETURN;

	DEClARE @eqId INT = NULL;
	SELECT @eqId = [QuestId] FROM [dbo].[Quest] WHERE [ChapterId] = @ecId AND [MilestoneId] = @tMileStoneId;
	IF(@eqId IS NOT NULL) RETURN;
	
	DEClARE @sCode VARCHAR(100) = NULL;
	SELECT @sCode = [Code] FROM [dbo].[Troop] WHERE [TroopId] = @tTroopId;
	IF(@sCode IS NULL) RETURN;

	DEClARE @tLvlId INT = NULL;
	SELECT @tLvlId = [TroopDataId] FROM [dbo].[TroopData] WHERE [TroopId] = @tTroopId AND [TroopLevel] = @Targetlevel;
	IF(@tLvlId IS NULL) RETURN;

	DECLARE @qtypeId INT;
	SELECT @qtypeId = [QuestTypeId] FROM [dbo].[QuestType] WHERE [Code] = 'TrainTroops';
	IF(@qtypeId IS NULL) RETURN;

	INSERT INTO [dbo].[Quest] (ChapterId, QuestTypeId, MilestoneId, Data) VALUES (@tChapterId, @qtypeId, @tMileStoneId, '{"TroopType":"' + @sCode + '","Level":' + CONVERT(VARCHAR(100),@tTargetlevel) + ',"Count":' + CONVERT(VARCHAR(100),@tCount) + '}');
END