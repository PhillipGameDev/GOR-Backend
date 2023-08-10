USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpdatePackageReward]    Script Date: 8/14/2022 3:13:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[UpdatePackageReward]
	@PackageId INT,
	@RewardId INT,
	@Count INT
AS
BEGIN
	DECLARE @case INT = 100, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = 'Package reward updated';
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	BEGIN TRY
		IF EXISTS (SELECT 1 FROM [dbo].[QuestReward] AS r INNER JOIN [dbo].[PackageQuestRel] AS pq ON r.[QuestId] = pq.[QuestId] 
					WHERE pq.[PackageId] = @PackageId AND r.[QuestRewardId] = @RewardId)
			UPDATE [dbo].[QuestReward] SET [Count] = @Count WHERE [QuestRewardId] = @RewardId;
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
