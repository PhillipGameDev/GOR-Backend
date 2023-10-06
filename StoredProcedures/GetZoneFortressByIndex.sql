USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetZoneFortressByIndex]    Script Date: 9/12/2023 7:42:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetZoneFortressByIndex]
	@PlayerId INT,
	@ZoneIndex SMALLINT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	DECLARE @id INT = NULL;
	DECLARE @userId INT = @PlayerId;

	DECLARE @existingId INT = NULL;
	DECLARE @twid INT = NULL;
	SELECT @existingId = p.[PlayerId], @twid = wt.[WorldId] FROM [dbo].[Player] AS p
	LEFT JOIN [dbo].[WorldTileData] AS wt ON wt.[WorldTileDataId] = p.[WorldTileId]
	WHERE p.[PlayerId] = @PlayerId;

	IF (@existingId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'No existing account found';
			EXEC [dbo].[GetMessage] @PlayerId, @message, @case, @error, @time, 1, 1;
		END
	ELSE
		BEGIN
			SELECT @id = zf.[ZoneFortressId] FROM [dbo].[ZoneFortress] AS zf WHERE (zf.[WorldId] = @twid) AND (zf.[ZoneIndex] = @ZoneIndex);
			EXEC [dbo].[GetZoneFortressById] @id;
		END
END