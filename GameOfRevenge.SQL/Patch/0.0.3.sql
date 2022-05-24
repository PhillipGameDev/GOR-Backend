USE [GameOfRevenge]
GO

CREATE OR ALTER PROCEDURE [dbo].[AddFirstTimeData]
	@Id INT
AS
BEGIN
	PRINT 'ADDED';
END
GO


CREATE OR ALTER PROCEDURE [dbo].[TryLoginOrRegister]
	@Identifier VARCHAR(1000),
	@Name VARCHAR(1000),
	@Accepted BIT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	DECLARE @tempIdentifier VARCHAR(1000) = LTRIM(RTRIM(@Identifier));
	DECLARE @tempName VARCHAR(1000) = LTRIM(RTRIM(ISNULL(@Name,'GuestAccount')));
	DECLARE @tempAccepted INT = ISNULL(@Accepted, 0);
	DECLARE @existingAccount INT = NULL;

	BEGIN TRY
		IF (@tempIdentifier IS NULL OR @tempIdentifier = '')
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
				SELECT @existingAccount = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerIdentifier] = @tempIdentifier;
				IF (@existingAccount IS NULL)
					BEGIN
						INSERT INTO [dbo].[Player] (PlayerIdentifier, Name, AcceptedTermAndCondition, IsAdmin, IsDeveloper) 
						VALUES (@tempIdentifier, @tempName, @tempAccepted, 0, 0);
						SELECT @existingAccount = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerIdentifier] = @tempIdentifier; 
						EXEC [dbo].[AddFirstTimeData] @existingAccount;
						SET @case = 100;
						SET @message = 'Created new account succesfully';
					END
				ELSE
					BEGIN
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

	SELECT p.[PlayerId], p.[PlayerIdentifier], p.[RavasAccountId], p.[Name], p.[AcceptedTermAndCondition], p.[IsAdmin], p.[IsDeveloper], p.[WorldId], p.[WorldTileId]
	FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @existingAccount;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO


CREATE OR ALTER PROCEDURE [dbo].[GetPlayerDetailsById]
	@PlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;

	DECLARE @existingId INT = NULL;

	SELECT @existingId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

	IF (@existingId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'No existing account found';
		END
	ELSE 
		BEGIN
			SET @case = 100;
			SET @message = 'Fetched existing account succesfully';
		END

	SELECT p.[PlayerId], p.[PlayerIdentifier], p.[RavasAccountId], p.[Name], p.[AcceptedTermAndCondition], p.[IsAdmin], p.[IsDeveloper], p.[WorldId], p.[WorldTileId]
	FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @existingId;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;

END
GO


CREATE OR ALTER PROCEDURE [dbo].[GetPlayerDetailsByIdentifier]
	@Identifier VARCHAR(1000)
AS
BEGIN
	DECLARE @tIdentifieer VARCHAR(1000) = LTRIM(RTRIM(@Identifier))
	DECLARE @id INT = NULL;
	SELECT @id = p.[PlayerId] FROM [dbo].[Player] AS p
	EXEC [dbo].[GetPlayerDetailsById] @id
END
GO




--CREATE OR ALTER PROCEDURE [dbo].[GetPlayerDetailsByRavasId]
--	@PlayerId INT
--AS
--BEGIN
--	DECLARE @case INT = 1, @error INT = 0;
--	DECLARE @tempuserId INT = NULL;
--	DECLARE @message NVARCHAR(MAX) = NULL;
--	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
--	DECLARE @userId INT = @PlayerId;

--	DECLARE @existingId INT = NULL;
--	SELECT TOP 1 @existingId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[RavasAccountId] = @userId;

--	IF (@userId IS NULL)
--		BEGIN
--			SET @case = 200;
--			SET @message = 'No existing account found';
--			SET @existingId = NULL;
--		END
--	IF (@existingId IS NULL)
--		BEGIN
--			SET @case = 200;
--			SET @message = 'No existing account found';
--		END
--	ELSE 
--		BEGIN
--			SET @case = 100;
--			SET @message = 'Fetched existing account succesfully';
--		END

--	SELECt p.[PlayerId], p.[PlayerIdentifier], p.[RavasAccountId], p.[Name], p.[AcceptedTermAndCondition], p.[IsAdmin], p.[IsDeveloper] 
--	FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @existingId;

--	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;

--END
--GO
