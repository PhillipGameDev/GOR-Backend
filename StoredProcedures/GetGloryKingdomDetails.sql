USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetGloryKingdomDetails]    Script Date: 9/27/2023 2:14:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetGloryKingdomDetails]
AS
BEGIN
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	SELECT TOP(1) gk.[GloryKingdomEventId], gk.[StartTime], gk.[EndTime] FROM [dbo].[GloryKingdomEvent] AS gk
	ORDER BY [StartTime] DESC;

	EXEC [dbo].[GetMessage] NULL, 'Glory Kingdom details', 100, 0, @time, 1, 1;
END