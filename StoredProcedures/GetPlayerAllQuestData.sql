USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerAllQuestData]    Script Date: 4/10/2023 12:57:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetPlayerAllQuestData]
	@PlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = @PlayerId;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @existingId INT = NULL;

	SELECT @existingId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @tempuserId;

	IF (@existingId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'No existing account found';
		END
	ELSE
		BEGIN
			SET @case = 100;
			SET @message = 'Player all quest';
		END

	SELECT [QuestUserDataRelId], [QuestId], [IsCompleted], [IsRedeemed], [CurrentData]
	FROM [dbo].[QuestUserDataRel] WHERE [PlayerId] = @existingId
/*	SELECT [QuestUserDataRelId], [QuestId], [IsCompleted], [IsRedeemed], 
			(CASE WHEN [IsCompleted] = 1 THEN NULL ELSE [CurrentData] END) AS [CurrentData]
	FROM [dbo].[QuestUserDataRel] WHERE [PlayerId] = @existingId*/

	EXEC [dbo].[GetMessage] @existingId, @message, @case, @error, @time, 1, 1;
END