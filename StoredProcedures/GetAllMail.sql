USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllMail]    Script Date: 4/20/2023 1:28:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetAllMail]
	@PlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @cId INT = NULL;
	SELECT @cId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @userId;
	
	IF (@cId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'Account does not exist'
		END
	ELSE
		BEGIN
			SET @case = 100;
			SET @message = 'Player all Mails'
		END

/*	SELECT e.[MailId], c.[Code] AS 'MailContentType', e.[Content], e.[IsRead], e.[CreateDate]*/
	SELECT e.[MailId], CAST(e.[MailContentTypeId] AS TINYINT) AS 'MailType', e.[Content], e.[IsRead], e.[CreateDate]
	FROM [dbo].[Mail] AS e
/*	INNER JOIN [dbo].[MailContentType] AS c ON c.[MailContentTypeId] = e.[MailContentTypeId]*/
	WHERE e.[PlayerId] = @cId;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END