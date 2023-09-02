USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetActiveUsers]    Script Date: 8/28/2023 6:54:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetActiveUsers]
AS
BEGIN
    DECLARE @case INT = 1, @error INT = 0;
    DECLARE @message NVARCHAR(MAX) = NULL;
    DECLARE @time DATETIME = GETUTCDATE();

    BEGIN TRY
        DECLARE @SixMonthsAgo DATE = DATEADD(MONTH, -6, @time);
        DECLARE @ThreeMonthsAgo DATE = DATEADD(MONTH, -3, @time);
        DECLARE @OneMonthsAgo DATE = DATEADD(MONTH, -1, @time);

        SELECT
            COUNT(CASE WHEN [LastLogin] > @OneMonthsAgo THEN [PlayerId] END) AS WithinOneMonth,
            COUNT(CASE WHEN [LastLogin] > @ThreeMonthsAgo THEN [PlayerId] END) AS WithinThreeMonths,
            COUNT(*) AS WithinSixMonths
        FROM [dbo].[Player]
        WHERE [LastLogin] > @SixMonthsAgo;

        SET @case = 100;
        SET @message = 'Active users';
    END TRY
    BEGIN CATCH
        SET @case = 0;
        SET @error = 1;
        SET @message = ERROR_MESSAGE();
    END CATCH
    
    EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
