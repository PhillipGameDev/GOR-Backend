USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllZoneFortress]    Script Date: 9/27/2023 2:14:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetAllZoneFortress]
AS
BEGIN
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	SELECT zf.[ZoneFortressId], zf.[WorldId], zf.[ZoneIndex], zf.[HitPoints], zf.[Attack], zf.[Defense], zf.[Finished], zf.[ClanId], c.[Name]
	FROM [dbo].[ZoneFortress] AS zf
	LEFT JOIN [dbo].[Clan] AS c ON c.[ClanId] = zf.[ClanId];

	EXEC [dbo].[GetMessage] NULL, 'All zone fortress complete data', 100, 0, @time, 1, 1;
END