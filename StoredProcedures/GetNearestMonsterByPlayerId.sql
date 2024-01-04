USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetNearestMonsterByPlayerId]    Script Date: 1/1/2024 12:24:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetNearestMonsterByPlayerId]
	@PlayerId INT
AS
BEGIN

	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @worldTileId INT = NULL;

	BEGIN TRY
		SELECT @worldTileId = p.[WorldTileId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

		IF (@worldTileId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Account does not exists';
			END
		ELSE
			BEGIN

				SELECT TOP(1) w.[Id], w.[MonsterDataId], m.[MonsterId], m.[Level], m.[Health], m.[Attack], m.[Defense],
						w.[WorldId], w.[X], w.[Y], w.[Health], w.[CreatedDate]
				FROM [dbo].[WorldTileData] as t
				INNER JOIN [dbo].[MonsterWorldData] as w ON t.[X] = w.[X] and t.[Y] = w.[Y]
				INNER JOIN [dbo].[MonsterData] as m ON w.[MonsterDataId] = m.[Id]
				WHERE t.[WorldTileDataId] = @worldTileId
				ORDER BY m.[Health];

				SET @case = 100;
				SET @message = 'Find the nearest Monster';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END