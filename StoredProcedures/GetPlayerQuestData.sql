USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerQuestData]    Script Date: 4/12/2023 5:25:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetPlayerQuestData]
	@PlayerId INT,
	@QuestId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @id INT = NULL;

	DECLARE @userId INT = @PlayerId;
	DECLARE @qId INT = @QuestId;

	DECLARE @existingId INT = NULL;
	SELECT @existingId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

	DECLARE @existingqId INT = NULL;
	SELECT @existingqId = p.[QuestId] FROM [dbo].[Quest] AS p WHERE p.[QuestId] = @qId;

	IF (@existingId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'No existing account found';
		END
	ELSE IF (@existingqId IS NULL)
		BEGIN
			SET @case = 201;
			SET @message = 'Quest data not found';
		END
	ELSE
		BEGIN
			SET @case = 100;
			SET @message = 'Quest data';
			SELECT @id = [QuestUserDataRelId] FROM [dbo].[QuestUserDataRel] WHERE [PlayerId] = @existingId AND [QuestId] = @existingqId;

			IF (@id IS NULL)
				BEGIN
					INSERT INTO [QuestUserDataRel] VALUES (@existingqId, @existingId, 0, NULL, 0);
					SELECT @id = [QuestUserDataRelId] FROM [dbo].[QuestUserDataRel] WHERE [PlayerId] = @existingId AND [QuestId] = @existingqId;
				END
		END

	SELECT [QuestUserDataRelId], [QuestId], [IsCompleted], [IsRedeemed], 
			(CASE WHEN [IsCompleted] = 1 THEN NULL ELSE [CurrentData] END) AS [CurrentData]
	FROM [dbo].[QuestUserDataRel] WHERE [QuestUserDataRelId] = @id;

	EXEC [dbo].[GetMessage] @existingId, @message, @case, @error, @time, 1, 1;
END