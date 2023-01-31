USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllChapterRewards]    Script Date: 12/14/2022 6:40:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetAllChapterRewards]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'Chapter rewards list fetched succesfully';

	SELECT s.[ChapterRewardId], s.[ChapterId], s.[DataTypeId], s.[ReqValueId], s.[Value], s.[Count] FROM [dbo].[ChapterReward] AS s

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
