USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerClanData]    Script Date: 10/18/2023 4:08:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetPlayerClanData]
	@PlayerId INT
AS
BEGIN

	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @clanId INT  = NULL;

	BEGIN TRY
		DECLARE @currentId INT = NULL;
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Account does not exists';
			END
		ELSE
			BEGIN
				SELECT @clanId = c.[ClanId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId;
				IF (@clanId IS NULL)
					BEGIN
						SET @case = 100;
						SET @message = 'Player does not have any clan data';
					END
				ELSE 
					BEGIN
						SET @case = 101;
						SET @message = 'Player current clan data';
					END
				
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT c.[ClanId], c.[Name], c.[Tag], c.[Description], c.[IsPublic], c.[BadgeGK]
	FROM [dbo].[Clan] AS c 
	WHERE c.[ClanId] = @clanId;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END