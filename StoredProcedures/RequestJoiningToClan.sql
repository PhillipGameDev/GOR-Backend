USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[RequestJoiningToClan]    Script Date: 10/18/2023 4:08:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--CREATE OR ALTER PROCEDURE [dbo].[InviteMemberToClan]
--	@PlayerId INT,
--	@TargetPlayerId INT,
--	@ClanId INT
--AS
--BEGIN
--	DECLARE @case INT = 1, @error INT = 0;
--	DECLARE @tempuserId INT = NULL;
--	DECLARE @message NVARCHAR(MAX) = NULL;
--	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
--	DECLARE @userId INT = @PlayerId;
--	DECLARE @ttuserId INT = @TargetPlayerId;
--	DECLARE @tempCId INT = @ClanId;

--	BEGIN TRY
--		DECLARE @currentId1 INT = NULL, @currentId2 INT = NULL;
--		SELECT @currentId1 = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;
--		SELECT @currentId2 = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @ttuserId;

--		DECLARE @existingClanId INT = NULL;
--		SELECT @existingClanId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @tempCId;

--		DECLARE @currentRoleId INT = NULL;
--		DECLARE @roldId1 INT = NULL, @roldId2 INT = NULL;
--		SELECT @roldId1 = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Owner'
--		SELECT @roldId2 = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Admin';
--		SELECT @currentRoleId = c.[ClanRoleId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId1 AND c.[ClanId] = @existingClanId;

--		DECLARE @existingClanMemberId INT = NULL;
--		SELECT @existingClanMemberId = c.[ClanMemberId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId2;

--		DECLARE @existingInviteFroClanId INT = NULL;
--		SELECT @existingInviteFroClanId = c.[ClanInviteId] FROM [dbo].[ClanInvite] AS c WHERE c.[PlayerId] = @currentId2 AND c.[ClanId] = @existingClanId AND c.[IsInvite] = 1;

--		select @existingInviteFroClanId

--		IF (@currentId1 IS NULL)
--			BEGIN
--				SET @case = 200;
--				SET @message = 'Account does not exists';
--			END
--		ELSE IF (@currentId2 IS NULL)
--			BEGIN
--				SET @case = 201;
--				SET @message = 'Target account does not exists';
--			END
--		ELSE IF (@existingClanId IS NULL)
--			BEGIN
--				SET @case = 203;
--				SET @message = 'Clan does not exists';
--			END
--		ELSE IF (@existingClanMemberId IS NOT NULL)
--			BEGIN
--				SET @case = 202;
--				SET @message = 'Target player already in a clan';
--			END
--		ELSE IF (@existingInviteFroClanId IS NOT NULL)
--			BEGIN
--				SET @case = 205;
--				SET @message = 'Already invited to the clan';
--			END
--		ELSE IF (@currentRoleId = @roldId1 OR @currentRoleId = @roldId2)
--			BEGIN
--				INSERT INTO [dbo].[ClanInvite] VALUES (@existingClanId, @currentId2, 1, NULL)
--				SET @case = 100;
--				SET @message = 'Invited to clan succesfully';
--			END
--		ELSE
--			BEGIN
--				SET @case = 204;
--				SET @message = 'Member does not have invite access';
--			END
--	END TRY
--	BEGIN CATCH
--		SET @case = 0;
--		SET @error = 1;
--		SET @message = ERROR_MESSAGE();
--	END CATCH

--	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
--END
--GO


ALTER   PROCEDURE [dbo].[RequestJoiningToClan]
	@PlayerId INT,
	@ClanId INT,
	@AboutMe VARCHAR(1000) = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @tempCId INT = @ClanId;
	DECLARE @tAboutMe VARCHAR(1000) = LTRIM(RTRIM(@AboutMe));

	BEGIN TRY
		DECLARE @currentId INT = NULL, @currentId2 INT = NULL;
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

		DECLARE @isPublic BIT = NULL;
		DECLARE @existingClanId INT = NULL;
		SELECT @existingClanId = c.[ClanId], @isPublic = c.[IsPublic] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @tempCId;
		SET @isPublic = ISNULL(@isPublic, 0);

		DECLARE @existingClanMemberId INT = NULL;
		SELECT @existingClanMemberId = c.[ClanMemberId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId;

		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Account does not exists';
			END
		ELSE IF (@existingClanId IS NULL)
			BEGIN
				SET @case = 203;
				SET @message = 'Clan does not exists';
			END
		ELSE IF (@existingClanMemberId IS NOT NULL)
			BEGIN
				SET @case = 202;
				SET @message = 'You are already in a clan';
			END
		ELSE
			BEGIN
				IF (@isPublic = 1)
					BEGIN
						DECLARE @roldId INT  = 4;
						SELECT @roldId = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Member'
						INSERT INTO [dbo].[ClanMember] VALUES (@existingClanId, @currentId, @roldId);
						
						DELETE FROM [dbo].[ClanInvite] WHERE [ToPlayerId] = @currentId;
						DELETE FROM [dbo].[ClanJoinRequest] WHERE [PlayerId] = @currentId;

						SET @case = 100;
						SET @message = 'Joined to clan succesfully';
					END
				ELSE
					BEGIN
						DECLARE @currentInvite INT = NULL;
						SELECT @currentInvite = c.[ClanJoinRequestId] 
						FROM [dbo].[ClanJoinRequest] AS c
						WHERE c.[ClanId] = @existingClanId AND c.[PlayerId] = @currentId

						IF (@currentInvite IS NULL) INSERT INTO [dbo].[ClanJoinRequest] VALUES (@existingClanId, @currentId, @tAboutMe)
						ELSE UPDATE [dbo].[ClanJoinRequest] SET [Message] = @tAboutMe
						WHERE [ClanId] = @existingClanId AND [PlayerId] = @currentId
						
						SET @case = 101;
						SET @message = 'Sended join request to clan succesfully';
					END
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END