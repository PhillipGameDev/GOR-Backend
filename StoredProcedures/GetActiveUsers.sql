USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetActiveUsers]    Script Date: 12/21/2022 11:23:59 AM ******/
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
		FROM [GameOfRevenge].[dbo].[Player]
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
