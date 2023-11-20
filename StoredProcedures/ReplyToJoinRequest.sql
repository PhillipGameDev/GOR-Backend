USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[ReplyToJoinRequest]    Script Date: 11/20/2023 9:46:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[ReplyToJoinRequest]
	@PlayerId INT,
	@ClanId INT,
	@Accept BIT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @tempCId INT = @ClanId;
	DECLARE @tempAccept BIT = @Accept;

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
				DECLARE @roldId INT  = 4;
				SELECT @roldId = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Member';
				
				IF (@Accept = 1)
					BEGIN
						INSERT INTO [dbo].[ClanMember] VALUES (@existingClanId, @currentId, @roldId);
						DELETE FROM [dbo].[ClanJoinRequest] WHERE [PlayerId] = @currentId;
					END
				ELSE
					BEGIN
						DELETE FROM [dbo].[ClanJoinRequest] WHERE [ClanId] = @tempCId AND [PlayerId] = @currentId;
					END

				SET @case = 100;
				SET @message = 'Joined to clan succesfully';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END