USE [GameOfRevenge]
GO

DELETE FROM [dbo].[HeroRequirement]
GO


DELETE FROM [dbo].[HeroBoost]
GO

DELETE FROM [dbo].[Hero]
GO

DBCC CHECKIDENT ('[Hero]', RESEED, 0);
GO
DBCC CHECKIDENT ('[HeroRequirement]', RESEED, 0);
GO
DBCC CHECKIDENT ('[HeroBoost]', RESEED, 0);
GO

INSERT INTO [dbo].[Hero] (Name, Code, Description)
VALUES 
	('Lord of the seas','LordOfThe seas', 'A common hero'),
	('The Vampire Sultan','TheVampireSultan', 'A common hero'),
	('Famous Warrior','FamousWarrior', 'A common hero'),
	('Light of Sultan','LightOfSultan', 'A common hero'),
	('Tribal Knight','TribalKnight', 'A common hero'),
	('Tiger Heart','TigerHeart', 'A common hero'),
	('Desert Knight',' DesertKnight', 'A uncommon hero'),
	('Justice Knight','JusticeKnight', 'A uncommon hero'),
	('Beauty Knight',' BeautyKnight', 'A uncommon hero'),
	('Alyamamah Eyes','AlyamamahEyes', 'A uncommon hero'),
	('Defensive Stance','DefensiveStance', 'A uncommon hero'),
	('King','King', 'A uncommon hero'),
	('Preemptive Strike','PreemptiveStrike', 'A rare hero'),
	('Haste Land','HasteLand', 'A rare hero'),
	('Raging Sky','RagingSky', 'A rare hero')
GO

CREATE OR ALTER PROCEDURE [dbo].[TempAddReqStruct]
	@HeroId INT,
	@StructId INT,
	@StructLvl INT
AS
BEGIN
	DECLARE @tHeroId INT = ISNULL(@HeroId,0)
	DECLARE @tStructId INT = ISNULL(@StructId,0)
	DECLARE @tStructLvl INT = ISNULL(@StructLvl,0)
	
	IF (@tHeroId <= 0) RETURN;
	IF (@tStructId <= 0) RETURN;
	IF (@tStructLvl <= 0) RETURN;

	DECLARE @vHeroId INT = NULL;
	SELECT @vHeroId = [HeroId] FROM [dbo].[Hero] WHERE [HeroId] = @tHeroId;
	IF (@vHeroId IS NULL) RETURN;

	DECLARE @vStructDataId INT = NULL;
	SELECT @vStructDataId = [StructureDataId] FROM [dbo].[StructureData] 
	WHERE [StructureLevel] = @tStructLvl AND [StructureId] = @StructId;
	IF (@vStructDataId IS NULL) RETURN;

	INSERT INTO [dbo].[HeroRequirement] VALUES (@tHeroId, @vStructDataId);
END
GO

DECLARE @structId INT;
SELECT @structId = [StructureId] FROM [dbo].[Structure] WHERE [Code] = 'CityCounsel'

DECLARE @plus INT = 1;
DECLARE @lvl FLOAT = 1;
DECLARE @heroId int;
DECLARE myCursor CURSOR FORWARD_ONLY FOR SELECT [HeroId] FROM [dbo].[Hero]
OPEN myCursor;
FETCH NEXT FROM myCursor INTO @heroId
WHILE @@FETCH_STATUS = 0 
BEGIN
	INSERT INTO [HeroBoost] VALUES (@heroId, 400 + @plus);
	INSERT INTO [HeroBoost] VALUES (@heroId, 500 + @plus);
	EXEC [dbo].[TempAddReqStruct] @heroId, @structId, @lvl
	SET @lvl = @lvl + 2;
	SET @plus = @plus + 1;
    FETCH NEXT FROM myCursor INTO @heroId
END;
CLOSE myCursor;
DEALLOCATE myCursor;
GO

DROP PROCEDURE [dbo].[TempAddReqStruct]
GO

UPDATE [dbo].[HeroRequirement]
SET [StructureDataId] = 30
WHERE [StructureDataId] = 29
GO

SELECT * FROM [dbo].[Hero]

SELECT r.[HeroRequirementId], h.[Name], s.[Code] AS 'Required Structure', sd.[StructureLevel] AS 'Required Level' FROM [dbo].[HeroRequirement] AS r
INNER JOIN [dbo].[Hero] AS h ON h.[HeroId] = r.[HeroId]
INNER JOIN [dbo].[StructureData] AS sd ON sd.[StructureDataId] = r.[StructureDataId]
INNER JOIN [dbo].[Structure] AS s ON s.[StructureId] = sd.[StructureId]

SELECT hbr.[HeroBoostId], h.[Name], t.[Code] AS 'Boost', b.[Percentage] FROM [dbo].[HeroBoost] AS hbr
INNER JOIN [dbo].[Hero] AS h ON h.[HeroId] = hbr.[HeroId]
INNER JOIN [dbo].[Boost] AS b ON b.[BoostId] = hbr.[BoostId]
INNER JOIN [dbo].[BoostType] AS t ON t.[BoostTypeId] = b.[BoostTypeId]
