USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddQuestReward]    Script Date: 8/14/2022 3:13:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[AddQuestReward]
	@QuestId INT,
	@DataType VARCHAR(50),
	@ValueId INT,
	@Value INT,
	@Count INT,
	@Log BIT = 1
AS
BEGIN
	DECLARE @case INT = 100, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = 'Reward added';
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @validId INT = NULL;

	BEGIN TRY
		IF ((@ValueId IS NOT NULL) AND (@Value IS NOT NULL) AND (@Count IS NOT NULL))
			BEGIN
				IF EXISTS (SELECT 1 FROM [dbo].[Quest] WHERE [QuestId] = @QuestId)
					BEGIN
						DECLARE @tempDataType VARCHAR(50) = NULL;
						IF (@DataType IS NOT NULL) SET @tempDataType = NULLIF(LTRIM(RTRIM(@DataType)), '');

						DECLARE @validTypeId INT = NULL;
						IF (@tempDataType IS NOT NULL) SELECT @validTypeId = [DataTypeId] FROM [dbo].[DataType] WHERE [Code] = @tempDataType;

						IF (@validTypeId IS NOT NULL)
							BEGIN
								SELECT @validId = q.[QuestRewardId] FROM [dbo].[QuestReward] as q INNER JOIN [dbo].[Item] as i ON q.[ItemId] = i.[Id]
										WHERE q.[QuestId] = @QuestId AND i.[DataTypeId] = @validTypeId AND i.[ValueId] = @ValueId AND i.[Value] = @Value;
								IF (@validId IS NULL)
									BEGIN
										DECLARE @tempTable table (QuestRewardId INT);

										INSERT INTO [dbo].[QuestReward]([QuestId], [Count])
										OUTPUT INSERTED.QuestRewardId INTO @tempTable
										VALUES (@QuestId, @Count);

										SELECT @validId = [QuestRewardId] FROM @tempTable;
									END
								ELSE
									BEGIN
										SET @case = 200;
										SET @message = 'Reward values already exist';
									END
							END
						ELSE
							BEGIN
								SET @case = 201;
								SET @message = 'Invalid data type code';
							END
					END
				ELSE
					BEGIN
						SET @case = 202;
						SET @message = 'Invalid quest Id';
					END
			END
		ELSE
			BEGIN
				SET @case = 203;
				SET @message = 'Invalid params';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	IF (@validId IS NOT NULL) SELECT @validId;

	IF (@Log = 1) EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
