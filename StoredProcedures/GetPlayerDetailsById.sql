USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerDetailsById]    Script Date: 6/15/2023 11:41:44 PM ******/
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
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @userId INT = @PlayerId;

	DECLARE @existingId INT = NULL;
	DECLARE @name NVARCHAR(200) = NULL;
	DECLARE @isAdmin BIT = NULL;
	DECLARE @isDeveloper BIT = NULL;
	DECLARE @kingLevel TINYINT = NULL;
	DECLARE @watchLevel TINYINT = NULL;
/*	DECLARE @vipLevel TINYINT = NULL;*/
	DECLARE @vipPoints INT = NULL;
	DECLARE @castleLevel TINYINT = NULL;
	DECLARE @shieldEndTime DATETIME = NULL;
	DECLARE @clanId INT = NULL;
	DECLARE @lastLogin DATETIME = NULL;

	SELECT @existingId = [PlayerId], @name = [Name], @isAdmin = [IsAdmin], @isDeveloper = [IsDeveloper],
			@vipPoints = [VIPPoints], @lastLogin = [LastLogin] FROM [dbo].[Player] 
	WHERE [PlayerId] = @userId;

	IF (@existingId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'No existing account found';
		END
	ELSE
		BEGIN
			BEGIN TRY
				DECLARE @json VARCHAR(MAX) = NULL;

	/*			SELECT @json = c.[Value] FROM [dbo].[PlayerData] as c WHERE c.[PlayerId] = @existingId AND c.[DataTypeId] = 7 AND c.[ValueId] = 3;
				IF (@json IS NOT NULL)
					BEGIN TRY
						SELECT
							@vipLevel = Level
						FROM OPENJSON (@json)
						WITH (Level TINYINT);
					END TRY
					BEGIN CATCH
					END CATCH*/

				/*castle level and watch level*/
				DECLARE @index INT = 1;
				DECLARE @castleValueId INT = 1;
				DECLARE @watchValueId INT = 3;
				WHILE @index <= 2
				BEGIN
				    SELECT @json = [Value] FROM [dbo].[PlayerData] WHERE [PlayerId] = @existingId AND [DataTypeId] = 2 
				        AND [ValueId] = (CASE WHEN @index = 1 THEN @castleValueId ELSE @watchValueId END);

				    SET @level = NULL;
				    IF (@json IS NOT NULL)
				    BEGIN
				        BEGIN TRY
				            SELECT
				                @level = ISNULL(Level, 0),
				                @startTime = CONVERT(DATETIMEOFFSET, StartTime),
				                @duration = ISNULL(Duration, 0)
				            FROM OPENJSON(@json)
				            WITH (Level TINYINT, StartTime VARCHAR(30), Duration INT);

				            IF (@duration > 0)
				            BEGIN
				                DECLARE @secs INT = @duration - DATEDIFF(ss, @startTime, @time);
				                IF (@secs > 0) SET @level -= 1;
				            END
				        END TRY
				        BEGIN CATCH
				        END CATCH
				    END
				    IF @index = 1
				        SET @castleLevel = @level;
				    ELSE
				        SET @watchLevel = @level;

				    SET @index += 1;
				END

				IF (@castleLevel IS NULL)
					BEGIN
						SET @case = 201;
						SET @message = 'No existing account';
					END
				ELSE
					BEGIN
						/*shield endtime*/
						SELECT @json = [Value] FROM [dbo].[PlayerData] WHERE [PlayerId] = @existingId AND [DataTypeId] = 10 AND [ValueId] = 1;
						IF (@json IS NOT NULL)
						BEGIN
							BEGIN TRY
								DECLARE @tempStartTime DATETIME = NULL;
								DECLARE @tempDuration INT = NULL;
								DECLARE @str VARCHAR(30);
								SELECT
									@tempStartTime = CONVERT(DATETIMEOFFSET, StartTime),
									@tempDuration = ISNULL(Duration, 0)
								FROM OPENJSON (@json)
								WITH (StartTime VARCHAR(30), Duration INT);

								IF (@tempDuration > 0)
									BEGIN
										SET @shieldEndTime = DATEADD(SECOND, @tempDuration, @tempStartTime);
										IF (@shieldEndTime < @time) SET @shieldEndTime = NULL;
									END
							END TRY
							BEGIN CATCH
							END CATCH
						END

						/*king level*/
						SELECT @json = [Value] FROM [dbo].[PlayerData] WHERE [PlayerId] = @existingId AND [DataTypeId] = 7 AND [ValueId] = 1;
						IF (@json IS NOT NULL)
						BEGIN
							BEGIN TRY
								SELECT
									@kingLevel = Level
								FROM OPENJSON (@json)
								WITH (Level TINYINT);
							END TRY
							BEGIN CATCH
							END CATCH
						END

						/*clan id*/
						SELECT @clanId = [ClanId] FROM [dbo].[ClanMember] WHERE [PlayerId] = @existingId;

						SET @case = 100;
						SET @message = 'Fetched existing account succesfully';
					END
			END TRY
			BEGIN CATCH
				SET @case = 0;
				SET @error = 1;
				SET @message = ERROR_MESSAGE();
			END CATCH
		END

	SELECT 'PlayerId' = @existingId, 'Name' = @name, 'IsAdmin' = @isAdmin, 'IsDeveloper' = @isDeveloper, 'KingLevel' = @kingLevel, 
			'CastleLevel' = @castleLevel, 'WatchLevel' = @watchLevel, 'ShieldEndTime' = @shieldEndTime,
			'VIPPoints' = @vipPoints, 'ClanId' = @clanId, 'LastLogin' = @lastLogin;

	IF (@Log = 1) EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END