USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[ResetAllDailyQuests]    Script Date: 12/21/2022 11:23:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[ResetAllDailyQuests]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @id INT = NULL;

	DECLARE @SIDE_QUEST INT = 99;
	DECLARE @DAILY_QUEST INT = -1;

	BEGIN TRY
		DECLARE @date DATE = CAST( @time AS DATE );
		SELECT @id = tbl.[Id] FROM [dbo].[ResetDate] as tbl WHERE tbl.[Date] = @date;
		IF (@id IS NULL)
			BEGIN
				UPDATE [dbo].[QuestUserDataRel]
				SET [CurrentData] = NULL
				FROM [dbo].[QuestUserDataRel] AS d
				INNER JOIN [dbo].[Quest] AS p ON d.[QuestId] = p.[QuestId]
				WHERE p.[MilestoneId] = @DAILY_QUEST;

				INSERT INTO [dbo].[ResetDate] (Date) VALUES (@date);

				SET @case = 100;
				SET @message = 'All quests reseted';
			END
		ELSE
			BEGIN
				SET @case = 101;
				SET @message = 'Quests already reseted';
			END
	END TRY
	BEGIN CATCH
		SET @currentId = NULL;
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH
	
	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
