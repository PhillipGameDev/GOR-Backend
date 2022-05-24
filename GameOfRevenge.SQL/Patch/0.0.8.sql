USE [GameOfRevenge]
GO

CREATE TABLE [dbo].[Technology]
(
	TechnologyId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(1000) NOT NULL,				--display name
	Code VARCHAR(100) NOT NULL,					--enum name
	CONSTRAINT [PK_Technology_TechnologyId] PRIMARY KEY CLUSTERED (TechnologyId ASC),
	CONSTRAINT [UQ_Technology_Code] UNIQUE NONCLUSTERED (Code ASC)
)
GO

CREATE TABLE [dbo].[TechnologyData]
(
	TechnologyDataId INT IDENTITY(1,1) NOT NULL,
	TechnologyId INT NOT NULL,
	TechnologyLevel INT NOT NULL,
	TechBonusValue INT NOT NULL,
	TimeTaken INT NOT NULL,
	CONSTRAINT [PK_TechnologyData_TechnologyDataId] PRIMARY KEY CLUSTERED (TechnologyDataId ASC),
	CONSTRAINT [FK_TechnologyData_Technology_TechnologyId] FOREIGN KEY (TechnologyId) REFERENCES [dbo].[Technology] (TechnologyId),
	CONSTRAINT [UQ_TechnologyData_UniqueCode] UNIQUE NONCLUSTERED (TechnologyId,TechnologyLevel)
)
GO

CREATE TABLE [dbo].[TechnologyRequirement]
(
	TechnologyRequirementId INT IDENTITY(1,1) NOT NULL,
	TechnologyDataId INT NOT NULL,
	DataTypeId INT NOT NULL,
	ReqValueId INT NULL,
	Value INT NOT NULL,
	CONSTRAINT [PK_TechnologyRequirement_TechnologyRequirementId] PRIMARY KEY CLUSTERED (TechnologyRequirementId ASC),
	CONSTRAINT [UQ_TechnologyRequirement_UniqueCode] UNIQUE NONCLUSTERED (TechnologyDataId, DataTypeId, ReqValueId),
	CONSTRAINT [FK_TechnologyRequirement_DataType_DataTypeId] FOREIGN KEY (DataTypeId) REFERENCES [dbo].[DataType] (DataTypeId),
	CONSTRAINT [FK_TechnologyData_TechnologyDataId] FOREIGN KEY (TechnologyDataId) REFERENCES [dbo].[TechnologyData] (TechnologyDataId)
)
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllTechnology]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'All technology types';

	SELECT * FROM [dbo].[Technology]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO


CREATE OR ALTER PROCEDURE [dbo].[GetAllTechnologyData]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'All technology types datas';

	SELECT * FROM [dbo].[TechnologyData]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllTechnologyDataRequirement]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'All technology requirements';

	SELECT * FROM [dbo].[TechnologyRequirement] AS s

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

INSERT INTO [dbo].[DataType] VALUES ('Technology', 'Technology')

