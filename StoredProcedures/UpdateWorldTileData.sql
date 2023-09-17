USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpdateWorldTileData]    Script Date: 9/16/2023 5:40:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[UpdateWorldTileData]
	@X INT,
	@Y INT,
	@WorldTileId INT = NULL,
	@WorldId INT = NULL
AS
BEGIN
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;
	DECLARE @case INT = 0;
	DECLARE @error INT = 0;

	DECLARE @tx INT = ISNULL(@X,0);
	DECLARE @ty INT = ISNULL(@Y,0);
	DECLARE @twtid INT = @WorldTileId;
	DECLARE @tid INT = @WorldId;
	DECLARE @id INT = NULL;

	BEGIN TRY
		IF (@twtid IS NULL)
			BEGIN
				SELECT @id = w.[WorldId] FROM [dbo].[World] AS w WHERE w.[WorldId] = @tid;
				IF (@id IS NULL)
					BEGIN
						SET @case = 200;
						SET @message = 'World does not exist';
					END
				ELSE
					BEGIN
						SELECT @twtid = w.[WorldTileDataId] FROM [dbo].[WorldTileData] AS w
						WHERE w.[WorldId] = @tid AND w.[X] = @tx AND w.[Y] = @ty;

						IF (@twtid IS NULL)
						BEGIN
							INSERT INTO [dbo].[WorldTileData] VALUES (@tid, @tx, @ty, '');
							SET @case = 100;
							SET @message = 'Created new tile data';
						END
					END
			END

		IF (@twtid IS NOT NULL)
			BEGIN
				UPDATE [dbo].[WorldTileData] SET [X] = @tx, [Y] = @ty, [TileData] = '' WHERE [WorldTileDataId] = @twtid;
				SET @case = 101;
				SET @message = 'Updated tile data';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH
	
	IF (@twtid IS NOT NULL)
		SELECT wt.[WorldTileDataId], wt.[WorldId], wt.[X], wt.[Y] FROM [dbo].[WorldTileData] AS wt
		WHERE wt.[WorldTileDataId] = @twtid;
	ELSE
		SELECT wt.[WorldTileDataId], wt.[WorldId], wt.[X], wt.[Y] FROM [dbo].[WorldTileData] AS wt
		WHERE wt.[WorldId] = @tid AND wt.[X] = @tx AND wt.[Y] = @ty;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END