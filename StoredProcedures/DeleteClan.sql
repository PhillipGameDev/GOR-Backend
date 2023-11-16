USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[DeleteClan]    Script Date: 10/18/2023 4:13:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[DeleteClan]
	@PlayerId INT,
	@ClanId INT
AS
BEGIN

	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @tempCId INT = @ClanId;

	BEGIN TRY
		DECLARE @currentId INT = NULL;
		DECLARE @existingClanId INT = NULL;
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;
		SELECT @existingClanId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @tempCId;

		DECLARE @currentRoleId INT = NULL;
		DECLARE @roldId INT = NULL;
		SELECT @roldId = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Owner';
		SELECT @currentRoleId = c.[ClanRoleId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId AND c.[ClanId] = @existingClanId;

		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Account does not exists';
			END
		ELSE IF (@existingClanId IS NULL)
			BEGIN
				SET @case = 201;
				SET @message = 'Clan does not exists';
			END
		ELSE IF (@currentRoleId = @roldId)
			BEGIN
				DELETE FROM [dbo].[ClanInvite] WHERE [ClanId] = @existingClanId;
				DELETE FROM [dbo].[ClanJoinRequest] WHERE [ClanId] = @existingClanId;
				DELETE FROM [dbo].[ClanMember] WHERE [ClanId] = @existingClanId;
				DELETE FROM [dbo].[Clan] WHERE [ClanId] = @existingClanId;

				SET @case = 100;
				SET @message = 'Clan deleted succesfully';

			END
		ELSE
			BEGIN
				SET @case = 202;
				SET @message = 'Member does not have Ownership access ';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END