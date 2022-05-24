USE [GameOfRevenge]
GO

--------- Mail START ---------
CREATE TABLE [dbo].[MailContentType]
(
	MailContentTypeId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(1000) NOT NULL,
	Code VARCHAR(100) NOT NULL,
	CONSTRAINT [PK_MailContentType_MailContentTypeId] PRIMARY KEY CLUSTERED (MailContentTypeId ASC),
	CONSTRAINT [UQ_MailContentTypeId_Code] UNIQUE NONCLUSTERED (Code)
)
GO

INSERT INTO [dbo].[MailContentType] VALUES ('String', 'String'), ('Battle Report', 'BattleReport'), ('Under Attack Report', 'UnderAttack')
GO

CREATE TABLE [dbo].[Mail]
(
	MailId INT IDENTITY(1,1) NOT NULL,
	PlayerId INT NOT NULL,
	MailContentTypeId INT NOT NULL,
	Content VARCHAR(MAX) NOT NULL,
	IsRead BIT NOT NULL,
	CONSTRAINT [PK_Mail_MailId] PRIMARY KEY CLUSTERED (MailId ASC),
	CONSTRAINT [FK_Mail_Player_PlayerId] FOREIGN KEY (PlayerId) REFERENCES [dbo].[Player] (PlayerId),
	CONSTRAINT [FK_Mail_MailContentType_MailContentTypeId] FOREIGN KEY (MailContentTypeId) REFERENCES [dbo].[MailContentType] (MailContentTypeId)
)
GO

CREATE OR ALTER PROCEDURE [dbo].[CreateMail]
	@PlayerId INT,
	@Content VARCHAR(MAX),
	@ContentType VARCHAR(100)
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
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
			INSERT INTO [dbo].[Mail] VALUES (@userId, @cId, @tcontent, 0)
			SET @case = 100;
			SET @message = 'Sended Mail request succesfully';
		END TRY
		BEGIN CATCH
			SET @case = 200;
			SET @message = 'Account does not exist'
		END CATCH

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[DeleteMail]
	@MailId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;
	DECLARE @tMailId INT = @MailId;

	DELETE FROM [dbo].[Mail] WHERE [MailId] = @tMailId;

	SET @case = 100;
	SET @message = 'Deleted message'

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[ReadMail]
	@MailId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;
	DECLARE @tMailId INT = @MailId;

	UPDATE [dbo].[Mail] 
	SET [IsRead] = 1
	WHERE [MailId] = @tMailId;

	SET @case = 100;
	SET @message = 'Marked message read'

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO


CREATE OR ALTER PROCEDURE [dbo].[GetAllMail]
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

	SELECT e.[MailId], e.[PlayerId], c.[Code] AS 'MailContentType', e.[Content], e.[IsRead]
	FROM [dbo].[Mail] AS e
	INNER JOIN [dbo].[MailContentType] AS c ON c.[MailContentTypeId] = e.[MailContentTypeId]
	WHERE e.[PlayerId] = @cId;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

--------- Mail END ---------