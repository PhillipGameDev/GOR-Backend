/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) b.[StructureId]
      ,c.[Code]
	  ,b.[StructureLevel]
	  ,b.[TimeToBuild]
    ,b.[InstantBuildCost]
      ,b.[StructureDataId]
      ,a.[StructureRequirementId]

      ,a.[DataTypeId]
      ,a.[ReqValueId]
      ,a.[Value]
  FROM [GameOfRevenge].[dbo].[StructureData] as b
  LEFT JOIN [GameOfRevenge].[dbo].[StructureRequirement] as a ON a.StructureDataId = b.StructureDataId
  INNER JOIN [GameOfRevenge].[dbo].[Structure] as c ON c.StructureId = b.StructureId
  WHERE c.Code = 'Embassy';

//LEVELS
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 1, 60, 0);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 2, 182, 0);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 3, 390, 1);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 4, 1560, 56);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 5, 2940, 150);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 6, 5040, 218);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 7, 7800, 293);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 8, 11520, 367);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 9, 15915, 442);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 10, 22395, 516);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 11, 30000, 591);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 12, 39885, 665);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 13, 54900, 740);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 14, 75960, 814);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 15, 100440, 889);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 16, 137880, 963);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 17, 189360, 1038);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 18, 217440, 1112);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 19, 245520, 1187);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 20, 273420, 1261);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 21, 315540, 1336);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 22, 357480, 1410);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 23, 404280, 1485);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 24, 471960, 1559);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 25, 539640, 1634);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 26, 616680, 1708);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 27, 703080, 1783);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 28, 798840, 1857);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 29, 899640, 1932);
INSERT INTO [GameOfRevenge].[dbo].[StructureData] (StructureId, StructureLevel, TimeToBuild, InstantBuildCost) VALUES (4, 30, 1029960, 2006);

//REQUIREMENTS


//STRUCTURE
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (61, 2, 10, 1);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (62, 2, 10, 2);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (63, 2, 10, 3);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (64, 2, 10, 4);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (65, 2, 10, 5);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (66, 2, 10, 6);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (67, 2, 10, 7);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (68, 2, 10, 8);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (69, 2, 10, 9);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (70, 2, 10, 10);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (71, 2, 10, 11);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (72, 2, 10, 12);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (73, 2, 10, 13);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (74, 2, 10, 14);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (75, 2, 10, 15);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (76, 2, 10, 16);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (77, 2, 10, 17);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (78, 2, 10, 18);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (79, 2, 10, 19);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (80, 2, 10, 20);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (81, 2, 10, 21);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (82, 2, 10, 22);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (83, 2, 10, 23);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (84, 2, 10, 24);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (85, 2, 10, 25);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (86, 2, 10, 26);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (87, 2, 10, 27);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (88, 2, 10, 28);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (89, 2, 10, 29);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (90, 2, 10, 30);


//FOOD
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (424, 1, 1, 500);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (454, 1, 1, 1000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (455, 1, 1, 1300);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (456, 1, 1, 1700);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (457, 1, 1, 2500);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (458, 1, 1, 3900);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (459, 1, 1, 7000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (460, 1, 1, 13000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (461, 1, 1, 26000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (462, 1, 1, 50000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (463, 1, 1, 80000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (464, 1, 1, 160000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (465, 1, 1, 306000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (466, 1, 1, 550000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (467, 1, 1, 840000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (468, 1, 1, 1100000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (469, 1, 1, 1540000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (470, 1, 1, 2000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (471, 1, 1, 2600000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (472, 1, 1, 3200000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (473, 1, 1, 4000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (474, 1, 1, 5000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (475, 1, 1, 6000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (476, 1, 1, 8000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (477, 1, 1, 11000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (478, 1, 1, 15000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (479, 1, 1, 20000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (480, 1, 1, 26000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (481, 1, 1, 34000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (482, 1, 1, 44000000);
------------------------
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (62, 1, 1, 800);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (63, 1, 1, 1500);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (64, 1, 1, 2800);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (65, 1, 1, 5000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (66, 1, 1, 8500);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (67, 1, 1, 12800);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (68, 1, 1, 19300);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (69, 1, 1, 29000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (70, 1, 1, 43500);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (71, 1, 1, 67500);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (72, 1, 1, 102500);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (73, 1, 1, 155000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (74, 1, 1, 232500);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (75, 1, 1, 350000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (76, 1, 1, 525000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (77, 1, 1, 787500);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (78, 1, 1, 1200000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (79, 1, 1, 1800000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (80, 1, 1, 2700000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (81, 1, 1, 4100000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (82, 1, 1, 5300000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (83, 1, 1, 6100000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (84, 1, 1, 7400000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (85, 1, 1, 8900000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (86, 1, 1, 9400000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (87, 1, 1, 10000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (88, 1, 1, 12040000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (89, 1, 1, 13000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (90, 1, 1, 15000000);


//WOOD
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (424, 1, 2, 500);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (454, 1, 2, 1400);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (455, 1, 2, 1900);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (456, 1, 2, 2600);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (457, 1, 2, 4000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (458, 1, 2, 6400);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (459, 1, 2, 12000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (460, 1, 2, 23000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (461, 1, 2, 45000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (462, 1, 2, 88000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (463, 1, 2, 140000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (464, 1, 2, 280000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (465, 1, 2, 536000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (466, 1, 2, 963000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (467, 1, 2, 1470000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (468, 1, 2, 1925000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (469, 1, 2, 2695000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (470, 1, 2, 3500000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (471, 1, 2, 4550000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (472, 1, 2, 5600000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (473, 1, 2, 7000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (474, 1, 2, 8750000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (475, 1, 2, 10500000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (476, 1, 2, 14000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (477, 1, 2, 19250000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (478, 1, 2, 26250000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (479, 1, 2, 35000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (480, 1, 2, 45500000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (481, 1, 2, 59500000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (482, 1, 2, 77000000);



//ORE
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (463, 1, 3, 140000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (464, 1, 3, 280000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (465, 1, 3, 536000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (466, 1, 3, 963000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (467, 1, 3, 1470000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (468, 1, 3, 1925000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (469, 1, 3, 2695000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (470, 1, 3, 3500000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (471, 1, 3, 4550000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (472, 1, 3, 5600000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (473, 1, 3, 7000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (474, 1, 3, 8750000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (475, 1, 3, 10500000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (476, 1, 3, 14000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (477, 1, 3, 19250000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (478, 1, 3, 26250000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (479, 1, 3, 35000000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (480, 1, 3, 45500000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (481, 1, 3, 59500000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (482, 1, 3, 77000000);
-----------------

INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (66, 1, 3, 1300);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (67, 1, 3, 2000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (68, 1, 3, 3200);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (69, 1, 3, 5200);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (70, 1, 3, 8200);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (71, 1, 3, 12500);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (72, 1, 3, 20000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (73, 1, 3, 30000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (74, 1, 3, 45000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (75, 1, 3, 67500);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (76, 1, 3, 102500);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (77, 1, 3, 155000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (78, 1, 3, 250000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (79, 1, 3, 375000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (80, 1, 3, 575000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (81, 1, 3, 875000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (82, 1, 3, 900000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (83, 1, 3, 1300000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (84, 1, 3, 1370000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (85, 1, 3, 1480000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (86, 1, 3, 1660000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (87, 1, 3, 1940000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (88, 1, 3, 20020000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (89, 1, 3, 23005000);
INSERT INTO [GameOfRevenge].[dbo].[StructureRequirement] (StructureDataId, DataTypeId, ReqValueId, Value) VALUES (90, 1, 3, 25000000);

