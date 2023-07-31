USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[RestorePlayerBackup]    Script Date: 7/30/2023 11:24:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[RestorePlayerBackup]
	@PlayerId INT,
	@BackupId BIGINT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @tPlayerId INT = @PlayerId;
	DECLARE @tBackupId BIGINT = @BackupId;

	BEGIN TRY
		DECLARE @json NVARCHAR(MAX) = NULL;
		SELECT @json = Data FROM [dbo].[PlayerBackup] WHERE PlayerId = @tPlayerId AND BackupId = @tBackupId;

		IF (@json IS NOT NULL)
			BEGIN
				DECLARE @playerDataJson NVARCHAR(MAX) = (SELECT JSON_QUERY(@json, '$.PlayerData'));

				SELECT 
				    PlayerDataId,
				    DataType,
				    ValueId,
				    Value
				INTO #TempPlayerData
				FROM OPENJSON(@playerDataJson)
				WITH (
				    PlayerDataId BIGINT '$.PlayerDataId',
				    DataType VARCHAR(100) '$.DataType',
				    ValueId INT '$.ValueId',
				    Value VARCHAR(MAX) '$.Value'
				);

				UPDATE PlayerData SET DataTypeId = 1020 WHERE PlayerId = @PlayerId

				MERGE INTO PlayerData AS target
				USING #TempPlayerData AS source
				ON (target.PlayerDataId = source.PlayerDataId AND target.PlayerId = @PlayerId)
				WHEN MATCHED THEN
				    UPDATE SET
				        target.DataTypeId = (SELECT DataTypeId FROM DataType WHERE Code = source.DataType),
--				        target.ValueId = source.ValueId,
				        target.Value = source.Value
				WHEN NOT MATCHED THEN
				    INSERT (PlayerId, DataTypeId, ValueId, Value)
				    VALUES (@tPlayerId, (SELECT DataTypeId FROM DataType WHERE Code = source.DataType), source.ValueId, source.Value);

				DROP TABLE #TempPlayerData;

				SET @case = 100;
				SET @message = 'Data restored';
			END
		ELSE
			BEGIN
				SET @case = 200;
				SET @message = 'BackupId or PlayerId do not match';
			END

	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
