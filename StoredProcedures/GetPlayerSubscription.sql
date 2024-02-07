USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerSubscription]    Script Date: 11/25/2023 1:05:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetPlayerSubscription]
	@PlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @validPlayerId INT = NULL;
	DECLARE @validId INT = NULL;

	SET @case = 100;
	SET @message = 'Player subscription';

	BEGIN TRY
		SELECT @validPlayerId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @PlayerId;

		IF (@validPlayerId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
		ELSE
			BEGIN
				SELECT TOP (1) @validId = [SubscriptionId] FROM [dbo].[PlayerSubscription] 
				WHERE [PlayerId] = @PlayerId ORDER BY [EndDate] DESC;
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT [SubscriptionId], [TransactionId], [TransactionDate], [EndDate], [LastModified], [ProductId], [Store]
	FROM [dbo].[PlayerSubscription] WHERE [SubscriptionId] = @validId;

	EXEC [dbo].[GetMessage] @validPlayerId, @message, @case, @error, @time, 1, 1;
END