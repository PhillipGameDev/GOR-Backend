USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[TryLoginOrRegister]    Script Date: 3/18/2023 11:35:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[TryLoginOrRegister]
	@Identifier VARCHAR(1000),
	@Accepted BIT,
	@Version INT = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @userId INT = NULL;

	DECLARE @tempIdentifier VARCHAR(1000) = LTRIM(RTRIM(ISNULL(@Identifier, '')));
	DECLARE @tempAccepted INT = ISNULL(@Accepted, 0);
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
		ELSE IF (@tempAccepted = 0)
			BEGIN
				SET @case = 201;
				SET @message = 'Accept terms and condition first';
			END
		ELSE
			BEGIN
				SELECT @existingAccount = p.[PlayerId], @existingFirebaseId = p.[FirebaseId], @existingVersion = p.[Version] 
				FROM [dbo].[Player] AS p WHERE (p.[FirebaseId] = @tempIdentifier) OR (p.[PlayerIdentifier] = @tempIdentifier);
				IF (@existingAccount IS NULL)
					BEGIN
						DECLARE @count INT = 0;
						SELECT @count = COUNT(*) FROM [dbo].[WorldTileData];
						IF (@count < 10000)
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
								VALUES (@tempIdentifier, @username, @tempAccepted, 0, 0, 0, @tempVersion, @time);

								SELECT @existingAccount = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerIdentifier] = @tempIdentifier; 
								EXEC [dbo].[AddFirstTimeData] @existingAccount;

								SET @case = 100;
								SET @message = 'Created new account succesfully';
							END
						ELSE
							BEGIN
								SET @case = 202;
								SET @message = 'Server capacity reached';
							END
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
							Invaded INT,
							VIPPoints INT,
							ClanId INT,
							LastLogin DATETIME
						);
						INSERT INTO @temp EXEC [dbo].[GetPlayerDetailsById] @existingAccount, 0;
						SET @info = (SELECT * FROM @temp FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

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

	SELECT p.[PlayerId], p.[PlayerIdentifier], p.[FirebaseId], p.[AcceptedTermAndCondition], 
			p.[IsAdmin], p.[IsDeveloper], p.[WorldTileId], p.[Name], p.[VIPPoints], 'Info' = @info
	FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @existingAccount;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END