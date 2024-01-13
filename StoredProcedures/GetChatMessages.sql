USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetChatMessages]    Script Date: 1/9/2024 11:44:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetChatMessages]
	@ChatId BIGINT = NULL,
	@Length INT = NULL,
	@AllianceId INT = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	DECLARE @tchatId BIGINT = ISNULL(@ChatId, 0);
	DECLARE @tlen INT = @Length;
	DECLARE @tclanId INT = ISNULL(@AllianceId, 0);

	IF (@tclanId <> 0) BEGIN
		DECLARE @currentCId INT = NULL;
		SELECT @currentCId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @tclanId;
		IF (@currentCId IS NULL) BEGIN
			SET @case = 200;
			SET @message = 'Clan does not exists';
		END
	END

	if (@case <> 200) BEGIN
		IF (@tchatId = 0)
			BEGIN
				if (@tclanId <> 0)
					SELECT @tchatId = (ISNULL(MAX([ChatId]), 0) + 1) FROM [dbo].[ClanChat];
				ELSE
					SELECT @tchatId = (ISNULL(MAX([ChatId]), 0) + 1) FROM [dbo].[Chat];
				IF (@tlen IS NULL) SET @tlen = 10;
			END
		ELSE
			IF (@tlen IS NULL) SET @tlen = 5;

		SET @case = 100;
		SET @message = 'Chat Messages';
	END

/*	SELECT TOP (@tlen) m.[ChatId], m.[PlayerId], p.[Name], p.[VIPPoints], m.[Content], m.[CreateDate] FROM [dbo].[Chat] AS m
	INNER JOIN [dbo].[Player] AS p ON m.[PlayerId] = p.[PlayerId]
	WHERE m.[ChatId] < @tchatId
	ORDER BY m.[ChatId] DESC;*/

	BEGIN TRY
		IF (@tclanId <> 0)
			SELECT TOP (@tlen) m.[ChatId], m.[PlayerId], p.[Name], CAST(JSON_VALUE(pd.Value, '$.Points') AS INT) AS 'VIPPoints', 
						CASE WHEN (m.[Flags] & 128) = 128 THEN NULL ELSE m.[Content] END AS 'Content', m.[CreateDate], m.[Flags] 
			FROM [dbo].[ClanChat] AS m
			INNER JOIN [dbo].[Player] AS p ON m.[PlayerId] = p.[PlayerId]
			INNER JOIN [dbo].[PlayerData] AS pd ON m.[PlayerId] = pd.[PlayerId] 
			WHERE m.[ClanId] = @tclanId AND m.[ChatId] < @tchatId AND pd.[DataTypeId] = 7 and pd.[ValueId] = 3
			ORDER BY m.[ChatId] DESC;
		ELSE
			SELECT TOP (@tlen) m.[ChatId], m.[PlayerId], p.[Name], CAST(JSON_VALUE(pd.Value, '$.Points') AS INT) AS 'VIPPoints', 
						CASE WHEN (m.[Flags] & 128) = 128 THEN NULL ELSE m.[Content] END AS 'Content', m.[CreateDate], m.[Flags] 
			FROM [dbo].[Chat] AS m
			INNER JOIN [dbo].[Player] AS p ON m.[PlayerId] = p.[PlayerId]
			INNER JOIN [dbo].[PlayerData] AS pd ON m.[PlayerId] = pd.[PlayerId] 
			WHERE m.[ChatId] < @tchatId AND pd.[DataTypeId] = 7 and pd.[ValueId] = 3
			ORDER BY m.[ChatId] DESC;
	END TRY
	BEGIN CATCH
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END