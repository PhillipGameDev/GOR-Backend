USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllQuests]    Script Date: 4/10/2023 12:30:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetAllQuests]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	SET @case = 100;
	SET @message = 'All Quests'

/*	SELECT q.[QuestId], q.[ChapterId], t.[Code] AS 'QuestType', q.[MilestoneId], q.[Data]  FROM [dbo].[Quest] AS q
	INNER JOIN [dbo].[Chapter] AS c ON c.[ChapterId] = q.[ChapterId]
	INNER JOIN [dbo].[QuestType] AS t ON t.[QuestTypeId] = q.[QuestTypeId]
	ORDER BY c.[ChapOrder], q.[MilestoneId]*/

	SELECT [QuestId], [MilestoneId] AS 'QuestGroup', [QuestTypeId] AS 'QuestType', [Data] FROM [dbo].[Quest]
	ORDER BY [MilestoneId]

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END