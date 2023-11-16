USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetClans]    Script Date: 10/17/2023 3:22:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetClans]
	@Tag VARCHAR(100) = '',
	@Name VARCHAR(10) = '',
	@AndClause BIT = 1,
	@Page INT = 1,
	@Count INT = 100
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
		IsPublic BIT NOT NULL,
		BadgeGK SMALLINT NOT NULL,
		Flag INT NOT NULL,
		Capacity INT NOT NULL,
		MemberCount INT NOT NULL
	)

	BEGIN TRY

	IF (@addtag = 1 AND @addName = 1)
		BEGIN
			IF (@tclause = 1) 
				INSERT INTO @tempTable
				SELECT c.[ClanId], c.[Name], c.[Tag], c.[Description], c.[IsPublic], c.[BadgeGK], c.[Flag], c.[Capacity],
				(SELECT COUNT(m.[ClanMemberId]) FROM [dbo].[ClanMember] as m WHERE m.[ClanId] = c.[ClanId]) as MemberCount
				FROM [dbo].[Clan] AS c 
				WHERE c.[Name] LIKE @tName AND c.[Tag] LIKE @tTag
				ORDER BY c.[ClanId]
				OFFSET @tempSkipCount ROWS FETCH NEXT @tempCount ROWS ONLY;
			ELSE 
				INSERT INTO @tempTable
				SELECT c.[ClanId], c.[Name], c.[Tag], c.[Description], c.[IsPublic], c.[BadgeGK], c.[Flag], c.[Capacity],
				(SELECT COUNT(m.[ClanMemberId]) FROM [dbo].[ClanMember] as m WHERE m.[ClanId] = c.[ClanId]) as MemberCount
				FROM [dbo].[Clan] AS c 
				WHERE c.[Name] LIKE @tName OR c.[Tag] LIKE @tTag
				ORDER BY c.[ClanId]
				OFFSET @tempSkipCount ROWS FETCH NEXT @tempCount ROWS ONLY;
		END
		
	ELSE IF (@addtag = 1)
		INSERT INTO @tempTable
		SELECT c.[ClanId], c.[Name], c.[Tag], c.[Description], c.[IsPublic], c.[BadgeGK], c.[Flag], c.[Capacity],
		(SELECT COUNT(m.[ClanMemberId]) FROM [dbo].[ClanMember] as m WHERE m.[ClanId] = c.[ClanId]) as MemberCount
		FROM [dbo].[Clan] AS c
		WHERE c.[Tag] LIKE @tTag
		ORDER BY c.[ClanId]
		OFFSET @tempSkipCount ROWS FETCH NEXT @tempCount ROWS ONLY;
	ELSE IF (@addName = 1)
		INSERT INTO @tempTable
		SELECT c.[ClanId], c.[Name], c.[Tag], c.[Description], c.[IsPublic], c.[BadgeGK], c.[Flag], c.[Capacity],
		(SELECT COUNT(m.[ClanMemberId]) FROM [dbo].[ClanMember] as m WHERE m.[ClanId] = c.[ClanId]) as MemberCount
		FROM [dbo].[Clan] AS c
		WHERE c.[Name] LIKE @tName
		ORDER BY c.[ClanId]
		OFFSET @tempSkipCount ROWS FETCH NEXT @tempCount ROWS ONLY;
	ELSE
		INSERT INTO @tempTable
		SELECT c.[ClanId], c.[Name], c.[Tag], c.[Description], c.[IsPublic], c.[BadgeGK], c.[Flag], c.[Capacity],
		(SELECT COUNT(m.[ClanMemberId]) FROM [dbo].[ClanMember] as m WHERE m.[ClanId] = c.[ClanId]) as MemberCount
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