USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[CreateMail]    Script Date: 1/3/2023 8:55:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[CreateMail]
	@PlayerId INT,
	@Content VARCHAR(MAX),
	@ContentType VARCHAR(100)
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @userId INT = @PlayerId;
	DECLARE @tcontent VARCHAR(MAX) = LTRIM(RTRIM(@Content));
	DECLARE @tcontentcode VARCHAR(100) = LTRIM(RTRIM(@ContentType));
	DECLARE @cId INT = NULL;
	SELECT @cId = [MailContentTypeId] FROM [dbo].[MailContentType] WHERE [Code] = @tcontentcode;
	
	IF (@cId IS NULL)
		BEGIN
			SET @case = 201;
			SET @message = 'Invalid content type'
		END
	ELSE
		BEGIN TRY
			INSERT INTO [dbo].[Mail] VALUES (@userId, @cId, @tcontent, 0, @time, 0)
			SET @case = 100;
			SET @message = 'Sended Mail request succesfully';
		END TRY
		BEGIN CATCH
			SET @case = 200;
			SET @message = 'Account does not exist'
		END CATCH

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END