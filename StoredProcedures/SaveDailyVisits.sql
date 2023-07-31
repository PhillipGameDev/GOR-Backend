USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[SaveDailyVisits]    Script Date: 12/21/2022 11:23:59 AM ******/
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

		IF NOT EXISTS (SELECT 1 FROM dbo.[DailyVisits] WHERE VisitDate = @yesterday)
		    BEGIN
		        SELECT @total = COUNT(DISTINCT PlayerId)
		        FROM [GameOfRevenge].[dbo].[Player]
		        WHERE CONVERT(DATE, LastLogin) = @yesterday;

		        INSERT INTO dbo.[DailyVisits] VALUES (@yesterday, @total);

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
