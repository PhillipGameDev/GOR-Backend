CREATE DATABASE [GameOfRevenge]
GO

USE [GameOfRevenge]
GO

CREATE TABLE [dbo].[TransactionLog]
(
	[TransactionLogId] BIGINT IDENTITY(1,1) NOT NULL,
	[ApplicationUserId] INT NULL,
	[Message] NVARCHAR(MAX) NULL,
	[Case] INT NULL,
	[IsError] BIT NULL,
	[StartTime] DATETIME NULL,
	[EndTime] DATETIME NULL,
	CONSTRAINT [PK_TransactionLog_TransactionLogId] PRIMARY KEY CLUSTERED (TransactionLogId ASC)
)
GO

CREATE OR ALTER PROCEDURE [dbo].[GetMessage]
	@UserId INT, 
	@Message NVARCHAR(MAX),
	@Case INT,
	@IsError BIT,
	@Time DATETIME = NULL,
	@Print BIT = 1,
	@Store BIT = 0
AS
BEGIN  
	DECLARE @tempTime DATETIME = ISNULL(@Time, CURRENT_TIMESTAMP), @tempUserId INT = @UserId, @tempMessage NVARCHAR(MAX) = LTRIM(RTRIM(@Message)), @tempCase INT = ISNULL(@Case,0), @tempIsError INT = ISNULL(@IsError,0), @tempPrint INT = ISNULL(@Print,0), @tempStoreLog INT = ISNULL(@Store,0);
	IF (@tempPrint = 1)
		SELECT CASE WHEN @tempIsError = 0 THEN @tempMessage ELSE 'Unexpected database error has occured, please contact support team' END AS 'Message', @tempCase AS 'Case';
	IF (@Store = 1 OR @IsError = 1)
		INSERT INTO [dbo].[TransactionLog] VALUES (@tempUserId, @tempMessage, @tempCase, @tempIsError, @tempTime, CURRENT_TIMESTAMP);
END;
GO

CREATE TABLE [dbo].[Quality]
(
	QualityId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(1000) NOT NULL,				--display name
	Code VARCHAR(100) NOT NULL,					--enum name
	CONSTRAINT [PK_Quality_QualityId] PRIMARY KEY CLUSTERED (QualityId ASC),
	CONSTRAINT [UQ_Quality_Code] UNIQUE NONCLUSTERED (Code ASC)
)
GO

INSERT INTO [dbo].[Quality] VALUES ('Very Bad','VeryBad'), ('Bad','Bad'), ('Normal','Normal'), ('Good','Good'), ('Very Good','VeryGood'), ('Best', 'Best'), ('Epic', 'Epic'), ('Legendary', 'Legendary')
GO

CREATE TABLE [dbo].[Rarity]
(
	RarityId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(1000) NOT NULL,				--display name
	Code VARCHAR(100) NOT NULL,					--enum name
	CONSTRAINT [PK_Rarity_RarityId] PRIMARY KEY CLUSTERED (RarityId ASC),
	CONSTRAINT [UQ_Rarity_Code] UNIQUE NONCLUSTERED (Code ASC)
)
GO

INSERT INTO [dbo].[Rarity] VALUES ('Common','Common'), ('UnCommon','UnCommon'), ('Rare','Rare')
GO

CREATE TABLE [dbo].[Resource]
(
	ResourceId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(1000) NOT NULL,				--display name
	Code VARCHAR(100) NOT NULL,					--enum name
	RarityId INT NOT NULL,
	CONSTRAINT [PK_Resource_ResourceId] PRIMARY KEY CLUSTERED (ResourceId ASC),
	CONSTRAINT [UQ_Resource_Code] UNIQUE NONCLUSTERED (Code ASC),
	CONSTRAINT [FK_Resource_Rarity_RarityId] FOREIGN KEY (RarityId) REFERENCES [dbo].[Rarity] (RarityId)
)
GO

INSERT INTO [dbo].[Resource] VALUES ('Food','Food', 1), ('Wood','Wood', 1), ('Ore','Ore', 1), ('Gems', 'Gems', 2)
GO

CREATE TABLE [dbo].[DataType]
(
	DataTypeId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(1000) NOT NULL,	--display name
	Code VARCHAR(100) NOT NULL,	--enum name
	CONSTRAINT [PK_DataType_DataTypeId] PRIMARY KEY CLUSTERED (DataTypeId ASC),
	CONSTRAINT [UQ_DataType_Code] UNIQUE NONCLUSTERED (Code ASC)
)
GO

INSERT INTO [dbo].[DataType] VALUES ('Resource', 'Resource'), ('Structure', 'Structure'), ('Troop', 'Troop'), ('Marching', 'Marching');
GO

INSERT INTO [dbo].[DataType] VALUES ('Inventory','Inventory')
GO

INSERT INTO [dbo].[DataType] VALUES ('ActiveBuffs','ActiveBuffs')
GO

CREATE TABLE [dbo].[StructurePlacementType]
(
	StructurePlacementTypeId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(1000) NOT NULL,				--display name
	Code VARCHAR(100) NOT NULL,					--enum name
	CONSTRAINT [PK_StructurePlacementType_StructurePlacementTypeId] PRIMARY KEY CLUSTERED (StructurePlacementTypeId ASC),
	CONSTRAINT [UQ_StructurePlacementType_Code] UNIQUE NONCLUSTERED (Code ASC)
)
GO

INSERT INTO [dbo].[StructurePlacementType] VALUES ('Rural', 'Rural'), ('Urban', 'Urban'), ('Fixed', 'Fixed');
GO

CREATE TABLE [dbo].[Structure]
(
	StructureId INT IDENTITY(1,1) NOT NULL,
	StructurePlacementTypeId INT NOT NULL,
	Name VARCHAR(1000) NOT NULL,				--display name
	Code VARCHAR(100) NOT NULL,					--enum name
	Description VARCHAR(5000) NULL,
	CONSTRAINT [PK_Structure_StructureId] PRIMARY KEY CLUSTERED (StructureId ASC),
	CONSTRAINT [UQ_Structure_Code] UNIQUE NONCLUSTERED (Code ASC),
	CONSTRAINT [FK_Structure_StructurePlacementType_StructurePlacementTypeId] FOREIGN KEY (StructurePlacementTypeId) REFERENCES [dbo].[StructurePlacementType] (StructurePlacementTypeId)
)
GO

DECLARE @ruralId INT, @urbanId INT, @fixedId INT;
SELECT @ruralId = [StructurePlacementTypeId] FROM [dbo].[StructurePlacementType] WHERE [Code] = 'Rural';
SELECT @urbanId = [StructurePlacementTypeId] FROM [dbo].[StructurePlacementType] WHERE [Code] = 'Urban';
SELECT @fixedId = [StructurePlacementTypeId] FROM [dbo].[StructurePlacementType] WHERE [Code] = 'Fixed';
GO

CREATE TABLE [dbo].[StructureData]
(
	StructureDataId INT IDENTITY(1,1) NOT NULL,
	StructureId INT NOT NULL,
	StructureLevel INT NOT NULL,
	HitPoint INT NULL,
	FoodProduction FLOAT NULL,
	WoodProduction FLOAT NULL,
	OreProduction FLOAT NULL,
	PopulationSupport INT NULL,
	StructureSupport INT NULL,
	SafeDeposit INT NULL,
	TimeToBuild INT NULL,
	ResourceCapicity FLOAT NULL,
	WoundedCapacity INT NULL,
	GiftCapacity INT NULL,
	CONSTRAINT [PK_StructureData_StructureDataId] PRIMARY KEY CLUSTERED (StructureDataId ASC),
	CONSTRAINT [UQ_StructureData_UniqueCode] UNIQUE NONCLUSTERED (StructureId, StructureLevel),
	CONSTRAINT [FK_StructureData_Structure_StructureId] FOREIGN KEY (StructureId) REFERENCES [dbo].[Structure] (StructureId)
)
GO

CREATE TABLE [dbo].[StructureLocation]
(
	StructureLocationId INT IDENTITY(1,1) NOT NULL,
	StructureId INT NOT NULL,
	LocationId INT NOT NULL,
	CONSTRAINT [PK_StructureLocation_StructureLocationId] PRIMARY KEY CLUSTERED (StructureLocationId ASC),
	CONSTRAINT [UQ_StructureLocation_LocationId] UNIQUE NONCLUSTERED (StructureId, LocationId ASC),
	CONSTRAINT [FK_StructureLocation_Structure_StructureId] FOREIGN KEY (StructureId) REFERENCES [dbo].[Structure] (StructureId)
)
GO

CREATE TABLE [dbo].[StructureBuildData]
(
	StructureBuildDataId INT IDENTITY(1,1) NOT NULL,
	TownHallLevel INT NOT NULL,
	BuildStructureId INT NOT NULL,
	MaxBuildCount INT NOT NULL,
	CONSTRAINT [PK_StructureBuildData_StructureBuildDataId] PRIMARY KEY CLUSTERED (StructureBuildDataId ASC),
	CONSTRAINT [UQ_StructureBuildData_UniqueCode] UNIQUE NONCLUSTERED (TownHallLevel, BuildStructureId),
	CONSTRAINT [FK_StructureBuildData_Structure_BuildStructureId] FOREIGN KEY (BuildStructureId) REFERENCES [dbo].[Structure] (StructureId)
)
GO

CREATE TABLE [dbo].[StructureRequirement]
(
	StructureRequirementId INT IDENTITY(1,1) NOT NULL,
	StructureDataId INT NOT NULL,
	DataTypeId INT NOT NULL,
	ReqValueId INT NULL,
	Value INT NOT NULL,
	CONSTRAINT [PK_StructureRequirement_StructureRequirementId] PRIMARY KEY CLUSTERED (StructureRequirementId ASC),
	CONSTRAINT [UQ_StructureRequirement_UniqueCode] UNIQUE NONCLUSTERED (StructureDataId, DataTypeId, ReqValueId),
	CONSTRAINT [FK_StructureRequirement_DataType_DataTypeId] FOREIGN KEY (DataTypeId) REFERENCES [dbo].[DataType] (DataTypeId),
	CONSTRAINT [FK_StructureData_StructureDataId] FOREIGN KEY (StructureDataId) REFERENCES [dbo].[StructureData] (StructureDataId)
)
GO

CREATE TABLE [dbo].[Troop]
(
	TroopId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(1000) NOT NULL,	--display name
	Code VARCHAR(100) NOT NULL,	--enum name
	Description VARCHAR(MAX),
	IsMelee BIT NOT NULL,
	IsMounted BIT NOT NULL,
	IsMagic BIT NOT NULL,
	IsSeige BIT NOT NULL,
	CONSTRAINT [PK_Troop_TroopId] PRIMARY KEY CLUSTERED (TroopId ASC),
	CONSTRAINT [UQ_Troop_Code] UNIQUE NONCLUSTERED (Code ASC)
)
GO

ALTER TABLE [dbo].[TroopData]
ADD [PowerValue] INT
GO

CREATE TABLE [dbo].[TroopData]
(
	TroopDataId INT IDENTITY(1,1) NOT NULL,
	TroopId INT NOT NULL,
	TroopLevel INT NOT NULL,
	Health FLOAT NOT NULL,
	WoundedThreshold FLOAT NULL,
	AttackDamage FLOAT NOT NULL,
	AttackRange FLOAT NOT NULL,
	AttackSpeed FLOAT NOT NULL,
	Defence FLOAT NOT NULL,
	MovementSpeed FLOAT NOT NULL,
	WeightLoad INT NOT NULL,
	TraningTime INT NOT NULL,
	OutputCount INT NOT NULL,
	CONSTRAINT [PK_TroopData_TroopDataId] PRIMARY KEY CLUSTERED (TroopDataId ASC),
	CONSTRAINT [UQ_TroopData_UniqueCode] UNIQUE NONCLUSTERED (TroopId, TroopLevel),
	CONSTRAINT [FK_TroopDataId_Troop_TroopId] FOREIGN KEY (TroopId) REFERENCES [dbo].[Troop] (TroopId)
)
GO

ALTER TABLE [dbo].[TroopData]
ADD [PowerValue] INT
GO

CREATE TABLE [dbo].[TroopRequirement]
(
	TroopRequirementId INT IDENTITY(1,1) NOT NULL,
	TroopDataId INT NOT NULL,
	DataTypeId INT NOT NULL,
	ReqValueId INT NULL,
	Value INT NOT NULL,
	CONSTRAINT [PK_TroopRequirement_TroopRequirementId] PRIMARY KEY CLUSTERED (TroopRequirementId ASC),
	CONSTRAINT [UQ_TroopRequirement_UniqueCode] UNIQUE NONCLUSTERED (TroopDataId, DataTypeId, ReqValueId),
	CONSTRAINT [FK_TroopRequirement_DataType_DataTypeId] FOREIGN KEY (DataTypeId) REFERENCES [dbo].[DataType] (DataTypeId),
	CONSTRAINT [FK_TroopData_TroopDataId] FOREIGN KEY (TroopDataId) REFERENCES [dbo].[TroopData] (TroopDataId)
)
GO

CREATE TABLE [dbo].[World]
(
	WorldId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(1000) NOT NULL,
	Code VARCHAR(100) NOT NULL,
	CONSTRAINT [PK_World_WorldId] PRIMARY KEY CLUSTERED (WorldId ASC),
	CONSTRAINT [UQ_World_Code] UNIQUE NONCLUSTERED (Code ASC)
)
GO

CREATE TABLE [dbo].[WorldTileData]
(
	WorldTileDataId INT IDENTITY(1,1) NOT NULL,
	WorldId INT NOT NULL,
	X INT NOT NULL,
	Y INT NOT NULL,
	TileData VARCHAR(MAX) NULL,
	CONSTRAINT [PK_WorldTileData_WorldTileDataId] PRIMARY KEY CLUSTERED (WorldTileDataId ASC),
	CONSTRAINT [UQ_WorldTileData_UniqueCode] UNIQUE NONCLUSTERED (WorldId, X, Y),
	CONSTRAINT [FK_WorldTileData_WorldId] FOREIGN KEY (WorldId) REFERENCES [dbo].[World] (WorldId)
)
GO

CREATE TABLE [dbo].[Player]
(
	PlayerId INT IDENTITY(1,1) NOT NULL,
	PlayerIdentifier VARCHAR(1000) NOT NULL,	-- used for guest accounts
	RavasAccountId INT NULL,					-- ravas account
	Name VARCHAR(1000) NOT NULL,
	AcceptedTermAndCondition BIT NOT NULL,
	IsAdmin BIT NOT NULL,
	IsDeveloper BIT NOT NULL,
	WorldId INT NULL,
	WorldTileId INT NULL,
	CONSTRAINT [PK_Player_PlayerId] PRIMARY KEY CLUSTERED (PlayerId ASC),
	CONSTRAINT [UQ_Player_PlayerIdentifier] UNIQUE NONCLUSTERED (PlayerIdentifier ASC),
)
GO

CREATE TABLE [dbo].[PlayerData]
(
	PlayerDataId BIGINT IDENTITY(1,1) NOT NULL,
	PlayerId INT NOT NULL,
	DataTypeId INT NOT NULL,
	ValueId INT NOT NULL,
	Value VARCHAR(MAX) NOT NULL,
	CONSTRAINT [PK_PlayerData_PlayerDataId] PRIMARY KEY CLUSTERED (PlayerDataId ASC),
	CONSTRAINT [FK_PlayerData_DataType_DataTypeId] FOREIGN KEY (DataTypeId) REFERENCES [dbo].[DataType] (DataTypeId),
	CONSTRAINT [UQ_PlayerData_UniqueCode] UNIQUE NONCLUSTERED (PlayerId, DataTypeId, ValueId)
)
GO

CREATE OR ALTER TRIGGER TRG_Player_Insert ON [dbo].[Player]
	INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
	DECLARE @PlayerIdentifier VARCHAR(1000);
    DECLARE @RavasAccountId INT;
	DECLARE @Name VARCHAR(1000);
	DECLARE @AcceptedTermAndCondition BIT;
	DECLARE @IsAdmin BIT;
	DECLARE @IsDeveloper BIT;

    SELECT @PlayerIdentifier = i.[PlayerIdentifier], @RavasAccountId = i.[RavasAccountId], @Name = i.[Name], @AcceptedTermAndCondition = i.[AcceptedTermAndCondition], @IsAdmin = i.[IsAdmin], @IsDeveloper = i.[IsDeveloper] FROM inserted AS i;

	IF (@RavasAccountId IS NULL)
		BEGIN
			INSERT INTO [dbo].[Player] (PlayerIdentifier, RavasAccountId, Name, AcceptedTermAndCondition, IsAdmin, IsDeveloper)
			VALUES (@PlayerIdentifier, @RavasAccountId, @Name, @AcceptedTermAndCondition, @IsAdmin, @IsDeveloper)
		END
	ELSE
		BEGIN
			DECLARE @existingId INT = NULL;
			SELECT @existingId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[RavasAccountId] = @RavasAccountId;

			IF (@existingId IS NULL)
				BEGIN
					INSERT INTO [dbo].[Player] (PlayerIdentifier, RavasAccountId, Name, AcceptedTermAndCondition, IsAdmin, IsDeveloper)
					VALUES (@PlayerIdentifier, @RavasAccountId, @Name, @AcceptedTermAndCondition, @IsAdmin, @IsDeveloper)
				END
			ELSE
				BEGIN
					RAISERROR('The unique constraint applies on RavasAccountId %d', 16, 1, @RavasAccountId);
					ROLLBACK TRANSACTION;
				END
		END
END
GO


--CREATE TABLE [dbo].[TimerQueue]
--(
--	TimerQueueId BIGINT IDENTITY(1,1) NOT NULL,
--	PlayerId INT NOT NULL,
--	PlayerDataId BIGINT NOT NULL,
--	StartTime DATETIME NOT NULL,
--	Endtime DATETIME NOT NULL,
--	NewValue VARCHAR(MAX) NOT NULL,
--	CONSTRAINT [PK_TimerQueue_TimerQueueId] PRIMARY KEY CLUSTERED (TimerQueueId ASC),
--	CONSTRAINT [FK_TimerQueue_Player_PlayerId] FOREIGN KEY (PlayerId) REFERENCES [dbo].[Player] (PlayerId),
--	CONSTRAINT [FK_TimerQueue_PlayerData_PlayerDataId] FOREIGN KEY (PlayerDataId) REFERENCES [dbo].[PlayerData] (PlayerDataId)
--)


--CREATE OR ALTER PROCEDURE [dbo].<name>
--	@<param>
--AS
--BEGIN
--	DECLARE @case INT = 1, @error INT = 0;
--	DECLARE @message NVARCHAR(MAX) = NULL;
--	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
--	DECLARE @userId INT = NULL;

--	BEGIN TRY
--		SET @case = 1;
--		SET @error = 1;
--		SET @message = 'Success';
--	END TRY
--	BEGIN CATCH
--		SET @case = 0;
--		SET @error = 1;
--		SET @message = ERROR_MESSAGE();
--	END CATCH

--	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
--END
--GO

