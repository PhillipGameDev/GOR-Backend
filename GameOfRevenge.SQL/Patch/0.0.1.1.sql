USE [GameOfRevenge]
GO

CREATE TABLE [dbo].[BoostType]
(
	BoostTypeId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(1000) NOT NULL,				--display name
	Code VARCHAR(100) NOT NULL,					--enum name
	Description VARCHAR(1000) NOT NULL,
	CONSTRAINT [PK_BoostType_BoostTypeId] PRIMARY KEY CLUSTERED (BoostTypeId ASC),
	CONSTRAINT [UQ_BoostType_Code] UNIQUE NONCLUSTERED (Code ASC),
)
GO

INSERT INTO [dbo].[BoostType] (Name, Code, Description) 
VALUES
	('Resource Production', 'ResourceProduction', 'Increases resource production rate'),
	('Construction Speed', 'ConstructionSpeed', 'Decreases construction of buildings time'),
	('Traning Speed', 'TraningSpeed', 'Decreases traning speed of troops'),
	('Recovery Speed', 'RecoverySpeed', 'Increases troop survival chance'),
	('Army Attack', 'ArmyAttack', 'Increases army attack damage'),
	('Army Defence', 'ArmyDefence', 'Increases army defence against all type of attack')
GO

CREATE TABLE [dbo].[Boost]
(
	BoostId INT IDENTITY(1,1) NOT NULL,
	BoostTypeId INT NOT NULL,
	Percentage INT NOT NULL,
	CONSTRAINT [PK_Boost_BoostId] PRIMARY KEY CLUSTERED (BoostId ASC),
	CONSTRAINT [UQ_Boost_Code] UNIQUE NONCLUSTERED (BoostTypeId, Percentage ASC)
)
GO

DECLARE @boostTypeId INT, @percentage INT = 1;
SELECT @boostTypeId = [BoostTypeId] FROM [dbo].[BoostType] WHERE [Code] = 'ResourceProduction'
WHILE (@percentage <= 100)
BEGIN 
	INSERT INTO [dbo].[Boost] VALUES (@boostTypeId, @percentage)
	SET @percentage = @percentage + 1
END
GO

DECLARE @boostTypeId INT, @percentage INT = 1;
SELECT @boostTypeId = [BoostTypeId] FROM [dbo].[BoostType] WHERE [Code] = 'ConstructionSpeed'
WHILE (@percentage <= 100)
BEGIN 
	INSERT INTO [dbo].[Boost] VALUES (@boostTypeId, @percentage)
	SET @percentage = @percentage + 1
END
GO

DECLARE @boostTypeId INT, @percentage INT = 1;
SELECT @boostTypeId = [BoostTypeId] FROM [dbo].[BoostType] WHERE [Code] = 'TraningSpeed'
WHILE (@percentage <= 100)
BEGIN 
	INSERT INTO [dbo].[Boost] VALUES (@boostTypeId, @percentage)
	SET @percentage = @percentage + 1
END
GO

DECLARE @boostTypeId INT, @percentage INT = 1;
SELECT @boostTypeId = [BoostTypeId] FROM [dbo].[BoostType] WHERE [Code] = 'RecoverySpeed'
WHILE (@percentage <= 100)
BEGIN 
	INSERT INTO [dbo].[Boost] VALUES (@boostTypeId, @percentage)
	SET @percentage = @percentage + 1
END
GO

DECLARE @boostTypeId INT, @percentage INT = 1;
SELECT @boostTypeId = [BoostTypeId] FROM [dbo].[BoostType] WHERE [Code] = 'ArmyAttack'
WHILE (@percentage <= 100)
BEGIN 
	INSERT INTO [dbo].[Boost] VALUES (@boostTypeId, @percentage)
	SET @percentage = @percentage + 1
END
GO

DECLARE @boostTypeId INT, @percentage INT = 1;
SELECT @boostTypeId = [BoostTypeId] FROM [dbo].[BoostType] WHERE [Code] = 'ArmyDefence'
WHILE (@percentage <= 100)
BEGIN 
	INSERT INTO [dbo].[Boost] VALUES (@boostTypeId, @percentage)
	SET @percentage = @percentage + 1
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllBoostTypes]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	BEGIN TRY
		SET @case = 100;
		SET @message = 'Boost type list';
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT * FROM [dbo].[BoostType]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllBoosts]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	BEGIN TRY
		SET @case = 100;
		SET @message = 'Boost type percentage list';
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT * FROM [dbo].[Boost]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO
