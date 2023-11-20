USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpdateClanRole]    Script Date: 11/20/2023 9:51:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[UpdateClanRole]
	@ClanId INT,
	@PlayerId INT,
	@RoleId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	BEGIN TRY

		DECLARE @existingClanId INT = NULL;
		SELECT @existingClanId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @ClanId;

		DECLARE @existingPlayerId INT = NULL;
		SELECT @existingPlayerId = c.[PlayerId] FROM [dbo].[Player] AS c WHERE c.[PlayerId] = @PlayerId;

		DECLARE @existingRoleId INT = NULL;
		SELECT @existingRoleId = c.[ClanRoleId] FROM [dbo].[ClanRole] AS c WHERE c.[ClanRoleId] = @RoleId;

		DECLARE @existingMemberId INT = NULL;
		SELECT @existingMemberId = c.[ClanMemberId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @PlayerId AND c.[ClanId] = @ClanId;

		IF (@existingClanId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Clan does not exists';
			END
		ELSE IF (@existingPlayerId IS NULL)
			BEGIN
				SET @case = 201;
				SET @message = 'Player does not exists';
			END
		ELSE IF (@existingMemberId IS NULL)
			BEGIN
				SET @case = 202;
				SET @message = 'Player does not a member of the clan';
			END
		ELSE IF (@existingRoleId IS NULL)
			BEGIN
				SET @case = 203;
				SET @message = 'Player does not exists';
			END
		ELSE
			BEGIN
				DECLARE @mRoleId INT  = 4;
				SELECT @mRoleId = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Member'

				UPDATE [dbo].[ClanMember] SET [ClanRoleId] = @mRoleId WHERE [ClanId] = @ClanId AND [ClanRoleId] = @RoleId;
				UPDATE [dbo].[ClanMember] SET [ClanRoleId] = @RoleId WHERE [ClanMemberId] = @existingMemberId;

				SET @case = 100;
				SET @message = 'Update clan role successfully.';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END