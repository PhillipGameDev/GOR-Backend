USE [GameOfRevenge]
GO

CREATE TABLE [dbo].[Tutorial]
(
	TutorialId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(1000) NOT NULL,				--display name
	Description VARCHAR(1000) NOT NULL,
	Code VARCHAR(100) NOT NULL,
	CONSTRAINT [PK_Tutorial_TutorialId] PRIMARY KEY CLUSTERED (TutorialId ASC),
	CONSTRAINT [UQ_Tutorial_Code] UNIQUE NONCLUSTERED (Code ASC),
)
GO

INSERT INTO [dbo].[Tutorial] (Name, Description, Code) VALUES 
	--('Start Tutorial', 'This will show you how to play the game', 'Start'),
	('Building Tutorial', 'This will show you how to build structures in the game', 'Building'),
	('Farming Tutorial', 'This will show you how to farm resources in the game', 'Farming'),
	('PvP Tutorial', 'This will show you how to attack other players in the game', 'Attack')

CREATE TABLE [dbo].[TutorialObjective]
(
	TutorialObjectiveId INT IDENTITY(1,1) NOT NULL,
	TutorialId INT NOT NULL,
	Description VARCHAR(1000) NOT NULL,
	StepIndex INT NOT NULL,
	CONSTRAINT [PK_TutorialObjective_TutorialObjectiveId] PRIMARY KEY CLUSTERED (TutorialObjectiveId ASC),
	CONSTRAINT [FK_TutorialObjective_Tutorial] FOREIGN KEY (TutorialId) REFERENCES [dbo].[Tutorial] (TutorialId),
	CONSTRAINT [UQ_TutorialObjective_Code] UNIQUE NONCLUSTERED (TutorialId, StepIndex ASC),
)
GO


DECLARE @tutId INT;
SELECT @tutId = [TutorialId] FROM [dbo].[Tutorial] WHERE [Code] = 'Building';
INSERT INTO [dbo].[TutorialObjective] (TutorialId, Description, StepIndex) VALUES 
	(@tutId, 'Construct farm building on rural area', 1),
	(@tutId, 'Upgrade city counsel to level 2', 3)
	--(@tutId, 'Speed up the construction', 4)
GO

DECLARE @tutId INT;
SELECT @tutId = [TutorialId] FROM [dbo].[Tutorial] WHERE [Code] = 'Farming';
INSERT INTO [dbo].[TutorialObjective] (TutorialId, Description, StepIndex) VALUES 
	(@tutId, 'Upgrade farm to level 2', 1),
	(@tutId, 'Build sawmill till level 2', 2),
	(@tutId, 'Collect food', 3),
	(@tutId, 'Collect wood', 4)
GO

DECLARE @tutId INT;
SELECT @tutId = [TutorialId] FROM [dbo].[Tutorial] WHERE [Code] = 'Attack';
INSERT INTO [dbo].[TutorialObjective] (TutorialId, Description, StepIndex) VALUES 
	(@tutId, 'Build barracks to train troops', 1),
	(@tutId, 'Train 10 level 1 swordmen', 2),
	(@tutId, 'Attack any enemy structure', 3)

--DROP TABLE [dbo].[PlayerTutorialObjectiveRel]
--GO
--DROP TABLE [dbo].[TutorialObjective]
--GO
--DROP TABLE [dbo].[Tutorial]
--GO
