USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerBackups]    Script Date: 1/4/2023 4:59:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetPlayerBackups]
	@PlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @tPlayerId INT = @PlayerId;

	BEGIN TRY
		SELECT TOP 50 BackupId, BackupDate, Description FROM [dbo].[PlayerBackup] WHERE PlayerId = @tPlayerId ORDER BY BackupId DESC;

		SET @case = 100;
		SET @message = 'Data retrieved';
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
