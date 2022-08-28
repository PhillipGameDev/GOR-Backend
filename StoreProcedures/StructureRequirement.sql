DECLARE @CASTLE INT = 1, @CASTLE_ID INT = 1; --30 / loc 1 / limit 0
DECLARE @SAWMILL INT = 6, @SAWMILL_ID INT = 11; --30 / loc 50 / limit 30
DECLARE @GATE INT = 4, @GATE_ID INT = 2; --30 / loc 1 / limit 30
DECLARE @MINE INT = 7, @MINE_ID INT = 12; --30 / loc 50 / limit 30
DECLARE @WATCHTOWER INT = 17, @WATCHTOWER_ID INT = 3; --30 / loc 1 / limit 30
DECLARE @FARM INT = 5, @FARM_ID INT = 10; --30 / loc 50 / limit 30
DECLARE @BARRACKS INT = 2, @BARRACKS_ID INT = 15; --30 / loc 25 / limit 30
DECLARE @INFIRMARY INT = 3, @INFIRMARY_ID INT = 18; --30 / loc 25 / limit 30
DECLARE @WORKSHOP INT = 10, @WORKSHOP_ID INT = 14; --30 / loc 25 / limit 30
DECLARE @SHOOTINGRANGE INT = 11, @SHOOTINGRANGE_ID INT = 16; --30 / loc 25 / limit 30
DECLARE @WAREHOUSE INT = 12, @WAREHOUSE_ID INT = 5; --30 / loc 1 / limit 30
DECLARE @ACADEMY INT = 14, @ACADEMY_ID INT = 9; --30 / loc 1 / limit 30
DECLARE @INFANTRYCAMP INT = 15, @INFANTRYCAMP_ID INT = 13; --30 / loc 50 / limit 30
DECLARE @STABLE INT = 18, @STABLE_ID INT = 17; --30 / loc 25 / limit 30
--DECLARE @SENTINELTOWER INT = 16, @SENTINELTOWER_ID INT = null

DECLARE @EMBASSY INT = 9, @EMBASSY_ID INT = 4; --1 / loc 1 / limit 30 (exceso limites) --AMPLIAR NIVELES A 30
DECLARE @MARKET INT = 13, @MARKET_ID INT = 6; --1 / loc 1 / limit 1
DECLARE @TRAININGHEROES INT = 19, @TRAININGHEROES_ID INT = 8; --1 / loc 1 / limit 1
DECLARE @BLACKSMITH INT = 8, @BLACKSMITH_ID INT = 7; --1 / loc 1 / limit 30 (exceso limites)

DECLARE @RESOURCE INT = 1;
DECLARE @STRUCTURE INT = 2;

DECLARE @GRAIN INT = 1;
DECLARE @LUMBER INT = 2;
DECLARE @IRON INT = 3;
DECLARE @SILVER INT = 4;

DECLARE @ID INT;

DELETE FROM [dbo].[StructureRequirement];

--CASTLE
SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 2;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2800);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2800);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 3;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 4000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 2);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 2);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @WAREHOUSE, 2);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 4;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 6000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 6000);
--INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @WALL, 2);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 3);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 3);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @INFANTRYCAMP, 3);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @INFIRMARY, 3);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @BARRACKS, 3);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @EMBASSY, 1);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @MINE, 3);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 5;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 10000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 10000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 4);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 4);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 6;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 17000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 17000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 5);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 5);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @ACADEMY, 5);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 7;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 35000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 35000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 6);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 6);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SHOOTINGRANGE, 6);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @STABLE, 6);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 8;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 65000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 65000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 7);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 7);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @WORKSHOP, 7);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @BLACKSMITH, 1);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 9;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 129000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 129000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 8);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 8);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 10;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 9);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 9);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 11;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 400000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 400000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 4000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 10);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 10);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 12;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 800000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 800000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 8000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 11);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 11);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 13;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1530000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1530000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 15300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 12);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 12);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 14;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2750000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2750000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 275000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 13);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 13);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 15;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 4200000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4200000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 420000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 14);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 14);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 16;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 5500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 5500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 550000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 34000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 15);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 15);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 17;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 7700000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 7700000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 770000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 48000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 16);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 16);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 18;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 10000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 10000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 62000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 17);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 17);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 19;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 13000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 13000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1300000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 81000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 18);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 18);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 20;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 16000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 16000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1600000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 100000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 19);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 19);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 21;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 20000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 20000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 2000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 125000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 20);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 20);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 22;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 25000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 25000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 2500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 165000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 21);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 21);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 23;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 30000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 30000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 3000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 187000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 22);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 22);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 24;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 40000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 40000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 4000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 23);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 23);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 25;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 55000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 55000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 5500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 343000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 24);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 24);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 26;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 75000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 75000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 7500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 468000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 25);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 25);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 27;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 100000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 100000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 10000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 625000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 26);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 26);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 28;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 130000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 130000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 13000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 812000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 27);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 27);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 29;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 170000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 170000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 17000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 1062000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 28);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 28);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @CASTLE_ID AND [StructureLevel] = 30;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 220000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 220000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 22000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 1375000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @SAWMILL, 29);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @FARM, 29);

--SAWMILL
SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 1;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 100);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 2;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 2);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 3;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 3);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 4;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 4);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 5;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 5);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 6;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 6);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 7;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 7);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 8;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 8);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 9;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 9);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 10;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 4500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 10);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 11;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 8700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 11);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 12;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 20100);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 12);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 13;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 44500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 13);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 14;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 90300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 14);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 15;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 188700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 15);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 16;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 336900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 16);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 17;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 600500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 17);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 18;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1080900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 18);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 19;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1700400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 19);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 20;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2550600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 20);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 21;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 3800100);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 21);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 22;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 5000300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 22);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 23;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 6400000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 23);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 24;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 8000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 24);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 25;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 25);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 26;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 26);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 27;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1465000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 27);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 28;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1800200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 28);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 29;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2200000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 29);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SAWMILL_ID AND [StructureLevel] = 30;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 30);

--FARM
SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 1;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 100);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 2;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 2);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 3;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 3);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 4;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 4);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 5;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 5);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 6;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 6);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 7;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 7);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 8;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 8);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 9;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 9);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 10;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 10);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 11;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 8700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 11);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 12;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 20100);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 12);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 13;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 44500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 13);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 14;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 90300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 14);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 15;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 188700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 15);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 16;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 336900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 16);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 17;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 600500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 17);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 18;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1080900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 18);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 19;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1700400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 19);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 20;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2550600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 20);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 21;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 3800100);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 21);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 22;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 5000300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 22);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 23;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 6400000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 23);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 24;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 8000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 24);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 25;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 25);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 26;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 26);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 27;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1465000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 27);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 28;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1800200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 28);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 29;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2200000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 29);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @FARM_ID AND [StructureLevel] = 30;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 30);

--WAREHOUSE
--INSERT INTO [dbo].[StructureRequirement] VALUES (@WAREHOUSE + @LEVEL1, @RESOURCE, @LUMBER, 2500000);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 2;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 2);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 3;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 3);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 4;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 4);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 5;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 800);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 5);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 6;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 6);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 7;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 3200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 7);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 8;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 6400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 8);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 9;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 10500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 9);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 10;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 22100);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 2210);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 10);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 11;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 37000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 3700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 11);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 12;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 49000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 4900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 12);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 13;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 62500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 6250);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 13);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 14;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 90300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 9030);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 14);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 15;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 188700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 18870);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 15);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 16;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 336900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 33690);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 3369);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 16);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 17;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 600500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 60050);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 6005);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 17);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 18;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1080900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 108090);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 10809);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 18);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 19;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1700400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 170040);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 17004);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 19);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 20;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2550600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 255060);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 25506);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 20);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 21;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 3800100);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 380010);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 38001);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 21);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 22;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 5000300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 500030);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 50003);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 22);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 23;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 6400000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 640000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 64000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 23);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 24;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 8000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 800000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 80000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 24);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 25;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 10000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 100000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 25);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 26;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 12500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 125000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 26);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 27;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 14650000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1465000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 146500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 27);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 28;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 18002000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1800200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 180020);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 28);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 29;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 22000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 2200000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 220000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 29);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WAREHOUSE_ID AND [StructureLevel] = 30;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 25000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 2500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 30);

--INFANTRY CAMP
SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 1;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 100);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 2);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 2;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 2);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 3;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 3);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 4;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 4);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 5;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 5);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 6;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 6);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 7;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1100);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 7);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 8;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 8);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 9;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 9);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 10;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 10);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 11;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 11);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 12;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 12);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 13;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2100);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 13);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 14;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 14);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 15;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 15);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 16;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 16);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 17;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 17);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 18;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 18);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 19;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 3100);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 19);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 20;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 20);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 21;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 3300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 21);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 22;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 3500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 22);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 23;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 3700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 23);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 24;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 3900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 24);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 25;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 25);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 26;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4100);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 26);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 27;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 27);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 28;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 28);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 29;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 29);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @INFANTRYCAMP_ID AND [StructureLevel] = 30;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 30);


--BARRACKS
SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 1;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 2);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 2;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 2);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 3;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 3);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 4;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 4);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 5;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 5);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 6;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 3900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 6400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 6);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 7;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 7000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 12000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 7);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 8;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 13000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 23000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 8);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 9;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 26000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 45000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 9);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 10;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 50000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 88000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 10);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 11;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 80000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 140000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 14000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 11);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 12;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 160000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 280000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 28000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 12);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 13;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 306000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 536000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 53600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 13);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 14;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 550000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 963000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 96300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 14);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 15;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 840000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1470000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 147000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 15);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 16;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1100000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1925000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 192500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 9600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 16);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 17;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1540000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2695000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 269500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 13400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 17);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 18;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 3500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 350000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 17500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 18);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 19;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2600000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4550000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 455000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 22700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 19);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 20;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 3200000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 5600000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 560000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 28000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 20);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 21;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 4000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 7000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 700000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 35000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 21);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 22;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 5000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 8750000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 875000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 43700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 22);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 23;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 6000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 10500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1050000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 52500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 23);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 24;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 8000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 14000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1400000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 70000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 24);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 25;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 11000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 19250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1925000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 96200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 25);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 26;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 15000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 26250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 2625000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 131200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 26);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 27;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 20000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 35000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 3500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 175000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 27);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 28;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 26000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 45500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 4550000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 227500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 28);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 29;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 34000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 59500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 5950000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 297500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 29);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BARRACKS_ID AND [StructureLevel] = 30;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 44000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 77000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 7700000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 385000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 30);


--SHOOTING RANGE
SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 1;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 2);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 2;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 2);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 3;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 3);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 4;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 4);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 5;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 5);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 6;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 3900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 6400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 6);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 7;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 7000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 12000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 7);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 8;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 13000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 23000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 8);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 9;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 26000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 45000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 9);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 10;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 50000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 88000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 10);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 11;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 80000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 140000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 14000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 11);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 12;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 160000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 280000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 28000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 12);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 13;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 306000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 536000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 53600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 13);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 14;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 550000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 963000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 96300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 14);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 15;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 840000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1470000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 147000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 15);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 16;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1100000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1925000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 192500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 9600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 16);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 17;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1540000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2695000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 269500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 13400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 17);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 18;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 3500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 350000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 17500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 18);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 19;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2600000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4550000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 455000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 22700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 19);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 20;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 3200000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 5600000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 560000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 28000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 20);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 21;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 4000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 7000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 700000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 35000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 21);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 22;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 5000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 8750000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 875000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 43700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 22);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 23;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 6000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 10500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1050000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 52500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 23);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 24;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 8000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 14000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1400000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 70000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 24);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 25;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 11000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 19250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1925000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 96200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 25);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 26;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 15000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 26250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 2625000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 131200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 26);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 27;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 20000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 35000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 3500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 175000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 27);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 28;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 26000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 45500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 4550000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 227500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 28);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 29;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 34000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 59500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 5950000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 297500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 29);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @SHOOTINGRANGE_ID AND [StructureLevel] = 30;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 44000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 77000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 7700000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 385000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 30);


--STABLE
SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 1;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 2);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 2;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 2);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 3;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 3);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 4;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 4);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 5;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 5);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 6;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 3900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 6400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 6);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 7;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 7000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 12000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 7);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 8;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 13000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 23000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 8);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 9;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 26000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 45000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 9);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 10;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 50000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 88000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 10);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 11;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 80000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 140000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 14000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 11);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 12;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 160000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 280000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 28000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 12);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 13;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 306000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 536000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 53600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 13);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 14;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 550000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 963000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 96300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 14);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 15;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 840000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1470000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 147000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 15);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 16;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1100000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1925000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 192500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 9600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 16);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 17;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1540000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2695000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 269500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 13400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 17);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 18;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 3500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 350000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 17500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 18);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 19;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2600000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4550000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 455000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 22700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 19);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 20;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 3200000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 5600000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 560000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 28000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 20);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 21;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 4000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 7000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 700000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 35000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 21);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 22;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 5000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 8750000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 875000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 43700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 22);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 23;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 6000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 10500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1050000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 52500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 23);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 24;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 8000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 14000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1400000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 70000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 24);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 25;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 11000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 19250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1925000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 96200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 25);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 26;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 15000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 26250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 2625000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 131200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 26);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 27;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 20000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 35000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 3500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 175000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 27);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 28;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 26000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 45500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 4550000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 227500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 28);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 29;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 34000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 59500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 5950000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 297500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 29);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @STABLE_ID AND [StructureLevel] = 30;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 44000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 77000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 7700000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 385000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 30);


--WORKSHOP
SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 1;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 500);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 2;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 2);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 3;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 3);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 4;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 4);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 5;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 5);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 6;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 3900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 6400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 6);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 7;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 7000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 12000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 7);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 8;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 13000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 23000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 8);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 9;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 26000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 45000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 9);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 10;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 50000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 88000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 10);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 11;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 80000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 140000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 14000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 11);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 12;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 160000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 280000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 28000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 12);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 13;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 306000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 536000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 53600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 13);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 14;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 550000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 963000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 96300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 14);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 15;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 840000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1470000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 147000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 15);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 16;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1100000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1925000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 192500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 9600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 16);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 17;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1540000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2695000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 269500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 13400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 17);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 18;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 3500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 350000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 17500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 18);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 19;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2600000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4550000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 455000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 22700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 19);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 20;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 3200000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 5600000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 560000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 28000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 20);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 21;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 4000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 7000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 700000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 35000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 21);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 22;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 5000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 8750000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 875000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 43700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 22);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 23;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 6000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 10500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1050000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 52500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 23);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 24;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 8000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 14000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1400000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 70000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 24);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 25;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 11000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 19250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1925000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 96200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 25);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 26;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 15000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 26250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 2625000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 131200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 26);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 27;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 20000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 35000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 3500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 175000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 27);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 28;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 26000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 45500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 4550000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 227500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 28);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 29;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 34000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 59500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 5950000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 297500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 29);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @WORKSHOP_ID AND [StructureLevel] = 30;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 44000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 77000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 7700000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 385000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 30);


--ACADEMY
SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 1;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 2);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 2;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 2);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 3;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 3);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 4;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 4);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 5;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 5);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 6;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 3900);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 6400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 6);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 7;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 7000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 12000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 7);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 8;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 13000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 23000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 8);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 9;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 26000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 45000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 9);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 10;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 50000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 88000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 10);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 11;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 80000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 140000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 14000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 11);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 12;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 160000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 280000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 28000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 12);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 13;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 306000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 536000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 53600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 13);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 14;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 550000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 963000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 96300);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 14);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 15;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 840000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1470000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 147000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 15);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 16;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1100000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 1925000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 192500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 9600);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 16);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 17;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 1540000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 2695000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 269500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 13400);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 17);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 18;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 3500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 350000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 17500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 18);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 19;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 2600000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 4550000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 455000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 22700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 19);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 20;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 3200000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 5600000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 560000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 28000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 20);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 21;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 4000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 7000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 700000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 35000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 21);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 22;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 5000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 8750000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 875000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 43700);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 22);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 23;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 6000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 10500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1050000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 52500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 23);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 24;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 8000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 14000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1400000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 70000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 24);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 25;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 11000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 19250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 1925000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 96200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 25);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 26;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 15000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 26250000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 2625000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 131200);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 26);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 27;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 20000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 35000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 3500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 175000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 27);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 28;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 26000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 45500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 4550000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 227500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 28);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 29;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 34000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 59500000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 5950000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 297500);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 29);

SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @ACADEMY_ID AND [StructureLevel] = 30;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 44000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @LUMBER, 77000000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @IRON, 7700000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @SILVER, 385000);
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 30);


--MINE
SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @MINE_ID AND [StructureLevel] = 1;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @RESOURCE, @GRAIN, 100);


--EMBASSY
SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @EMBASSY_ID AND [StructureLevel] = 1;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 3);


--BLACKSMITH
SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @BLACKSMITH_ID AND [StructureLevel] = 1;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 7);


--MARKET
SELECT @ID = [StructureDataId] FROM [StructureData] WHERE [StructureId] = @MARKET_ID AND [StructureLevel] = 1;
INSERT INTO [dbo].[StructureRequirement] VALUES (@ID, @STRUCTURE, @CASTLE, 3);
