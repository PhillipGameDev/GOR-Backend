USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetClanData]    Script Date: 10/16/2023 5:06:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetClanData]
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
				SET @message = 'Clan data fetched succesfully';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT c.[ClanId], c.[Name], c.[Tag], c.[Description], c.[IsPublic], c.[BadgeGK], c.[Flag], c.[Capacity], 
	(SELECT COUNT(m.[ClanMemberId]) FROM [dbo].[ClanMember] as m WHERE m.[ClanId] = c.[ClanId]) as MemberCount
	FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @currentCId;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END