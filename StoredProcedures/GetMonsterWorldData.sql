USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetMonsterWorldData]    Script Date: 11/25/2023 1:08:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetMonsterWorldData]
	@WorldId INT = null,
	@WorldTileId INT = null,
	@MonsterId INT = null
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	BEGIN TRY
		IF (@WorldId IS NOT NULL)
			BEGIN
				SELECT	w.[Id], w.[MonsterDataId], m.[MonsterId], m.[Level], m.[Health], m.[Attack], m.[Defense],
						w.[WorldId], w.[X], w.[Y], w.[Health], w.[CreatedDate]
				FROM [dbo].[MonsterWorldData] as w
				INNER JOIN [dbo].[MonsterData] as m ON w.[MonsterDataId] = m.[Id]
				WHERE @WorldId = 0 OR w.[WorldId] = @WorldId;

				SET @case = 100;
				SET @message = 'Fetched all monster datas';
			END
		ELSE IF (@WorldTileId IS NOT NULL)
			BEGIN
				SELECT	w.[Id], w.[MonsterDataId], m.[MonsterId], m.[Level], m.[Health], m.[Attack], m.[Defense],
						w.[WorldId], w.[X], w.[Y], w.[Health], w.[CreatedDate]
				FROM [dbo].[MonsterWorldData] as w
				INNER JOIN [dbo].[MonsterData] as m ON w.[MonsterDataId] = m.[Id]
				INNER JOIN [dbo].[WorldTileData] as t ON t.[WorldId] = w.[WorldId]
				WHERE t.[WorldTileDataId] = @WorldTileId;

				SET @case = 100;
				SET @message = 'Fetched all monster datas by tileId';
			END
		ELSE IF (@MonsterId IS NOT NULL)
			BEGIN
				SELECT	w.[Id], w.[MonsterDataId], m.[MonsterId], m.[Level], m.[Health], m.[Attack], m.[Defense],
						w.[WorldId], w.[X], w.[Y], w.[Health], w.[CreatedDate]
				FROM [dbo].[MonsterWorldData] as w
				INNER JOIN [dbo].[MonsterData] as m ON w.[MonsterDataId] = m.[Id]
				WHERE w.[Id] = @MonsterId;

				SET @case = 100;
				SET @message = 'Fetched a single monster data';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] null, @message, @case, @error, @time, 1, 1;
END