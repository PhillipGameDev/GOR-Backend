USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddBattleHistory]    Script Date: 11/25/2023 1:05:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AddBattleHistory]
	@PlayerId INT,
	@IsAttacker BIT,
	@Replay VARCHAR(MAX)
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @validPlayerId INT = NULL;
	DECLARE @validId INT = NULL;

	SET @case = 100;
	SET @message = 'Inserted a battle history';

	BEGIN TRY

		SELECT @validPlayerId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @PlayerId;

		IF (@validPlayerId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
		ELSE
			BEGIN
				DECLARE @tempTable table (Id INT);
				INSERT INTO [dbo].[BattleHistory]
				OUTPUT INSERTED.Id INTO @tempTable
				VALUES ( @PlayerId, @IsAttacker, @Replay );

				SELECT @validId = [Id] FROM @tempTable;

				SELECT * FROM [dbo].[BattleHistory] WHERE [Id] = @validId;
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] null, @message, @case, @error, @time, 1, 1;
END