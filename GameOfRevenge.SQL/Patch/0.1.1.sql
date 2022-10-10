USE [GameOfRevenge]
GO

CREATE TABLE [dbo].[Hero]
(
	HeroId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(1000) NOT NULL,				--display name
	Code VARCHAR(100) NOT NULL,					--enum name
	Description VARCHAR(1000) NOT NULL,	
	CONSTRAINT [PK_Hero_HeroId] PRIMARY KEY CLUSTERED (HeroId ASC),
	CONSTRAINT [UQ_Hero_Code] UNIQUE NONCLUSTERED (Code ASC),
)
GO

CREATE TABLE [dbo].[HeroRequirement]
(
	HeroRequirementId INT IDENTITY(1,1) NOT NULL,
	HeroId INT NOT NULL,
	StructureDataId INT NULL,
	CONSTRAINT [PK_HeroRequirement_HeroRequirementId] PRIMARY KEY CLUSTERED (HeroRequirementId ASC),
	CONSTRAINT [FK_HeroRequirement_DataType_DataTypeId] FOREIGN KEY (StructureDataId) REFERENCES [dbo].[StructureData] (StructureDataId),
	CONSTRAINT [FK_HeroRequirement_Hero] FOREIGN KEY (HeroId) REFERENCES [dbo].[Hero] (HeroId),
	CONSTRAINT [UQ_HeroRequirement_Code] UNIQUE NONCLUSTERED (HeroId, StructureDataId),
)
GO

CREATE TABLE [dbo].[HeroBoost]
(
	HeroBoostId INT IDENTITY(1,1) NOT NULL,
	HeroId INT NOT NULL,
	BoostId INT NULL,
	CONSTRAINT [PK_HeroBoost_HeroBoostId] PRIMARY KEY CLUSTERED (HeroBoostId ASC),
	CONSTRAINT [FK_HeroBoost_DataType_DataTypeId] FOREIGN KEY (BoostId) REFERENCES [dbo].[Boost] (BoostId),
	CONSTRAINT [FK_HeroBoost_Hero] FOREIGN KEY (HeroId) REFERENCES [dbo].[Hero] (HeroId),
	CONSTRAINT [UQ_HeroBoost_Code] UNIQUE NONCLUSTERED (HeroId, BoostId),
)
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllHeroes]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	BEGIN TRY
		SET @case = 100;
		SET @message = 'Hero list';
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT [HeroId], [Name], [Code], [Description] FROM [dbo].[Hero]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllHeroRequirements]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'All Hero req datas';

	SELECT [HeroRequirementId], [HeroId], [StructureDataId] FROM [dbo].[HeroRequirement]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllHeroBoosts]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'All Hero req datas';

	SELECT [HeroBoostId], [HeroId], [BoostId] FROM [dbo].[HeroBoost]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO
