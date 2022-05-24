USE [GameOfRevenge]
GO

--drop TABLE [dbo].[QuestUserDataRel]
--drop TABLE [dbo].[Quest]
--drop TABLE [dbo].[QuestType]
--drop TABLE [dbo].[Chapter]

CREATE TABLE [dbo].[Chapter]
(
	ChapterId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(1000) NOT NULL,
	Description VARCHAR(MAX) NOT NULL,
	Code VARCHAR(1000) NOT NULL,
	ChapOrder FLOAT NULL,
	CONSTRAINT [PK_Chapter_ChapterId] PRIMARY KEY CLUSTERED (ChapterId ASC),
	CONSTRAINT [UQ_Chapter_Code] UNIQUE (Code ASC),
	CONSTRAINT [UQ_Chapter_ChapOrder] UNIQUE (ChapOrder ASC)
)
GO

--constant data cannot change
CREATE TABLE [dbo].[QuestType]
(
	QuestTypeId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(MAX) NOT NULL,
	Code VARCHAR(1000) NOT NULL,
	CONSTRAINT [PK_QuestType_QuestTypeId] PRIMARY KEY CLUSTERED (QuestTypeId ASC),
	CONSTRAINT [UQ_QuestType_Code] UNIQUE (Code ASC)
)
GO

INSERT INTO [dbo].[QuestType] (Name, Code) VALUES
	('Building upgrade', 'BuildingUpgrade'),
	('Have x building count', 'XBuildingCount'),
	('Collection resource', 'CollectionResource'),
	('Train troops', 'TrainTroops'),
	('Have x troop count', 'XTroopCount')
GO

CREATE TABLE [dbo].[Quest]
(
	QuestId INT IDENTITY(1,1) NOT NULL, 
	ChapterId INT NOT NULL,
	QuestTypeId INT NOT NULL,
	MilestoneId INT NOT NULL,
	Data VARCHAR(MAX) NOT NULL,
	CONSTRAINT [PK_Quest_QuestId] PRIMARY KEY CLUSTERED (QuestId ASC),
	CONSTRAINT [FK_Quest_ChapterId] FOREIGN KEY (ChapterId) REFERENCES [dbo].[Chapter] (ChapterId),
	CONSTRAINT [FK_Quest_QuestTypeId] FOREIGN KEY (QuestTypeId) REFERENCES [dbo].[QuestType] (QuestTypeId),
	CONSTRAINT [UQ_Quest_Code] UNIQUE (ChapterId, QuestTypeId, MilestoneId)
)
GO

CREATE TABLE [dbo].[ChapterReward]
(
	ChapterRewardId INT IDENTITY(1,1) NOT NULL,
	ChapterId INT NOT NULL,
	DataTypeId INT NOT NULL,
	ReqValueId INT NULL,
	Value INT NOT NULL,
	CONSTRAINT [PK_ChapterReward_ChapterRewardId] PRIMARY KEY CLUSTERED (ChapterRewardId ASC),
	CONSTRAINT [UQ_ChapterReward_UniqueCode] UNIQUE NONCLUSTERED (ChapterId, DataTypeId, ReqValueId),
	CONSTRAINT [FK_ChapterReward_DataType_DataTypeId] FOREIGN KEY (DataTypeId) REFERENCES [dbo].[DataType] (DataTypeId),
	CONSTRAINT [FK_ChapterReward_ChapterId] FOREIGN KEY (ChapterId) REFERENCES [dbo].[Chapter] (ChapterId)
)
GO

CREATE TABLE [dbo].[QuestReward]
(
	QuestRewardId INT IDENTITY(1,1) NOT NULL,
	QuestId INT NOT NULL,
	DataTypeId INT NOT NULL,
	ReqValueId INT NULL,
	Value INT NOT NULL,
	CONSTRAINT [PK_QuestReward_QuestRewardId] PRIMARY KEY CLUSTERED (QuestRewardId ASC),
	CONSTRAINT [UQ_QuestReward_UniqueCode] UNIQUE NONCLUSTERED (QuestId, DataTypeId, ReqValueId),
	CONSTRAINT [FK_QuestReward_DataType_DataTypeId] FOREIGN KEY (DataTypeId) REFERENCES [dbo].[DataType] (DataTypeId),
	CONSTRAINT [FK_QuestReward_QuestId] FOREIGN KEY (QuestId) REFERENCES [dbo].[Quest] (QuestId)
)
GO

CREATE TABLE [dbo].[ChapterUserDataRel]
(
	ChapterUserDataRelId INT IDENTITY(1,1) NOT NULL, 
	ChapterId INT NOT NULL,
	PlayerId INT NOT NULL,
	IsRedeemed BIT NOT NULL,
	CONSTRAINT [PK_ChapterUserDataRel_ChapterUserDataRelId] PRIMARY KEY CLUSTERED (ChapterUserDataRelId ASC),
	CONSTRAINT [FK_ChapterUserDataRel_ChapterId] FOREIGN KEY (ChapterId) REFERENCES [dbo].[Chapter] (ChapterId),
	CONSTRAINT [FK_ChapterUserDataRel_PlayerId] FOREIGN KEY (PlayerId) REFERENCES [dbo].[Player] (PlayerId),
	CONSTRAINT [UQ_ChapterUserDataRel_Code] UNIQUE (ChapterId, PlayerId)
)
GO


CREATE TABLE [dbo].[QuestUserDataRel]
(
	QuestUserDataRelId INT IDENTITY(1,1) NOT NULL, 
	QuestId INT NOT NULL,
	PlayerId INT NOT NULL,
	IsCompleted BIT NOT NULL,
	CurrentData VARCHAR(MAX) NULL,
	IsRedeemed BIT NOT NULL,
	CONSTRAINT [PK_QuestUserDataRel_QuestUserDataRelId] PRIMARY KEY CLUSTERED (QuestUserDataRelId ASC),
	CONSTRAINT [FK_QuestUserDataRel_QuestId] FOREIGN KEY (QuestId) REFERENCES [dbo].[Quest] (QuestId),
	CONSTRAINT [FK_QuestUserDataRel_PlayerId] FOREIGN KEY (PlayerId) REFERENCES [dbo].[Player] (PlayerId),
	CONSTRAINT [UQ_QuestUserDataRel_Code] UNIQUE (QuestId, PlayerId)
)
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllChapters]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'All chapters'

	SELECT * FROM [dbo].[Chapter]
	ORDER BY [ChapOrder]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllQuestTypes]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'All Quests Types'

	SELECT [QuestTypeId], [Name], [Code] FROM [dbo].[QuestType]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllQuests]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'All Quests'

	SELECT q.[QuestId], q.[ChapterId], t.[Code] AS 'QuestType', q.[MilestoneId], q.[Data]  FROM [dbo].[Quest] AS q
	INNER JOIN [dbo].[Chapter] AS c ON c.[ChapterId] = q.[ChapterId]
	INNER JOIN [dbo].[QuestType] AS t ON t.[QuestTypeId] = q.[QuestTypeId]
	ORDER BY c.[ChapOrder], q.[MilestoneId]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllQuestRewards]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'Quest rewards list fetched succesfully';

	SELECT s.[QuestRewardId], s.[QuestId], s.[DataTypeId], s.[ReqValueId], s.[Value] FROM [dbo].[QuestReward] AS s

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllChapterRewards]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'Chapter rewards list fetched succesfully';

	SELECT s.[ChapterRewardId], s.[ChapterId], s.[DataTypeId], s.[ReqValueId], s.[Value] FROM [dbo].[ChapterReward] AS s

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO


CREATE OR ALTER PROCEDURE [dbo].[GetPlayerAllQuestData]
	@PlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = @PlayerId;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @existingId INT = NULL;

	SELECT @existingId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @tempuserId;

	IF (@existingId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'No existing account found';
		END
	ELSE
		BEGIN
			SET @case = 100;
			SET @message = 'Player all quest';
		END

	SELECT us.* FROM [dbo].[QuestUserDataRel] AS us
	--INNER JOIN [dbo].[Quest] AS q ON q.[QuestId] = us.[QuestId]
	--INNER JOIN [dbo].[Chapter] AS c ON c.[ChapterId] = q.[ChapterId]
	WHERE [PlayerId] = @existingId
	--ORDER BY c.[ChapOrder], q.[MilestoneId]

	EXEC [dbo].[GetMessage] @existingId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetPlayerAllChapterData]
	@PlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = @PlayerId;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @existingId INT = NULL;

	SELECT @existingId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @tempuserId;

	IF (@existingId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'No existing account found';
		END
	ELSE
		BEGIN
			SET @case = 100;
			SET @message = 'Player all quest';
		END

	SELECT us.* FROM [dbo].[ChapterUserDataRel] AS us
	--INNER JOIN [dbo].[Chapter] AS c ON c.[ChapterId] = us.[ChapterId]
	WHERE [PlayerId] = @existingId
	--ORDER BY c.[ChapOrder]

	EXEC [dbo].[GetMessage] @existingId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetPlayerQuestData]
	@PlayerId INT,
	@QuestId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @id INT = NULL;

	DECLARE @userId INT = @PlayerId;
	DECLARE @qId INT = @QuestId;

	DECLARE @existingId INT = NULL;
	SELECT @existingId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

	DECLARE @existingqId INT = NULL;
	SELECT @existingqId = p.[QuestId] FROM [dbo].[Quest] AS p WHERE p.[QuestId] = @qId;

	IF (@existingId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'No existing account found';
		END
	ELSE IF (@existingqId IS NULL)
		BEGIN
			SET @case = 201;
			SET @message = 'Quest data not found';
		END
	ELSE
		BEGIN
			SET @case = 100;
			SET @message = 'Quest data';
			SELECT @id = [QuestUserDataRelId] FROM [dbo].[QuestUserDataRel] WHERE [PlayerId] = @existingId AND [QuestId] = @existingqId;

			IF (@id IS NULL)
				BEGIN
					INSERT INTO [QuestUserDataRel] VALUES (@existingqId, @existingId, 0, NULL, 0);
					SELECT @id = [QuestUserDataRelId] FROM [dbo].[QuestUserDataRel] WHERE [PlayerId] = @existingId AND [QuestId] = @existingqId;
				END
		END

	SELECT * FROM [dbo].[QuestUserDataRel]
	WHERE [QuestUserDataRelId] = @id;

	EXEC [dbo].[GetMessage] @existingId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetPlayerChapterData]
	@PlayerId INT,
	@ChapterId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @id INT = NULL;

	DECLARE @userId INT = @PlayerId;
	DECLARE @qId INT = @ChapterId;

	DECLARE @existingId INT = NULL;
	SELECT @existingId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

	DECLARE @existingqId INT = NULL;
	SELECT @existingqId = p.[ChapterId] FROM [dbo].[Chapter] AS p WHERE p.[ChapterId] = @qId;

	IF (@existingId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'No existing account found';
		END
	ELSE IF (@existingqId IS NULL)
		BEGIN
			SET @case = 201;
			SET @message = 'Chapter data not found';
		END
	ELSE
		BEGIN
			SET @case = 100;
			SET @message = 'Chapter data';
			SELECT @id = [ChapterUserDataRelId] FROM [dbo].[ChapterUserDataRel] WHERE [PlayerId] = @existingId AND [ChapterId] = @existingqId;

			IF (@id IS NULL)
				BEGIN
					INSERT INTO [ChapterUserDataRel] VALUES (@existingqId, @existingId, 0);
					SELECT @id = [ChapterUserDataRelId] FROM [dbo].[ChapterUserDataRel] WHERE [PlayerId] = @existingId AND [ChapterId] = @existingqId;
				END
		END

	SELECT * FROM [dbo].[QuestUserDataRel]
	WHERE [QuestUserDataRelId] = @id;

	EXEC [dbo].[GetMessage] @existingId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AddOrUpdatePlayerQuestData]
	@PlayerId INT,
	@QuestId INT,
	@IsCompleted BIT,
	@Data VARCHAR(MAX)
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	DECLARE @userId INT = @PlayerId;
	DECLARE @qId INT = @QuestId;
	DECLARE @tData VARCHAR(MAX) = @Data;
	DECLARE @completed BIT = ISNULL(@IsCompleted,0);

	DECLARE @existingId INT = NULL;
	SELECT @existingId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

	DECLARE @existingqId INT = NULL, @chapId INT;
	SELECT @existingqId = p.[QuestId], @chapId = p.[ChapterId] FROM [dbo].[Quest] AS p WHERE p.[QuestId] = @qId;

	IF (@existingId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'No existing account found';
		END
	ELSE IF (@existingqId IS NULL)
		BEGIN
			SET @case = 201;
			SET @message = 'Quest data not found';
		END
	ELSE
		BEGIN
			EXEC [AddOrUpdatePlayerChapterDataSp] @existingId, @chapId;
			
			DECLARE @id INT;
			SELECT @id = [QuestUserDataRelId] FROM [dbo].[QuestUserDataRel] WHERE [PlayerId] = @existingId AND [QuestId] = @existingqId;

			IF	(@id IS NULL)
				BEGIN
					SET @case = 100;
					SET @message = 'Added new quest data';

					INSERT INTO [dbo].[QuestUserDataRel] VALUES (@existingqId, @existingId, @completed, @tData, 0);
					SELECT @id = [QuestUserDataRelId] FROM [dbo].[QuestUserDataRel] WHERE [PlayerId] = @existingId AND [QuestId] = @existingqId;
				END
			ELSE
				BEGIN
					SET @case = 101;
					SET @message = 'Updated quest data';

					UPDATE [dbo].[QuestUserDataRel]
					SET [IsCompleted] = @completed, [CurrentData] = @tData, [isRedeemed] = 0
					WHERE [QuestUserDataRelId] = @id;
				END
		END

	SELECT * FROM [dbo].[QuestUserDataRel]
	WHERE [QuestUserDataRelId] = @id;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO


CREATE OR ALTER PROCEDURE [dbo].[AddOrUpdatePlayerChapterDataSp]
	@PlayerId INT,
	@ChapterId INT
AS
BEGIN
	BEGIN TRY
		INSERT INTO [dbo].[ChapterUserDataRel] VALUES (@ChapterId, @PlayerId, 0);
	END TRY
	BEGIN CATCH
		RETURN;
	END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AddOrUpdatePlayerChapterData]
	@PlayerId INT,
	@ChapterId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	DECLARE @userId INT = @PlayerId;
	DECLARE @qId INT = @ChapterId;

	DECLARE @existingId INT = NULL;
	SELECT @existingId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

	DECLARE @existingqId INT = NULL;
	SELECT @existingqId = p.[ChapterId] FROM [dbo].[Chapter] AS p WHERE p.[ChapterId] = @qId;

	IF (@existingId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'No existing account found';
		END
	ELSE IF (@existingqId IS NULL)
		BEGIN
			SET @case = 201;
			SET @message = 'Chapter data not found';
		END
	ELSE
		BEGIN
			DECLARE @id INT;
			SELECT @id = [ChapterUserDataRelId] FROM [dbo].[ChapterUserDataRel] WHERE [PlayerId] = @existingId AND [ChapterId] = @existingqId;

			IF (@id IS NULL)
				BEGIN
					SET @case = 100;
					SET @message = 'Added new Chapter data';

					INSERT INTO [dbo].[ChapterUserDataRel] VALUES (@existingqId, @existingId, 0);
					SELECT @id = [ChapterUserDataRelId] FROM [dbo].[ChapterUserDataRel] WHERE [PlayerId] = @existingId AND [ChapterId] = @existingqId;
				END
			ELSE
				BEGIN
					SET @case = 101;
					SET @message = 'Updated Chapter data';

					--UPDATE [dbo].[ChapterUserDataRel]
					--SET [IsCompleted] = @completed, [CurrentData] = @tData, [isRedeemed] = 0
					--WHERE [ChapterUserDataRelId] = @id;
				END
		END

	SELECT * FROM [dbo].[ChapterUserDataRel]
	WHERE [ChapterUserDataRelId] = @id;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[RedeemQuestReward]
	@PlayerQuestUserId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @tPlayerQuestUserId INT = @PlayerQuestUserId;

	UPDATE [dbo].[QuestUserDataRel]
	SET [isRedeemed] = 1
	WHERE [QuestUserDataRelId] = @tPlayerQuestUserId;

	SET @case = 100;
	SET @message = 'Updated quest reward';


	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[RedeemChapterReward]
	@PlayerChapterUserId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @tPlayerChapterUserId INT = @PlayerChapterUserId;

	UPDATE [dbo].[ChapterUserDataRel]
	SET [isRedeemed] = 1
	WHERE [ChapterUserDataRelId] = @tPlayerChapterUserId;

	SET @case = 100;
	SET @message = 'Updated chapter reward';

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
GO



--CREATE TABLE [dbo].[DailyQuest]
--(
--	DailyQuestId INT IDENTITY(1,1) NOT NULL,
--	QuestTypeId INT NOT NULL,
--	WeekId INT NOT NULL,
--	Data VARCHAR(MAX) NOT NULL,
--	CONSTRAINT [PK_DailyQuest_DailyQuestId] PRIMARY KEY CLUSTERED (DailyQuestId ASC),
--	CONSTRAINT [FK_DailyQuest_QuestTypeId] FOREIGN KEY (QuestTypeId) REFERENCES [dbo].[QuestType] (QuestTypeId)
--)
--GO

--CREATE TABLE [dbo].[DailyQuestReward]
--(
--	DailyQuestRewardId INT IDENTITY(1,1) NOT NULL,
--	DailyQuestId INT NOT NULL,
--	DataTypeId INT NOT NULL,
--	ReqValueId INT NULL,
--	Value INT NOT NULL,
--	CONSTRAINT [PK_DailyQuestReward_DailyQuestRewardId] PRIMARY KEY CLUSTERED (DailyQuestRewardId ASC),
--	CONSTRAINT [UQ_DailyQuestReward_UniqueCode] UNIQUE NONCLUSTERED (DailyQuestId, DataTypeId, ReqValueId),
--	CONSTRAINT [FK_DailyQuestReward_DataType_DataTypeId] FOREIGN KEY (DataTypeId) REFERENCES [dbo].[DataType] (DataTypeId),
--	CONSTRAINT [FK_DailyQuestReward_DailyQuestId] FOREIGN KEY (DailyQuestId) REFERENCES [dbo].[DailyQuest] (DailyQuestId)
--)
--GO

