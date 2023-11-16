USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllQuestRewards]    Script Date: 11/4/2023 9:19:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetAllQuestRewards]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'Quest rewards list fetched succesfully';

	SELECT s.[QuestRewardId], s.[QuestId], s.[ItemId], i.[DataTypeId], i.[ValueId], i.[Value], s.[Count] FROM [dbo].[QuestReward] AS s
	INNER JOIN [dbo].[Item] as i on s.ItemId = i.Id

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END