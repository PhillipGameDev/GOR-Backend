USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddClaimRewardsRequest]    Script Date: 11/25/2023 1:05:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AddClaimRewardsRequest]
	@PlayerId INT,
	@ItemName VARCHAR(20),
	@Amount INT,
	@EventId VARCHAR(50),
	@Timestamp VARCHAR(20)
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	DECLARE @validId INT = NULL;

	BEGIN TRY
		BEGIN
			SELECT @validId = [RequestId] FROM [dbo].[ClaimRewardsRequests]
			WHERE [PlayerId] = @PlayerId AND [Timestamp] = @Timestamp;

			IF (@validId IS NULL)
				BEGIN
					INSERT INTO [dbo].[ClaimRewardsRequests] ([PlayerId], [ItemName], [Amount], [EventId], [Timestamp])
					VALUES (@PlayerId, @ItemName, @Amount, @EventId, @Timestamp);
					SET @case = 100;
					SET @message = 'Inserted request';
				END
			ELSE
				BEGIN
					SET @case = 101;
					SET @message = 'Already inserted request';
				END
		END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] @PlayerId, @message, @case, @error, @time, 1, 1;
END