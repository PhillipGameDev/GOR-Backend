USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddPlayerSubscription]    Script Date: 11/25/2023 1:05:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AddPlayerSubscription]
	@PlayerId INT,
	@Store VARCHAR(20),
	@TransactionId VARCHAR(50),
	@TransactionDate DATETIME,
	@ProductId VARCHAR(20),
	@Days INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @validPlayerId INT = NULL;
	DECLARE @validId INT = NULL;
	DECLARE @endDate DATETIME = NULL;
	DECLARE @newEndDate DATETIME = NULL;

	SET @case = 100;
	SET @message = 'Inserted subscription';

	BEGIN TRY
		SELECT @validPlayerId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @PlayerId;

		IF (@validPlayerId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
		ELSE
			BEGIN
				SELECT @validId = [SubscriptionId], @endDate = [EndDate] FROM [dbo].[PlayerSubscription]
				WHERE [PlayerId] = @PlayerId AND [Store] = @Store AND [TransactionId] = @TransactionId;

				SELECT @newEndDate = DATEADD(day, @Days, @TransactionDate);
				IF (@validId IS NULL)
					BEGIN
						INSERT INTO [dbo].[PlayerSubscription]
						VALUES (@PlayerId, @TransactionId, @TransactionDate, @newEndDate, @time, @ProductId, @Store);
					END
				ELSE IF (@endDate <> @newEndDate)
					BEGIN
						UPDATE [dbo].[PlayerSubscription] SET [EndDate] = @newEndDate, [LastModified] = @time
						WHERE [SubscriptionId] = @validId;
					END
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] @validPlayerId, @message, @case, @error, @time, 1, 1;
END