USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[CreateClan]    Script Date: 10/18/2023 4:13:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[CreateClan]
	@PlayerId INT,
	@Name VARCHAR(100),
	@Tag VARCHAR(10),
	@Description VARCHAR(5000),
	@IsPublic BIT,
	@Capacity INT
AS
BEGIN
	
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @tempName VARCHAR(100) = LTRIM(RTRIM(@Name));
	DECLARE @tempTag VARCHAR(10) = LTRIM(RTRIM(@Tag));
	DECLARE @tempDescription VARCHAR(5000) = LTRIM(RTRIM(@Description));
	DECLARE @clanId INT = NULL;
	DECLARE @tisp BIT = ISNULL(@IsPublic, 0);
	DECLARE @tempCapacity INT = ISNULL(@Capacity, 10);

	BEGIN TRY
		DECLARE @currentId INT = NULL;
		DECLARE @currentClanMemberId INT = NULL;
		DECLARE @nameTag VARCHAR(10) = NULL;

		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

		DELETE FROM [dbo].[ClanInvite] WHERE [ToPlayerId] = @currentId;
		DELETE FROM [dbo].[ClanJoinRequest] WHERE [PlayerId] = @currentId;

		SELECT @nameTag = c.[Tag] FROM [dbo].[Clan] AS c WHERE c.[Tag] = @tempTag;
		SELECT @currentClanMemberId = c.[ClanMemberId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId;

		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Account does not exists';
			END
		ELSE IF (@nameTag IS NOT NULL)
			BEGIN
				SET @case = 201;
				SET @message = 'Clan with same tag already exists';
			END
		ELSE IF (@currentClanMemberId IS NOT NULL)
			BEGIN
				SET @case = 202;
				SET @message = 'Member is already part of another clan';
			END
		ELSE
			BEGIN
				--add clan
				INSERT INTO [dbo].[Clan] (Name, Tag, Description, IsPublic, Capacity)
				VALUES (@tempName, @tempTag, @tempDescription, @tisp, @tempCapacity);

				--add owner role
				SELECT @clanId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[Tag] = @tempTag;
				DECLARE @roldId INT = NULL;
				SELECT @roldId = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Owner';
				INSERT INTO [dbo].[ClanMember] VALUES (@clanId, @userId, @roldId);

				SET @nameTag = @tempTag;
				SET @case = 100;
				SET @message = 'Clan created succesfully';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT c.[ClanId], c.[Name], c.[Tag], c.[Description], c.[IsPublic], c.[BadgeGK], c.[Flag], c.[Capacity],
	(SELECT COUNT(m.[ClanMemberId]) FROM [dbo].[ClanMember] as m WHERE m.[ClanId] = c.[ClanId]) as MemberCount
	FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @clanId;
	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END