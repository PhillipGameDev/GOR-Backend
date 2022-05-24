--DROP TABLE [dbo].[InventoryRequirement]
--GO
--DROP TABLE [dbo].[Inventory]
--GO
USE [GameOfRevenge]
GO

CREATE TABLE [dbo].[Inventory]
(
	InventoryId INT IDENTITY(1,1) NOT NULL,
	Name VARCHAR(1000) NOT NULL,				--display name
	Code VARCHAR(100) NOT NULL,					--enum name
	RarityId INT NOT NULL,
	CONSTRAINT [PK_Inventory_InventoryId] PRIMARY KEY CLUSTERED (InventoryId ASC),
	CONSTRAINT [UQ_Inventory_Code] UNIQUE NONCLUSTERED (Code ASC),
	CONSTRAINT [FK_Inventory_Rarity_RarityId] FOREIGN KEY (RarityId) REFERENCES [dbo].[Rarity] (RarityId)
)
GO

INSERT INTO [dbo].[Inventory] (Name, Code, RarityId)
VALUES ('Recall Orders','RecallOrders', 1), ('Shield','Shield', 1), ('Blessing','Blessing', 1), ('Life Saver', 'LifeSaver', 2), ('Production Boost', 'ProductionBoost', 3), ('Speed Gathering','SpeedGathering', 1)
GO

CREATE TABLE [dbo].[InventoryRequirement]
(
	InventoryRequirementId INT IDENTITY(1,1) NOT NULL,
	InventoryId INT NOT NULL,
	DataTypeId INT NOT NULL,
	ReqValueId INT NULL,
	Value INT NOT NULL,
	CONSTRAINT [PK_InventoryRequirement_InventoryRequirementId] PRIMARY KEY CLUSTERED (InventoryRequirementId ASC),
	CONSTRAINT [UQ_InventoryRequirement_UniqueCode] UNIQUE NONCLUSTERED (InventoryId, DataTypeId, ReqValueId),
	CONSTRAINT [FK_InventoryRequirement_DataType_DataTypeId] FOREIGN KEY (DataTypeId) REFERENCES [dbo].[DataType] (DataTypeId),
	CONSTRAINT [FK_InventoryRequirement_Inventory] FOREIGN KEY (InventoryId) REFERENCES [dbo].[Inventory] (InventoryId)
)
GO


CREATE OR ALTER PROCEDURE [dbo].[GetAllInventoryItems]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	BEGIN TRY
		SET @case = 100;
		SET @message = 'Inventory list';
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT i.[InventoryId], r.[Code] AS 'Rarity', i.[Name], i.[Code]  FROM [dbo].[Inventory] AS i
	INNER JOIN [dbo].[Rarity] AS r ON r.[RarityId] = i.[RarityId]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO


CREATE OR ALTER PROCEDURE [dbo].[GetAllInventoryRequirements]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'All Inventory req datas';

	SELECT [InventoryRequirementId], [InventoryId], [DataTypeId], [ReqValueId], [Value] FROM [dbo].[InventoryRequirement]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO