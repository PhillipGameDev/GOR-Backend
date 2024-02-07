USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllSubscriptionsNotRewarded]    Script Date: 11/25/2023 1:05:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetAllSubscriptionsNotRewarded]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	DECLARE @today DATE = CAST(GETUTCDATE() AS DATE);

	SELECT [PlayerId], [SubscriptionId], [ProductId] FROM [dbo].[PlayerSubscription] WHERE [SubscriptionId] NOT IN (
		SELECT ps.[SubscriptionId] FROM [dbo].[PlayerSubscription] AS ps
	    LEFT JOIN [dbo].[SubscriptionDailyRewards] AS sdr ON ps.[SubscriptionId] = sdr.[SubscriptionId]
		WHERE ps.[EndDate] >= CAST(@today AS DATETIME) AND sdr.[ProcessDate] = @today
	);
	SET @case = 100;
	SET @message = 'All subscriptions not rewarded';

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END