USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddSubscriptionDailyReward]    Script Date: 11/25/2023 1:05:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AddSubscriptionDailyReward]
	@PlayerId INT,
	@SubscriptionId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @validPlayerId INT = NULL;
	DECLARE @existingId BIGINT = NULL;

	BEGIN TRY
		SELECT @validPlayerId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @PlayerId;

		IF (@validPlayerId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
		ELSE
			BEGIN
				SELECT @existingId = sdr.[Id] FROM [dbo].[SubscriptionDailyRewards] AS sdr
				LEFT JOIN [dbo].[PlayerSubscription] AS ps ON ps.[SubscriptionId] = sdr.[SubscriptionId]
				WHERE ps.[PlayerId] = @PlayerId AND ps.[SubscriptionId] = @SubscriptionId;

				IF (@existingId IS NULL)
					BEGIN
						INSERT INTO [dbo].[SubscriptionDailyRewards] ([SubscriptionId])
						VALUES (@SubscriptionId);

						SET @case = 100;
					END
				ELSE SET @case = 101;

				SET @message = 'Inserted subscription reward';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] @validPlayerId, @message, @case, @error, @time, 1, 1;
END