USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpdateMonsterHealthWithRespawn]    Script Date: 12/22/2023 3:52:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[UpdateMonsterHealthWithRespawn]
	@MonsterWorldId INT,
	@Health INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	DECLARE @validId INT = NULL;
	DECLARE @monsterDataId INT = NULL;

	SET @case = 100;
	SET @message = 'Updated the monster health data';

	BEGIN TRY

		SELECT @validId = [Id], @monsterDataId = [MonsterDataId] FROM [dbo].[MonsterWorldData] WHERE [Id] = @MonsterWorldId;

		IF (@validId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Monster data does not exists';
			END
		ELSE
			BEGIN
				IF (@Health = 0)
					BEGIN
						SELECT @Health = [Health] FROM [dbo].[MonsterData] WHERE [Id] = @monsterDataId;
						UPDATE [dbo].[MonsterWorldData] SET [Health] = @Health WHERE [Id] = @MonsterWorldId;
					END
				ELSE
					BEGIN
						UPDATE [dbo].[MonsterWorldData] SET [Health] = @Health WHERE [Id] = @MonsterWorldId;
					END
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] null, @message, @case, @error, @time, 1, 1;
END