USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetDailyVisits]    Script Date: 8/28/2023 6:58:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetDailyVisits]
AS
BEGIN
    DECLARE @case INT = 1, @error INT = 0;
    DECLARE @message NVARCHAR(MAX) = NULL;
    DECLARE @time DATETIME = GETUTCDATE();

    BEGIN TRY
        DECLARE @StartDate DATE = DATEADD(DAY, -29, @time);
        DECLARE @EndDate DATE = @time;

        WITH DateRange AS (
            SELECT @StartDate AS Date
            UNION ALL
            SELECT DATEADD(DAY, 1, Date)
            FROM DateRange
            WHERE Date < @EndDate
        ),
        DailyVisitsData AS (
            SELECT dr.Date, COALESCE(dv.Total, 0) AS Total
            FROM DateRange dr
            LEFT JOIN [dbo].[DailyVisits] dv ON dr.Date = dv.VisitDate
        )

        SELECT
            STRING_AGG(new_users, ',') AS NewUsers,
            STRING_AGG(dv.Total, ',') AS Logged
        FROM (
            SELECT
                dr.Date,
                COUNT(CASE WHEN DATEDIFF(DAY, p.[CreateDate], dr.Date) = 0 THEN p.[PlayerId] END) AS new_users
            FROM DateRange dr
            LEFT JOIN [dbo].[Player] p ON CONVERT(DATE, p.[LastLogin]) = dr.Date
            GROUP BY dr.Date
        ) AS NewUsersData
        LEFT JOIN DailyVisitsData dv ON NewUsersData.Date = dv.Date;

        SET @case = 100;
        SET @message = 'Daily visits';
    END TRY
    BEGIN CATCH
        SET @case = 0;
        SET @error = 1;
        SET @message = ERROR_MESSAGE();
    END CATCH
    
    EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
