USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddGloryKingdomDetails]    Script Date: 9/16/2023 5:40:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[AddGloryKingdomDetails]
	@StartTime DATETIME,
	@EndTime DATETIME
AS
BEGIN
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @case INT = 0;
	DECLARE @error INT = 0;

	DECLARE @lastEndTime DATETIME = NULL;

	BEGIN TRY
		IF ((@StartTime IS NULL) OR (@EndTime IS NULL)) BEGIN
			SET @case = 200;
			SET @message = 'Invalid values';
		END
		ELSE BEGIN
			SELECT TOP(1) @lastEndTime = gk.[EndTime] FROM [dbo].[GloryKingdomEvent] AS gk ORDER BY [StartTime] DESC;

			IF (@lastEndTime > GETUTCDATE()) BEGIN
				SET @case = 201;
				SET @message = 'An event is currently running';
			END
			ELSE BEGIN
				INSERT INTO [dbo].[GloryKingdomEvent] VALUES (@StartTime, @EndTime);

				SET @case = 100;
				SET @message = 'Created new event';
			END
		END

	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH
	
	SELECT TOP(1) gk.[GloryKingdomEventId], gk.[StartTime], gk.[EndTime] FROM [dbo].[GloryKingdomEvent] AS gk
	ORDER BY [StartTime] DESC;

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END