USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddOrUpdatePlayerQuestData]    Script Date: 11/2/2022 3:44:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[AddOrUpdatePlayerQuestData]
	@PlayerId INT,
	@QuestId INT,
	@IsCompleted BIT,
	@Data VARCHAR(MAX) = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	DECLARE @userId INT = @PlayerId;
	DECLARE @qId INT = @QuestId;
	DECLARE @tData VARCHAR(MAX) = @Data;
	DECLARE @completed BIT = ISNULL(@IsCompleted,0);

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
			/* INIT CHAPTER PROGRESS RECORD */
			DECLARE @chapId INT = NULL;
			SELECT @chapId = p.[ChapterId] FROM [dbo].[ChapterQuestRel] as p WHERE p.[QuestId] = @qId;
			IF (@chapId IS NOT NULL) EXEC [AddOrUpdatePlayerChapterDataSp] @existingId, @chapId;
			
			DECLARE @id INT;
			SELECT @id = [QuestUserDataRelId] FROM [dbo].[QuestUserDataRel] WHERE [PlayerId] = @existingId AND [QuestId] = @existingqId;

			IF (@id IS NULL)
				BEGIN
					SET @case = 100;
					SET @message = 'Added new quest data';

					INSERT INTO [dbo].[QuestUserDataRel] VALUES (@existingqId, @existingId, @completed, @tData, 0);

					SELECT @id = [QuestUserDataRelId] FROM [dbo].[QuestUserDataRel] WHERE [PlayerId] = @existingId AND [QuestId] = @existingqId;
				END
			ELSE
				BEGIN
					SET @case = 101;
					SET @message = 'Updated quest data';

					IF (@tData IS NULL)
						UPDATE [dbo].[QuestUserDataRel] SET [IsCompleted] = @completed
						WHERE [QuestUserDataRelId] = @id;
					ELSE
						UPDATE [dbo].[QuestUserDataRel] SET [IsCompleted] = @completed, [CurrentData] = @tData
						WHERE [QuestUserDataRelId] = @id;
				END
		END

	SELECT * FROM [dbo].[QuestUserDataRel]
	WHERE [QuestUserDataRelId] = @id;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
