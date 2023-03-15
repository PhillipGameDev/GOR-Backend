USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetClanMembers]    Script Date: 3/14/2023 4:43:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetClanMembers]
	@ClanId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;
	DECLARE @tClanId INT = @ClanId;
	DECLARE @currentCId INT = NULL;

	BEGIN TRY
		SELECT @currentCId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @tClanId;
		IF (@currentCId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Clan does not exists';
			END
		ELSE
			BEGIN
				SET @case = 100;
				SET @message = 'Clan members list fetched succesfully';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT c.[ClanMemberId], c.[ClanId], r.[Code] AS 'Role', c.[PlayerId], p.[Name], t.[X], t.[Y]
	FROM [dbo].[ClanMember] AS c 
	INNER JOIN [dbo].[ClanRole] AS r ON r.[ClanRoleId] = c.[ClanRoleId]
	INNER JOIN [dbo].[Player] AS p ON p.[PlayerId] = c.[PlayerId]
	LEFT JOIN [dbo].[WorldTileData] AS t ON p.[WorldTileId] = t.[WorldTileDataId]
	WHERE c.[ClanId] = @currentCId;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END