USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllMail]    Script Date: 12/1/2023 4:08:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetAllMail]
	@PlayerId INT,
	@LastTime DATETIME = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @timeAgo DATETIME = NULL;
	DECLARE @userId INT = @PlayerId;
	DECLARE @cId INT = NULL;
	SELECT @cId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @userId;
	SELECT @timeAgo = DATEADD(DAY, -10, @time);
	
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
	SELECT e.[MailId], CAST(e.[MailContentTypeId] AS TINYINT) AS 'MailType', e.[Content], e.[IsRead], e.[IsSaved], e.[CreateDate]
	FROM [dbo].[Mail] AS e
/*	INNER JOIN [dbo].[MailContentType] AS c ON c.[MailContentTypeId] = e.[MailContentTypeId]*/
	WHERE e.[PlayerId] = @cId AND (@LastTime IS NULL OR e.[CreateDate] > @LastTime) AND (e.[IsSaved] = 1 OR e.[CreateDate] >= @timeAgo);

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END