USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[SaveDailyVisits]    Script Date: 8/28/2023 7:03:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[SaveDailyVisits]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	BEGIN TRY
		DECLARE @yesterday DATE = CONVERT(DATE, DATEADD(DAY, -1, @time));
		DECLARE @total INT;

		IF NOT EXISTS (SELECT 1 FROM [dbo].[DailyVisits] dv WHERE dv.VisitDate = @yesterday)
		    BEGIN
		        SELECT @total = COUNT(DISTINCT p.PlayerId)
		        FROM [dbo].[Player] p
		        WHERE CONVERT(DATE, p.LastLogin) = @yesterday;

		        INSERT INTO [dbo].[DailyVisits] VALUES (@yesterday, @total);

		        SET @case = 100;
		        SET @message = 'Daily visits generated';
		    END
	    ELSE
		    BEGIN
		        SET @case = 101;
		        SET @message = 'Daily visits already generated';
		    END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH
	
	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
