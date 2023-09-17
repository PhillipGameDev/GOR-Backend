USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetWorldTilesData]    Script Date: 9/16/2023 6:41:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetWorldTilesData]
	@WorldId INT
AS
BEGIN
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;
	DECLARE @case INT = 0;
	DECLARE @error INT = 0;
	DECLARE @tid INT = @WorldId;
	DECLARE @id INT = NULL;
	
	BEGIN TRY
		SELECT @id = w.[WorldId] FROM [dbo].[World] AS w WHERE w.[WorldId] = @tid;
		IF(@id IS NULL)
			BEGIN
				Set @case = 200;
				SET @message = 'World does not exist';
			END
		ELSE
			BEGIN
				Set @case = 100;
				SET @message = 'World tile data';
			END

	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT wt.[WorldTileDataId], wt.[WorldId], wt.[X], wt.[Y] FROM [dbo].[WorldTileData] AS wt;
/*	SELECT wt.[WorldTileDataId], wt.[WorldId], wt.[X], wt.[Y], wt.[TileData] FROM [dbo].[WorldTileData] AS wt
	WHERE wt.[TileData] != NULL OR LTRIM(RTRIM(wt.[TileData])) != '';*/
	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END