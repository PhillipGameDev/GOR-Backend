USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetClanUnions]    Script Date: 11/20/2023 9:42:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetClanUnions]
	@ClanId INT,
	@Accepted BIT = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	BEGIN TRY
		DECLARE @existingClanId INT = NULL;
		SELECT @existingClanId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @ClanId;

		IF (@existingClanId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Clan does not exists';
			END
		ELSE
			BEGIN
				SELECT u.*, c.*,
				(SELECT COUNT(m.[ClanMemberId]) FROM [dbo].[ClanMember] as m WHERE m.[ClanId] = c.[ClanId]) as MemberCount
				FROM [dbo].[Union] as u 
				INNER JOIN [dbo].[Clan] as c ON c.[ClanId] = u.[FromClanId]
				WHERE ([FromClanId] = @ClanId OR [ToClanId] = @ClanId) AND (@Accepted IS NULL OR [Accepted] = @Accepted)

				SET @case = 100;
				SET @message = 'Fetched Union Data Successfully!';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END