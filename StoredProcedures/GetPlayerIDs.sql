USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerIDs]    Script Date: 9/14/2023 8:27:04 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetPlayerIDs]
	@PlayerId BIGINT = NULL,
	@Length INT = NULL,
	@GetCoords BIT = 0,
	@GetTileId BIT = 0,
	@Log BIT = 1
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	DECLARE @tplayerId BIGINT = ISNULL(@PlayerId, 1);
	DECLARE @tlen INT = ISNULL(@Length, 10);

	SET @case = 100;
	SET @message = 'Players';

	IF (@tlen = 0)
		IF (@GetCoords = 1)
			IF (@GetTileId = 1)
				SELECT p.[PlayerId], wt.[X], wt.[Y], wt.[WorldTileDataId] FROM [dbo].[Player] AS p 
				INNER JOIN [dbo].[WorldTileData] AS wt ON wt.[WorldTileDataId] = p.[WorldTileId]
				WHERE p.[PlayerId] >= @tplayerId;
			ELSE
				SELECT p.[PlayerId], wt.[X], wt.[Y] FROM [dbo].[Player] AS p 
				INNER JOIN [dbo].[WorldTileData] AS wt ON wt.[WorldTileDataId] = p.[WorldTileId]
				WHERE p.[PlayerId] >= @tplayerId;
		ELSE
			SELECT p.[PlayerId] FROM [dbo].[Player] AS p
			WHERE p.[PlayerId] >= @tplayerId;
	ELSE
		IF (@GetCoords = 1)
			IF (@GetTileId = 1)
				SELECT TOP (@tlen) p.[PlayerId], wt.[X], wt.[Y], wt.[WorldTileDataId] FROM [dbo].[Player] AS p
				INNER JOIN [dbo].[WorldTileData] AS wt ON wt.[WorldTileDataId] = p.[WorldTileId]
				WHERE p.[PlayerId] >= @tplayerId;
			ELSE
				SELECT TOP (@tlen) p.[PlayerId], wt.[X], wt.[Y] FROM [dbo].[Player] AS p
				INNER JOIN [dbo].[WorldTileData] AS wt ON wt.[WorldTileDataId] = p.[WorldTileId]
				WHERE p.[PlayerId] >= @tplayerId;
		ELSE
			SELECT TOP (@tlen) p.[PlayerId] FROM [dbo].[Player] AS p
			WHERE p.[PlayerId] >= @tplayerId;

	IF (@Log = 1) EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END