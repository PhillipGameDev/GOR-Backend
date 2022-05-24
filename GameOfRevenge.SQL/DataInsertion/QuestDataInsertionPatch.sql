USE [GameOfRevenge]
GO

DELETE FROM [dbo].[QuestUserDataRel]
GO
DELETE FROM [dbo].[Quest]
GO
DELETE FROM [dbo].[QuestType]
GO
DELETE FROM [dbo].[Chapter]
GO
DBCC CHECKIDENT ('[QuestUserDataRel]', RESEED, 0);
GO
DBCC CHECKIDENT ('[Quest]', RESEED, 0);
GO
DBCC CHECKIDENT ('[QuestType]', RESEED, 0);
go
DBCC CHECKIDENT ('[Chapter]', RESEED, 0);
GO

INSERT INTO [dbo].[Chapter] (Name, Description, Code, ChapOrder) VALUES
	('Erupted disaster', 'Locust infestation in the city, which destroys resources. The player needs to build resource buildings to increase resource production', 'q1', 1),
	('Army training', 'General Abu Zayd has raised a riot, a battle is coming, the player must prepare his army for battle', 'q2', 2),
	('Defending the city', 'The rebels surrounded the city. The player must defend it with his army', 'q3', 3),
	('Castle defense', 'The player prepares the defense of the city, waiting for Abu Zayd''s attack on the kingdom.', 'q4', 4),
	('Strength in number', 'The player needs to join a guild to get help from allies in difficult times.', 'q5', 5),
	('The Villain', 'The player is informed that the villain responsible for the death of the deceased ruler was the players subordinate. Now the player needs to increase the level of his troops in order to avenge the deceased overlord', 'q6', 6),
	('War with the invaders', 'The traitor Anshur conspired with the invaders to remove the player from the throne. We need to destroy them', 'q7', 7),
	('Collect 500 Grain in the World', 'Dispatch troops to Farms  to collect Grain', 'q8', 8),
	('Train 10 Infantry', 'Train Infantry in the Barracks', 'q9', 9),
	('Upgrade the Wall to Level 4', 'Upgrade the wall  to increase trap capacity and increase the City Defense limit', 'q10', 10),
	('Research Technology 30 Times', 'Research new technology in the Institute', 'q11', 11),
	('Collect 13,000,000 Grain in the World', 'Dispatch troops to Farms  to collect Grain', 'q12', 12),
	('Collect 13,000,000 Lumber in the World', 'Dispatch troops to Lumber Mills  to collect Lumber', 'q13', 13),
	('Open 6 Resource Zones Outside the Wall', 'Open new zones for resource fields on the forest outside the Castle Wall', 'q14', 14),
	('Reach King Level 30', 'Upgrading buildings, killing oponents and completing quests will all earn you King EXP to advance your King level and release Skill Points', 'q15', 15)
GO

INSERT INTO [dbo].[QuestType] (Name, Code) VALUES
	('Building upgrade', 'BuildingUpgrade'),
	('Have x building count', 'XBuildingCount'),
	('Resource collection', 'ResourceCollection'),
	('Train troops', 'TrainTroops'),
	('Have x troop count', 'XTroopCount')
GO

--*********************************************************************** QUEST TEMPLATES **********************************************************************************************************************************

CREATE OR ALTER PROCEDURE [dbo].[AddQuestUpgradeStructureTemplate]
	@ChapterId INT,
	@MileStoneId INT,
	@StructureId INT,
	@Targetlevel INT
AS
BEGIN
	DECLARE @tChapterId INT = @ChapterId;
	DECLARE @tMileStoneId INT = @MileStoneId;
	DECLARE @tStructureId INT = @StructureId;
	DECLARE @tTargetlevel INT = @Targetlevel;

	DEClARE @ecId INT = NULL;
	SELECT @ecId = [ChapterId] FROM [dbo].[Chapter] WHERE [ChapterId] = @tChapterId;
	IF(@ecId IS NULL) RETURN;

	DEClARE @eqId INT = NULL;
	SELECT @eqId = [QuestId] FROM [dbo].[Quest] WHERE [ChapterId] = @ecId AND [MilestoneId] = @tMileStoneId;
	IF(@eqId IS NOT NULL) RETURN;
	
	DEClARE @sCode VARCHAR(100) = NULL;
	SELECT @sCode = [Code] FROM [dbo].[Structure] WHERE [StructureId] = @tStructureId;
	IF(@sCode IS NULL) RETURN;

	DEClARE @tLvlId INT = NULL;
	SELECT @tLvlId = [StructureDataId] FROM [dbo].[StructureData] WHERE [StructureId] = @tStructureId AND [StructureLevel] = @Targetlevel;
	IF(@tLvlId IS NULL) RETURN;

	DECLARE @qtypeId INT;
	SELECT @qtypeId = [QuestTypeId] FROM [dbo].[QuestType] WHERE [Code] = 'BuildingUpgrade';
	IF(@qtypeId IS NULL) RETURN;

	INSERT INTO [dbo].[Quest] (ChapterId, QuestTypeId, MilestoneId, Data) VALUES (@tChapterId, @qtypeId, @tMileStoneId, '{"StructureType":"' + @sCode + '","Level":' + CONVERT(VARCHAR(100),@tTargetlevel) + '}');
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AddQuestXStructureTemplate]
	@ChapterId INT,
	@MileStoneId INT,
	@StructureId INT,
	@Count INT
AS
BEGIN
	DECLARE @tChapterId INT = @ChapterId;
	DECLARE @tMileStoneId INT = @MileStoneId;
	DECLARE @tStructureId INT = @StructureId;
	DECLARE @tCount INT = @Count;

	DEClARE @ecId INT = NULL;
	SELECT @ecId = [ChapterId] FROM [dbo].[Chapter] WHERE [ChapterId] = @tChapterId;
	IF(@ecId IS NULL) RETURN;

	DEClARE @eqId INT = NULL;
	SELECT @eqId = [QuestId] FROM [dbo].[Quest] WHERE [ChapterId] = @ecId AND [MilestoneId] = @tMileStoneId;
	IF(@eqId IS NOT NULL) RETURN;
	
	DEClARE @sCode VARCHAR(100) = NULL;
	SELECT @sCode = [Code] FROM [dbo].[Structure] WHERE [StructureId] = @tStructureId;
	IF(@sCode IS NULL) RETURN;

	DECLARE @qtypeId INT;
	SELECT @qtypeId = [QuestTypeId] FROM [dbo].[QuestType] WHERE [Code] = 'XBuildingCount';
	IF(@qtypeId IS NULL) RETURN;

	INSERT INTO [dbo].[Quest] (ChapterId, QuestTypeId, MilestoneId, Data) VALUES (@tChapterId, @qtypeId, @tMileStoneId, '{"StructureType":"' + @sCode + '","Count":' + CONVERT(VARCHAR(100),@tCount) + '}');
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AddQuestTraningTroopTemplate]
	@ChapterId INT,
	@MileStoneId INT,
	@TroopId INT,
	@Targetlevel INT,
	@Count INT
AS
BEGIN
	DECLARE @tChapterId INT = @ChapterId;
	DECLARE @tMileStoneId INT = @MileStoneId;
	DECLARE @tTroopId INT = @TroopId;
	DECLARE @tCount INT = @Count;
	DECLARE @tTargetlevel INT = @Targetlevel;

	DEClARE @ecId INT = NULL;
	SELECT @ecId = [ChapterId] FROM [dbo].[Chapter] WHERE [ChapterId] = @tChapterId;
	IF(@ecId IS NULL) RETURN;

	DEClARE @eqId INT = NULL;
	SELECT @eqId = [QuestId] FROM [dbo].[Quest] WHERE [ChapterId] = @ecId AND [MilestoneId] = @tMileStoneId;
	IF(@eqId IS NOT NULL) RETURN;
	
	DEClARE @sCode VARCHAR(100) = NULL;
	SELECT @sCode = [Code] FROM [dbo].[Troop] WHERE [TroopId] = @tTroopId;
	IF(@sCode IS NULL) RETURN;

	DEClARE @tLvlId INT = NULL;
	SELECT @tLvlId = [TroopDataId] FROM [dbo].[TroopData] WHERE [TroopId] = @tTroopId AND [TroopLevel] = @Targetlevel;
	IF(@tLvlId IS NULL) RETURN;

	DECLARE @qtypeId INT;
	SELECT @qtypeId = [QuestTypeId] FROM [dbo].[QuestType] WHERE [Code] = 'TrainTroops';
	IF(@qtypeId IS NULL) RETURN;

	INSERT INTO [dbo].[Quest] (ChapterId, QuestTypeId, MilestoneId, Data) VALUES (@tChapterId, @qtypeId, @tMileStoneId, '{"TroopType":"' + @sCode + '","Level":' + CONVERT(VARCHAR(100),@tTargetlevel) + ',"Count":' + CONVERT(VARCHAR(100),@tCount) + '}');
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AddQuestXTroopTemplate]
	@ChapterId INT,
	@MileStoneId INT,
	@TroopId INT,
	@Count INT
AS
BEGIN
	DECLARE @tChapterId INT = @ChapterId;
	DECLARE @tMileStoneId INT = @MileStoneId;
	DECLARE @tTroopId INT = @TroopId;
	DECLARE @tCount INT = @Count;

	DEClARE @ecId INT = NULL;
	SELECT @ecId = [ChapterId] FROM [dbo].[Chapter] WHERE [ChapterId] = @tChapterId;
	IF(@ecId IS NULL) RETURN;

	DEClARE @eqId INT = NULL;
	SELECT @eqId = [QuestId] FROM [dbo].[Quest] WHERE [ChapterId] = @ecId AND [MilestoneId] = @tMileStoneId;
	IF(@eqId IS NOT NULL) RETURN;
	
	DEClARE @sCode VARCHAR(100) = NULL;
	SELECT @sCode = [Code] FROM [dbo].[Troop] WHERE [TroopId] = @tTroopId;
	IF(@sCode IS NULL) RETURN;

	DEClARE @tLvlId INT = NULL;
	SELECT @tLvlId = [TroopId] FROM [dbo].[Troop] WHERE [TroopId] = @tTroopId
	IF(@tLvlId IS NULL) RETURN;

	DECLARE @qtypeId INT;
	SELECT @qtypeId = [QuestTypeId] FROM [dbo].[QuestType] WHERE [Code] = 'XTroopCount';
	IF(@qtypeId IS NULL) RETURN;

	INSERT INTO [dbo].[Quest] (ChapterId, QuestTypeId, MilestoneId, Data) VALUES (@tChapterId, @qtypeId, @tMileStoneId, '{"TroopType":"' + @sCode + '","Count":' + CONVERT(VARCHAR(100),@tCount) + '}');
END
GO

CREATE OR ALTER PROCEDURE [dbo].[AddQuestResourceTrackingTemplate]
	@ChapterId INT,
	@MileStoneId INT,
	@ResourceId INT,
	@Count INT
AS
BEGIN
	DECLARE @tChapterId INT = @ChapterId;
	DECLARE @tMileStoneId INT = @MileStoneId;
	DECLARE @tResourceId INT = @ResourceId;
	DECLARE @tCount INT = @Count;

	DEClARE @ecId INT = NULL;
	SELECT @ecId = [ChapterId] FROM [dbo].[Chapter] WHERE [ChapterId] = @tChapterId;
	IF(@ecId IS NULL) RETURN;

	DEClARE @eqId INT = NULL;
	SELECT @eqId = [QuestId] FROM [dbo].[Quest] WHERE [ChapterId] = @ecId AND [MilestoneId] = @tMileStoneId;
	IF(@eqId IS NOT NULL) RETURN;
	
	DEClARE @sCode VARCHAR(100) = NULL;
	SELECT @sCode = [Code] FROM [dbo].[Resource] WHERE [ResourceId] = @tResourceId;
	IF(@sCode IS NULL) RETURN;

	DECLARE @qtypeId INT;
	SELECT @qtypeId = [QuestTypeId] FROM [dbo].[QuestType] WHERE [Code] = 'ResourceCollection';
	IF(@qtypeId IS NULL) RETURN;

	INSERT INTO [dbo].[Quest] (ChapterId, QuestTypeId, MilestoneId, Data) VALUES (@tChapterId, @qtypeId, @tMileStoneId, '{"ResourceType":"' + @sCode + '","Count":' + CONVERT(VARCHAR(100),@tCount) + '}');
END
GO

--'{"StructureType":"Farm","Level":1}'
--'{"StructureType":"Farm","Count":4}'
--'{"ResourceType":"Wood","Count":1}'
--'{"TroopType":"Archer","Count":1}'
--'{"Level":1,"TroopType":"Archer","Count":1}'
GO

DECLARE @chapId INT;
SELECT @chapId = [ChapterId] FROM [dbo].[Chapter] WHERE [Code] = 'q1';

DECLARE @cityId INT, @sawmillId INT, @farmId INT, @oreId INT;
SELECT @cityId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'CityCounsel'
SELECT @sawmillId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Sawmill'
SELECT @farmId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Farm'
SELECT @oreId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Mine'

EXEC [dbo].[AddQuestUpgradeStructureTemplate] @chapId, 1, @cityId, 2
EXEC [dbo].[AddQuestUpgradeStructureTemplate] @chapId, 2, @farmId, 1
EXEC [dbo].[AddQuestUpgradeStructureTemplate] @chapId, 4, @sawmillId, 1
EXEC [dbo].[AddQuestUpgradeStructureTemplate] @chapId, 3, @oreId, 1
GO

DECLARE @chapId INT;
SELECT @chapId = [ChapterId] FROM [dbo].[Chapter] WHERE [Code] = 'q2';

DECLARE @cityId INT, @barakId INT, @troopid INT
SELECT @cityId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'CityCounsel'
SELECT @barakId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Barracks'
SELECT @troopid = [TroopId] FROM [dbo].[Troop] WHERE [Code] = 'Swordsmen'

EXEC [dbo].[AddQuestUpgradeStructureTemplate] @chapId, 1, @cityId, 3
EXEC [dbo].[AddQuestUpgradeStructureTemplate] @chapId, 2, @barakId, 1
EXEC [dbo].[AddQuestTraningTroopTemplate] @chapId, 3, @troopid, 1, 25
GO

DECLARE @chapId INT;
SELECT @chapId = [ChapterId] FROM [dbo].[Chapter] WHERE [Code] = 'q3';

DECLARE @cityId INT, @barakId INT, @troopid INT
SELECT @cityId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'CityCounsel'
SELECT @barakId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'ShootingRange'
SELECT @troopid = [TroopId] FROM [dbo].[Troop] WHERE [Code] = 'Archer'

EXEC [dbo].[AddQuestUpgradeStructureTemplate] @chapId, 1, @cityId, 4
EXEC [dbo].[AddQuestUpgradeStructureTemplate] @chapId, 2, @barakId, 1
EXEC [dbo].[AddQuestTraningTroopTemplate] @chapId, 3, @troopid, 1, 25
GO

DECLARE @chapId INT;
SELECT @chapId = [ChapterId] FROM [dbo].[Chapter] WHERE [Code] = 'q4';

DECLARE @troopid1 INT, @troopid2 INT, @troopid3 INT
SELECT @troopid1 = [TroopId] FROM [dbo].[Troop] WHERE [Code] = 'Swordsmen'
SELECT @troopid2 = [TroopId] FROM [dbo].[Troop] WHERE [Code] = 'Archer'

DECLARE @cityId INT, @sawmillId INT, @farmId INT, @oreId INT;
SELECT @cityId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'CityCounsel'
SELECT @sawmillId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Sawmill'
SELECT @farmId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Farm'
SELECT @oreId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'Mine'

EXEC [dbo].[AddQuestUpgradeStructureTemplate] @chapId, 1, @cityId, 7
EXEC [dbo].[AddQuestXTroopTemplate] @chapId, 2, @troopid1, 4
EXEC [dbo].[AddQuestXTroopTemplate] @chapId, 3, @troopid2, 1
EXEC [dbo].[AddQuestXStructureTemplate] @chapId, 4, @sawmillId, 2
EXEC [dbo].[AddQuestXStructureTemplate] @chapId, 5, @farmId, 2
EXEC [dbo].[AddQuestXStructureTemplate] @chapId, 6, @oreId, 2
GO

DECLARE @chapId INT;
SELECT @chapId = [ChapterId] FROM [dbo].[Chapter] WHERE [Code] = 'q5';

DECLARE @resid1 INT, @resid2 INT, @resid3 INT;
SELECT @resid1 = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Wood'
SELECT @resid2 = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Ore'
SELECT @resid3 = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Food'

EXEC [dbo].[AddQuestResourceTrackingTemplate] @chapId, 1, @resid1, 10000
EXEC [dbo].[AddQuestResourceTrackingTemplate] @chapId, 2, @resid2, 10000
EXEC [dbo].[AddQuestResourceTrackingTemplate] @chapId, 3, @resid3, 10000
GO

--**********************************ChapterRewards*************************************
--******************************************************************************

DECLARE @resDatatypeId INT;
SELECT @resDatatypeId = [DataTypeId] FROM [dbo].[DataType] WHERE [Code] = 'Resource';

DECLARE @woodId INT;
SELECT @woodId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Wood';

DECLARE @foodId INT;
SELECT @foodId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Food';

DECLARE @oreId INT;
SELECT @oreId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Ore';

DECLARE @id int, @lvl INT = 1;
DECLARE myCursor CURSOR FORWARD_ONLY FOR SELECT [ChapterId] FROM [dbo].[Chapter]
OPEN myCursor;
FETCH NEXT FROM myCursor INTO @id
WHILE @@FETCH_STATUS = 0 BEGIN
	DECLARE @f INT = @lvl * @lvl * 1000;
	DECLARE @w INT = @lvl * @lvl * 750;
	DECLARE @o INT = @lvl * @lvl * 500;
    SET @lvl = @lvl + 1;

	INSERT INTO [dbo].[ChapterReward] VALUES (@id, @resDatatypeId, @foodId, @f);
	INSERT INTO [dbo].[ChapterReward] VALUES (@id, @resDatatypeId, @woodId, @w);
	INSERT INTO [dbo].[ChapterReward] VALUES (@id, @resDatatypeId, @oreId, @o);

    FETCH NEXT FROM myCursor INTO @id
END;
CLOSE myCursor;
DEALLOCATE myCursor;
GO

--**********************************QuestRewards*************************************
--******************************************************************************

DECLARE @resDatatypeId INT;
SELECT @resDatatypeId = [DataTypeId] FROM [dbo].[DataType] WHERE [Code] = 'Resource';

DECLARE @woodId INT;
SELECT @woodId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Wood';

DECLARE @foodId INT;
SELECT @foodId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Food';

DECLARE @oreId INT;
SELECT @oreId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Ore';

DECLARE @id int, @lvl INT = 1;
DECLARE myCursor CURSOR FORWARD_ONLY FOR SELECT [QuestId] FROM [dbo].[Quest]
OPEN myCursor;
FETCH NEXT FROM myCursor INTO @id
WHILE @@FETCH_STATUS = 0 BEGIN
	DECLARE @f INT = @lvl * 20 * 1000;
	DECLARE @w INT = @lvl * 20 * 750;
	DECLARE @o INT = @lvl * 20 * 500;
    SET @lvl = @lvl + 1;

	INSERT INTO [dbo].[QuestReward] VALUES (@id, @resDatatypeId, @foodId, @f);
	INSERT INTO [dbo].[QuestReward] VALUES (@id, @resDatatypeId, @woodId, @w);
	INSERT INTO [dbo].[QuestReward] VALUES (@id, @resDatatypeId, @oreId, @o);

    FETCH NEXT FROM myCursor INTO @id
END;
CLOSE myCursor;
DEALLOCATE myCursor;
GO



--***************************************************************************************


--select * from DataType
--select * from troop
--select * from structure
--select * from resource

--SELECT * FROM [dbo].[QuestType]
--SELECT * FROM [dbo].[Chapter]
--SELECT * FROM [dbo].[Quest]

--SELECT * FROM [dbo].[ChapterReward]
--order by chapterid, datatypeid, reqvalueid