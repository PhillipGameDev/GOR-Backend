USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddZoneFortress]    Script Date: 12/20/2023 6:34:38 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[AddZoneFortress]
	@WorldId INT,
	@ZoneIndex INT,
	@HitPoints INT = NULL,
	@Attack INT = NULL,
	@Defense INT = NULL,
	@Data VARCHAR(MAX) = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	DECLARE @id INT = NULL;

	DECLARE @vHitPoints INT = ISNULL(@HitPoints, 8000000);
	DECLARE @vAttack INT = ISNULL(@Attack, 2500000);
	DECLARE @vDefense INT = ISNULL(@Defense, 400000);
	DECLARE @tdata VARCHAR(MAX) = ISNULL(@Data, '{"PlayerTroops":[{"Troops":[{"Id":0,"TroopMainType":0,"TroopType":1,"MonsterType":0,"TroopData":[{"Level":1,"Count":100000}]}]}],"StartTime":"2023-12-14T13:00:04.7927798Z","Duration":86400}');

	BEGIN TRY
		SET @case = 100;
		SET @message = 'Success';

		DECLARE @tempTable table (Id INT);

		INSERT INTO [dbo].[ZoneFortress] 
		OUTPUT INSERTED.[ZoneFortressId] INTO @tempTable
		VALUES (@WorldId, @ZoneIndex, @vHitPoints, @vAttack, @vDefense, 0, NULL, NULL, @tdata);

		SELECT @id = [Id] FROM @tempTable;

		SELECT zf.[ZoneFortressId], zf.[WorldId], zf.[ZoneIndex], zf.[HitPoints], zf.[Attack], zf.[Defense], zf.[Finished], zf.[ClanId], c.[Name], 
			zf.[PlayerId], zf.[Data]
			FROM [dbo].[ZoneFortress] AS zf
			LEFT JOIN [dbo].[Clan] AS c ON c.[ClanId] = zf.[ClanId]
			WHERE zf.[ZoneFortressId] = @id;
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END