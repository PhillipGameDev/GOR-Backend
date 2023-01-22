USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerDetailsById]    Script Date: 1/22/2023 9:04:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetPlayerDetailsById]
	@PlayerId INT,
	@Log BIT = 1
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;

	DECLARE @existingId INT = NULL;
	DECLARE @name VARCHAR(1000) = NULL;
	DECLARE @isAdmin BIT = NULL;
	DECLARE @isDeveloper BIT = NULL;
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
		BEGIN TRY
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

			SELECT @json = c.[Value] FROM [dbo].[PlayerData] as c WHERE c.[PlayerId] = @existingId AND c.[DataTypeId] = 2 AND c.[ValueId] = 1;
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

			SET @case = 100;
			SET @message = 'Fetched existing account succesfully';
		END TRY
		BEGIN CATCH
			SET @case = 0;
			SET @error = 1;
			SET @message = ERROR_MESSAGE();
		END CATCH

	SELECT 'PlayerId' = @existingId, 'Name' = @name, 'IsAdmin' = @isAdmin, 'IsDeveloper' = @isDeveloper, 'KingLevel' = @kingLevel, 'VIPLevel' = @vipLevel, 'CastleLevel' = @castleLevel, 'ClanId' = @clanId;

	IF (@Log = 1) EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END