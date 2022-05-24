USE [GameOfRevenge]
GO

CREATE TABLE [dbo].[Clan]
(
	ClanId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(100) NOT NULL,
	Tag VARCHAR(10) NOT NULL,
	IsPublic BIT NOT NULL,
	Description VARCHAR(5000) NOT NULL,
	CONSTRAINT [PK_Clan_ClanId] PRIMARY KEY CLUSTERED (ClanId ASC),
	CONSTRAINT [UQ_Clan_Tag] UNIQUE NONCLUSTERED (Tag ASC)
)
GO

CREATE TABLE [dbo].[ClanRole]
(
	ClanRoleId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(100) NOT NULL,
	Code VARCHAR(100) NOT NULL,
	CONSTRAINT [PK_ClanRole_ClanRoleId] PRIMARY KEY CLUSTERED (ClanRoleId ASC),
	CONSTRAINT [UQ_ClanRole_Code] UNIQUE NONCLUSTERED (Code ASC)
)
GO

INSERT INTO [dbo].[ClanRole] VALUES ('Owner', 'Owner'), ('Admin', 'Admin'), ('Moderator', 'Moderator'), ('Member', 'Member')
GO

CREATE TABLE [dbo].[ClanMember]
(
	ClanMemberId INT IDENTITY(1,1) NOT NULL,
	ClanId INT NOT NULL,
	PlayerId INT NOT NULL,
	ClanRoleId INT NOT NULL,
	CONSTRAINT [PK_ClanMember_ClanMemberId] PRIMARY KEY CLUSTERED (ClanMemberId ASC),
	CONSTRAINT [UQ_ClanMember_Unique] UNIQUE NONCLUSTERED (PlayerId),
	CONSTRAINT [FK_ClanMember_MemberId] FOREIGN KEY (PlayerId) REFERENCES [dbo].[Player] (PlayerId),
	CONSTRAINT [FK_ClanMember_ClanRole_ClanRoleId] FOREIGN KEY (ClanRoleId) REFERENCES [dbo].[ClanRole] (ClanRoleId)
)
GO


CREATE TABLE [dbo].[ClanInvite]
(
	ClanInviteId INT IDENTITY(1,1) NOT NULL,
	ClanId INT NOT NULL,
	FromPlayerId INT NOT NULL,
	ToPlayerId INT NOT NULL,
	CONSTRAINT [PK_ClanInvite_ClanInviteId] PRIMARY KEY CLUSTERED (ClanInviteId ASC),
	CONSTRAINT [FK_ClanInvite_FromPlayerId] FOREIGN KEY (FromPlayerId) REFERENCES [dbo].[Player] (PlayerId),
	CONSTRAINT [FK_ClanInvite_ToPlayerId] FOREIGN KEY (ToPlayerId) REFERENCES [dbo].[Player] (PlayerId),
	CONSTRAINT [UQ_ClanInvite_Unique] UNIQUE NONCLUSTERED (ClanId, ToPlayerId),
)
GO

CREATE TABLE [dbo].[ClanJoinRequest]
(
	ClanJoinRequestId INT IDENTITY(1,1) NOT NULL,
	ClanId INT NOT NULL,
	PlayerId INT NOT NULL,
	Message VARCHAR(1000) NULL,
	CONSTRAINT [PK_ClanJoinRequest_ClanJoinRequestId] PRIMARY KEY CLUSTERED (ClanJoinRequestId ASC),
	CONSTRAINT [FK_ClanJoinRequest_PlayerId] FOREIGN KEY (PlayerId) REFERENCES [dbo].[Player] (PlayerId),
	CONSTRAINT [UQ_ClanJoinRequest_Unique] UNIQUE NONCLUSTERED (ClanId, PlayerId),
)
GO

CREATE OR ALTER PROCEDURE [dbo].[CreateClan]
	@PlayerId INT,
	@Name VARCHAR(100),
	@Tag VARCHAR(10),
	@Description VARCHAR(5000),
	@IsPublic BIT
AS
BEGIN
	
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @tempName VARCHAR(100) = LTRIM(RTRIM(@Name));
	DECLARE @tempTag VARCHAR(10) = LTRIM(RTRIM(@Tag));
	DECLARE @tempDescription VARCHAR(5000) = LTRIM(RTRIM(@Description));
	DECLARE @clanId INT = NULL;
	DECLARE @tisp BIT = ISNULL(@IsPublic, 0);

	BEGIN TRY
		DECLARE @currentId INT = NULL;
		DECLARE @currentClanMemberId INT = NULL;
		DECLARE @nameTag VARCHAR(10) = NULL;

		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

		DELETE FROM [dbo].[ClanInvite] WHERE [ToPlayerId] = @currentId;
		DELETE FROM [dbo].[ClanJoinRequest] WHERE [PlayerId] = @currentId;

		SELECT @nameTag = c.[Tag] FROM [dbo].[Clan] AS c WHERE c.[Tag] = @tempTag;
		SELECT @currentClanMemberId = c.[ClanMemberId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId;

		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Account does not exists';
			END
		ELSE IF (@nameTag IS NOT NULL)
			BEGIN
				SET @case = 201;
				SET @message = 'Clan with same tag already exists';
			END
		ELSE IF (@currentClanMemberId IS NOT NULL)
			BEGIN
				SET @case = 202;
				SET @message = 'Member is already part of another clan';
			END
		ELSE
			BEGIN
				--add clan
				INSERT INTO [dbo].[Clan] (Name, Tag, Description, IsPublic)
				VALUES (@tempName, @tempTag, @tempDescription, @tisp);

				--add owner role
				SELECT @clanId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[Tag] = @tempTag;
				DECLARE @roldId INT = NULL;
				SELECT @roldId = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Owner';
				INSERT INTO [dbo].[ClanMember] VALUES (@clanId, @userId, @roldId);

				SET @nameTag = @tempTag;
				SET @case = 100;
				SET @message = 'Clan created succesfully';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT c.[ClanId], c.[Name], c.[Tag], c.[Description], c.[IsPublic]
	FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @clanId;
	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[DeleteClan]
	@PlayerId INT,
	@ClanId INT
AS
BEGIN

	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @tempCId INT = @ClanId;

	BEGIN TRY
		DECLARE @currentId INT = NULL;
		DECLARE @existingClanId INT = NULL;
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;
		SELECT @existingClanId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @tempCId;

		DECLARE @currentRoleId INT = NULL;
		DECLARE @roldId INT = NULL;
		SELECT @roldId = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Owner';
		SELECT @currentRoleId = c.[ClanRoleId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId AND c.[ClanId] = @existingClanId;

		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Account does not exists';
			END
		ELSE IF (@existingClanId IS NULL)
			BEGIN
				SET @case = 201;
				SET @message = 'Clan does not exists';
			END
		ELSE IF (@currentRoleId = @roldId)
			BEGIN
				DELETE FROM [dbo].[ClanInvite] WHERE [ClanId] = @existingClanId;
				DELETE FROM [dbo].[ClanJoinRequest] WHERE [ClanId] = @existingClanId;
				DELETE FROM [dbo].[ClanMember] WHERE [ClanId] = @existingClanId;
				DELETE FROM [dbo].[Clan] WHERE [ClanId] = @existingClanId;

				SET @case = 100;
				SET @message = 'Clan deleted succesfully';

			END
		ELSE
			BEGIN
				SET @case = 202;
				SET @message = 'Member does not have Ownership access ';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetClanData]
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

	SELECT c.[ClanId], c.[Name], c.[Tag], c.[Description], c.[IsPublic]
	FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @currentCId;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetClanMembers]
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
				SET @message = 'Clan members list fetched succesfully';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT c.[ClanMemberId], c.[ClanId], r.[Code] AS 'Role', c.[PlayerId] FROM [dbo].[ClanMember] AS c 
	INNER JOIN [dbo].[ClanRole] AS r ON r.[ClanRoleId] = c.[ClanRoleId]
	WHERE c.[ClanId] = @currentCId;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetClanInvites]
	@PlayerId INT,
	@ClanId INT
AS
BEGIN

	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @tempCId INT = @ClanId;

	BEGIN TRY
		DECLARE @currentId INT = NULL;
		DECLARE @existingClanId INT = NULL;
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;
		SELECT @existingClanId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @tempCId;

		DECLARE @currentRoleId INT = NULL;
		DECLARE @roldId1 INT = NULL, @roldId2 INT = NULL, @roldId3 INT = NULL;
		SELECT @roldId1 = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Owner';
		SELECT @roldId2 = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Admin';
		SELECT @roldId3 = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Moderator';
		SELECT @currentRoleId = c.[ClanRoleId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId AND c.[ClanId] = @existingClanId;

		IF (@currentId IS NULL)
			BEGIN
				SET @currentId = NULL;
				SET @existingClanId = NULL;
				SET @case = 200;
				SET @message = 'Account does not exists';
			END
		ELSE IF (@existingClanId IS NULL)
			BEGIN
				SET @currentId = NULL;
				SET @existingClanId = NULL;
				SET @case = 201;
				SET @message = 'Clan does not exists';
			END
		ELSE IF (@currentRoleId = @roldId1 OR @currentRoleId = @roldId2 OR @currentRoleId = @roldId3)
			BEGIN
				SET @userId = @currentId;
				SET @tempCId = @existingClanId;
				SET @case = 100;
				SET @message = 'Clan invites list fetched succesfully';
			END
		ELSE
			BEGIN
				SET @currentId = NULL;
				SET @existingClanId = NULL;
				SET @case = 202;
				SET @message = 'Member does not have invite access ';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT c.[ClanInviteId], c.[ClanId], c.[FromPlayerId], c.[ToPlayerId]
	FROM [dbo].[ClanInvite] AS c 
	WHERE c.[ClanId] = @tempCId;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetClanJoinRequests]
	@PlayerId INT,
	@ClanId INT
AS
BEGIN

	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @tempCId INT = @ClanId;

	BEGIN TRY
		DECLARE @currentId INT = NULL;
		DECLARE @existingClanId INT = NULL;
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;
		SELECT @existingClanId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @tempCId;

		DECLARE @currentRoleId INT = NULL;
		DECLARE @roldId1 INT = NULL, @roldId2 INT = NULL, @roldId3 INT = NULL;
		SELECT @roldId1 = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Owner';
		SELECT @roldId2 = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Admin';
		SELECT @roldId3 = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Moderator';
		SELECT @currentRoleId = c.[ClanRoleId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId AND c.[ClanId] = @existingClanId;

		IF (@currentId IS NULL)
			BEGIN
				SET @currentId = NULL;
				SET @existingClanId = NULL;
				SET @case = 200;
				SET @message = 'Account does not exists';
			END
		ELSE IF (@existingClanId IS NULL)
			BEGIN
				SET @currentId = NULL;
				SET @existingClanId = NULL;
				SET @case = 201;
				SET @message = 'Clan does not exists';
			END
		ELSE IF (@currentRoleId = @roldId1 OR @currentRoleId = @roldId2 OR @currentRoleId = @roldId3)
			BEGIN
				SET @userId = @currentId;
				SET @tempCId = @existingClanId;
				SET @case = 100;
				SET @message = 'Clan join request list fetched succesfully';
			END
		ELSE
			BEGIN
				SET @currentId = NULL;
				SET @existingClanId = NULL;
				SET @case = 202;
				SET @message = 'Member does not have invite access ';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT c.[ClanJoinRequestId], c.[ClanId], c.[PlayerId], c.[Message]
	FROM [dbo].[ClanJoinRequest] AS c 
	WHERE c.[ClanId] = @tempCId;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetClans]
	@Tag VARCHAR(100) = '',
	@Name VARCHAR(10) = '',
	@AndClause BIT = 1,
	@Page INT = 1,
	@Count INT = 10
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	DECLARE @tclause BIT = ISNULL(@AndClause, 1);
	DECLARE @tTag VARCHAR(10) = '%' + LTRIM(RTRIM(ISNULL(@Tag,''))) + '%';
	DECLARE @tName VARCHAR(100)  = '%' + LTRIM(RTRIM(ISNULL(@Name,''))) + '%';

	DECLARE @tempPage INT = IIF(@Page < 1, 1, @Page), @tempCount INT = IIF(@Count < 1, 1, @Count);
	DECLARE @tempSkipCount INT = (@tempPage-1) * @tempCount
	DECLARE @maxRows FLOAT = 0, @printCount INT;

	DECLARE @addtag BIT = 0, @addName BIT = 0;
	IF(@tTag != '%%') SET @addtag = 1;
	IF(@tName != '%%') SET @addName = 1

	IF (@addtag = 1 AND @addName = 1) 
		BEGIN
			IF (@tclause = 1) 
				SELECT @maxRows = COUNT(*) FROM [dbo].[Clan] WHERE [Name] LIKE @tName AND [Tag] LIKE @tTag;
			ELSE 
				SELECT @maxRows = COUNT(*) FROM [dbo].[Clan] WHERE [Name] LIKE @tName OR [Tag] LIKE @tTag;
		END
	ELSE IF (@addtag = 1) SELECT @maxRows = COUNT(*) FROM [dbo].[Clan] WHERE [Tag] LIKE @tTag;
	ELSE IF (@addName = 1) SELECT @maxRows = COUNT(*) FROM [dbo].[Clan] WHERE [Name] LIKE @tName;
	ELSE SELECT @maxRows = COUNT(*) FROM [dbo].[Clan];
	
	DECLARE @tempTable TABLE
	(
		ClanId INT NOT NULL,
		Name VARCHAR(100) NOT NULL,
		Tag VARCHAR(10) NOT NULL,
		Description VARCHAR(MAX) NOT NULL,
		IsPublic BIT NULL
	)

	BEGIN TRY

	IF (@addtag = 1 AND @addName = 1)
		BEGIN
			IF (@tclause = 1) 
				INSERT INTO @tempTable
				SELECT c.[ClanId], c.[Name], c.[Tag], c.[Description], c.[IsPublic]
				FROM [dbo].[Clan] AS c 
				WHERE c.[Name] LIKE @tName AND c.[Tag] LIKE @tTag
				ORDER BY c.[ClanId]
				OFFSET @tempSkipCount ROWS FETCH NEXT @tempCount ROWS ONLY;
			ELSE 
				INSERT INTO @tempTable
				SELECT c.[ClanId], c.[Name], c.[Tag], c.[Description], c.[IsPublic]
				FROM [dbo].[Clan] AS c 
				WHERE c.[Name] LIKE @tName OR c.[Tag] LIKE @tTag
				ORDER BY c.[ClanId]
				OFFSET @tempSkipCount ROWS FETCH NEXT @tempCount ROWS ONLY;
		END
		
	ELSE IF (@addtag = 1)
		INSERT INTO @tempTable
		SELECT c.[ClanId], c.[Name], c.[Tag], c.[Description], c.[IsPublic]
		FROM [dbo].[Clan] AS c
		WHERE c.[Tag] LIKE @tTag
		ORDER BY c.[ClanId]
		OFFSET @tempSkipCount ROWS FETCH NEXT @tempCount ROWS ONLY;
	ELSE IF (@addName = 1)
		INSERT INTO @tempTable
		SELECT c.[ClanId], c.[Name], c.[Tag], c.[Description], c.[IsPublic]
		FROM [dbo].[Clan] AS c
		WHERE c.[Name] LIKE @tName
		ORDER BY c.[ClanId]
		OFFSET @tempSkipCount ROWS FETCH NEXT @tempCount ROWS ONLY;
	ELSE
		INSERT INTO @tempTable
		SELECT c.[ClanId], c.[Name], c.[Tag], c.[Description], c.[IsPublic]
		FROM [dbo].[Clan] AS c
		ORDER BY c.[ClanId]
		OFFSET @tempSkipCount ROWS FETCH NEXT @tempCount ROWS ONLY;

		SET @case = 100;
		SET @message = 'Clan list'
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT @printCount = COUNT(*) FROM @tempTable;
	SELECT * FROM @tempTable;


	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;

	SELECT
		CONVERT(int, CEILING(@maxRows/@tempCount)) AS 'MaxPages',
		CONVERT(int, @maxRows) AS 'MaxRows', 
		@tempPage AS 'CurrentPage',
		@tempCount AS 'CurrentCount', 
		@tempSkipCount AS 'StartIndex', 
		@tempSkipCount + @printCount AS 'EndIndex'
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetPlayerClanData]
	@PlayerId INT
AS
BEGIN

	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @clanId INT  = NULL;

	BEGIN TRY
		DECLARE @currentId INT = NULL;
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Account does not exists';
			END
		ELSE
			BEGIN
				SELECT @clanId = c.[ClanId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId;
				IF (@clanId IS NULL)
					BEGIN
						SET @case = 100;
						SET @message = 'Player does not have any clan data';
					END
				ELSE 
					BEGIN
						SET @case = 101;
						SET @message = 'Player current clan data';
					END
				
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT c.[ClanId], c.[Name], c.[Tag], c.[Description], c.[IsPublic]
	FROM [dbo].[Clan] AS c 
	WHERE c.[ClanId] = @clanId;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetPlayerClanInvitations]
	@PlayerId INT
AS
BEGIN

	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @currentId INT = NULL;

	BEGIN TRY
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Account does not exists';
			END
		ELSE
			BEGIN
				SET @case = 100;
				SET @message = 'Player all clan invites';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT c.[ClanInviteId], c.[ClanId], c.[FromPlayerId], c.[ToPlayerId]
	FROM [dbo].[ClanInvite] AS c 
	WHERE c.[ToPlayerId] = @currentId;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO



--CREATE OR ALTER PROCEDURE [dbo].[InviteMemberToClan]
--	@PlayerId INT,
--	@TargetPlayerId INT,
--	@ClanId INT
--AS
--BEGIN
--	DECLARE @case INT = 1, @error INT = 0;
--	DECLARE @tempuserId INT = NULL;
--	DECLARE @message NVARCHAR(MAX) = NULL;
--	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
--	DECLARE @userId INT = @PlayerId;
--	DECLARE @ttuserId INT = @TargetPlayerId;
--	DECLARE @tempCId INT = @ClanId;

--	BEGIN TRY
--		DECLARE @currentId1 INT = NULL, @currentId2 INT = NULL;
--		SELECT @currentId1 = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;
--		SELECT @currentId2 = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @ttuserId;

--		DECLARE @existingClanId INT = NULL;
--		SELECT @existingClanId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @tempCId;

--		DECLARE @currentRoleId INT = NULL;
--		DECLARE @roldId1 INT = NULL, @roldId2 INT = NULL;
--		SELECT @roldId1 = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Owner'
--		SELECT @roldId2 = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Admin';
--		SELECT @currentRoleId = c.[ClanRoleId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId1 AND c.[ClanId] = @existingClanId;

--		DECLARE @existingClanMemberId INT = NULL;
--		SELECT @existingClanMemberId = c.[ClanMemberId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId2;

--		DECLARE @existingInviteFroClanId INT = NULL;
--		SELECT @existingInviteFroClanId = c.[ClanInviteId] FROM [dbo].[ClanInvite] AS c WHERE c.[PlayerId] = @currentId2 AND c.[ClanId] = @existingClanId AND c.[IsInvite] = 1;

--		select @existingInviteFroClanId

--		IF (@currentId1 IS NULL)
--			BEGIN
--				SET @case = 200;
--				SET @message = 'Account does not exists';
--			END
--		ELSE IF (@currentId2 IS NULL)
--			BEGIN
--				SET @case = 201;
--				SET @message = 'Target account does not exists';
--			END
--		ELSE IF (@existingClanId IS NULL)
--			BEGIN
--				SET @case = 203;
--				SET @message = 'Clan does not exists';
--			END
--		ELSE IF (@existingClanMemberId IS NOT NULL)
--			BEGIN
--				SET @case = 202;
--				SET @message = 'Target player already in a clan';
--			END
--		ELSE IF (@existingInviteFroClanId IS NOT NULL)
--			BEGIN
--				SET @case = 205;
--				SET @message = 'Already invited to the clan';
--			END
--		ELSE IF (@currentRoleId = @roldId1 OR @currentRoleId = @roldId2)
--			BEGIN
--				INSERT INTO [dbo].[ClanInvite] VALUES (@existingClanId, @currentId2, 1, NULL)
--				SET @case = 100;
--				SET @message = 'Invited to clan succesfully';
--			END
--		ELSE
--			BEGIN
--				SET @case = 204;
--				SET @message = 'Member does not have invite access';
--			END
--	END TRY
--	BEGIN CATCH
--		SET @case = 0;
--		SET @error = 1;
--		SET @message = ERROR_MESSAGE();
--	END CATCH

--	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
--END
--GO


CREATE OR ALTER PROCEDURE [dbo].[RequestJoiningToClan]
	@PlayerId INT,
	@ClanId INT,
	@AboutMe VARCHAR(1000) = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @tempCId INT = @ClanId;
	DECLARE @tAboutMe VARCHAR(1000) = LTRIM(RTRIM(@AboutMe));

	BEGIN TRY
		DECLARE @currentId INT = NULL, @currentId2 INT = NULL;
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

		DECLARE @isPublic BIT = NULL;
		DECLARE @existingClanId INT = NULL;
		SELECT @existingClanId = c.[ClanId], @isPublic = c.[IsPublic] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @tempCId;
		SET @isPublic = ISNULL(@isPublic, 0);

		DECLARE @existingClanMemberId INT = NULL;
		SELECT @existingClanMemberId = c.[ClanMemberId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId;

		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Account does not exists';
			END
		ELSE IF (@existingClanId IS NULL)
			BEGIN
				SET @case = 203;
				SET @message = 'Clan does not exists';
			END
		ELSE IF (@existingClanMemberId IS NOT NULL)
			BEGIN
				SET @case = 202;
				SET @message = 'You are already in a clan';
			END
		ELSE
			BEGIN
				IF (@isPublic = 1)
					BEGIN
						DECLARE @roldId INT  = 4;
						SELECT @roldId = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Member'
						INSERT INTO [dbo].[ClanMember] VALUES (@existingClanId, @currentId, @roldId);
						
						DELETE FROM [dbo].[ClanInvite] WHERE [ToPlayerId] = @currentId;
						DELETE FROM [dbo].[ClanJoinRequest] WHERE [PlayerId] = @currentId;

						SET @case = 100;
						SET @message = 'Joined to clan succesfully';
					END
				ELSE
					BEGIN
						DECLARE @currentInvite INT = NULL;
						SELECT @currentInvite = c.[ClanJoinRequestId] 
						FROM [dbo].[ClanJoinRequest] AS c
						WHERE c.[ClanId] = @existingClanId AND c.[PlayerId] = @currentId

						IF (@currentInvite IS NULL) INSERT INTO [dbo].[ClanJoinRequest] VALUES (@existingClanId, @currentId, @tAboutMe)
						ELSE UPDATE [dbo].[ClanJoinRequest] SET [Message] = @tAboutMe
						WHERE [ClanId] = @existingClanId AND [PlayerId] = @currentId
						
						SET @case = 101;
						SET @message = 'Sended join request to clan succesfully';
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
GO

--CREATE OR ALTER PROCEDURE [dbo].[ReplyToJoinRequest]
--	@PlayerId INT,
--	@RequestId INT,
--	@Accept BIT
--AS
--BEGIN

--	DECLARE @case INT = 1, @error INT = 0;
--	DECLARE @tempuserId INT = NULL;
--	DECLARE @message NVARCHAR(MAX) = NULL;
--	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
--	DECLARE @userId INT = @PlayerId;
--	DECLARE @tRequestId INT = @RequestId;
--	DECLARE @tAccept BIT = @Accept;

--	BEGIN TRY
--		DECLARE @currentId INT = NULL;
--		DECLARE @existingClanId INT = NULL;
--		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;
--		SELECT @existingClanId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @tempCId;

--		DECLARE @currentRoleId INT = NULL;
--		DECLARE @roldId1 INT = NULL, @roldId2 INT = NULL, @roldId3 INT = NULL;
--		SELECT @roldId1 = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Owner';
--		SELECT @roldId2 = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Admin';
--		SELECT @roldId3 = r.[ClanRoleId] FROM [dbo].[ClanRole] AS r WHERE r.[Code] = 'Moderator';
--		SELECT @currentRoleId = c.[ClanRoleId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @currentId AND c.[ClanId] = @existingClanId;

--		IF (@currentId IS NULL)
--			BEGIN
--				SET @currentId = NULL;
--				SET @existingClanId = NULL;
--				SET @case = 200;
--				SET @message = 'Account does not exists';
--			END
--		ELSE IF (@existingClanId IS NULL)
--			BEGIN
--				SET @currentId = NULL;
--				SET @existingClanId = NULL;
--				SET @case = 201;
--				SET @message = 'Clan does not exists';
--			END
--		ELSE IF (@currentRoleId = @roldId1 OR @currentRoleId = @roldId2 OR @currentRoleId = @roldId3)
--			BEGIN
--				SET @userId = @currentId;
--				SET @tempCId = @existingClanId;
--				SET @case = 100;
--				SET @message = 'Clan join request list fetched succesfully';
--			END
--		ELSE
--			BEGIN
--				SET @currentId = NULL;
--				SET @existingClanId = NULL;
--				SET @case = 202;
--				SET @message = 'Member does not have invite access ';
--			END
--	END TRY
--	BEGIN CATCH
--		SET @case = 0;
--		SET @error = 1;
--		SET @message = ERROR_MESSAGE();
--	END CATCH

--	SELECT c.[ClanJoinRequestId], c.[ClanId], c.[PlayerId], c.[Message]
--	FROM [dbo].[ClanJoinRequest] AS c 
--	WHERE c.[ClanId] = @tempCId;

--	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
--END
--GO





--invite/accept/reject members
--promote/demote/kick members