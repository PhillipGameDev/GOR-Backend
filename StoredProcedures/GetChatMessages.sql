USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetChatMessages]    Script Date: 3/18/2023 3:35:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetChatMessages]
	@ChatId BIGINT = NULL,
	@Length INT = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	DECLARE @tchatId BIGINT = ISNULL(@ChatId, 0);
	DECLARE @tlen INT = @Length;

	IF (@tchatId = 0)
		BEGIN
			SELECT @tchatId = (ISNULL(MAX([ChatId]), 0) + 1) FROM [dbo].[Chat];
			IF (@tlen IS NULL) SET @tlen = 10;
		END
	ELSE
		IF (@tlen IS NULL) SET @tlen = 5;


	SET @case = 100;
	SET @message = 'Chat Messages';

/*	SELECT TOP (@tlen) m.[ChatId], m.[PlayerId], p.[Name], p.[VIPPoints], m.[Content], m.[CreateDate] FROM [dbo].[Chat] AS m
	INNER JOIN [dbo].[Player] AS p ON m.[PlayerId] = p.[PlayerId]
	WHERE m.[ChatId] < @tchatId
	ORDER BY m.[ChatId] DESC;*/

	SELECT TOP (@tlen) m.[ChatId], m.[PlayerId], p.[Name], CAST(JSON_VALUE(pd.Value, '$.Points') AS INT) AS 'VIPPoints', m.[Content], m.[CreateDate] 
	FROM [dbo].[Chat] AS m
	INNER JOIN [dbo].[Player] AS p ON m.[PlayerId] = p.[PlayerId]
	INNER JOIN [dbo].[PlayerData] AS pd ON m.[PlayerId] = pd.[PlayerId] 
	WHERE m.[ChatId] < @tchatId AND pd.[DataTypeId] = 7 and pd.[ValueId] = 3
	ORDER BY m.[ChatId] DESC;

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END