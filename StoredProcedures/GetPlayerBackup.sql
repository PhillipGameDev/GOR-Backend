USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerBackup]    Script Date: 1/4/2023 4:59:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetPlayerBackup]
	@BackupId BIGINT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @tBackupId BIGINT = @BackupId;

	BEGIN TRY
		SELECT BackupId, BackupDate, Description, Data FROM [dbo].[PlayerBackup] WHERE BackupId = @tBackupId;

		SET @case = 100;
		SET @message = 'Backup retrieved';
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
