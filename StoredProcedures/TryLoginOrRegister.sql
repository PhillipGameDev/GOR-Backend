USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[TryLoginOrRegister]    Script Date: 12/22/2023 3:24:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[TryLoginOrRegister]
	@Identifier VARCHAR(1000),
	@Version INT = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @userId INT = NULL;

	DECLARE @tempIdentifier VARCHAR(1000) = LTRIM(RTRIM(ISNULL(@Identifier, '')));
	DECLARE @tempVersion INT = ISNULL(@Version, 0);

	DECLARE @existingAccount INT = NULL;
	DECLARE @existingFirebaseId VARCHAR(1000) = NULL;
	DECLARE @existingVersion INT = 0;
	DECLARE @info VARCHAR(1000) = NULL;

	BEGIN TRY
		IF (@tempIdentifier = '')
			BEGIN
				SET @case = 200;
				SET @message = 'Invalid Identifier';
			END
		ELSE
			BEGIN
				SELECT @existingAccount = p.[PlayerId], @existingFirebaseId = p.[FirebaseId], @existingVersion = p.[Version] 
				FROM [dbo].[Player] AS p WHERE (p.[FirebaseId] = @tempIdentifier) OR (p.[PlayerIdentifier] = @tempIdentifier);

				IF (@existingAccount IS NULL)
					BEGIN
						DECLARE @username VARCHAR(50);
						DECLARE @attempt INT = 1;
						DECLARE @maxAttempts INT = 5;
						DECLARE @val INT = 1000;

						WHILE (@attempt <= @maxAttempts) BEGIN
						    DECLARE @num INT = (ABS(CHECKSUM(NEWID())) % (@val * 9)) + @val
						    SET @username = 'Guest' + CAST(@num AS VARCHAR)
						    IF NOT EXISTS (SELECT 1 FROM Player WHERE Name = @username) BREAK;

						    SET @attempt = @attempt + 1
						    IF (@attempt = 6) OR (@attempt = 11)
						    BEGIN
							    SET @maxAttempts = @maxAttempts + 5;
							    SET @val = @val * 10;
						    END
						END

						INSERT INTO [dbo].[Player] (PlayerIdentifier, Name, AcceptedTermAndCondition, IsAdmin, IsDeveloper, VIPPoints, Version, LastLogin) 
						VALUES (@tempIdentifier, @username, 1, 0, 0, 0, @tempVersion, @time);

						SELECT @existingAccount = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerIdentifier] = @tempIdentifier; 
						EXEC [dbo].[AddFirstTimeData] @existingAccount;
						/*TODO: update AddFirstTimeData to add our current user default data, after that we should generate @info data*/

						SET @case = 100;
						SET @message = 'Created new account succesfully';
					END
				ELSE
					BEGIN
						/*IF (@tempVersion > @existingVersion) */
						UPDATE [dbo].[Player] SET [Version] = @tempVersion, [LastLogin] = @time WHERE [PlayerId] = @existingAccount;

						DECLARE @temp TABLE (
							PlayerId INT,
							Name VARCHAR(1000),
							IsAdmin BIT,
							IsDeveloper BIT,
							KingLevel TINYINT,
							CastleLevel TINYINT,
							WatchLevel TINYINT,
							ShieldEndTime DATETIME,
							VIPPoints INT,
							ClanId INT,
							RegisteredDate DATETIME,
							LastLogin DATETIME,
							WorldTileId INT
						);
						INSERT INTO @temp EXEC [dbo].[GetPlayerDetailsById] @existingAccount, 0;
						SET @info = (
						    SELECT 
						        PlayerId,
						        Name,
						        IsAdmin,
						        IsDeveloper,
						        KingLevel,
						        CastleLevel,
						        WatchLevel,
						        ShieldEndTime,
						        VIPPoints,
						        ClanId AS AllianceId,
						        RegisteredDate,
						        LastLogin,
								WorldTileId
						    FROM @temp
						    FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
						);

						SET @case = 101;
						SET @message = 'Fetched existing account succesfully';
					END
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT p.[PlayerId], p.[PlayerIdentifier], p.[FirebaseId], p.[AcceptedTermAndCondition], p.[IsAdmin], p.[IsDeveloper],
			p.[WorldTileId], wt.[X], wt.[Y], p.[Name], p.[VIPPoints], 'Info' = @info
	FROM [dbo].[Player] AS p
	LEFT JOIN [dbo].[WorldTileData] AS wt ON wt.[WorldTileDataId] = p.[WorldTileId]
	WHERE p.[PlayerId] = @existingAccount;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END