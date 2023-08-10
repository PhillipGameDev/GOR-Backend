USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[IncrementPackageReward]    Script Date: 8/14/2022 3:13:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[IncrementPackageReward]
	@PackageId INT,
	@RewardId INT,
	@Count INT
AS
BEGIN
	DECLARE @case INT = 100, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = 'Package reward updated';
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @tempCount INT = ISNULL(@Count, 0);

	BEGIN TRY
		DECLARE @validId INT = NULL;
		DECLARE @oldCount VARCHAR(MAX) = NULL;

		SELECT @validId = r.[QuestRewardId], @oldCount = r.[Count] FROM [dbo].[QuestReward] AS r 
				INNER JOIN [dbo].[PackageQuestRel] AS pq ON r.[QuestId] = pq.[QuestId] 
				WHERE pq.[PackageId] = @PackageId AND r.[QuestRewardId] = @RewardId;
		IF (@validId IS NOT NULL)
			BEGIN
				DECLARE @finalCount BIGINT = CONVERT(BIGINT, ISNULL(@oldCount, '0')) + @tempCount;
				IF (@finalCount < 0) SET @finalCount = 0;

				UPDATE [dbo].[QuestReward] SET [Count] = @finalCount WHERE [QuestRewardId] = @RewardId;
			END
		ELSE
			BEGIN
				SET @case = 200;
				SET @message = 'Invalid params';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
