
USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetClanJoinRequests]    Script Date: 10/16/2023 5:06:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetClanJoinRequests]
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
		DECLARE @roldId1 INT = NULL, @roldId2 INT = NULL, @roldId3 INT = NULL;
		SELECT @roldId1 = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Owner';
		SELECT @roldId2 = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Admin';
		SELECT @roldId3 = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Moderator';
		SELECT @currentRoleId = c.[ClanRoleId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId AND c.[ClanId] = @existingClanId;

		IF (@currentId IS NULL)
			BEGIN
				SET @currentId = NULL;
				SET @existingClanId = NULL;
				SET @case = 200;
				SET @message = 'Account does not exists';
			END
		ELSE IF (@existingClanId IS NULL)
			BEGIN
				SET @currentId = NULL;
				SET @existingClanId = NULL;
				SET @case = 201;
				SET @message = 'Clan does not exists';
			END
		ELSE IF (@currentRoleId = @roldId1 OR @currentRoleId = @roldId2 OR @currentRoleId = @roldId3)
			BEGIN
				SET @userId = @currentId;
				SET @tempCId = @existingClanId;
				SET @case = 100;
				SET @message = 'Clan join request list fetched succesfully';
			END
		ELSE
			BEGIN
				SET @currentId = NULL;
				SET @existingClanId = NULL;
				SET @case = 202;
				SET @message = 'Member does not have invite access ';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT c.*, p.Name FROM [dbo].[ClanJoinRequest] as c 
	INNER JOIN [dbo].[Player] as p ON p.[PlayerId] = c.[PlayerId]
	WHERE c.[ClanId] = @tempCId;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END