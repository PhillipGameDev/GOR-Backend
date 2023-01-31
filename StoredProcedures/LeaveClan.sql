USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[LeaveClan]    Script Date: 1/10/2023 10:14:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[LeaveClan]
	@PlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;

	BEGIN TRY
		DECLARE @currentId INT = NULL, @currentId2 INT = NULL;
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Account does not exists';
			END
		ELSE
			BEGIN
				DECLARE @existingClanMemberId INT = NULL;
				SELECT @existingClanMemberId = c.[ClanMemberId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId;

				IF (@existingClanMemberId IS NULL)
					BEGIN
						SET @case = 201;
						SET @message = 'You are not a member';
					END
				ELSE
					BEGIN
						DELETE FROM [dbo].[ClanMember] WHERE [ClanMemberId] = @existingClanMemberId;
						SET @case = 100;
						SET @message = 'You have left the group';
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