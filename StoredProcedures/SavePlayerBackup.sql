USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[SavePlayerBackup]    Script Date: 1/4/2023 4:59:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[SavePlayerBackup]
	@PlayerId INT,
	@Description VARCHAR(1000),
	@Data NVARCHAR(MAX)
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @tPlayerId INT = @PlayerId;
	DECLARE @tDescription VARCHAR(1000) = @Description;
	DECLARE @tData NVARCHAR(MAX) = @Data;

	BEGIN TRY
		INSERT INTO [dbo].[PlayerBackup] (PlayerId, Description, Data) VALUES (@tPlayerId, @tDescription, @tData);

		SET @case = 100;
		SET @message = 'Data stored';
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
