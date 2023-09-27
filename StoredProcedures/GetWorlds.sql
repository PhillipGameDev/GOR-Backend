USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetWorlds]    Script Date: 9/27/2023 2:14:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetWorlds]
AS
BEGIN
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	SELECT w.[WorldId], w.[Name], w.[Code], w.[ZoneX], w.[ZoneY], w.[ZoneSize], w.[CurrentZone] FROM [dbo].[World] AS w;
	EXEC [dbo].[GetMessage] NULL, 'All world complete data', 1, 0, @time, 1, 1;
END