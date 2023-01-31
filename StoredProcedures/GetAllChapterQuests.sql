USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllChapterQuests]    Script Date: 12/14/2022 5:37:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetAllChapterQuests]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'All Charpter Quests'

	SELECT q.[ChapterQuestId], q.[ChapterId], q.[QuestId]  FROM [dbo].[ChapterQuestRel] AS q
	ORDER BY q.[ChapterId], q.[QuestId]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
