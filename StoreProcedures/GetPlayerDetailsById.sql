USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerDetailsById]    Script Date: 1/18/2023 5:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetPlayerDetailsById]
	@PlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;

	DECLARE @existingId INT = NULL;
	DECLARE @name VARCHAR(1000) = NULL;
	DECLARE @isAdmin BIT = 0;
	DECLARE @isDeveloper BIT = 0;
	DECLARE @kingLevel TINYINT = NULL;
	DECLARE @vipLevel TINYINT = NULL;
	DECLARE @castleLevel TINYINT = NULL;
	DECLARE @clanId INT = NULL;

	SELECT @existingId = p.[PlayerId], @name = p.[Name] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

	IF (@existingId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'No existing account found';
		END
	ELSE 
		BEGIN
			SET @case = 100;
			SET @message = 'Fetched existing account succesfully';

			DECLARE @json VARCHAR(MAX) = NULL;

			SELECT @json = c.[Value] FROM [dbo].[PlayerData] as c WHERE c.[PlayerId] = @existingId AND c.[DataTypeId] = 7 AND c.[ValueId] = 1;
			IF (@json IS NOT NULL)
				BEGIN TRY
					SELECT
						@kingLevel = Level
					FROM OPENJSON (@json)
					WITH (Level TINYINT);
				END TRY
				BEGIN CATCH
				END CATCH

			SELECT @json = c.[Value] FROM [dbo].[PlayerData] as c WHERE c.[PlayerId] = @existingId AND c.[DataTypeId] = 7 AND c.[ValueId] = 3;
			IF (@json IS NOT NULL)
				BEGIN TRY
					SELECT
						@vipLevel = Level
					FROM OPENJSON (@json)
					WITH (Level TINYINT);
				END TRY
				BEGIN CATCH
				END CATCH

			SELECT @json = c.[Value] FROM [dbo].[PlayerData] as c WHERE c.[PlayerId] = 71 AND c.[DataTypeId] = 2 AND c.[ValueId] = 1;
			IF (@json IS NOT NULL)
				BEGIN TRY
					DECLARE @tempStartTime DATETIME = NULL;
					DECLARE @tempDuration INT = NULL;
					DECLARE @str VARCHAR(30);
					SELECT
						@castleLevel = ISNULL(Level, 0),
						@tempStartTime = CONVERT(DATETIMEOFFSET, StartTime) ,
						@str = StartTime,
						@tempDuration = ISNULL(Duration, 0)
					FROM OPENJSON (@json)
					WITH (Level TINYINT, StartTime VARCHAR(30), Duration INT);

					IF (@tempDuration > 0)
						BEGIN
							DECLARE @secs INT = @tempDuration - DATEDIFF(ss, @tempStartTime, GETUTCDATE());
							IF (@secs > 0) SET @castleLevel -= 1;
						END
				END TRY
				BEGIN CATCH
				END CATCH

			SELECT @clanId = c.[ClanId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @existingId;
		END

	SELECT 'PlayerId' = @existingId, 'Name' = @name, 'IsAdmin' = @isAdmin, 'IsDeveloper' = @isDeveloper, 'KingLevel' = @kingLevel, 'VIPLevel' = @vipLevel, 'CastleLevel' = @castleLevel, 'ClanId' = @clanId;

/*	SELECT p.[PlayerId], p.[PlayerIdentifier], p.[RavasAccountId], p.[Name], p.[AcceptedTermAndCondition], 
			p.[IsAdmin], p.[IsDeveloper], p.[WorldId], p.[WorldTileId], 'Info' = @info
	FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @existingId;*/

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END