USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddMonsterToWorld]    Script Date: 11/25/2023 1:06:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AddMonsterToWorld]
	@WorldId INT,
	@MonsterDataId INT,
	@X INT,
	@Y INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @validWorldId INT = NULL;

	DECLARE @monsterHealth INT = NULL;
	DECLARE @validId INT = NULL;

	SET @case = 100;
	SET @message = 'Inserted a monster data to the world';

	BEGIN TRY

		SELECT @validWorldId = [WorldId] FROM [dbo].[World] WHERE [WorldId] = @WorldId;
		SELECT @monsterHealth = [Health] FROM [dbo].[MonsterData] WHERE [Id] = @MonsterDataId;

		IF (@validWorldId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
		ELSE IF (@monsterHealth IS NULL)
			BEGIN
				SET @case = 201;
				SET @message = 'Monster does not exists';
			END
		ELSE
			BEGIN
				INSERT INTO [dbo].[MonsterWorldData] 
				VALUES ( @MonsterDataId, @WorldId, @X, @Y, @monsterHealth, @time );
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] null, @message, @case, @error, @time, 1, 1;
END