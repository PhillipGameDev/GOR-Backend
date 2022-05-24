USE [GameOfRevenge]
GO

DELETE FROM [dbo].[InventoryRequirement]
GO
DELETE FROM [dbo].[Inventory]
GO
DBCC CHECKIDENT ('[Inventory]', RESEED, 0);
GO
DBCC CHECKIDENT ('[InventoryRequirement]', RESEED, 0);
GO


INSERT INTO [dbo].[Inventory] (Name, Code, RarityId)
VALUES ('Recall Orders','RecallOrders', 1), ('Shield','Shield', 1), ('Blessing','Blessing', 1), ('Life Saver', 'LifeSaver', 2), ('Production Boost', 'ProductionBoost', 3), ('Speed Gathering','SpeedGathering', 1)
GO

DECLARE @datatypeResId INT;
SELECT @datatypeResId = [DataTypeId] FROM [dbo].[DataType] WHERE [Code] = 'Resource';

DECLARE @gemId INT
SELECT @gemId = [ResourceId] FROM [dbo].[Resource] WHERE [Code] = 'Gems';

DECLARE @id int
DECLARE myCursor CURSOR FORWARD_ONLY FOR SELECT [InventoryId] FROM [dbo].[Inventory]
OPEN myCursor;
FETCH NEXT FROM myCursor INTO @id
WHILE @@FETCH_STATUS = 0 BEGIN
	DECLARE @g INT = 50;
	INSERT INTO [dbo].[InventoryRequirement] VALUES (@id, @datatypeResId, @gemId, @g)
    FETCH NEXT FROM myCursor INTO @id
END;
CLOSE myCursor;
DEALLOCATE myCursor;
GO

SELECT * FROM [Inventory]
SELECT * FROM [InventoryRequirement]