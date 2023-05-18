USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllQuestRewards]    Script Date: 5/16/2023 2:00:56 PM ******/
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

	SELECT s.[QuestRewardId], s.[QuestId], s.[DataTypeId], s.[ReqValueId], s.[Value], s.[Count] FROM [dbo].[QuestReward] AS s
	WHERE ([DataTypeId] <> 8) OR (([ReqValueId] <> 14) AND ([ReqValueId] <> 7));

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END