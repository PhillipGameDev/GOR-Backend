USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetWorldZonePlayers]    Script Date: 9/14/2023 8:27:04 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetWorldZonePlayers]
	@MinX SMALLINT,
	@MinY SMALLINT,
	@MaxX SMALLINT,
	@MaxY SMALLINT,
	@GetCoords BIT = 0,
	@GetTileId BIT = 0,
	@Log BIT = 1
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	SET @case = 100;
	SET @message = 'Players';

	IF (@GetCoords = 1)
		IF (@GetTileId = 1)
			SELECT p.[PlayerId], wt.[X], wt.[Y], wt.[WorldTileDataId] FROM [dbo].[Player] AS p 
			INNER JOIN [dbo].[WorldTileData] AS wt ON wt.[WorldTileDataId] = p.[WorldTileId]
			WHERE (wt.[X] >= @MinX) AND (wt.[X] <= @MaxX) AND (wt.[Y] >= @MinY) AND (wt.[Y] <= @MaxY);
		ELSE
			SELECT p.[PlayerId], wt.[X], wt.[Y] FROM [dbo].[Player] AS p 
			INNER JOIN [dbo].[WorldTileData] AS wt ON wt.[WorldTileDataId] = p.[WorldTileId]
			WHERE (wt.[X] >= @MinX) AND (wt.[X] <= @MaxX) AND (wt.[Y] >= @MinY) AND (wt.[Y] <= @MaxY);
	ELSE
		SELECT p.[PlayerId] FROM [dbo].[Player] AS p
		INNER JOIN [dbo].[WorldTileData] AS wt ON wt.[WorldTileDataId] = p.[WorldTileId]
		WHERE (wt.[X] >= @MinX) AND (wt.[X] <= @MaxX) AND (wt.[Y] >= @MinY) AND (wt.[Y] <= @MaxY);

	IF (@Log = 1) EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END