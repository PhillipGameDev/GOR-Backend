USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetClanJoinReqs]    Script Date: 11/20/2023 9:34:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetClanJoinReqs]
	@ClanId INT
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
				SELECT c.*, p.Name FROM [dbo].[ClanJoinRequest] as c 
				INNER JOIN [dbo].[Player] as p ON p.[PlayerId] = c.[PlayerId]
				WHERE [ClanId] = @ClanId AND [IsNew] = 1;

				SET @case = 100;
				SET @message = 'Fetched Clan join requests Successfully!';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END