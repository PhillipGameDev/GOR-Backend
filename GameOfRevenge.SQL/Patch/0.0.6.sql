
CREATE OR ALTER PROCEDURE [dbo].[CreateWorld]
	@Name VARCHAR(1000),
	@Code VARCHAR(100)
AS
BEGIN
	DECLARE @tname VARCHAR(1000) = LTRIM(RTRIm(@Name));
	DECLARE @tcode VARCHAR(100) = LTRIM(RTRIm(@Code));
	
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;
	DECLARE @case INT = 0;
	DECLARE @error INT = 0;

	BEGIN TRY
		SET @case = 100;
		SET @message = 'Success';
		INSERT INTO [dbo].[World] VALUES (@tname, @tcode);
	END TRY
	BEGIN CATCH
		SET @case = 200;
		SET @error = 0;
		SET @message = 'World already exist with that code';
	END CATCH
	SELECT w.[WorldId], w.[Name], w.[Code] FROM [dbo].[World] AS w WHERE w.[Code] = @Code;
	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO


CREATE OR ALTER PROCEDURE [dbo].[GetWorlds]
AS
BEGIN
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	SELECT w.[WorldId], w.[Name], w.[Code] FROM [dbo].[World] AS w;
	EXEC [dbo].[GetMessage] NULL, 'All world complete data', 1, 0, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetWorldById]
	@WorldId INT
AS
BEGIN
	DECLARE @tid INT =NULL;
	DECLARE @id INT = @WorldId;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;
	DECLARE @case INT = 0;
	DECLARE @error INT = 0;


	BEGIN TRY
		SELECT @tid = w.[WorldId] FROM [dbo].[World] AS w WHERE w.[WorldId] = @id;
		IF(@tid IS NULL)
			BEGIN
				Set @case = 200;
				SET @message = 'World does not exist';
			END
		ELSE
			BEGIN
				Set @case = 100;
				SET @message = 'World found';
			END

	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT w.[WorldId], w.[Name], w.[Code] FROM [dbo].[World] AS w WHERE w.[WorldId] = @tid;

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetWorldByCode]
	@WorldCode VARCHAR(100)
AS
BEGIN
	DECLARE @tcode VARCHAR(100) = LTRIM(RTRIM(@WorldCode));
	DECLARE @id INT = NULL;
	SELECT @id = w.[WorldId] FROM [dbo].[World] AS w WHERE w.[Code] = @tcode;
	EXEC [dbo].[GetWorldById] @id
END
GO


CREATE OR ALTER PROCEDURE [dbo].[GetWorldTileData]-- 1
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

	SELECT wt.[WorldTileDataId], wt.[WorldId], wt.[x], wt.[y], wt.[TileData] FROM [dbo].[WorldTileData] AS wt WHERE wt.[TileData] != NULL OR LTRIM(RTRIM(wt.[TileData])) != '';
	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO


CREATE OR ALTER PROCEDURE [dbo].[UpdateWorldTileData]
	@WorldId INT,
	@X INT,
	@Y INT,
	@Data VARCHAR(MAX) = NULL
AS
BEGIN
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;
	DECLARE @case INT = 0;
	DECLARE @error INT = 0;
	DECLARE @tid INT = @WorldId;
	DECLARE @id INT = NULL;
	DECLARE @tx INT = ISNULL(@X,0);
	DECLARE @ty INT = ISNULL(@Y,0);
	DECLARE @extId INT = NULL;
	DECLARE @tval VARCHAR(MAX) = LTRIM(RTRIM(@Data));

	BEGIN TRY
		SELECT @id = w.[WorldId] FROM [dbo].[World] AS w WHERE w.[WorldId] = @tid;
		IF(@id IS NULL)
			BEGIN
				Set @case = 200;
				SET @message = 'World does not exist';
			END
		ELSE
			BEGIN
				SELECT @extId = w.[WorldId] FROM [dbo].[WorldTileData] AS w WHERE w.[WorldId] = @tid AND w.[X] = @tx AND w.[Y] = @ty;
				IF(@extId IS NULL)
					BEGIN
						INSERT INTO [dbo].[WorldTileData] VALUES (@tid, @tx, @ty, @tval);
						Set @case = 100;
						SET @message = 'Created new tile data';
					END
				ELSE
					BEGIN
						UPDATE [dbo].[WorldTileData] 
						SET [TileData] = @tval
						WHERE [WorldId] = @tid AND [X] = @tx AND [Y] = @ty;

						Set @case = 101;
						SET @message = 'Updated tile data';
					END
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH
	
	SELECT wt.[WorldTileDataId], wt.[WorldId], wt.[x], wt.[y], wt.[TileData] FROM [dbo].[WorldTileData] AS wt
	WHERE wt.[WorldId] = @tid AND wt.[X] = @tx AND wt.[Y] = @ty;
	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO
