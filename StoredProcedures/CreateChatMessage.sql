USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[CreateChatMessage]    Script Date: 1/9/2024 11:55:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[CreateChatMessage]
	@PlayerId INT,
	@AllianceId INT = NULL,
	@Content NVARCHAR(1000)
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	DECLARE @userId INT = @PlayerId;
	DECLARE @clanId INT = ISNULL(@AllianceId, 0);
	DECLARE @tcontent NVARCHAR(1000) = LTRIM(RTRIM(@Content));

	DECLARE @name NVARCHAR(200) = NULL;
	DECLARE @vipPoints INT = NULL;

	SELECT @name = p.[Name], @vipPoints = p.[VIPPoints] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

	BEGIN TRY
		IF (@clanId <> 0) BEGIN
			DECLARE @currentCId INT = NULL;
			SELECT @currentCId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @clanId;
			IF (@currentCId IS NULL) BEGIN
				SET @case = 201;
				SET @message = 'Clan does not exists';
			END
			ELSE BEGIN
				INSERT INTO [dbo].[ClanChat] ([PlayerId], [ClanId], [Content], [CreateDate], [Flags])
				OUTPUT INSERTED.ChatId, @userId AS 'PlayerId', @name AS 'Name', @vipPoints AS 'VIPPoints', INSERTED.Content, INSERTED.CreateDate, CAST(0 AS TINYINT) AS 'Flags'
				VALUES (@userId, @clanId, @tcontent, @time, 0)

				SET @case = 100;
				SET @message = 'Saved message succesfully';
			END
		END
		ELSE BEGIN
			INSERT INTO [dbo].[Chat] ([PlayerId], [Content], [CreateDate], [Flags])
			OUTPUT INSERTED.ChatId, @userId AS 'PlayerId', @name AS 'Name', @vipPoints AS 'VIPPoints', INSERTED.Content, INSERTED.CreateDate, CAST(0 AS TINYINT) AS 'Flags'
			VALUES (@userId, @tcontent, @time, 0)

			SET @case = 100;
			SET @message = 'Saved message succesfully';
		END
	END TRY
	BEGIN CATCH
		SET @case = 200;
		SET @message = 'Account does not exist';
	END CATCH

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END