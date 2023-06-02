USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[SaveMail]    Script Date: 6/1/2023 2:38:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[SaveMail]
	@MailId INT,
	@Saved BIT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;
	DECLARE @tMailId INT = @MailId;
	DECLARE @tSaved BIT = @Saved;

	UPDATE [dbo].[Mail] 
	SET [IsSaved] = @tSaved
	WHERE [MailId] = @tMailId;

	SET @case = 100;
	SET @message = 'Marked message save'

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END