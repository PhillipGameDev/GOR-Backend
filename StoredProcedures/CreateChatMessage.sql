USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[CreateChatMessage]    Script Date: 2/26/2023 3:09:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[CreateChatMessage]
	@PlayerId INT,
	@Content VARCHAR(MAX)
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	DECLARE @userId INT = @PlayerId;
	DECLARE @tcontent VARCHAR(MAX) = LTRIM(RTRIM(@Content));

	DECLARE @name VARCHAR(1000) = NULL;
	DECLARE @vipPoints INT = NULL;

	SELECT @name = p.[Name], @vipPoints = p.[VIPPoints] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

	BEGIN TRY
		DECLARE @tempTable table (ChatId BIGINT);

		INSERT INTO [dbo].[Chat] 
		OUTPUT INSERTED.ChatId INTO @tempTable
		VALUES (@userId, @tcontent, @time)

		SELECT [ChatId], @userId AS 'PlayerId', @vipPoints AS 'VIPPoints', @tcontent AS 'Content', @time AS 'CreateDate' FROM @tempTable;

		SET @case = 100;
		SET @message = 'Saved message succesfully';
	END TRY
	BEGIN CATCH
		SET @case = 200;
		SET @message = 'Account does not exist'
	END CATCH

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END