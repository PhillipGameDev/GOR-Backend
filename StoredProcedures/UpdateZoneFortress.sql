USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpdateZoneFortress]    Script Date: 9/12/2023 7:42:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[UpdateZoneFortress]
	@ZoneFortressId INT,
	@HitPoints INT = NULL,
	@Attack INT = NULL,
	@Defense INT = NULL,
	@PlayerId INT = NULL,
	@Finished BIT = NULL,
	@Data VARCHAR(MAX) = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	DECLARE @id INT = NULL;
	DECLARE @tid INT = @ZoneFortressId;

	DECLARE @tclanid INT = NULL;
	DECLARE @tplayerid INT = NULL;
	DECLARE @tfinished BIT = NULL;
	DECLARE @tdata VARCHAR(MAX) = NULL;

	BEGIN TRY
		SELECT @id = zf.[ZoneFortressId], @tclanid = zf.[ClanId], @tplayerid = zf.[PlayerId], @tfinished = zf.[Finished], @tdata = zf.[Data]
		FROM [dbo].[ZoneFortress] AS zf WHERE zf.[ZoneFortressId] = @tid;

		IF (@id IS NULL) BEGIN
			SET @case = 200;
			SET @message = 'Zone fortress does not exist';
		END
		ELSE BEGIN
			SET @case = 100;
			SET @message = 'Zone fortress data updated';

			IF (@HitPoints IS NOT NULL) BEGIN
				IF (@PlayerId IS NOT NULL) BEGIN
					IF (@PlayerId = 0) BEGIN
						SET @tclanid = NULL;
						SET @tplayerid = NULL;
					END

					SELECT @tclanid = c.[ClanId], @tplayerid = c.[PlayerId] FROM [dbo].[ClanMember] AS c WHERE c.[PlayerId] = @PlayerId;
				END

				IF (@Data IS NOT NULL) SET @tdata = @Data;
				IF (@Finished IS NOT NULL) SET @tfinished = @Finished;
				UPDATE [dbo].[ZoneFortress] SET [HitPoints] = @HitPoints, [Attack] = @Attack, [Defense] = @Defense, 
												[ClanId] = @tclanid, [PlayerId] = @tplayerid, [Finished] = @tfinished, [Data] = @tdata
				WHERE [ZoneFortressId] = @id;
			END
			ELSE IF (@Finished IS NOT NULL) BEGIN
				IF (@Data IS NOT NULL) SET @tdata = @Data;
				UPDATE [dbo].[ZoneFortress] SET [Finished] = @tfinished, [Data] = @tdata WHERE [ZoneFortressId] = @id;
			END
			ELSE IF (@Data IS NOT NULL) BEGIN
				UPDATE [dbo].[ZoneFortress] SET [Data] = @Data WHERE [ZoneFortressId] = @id;
			END
		END

	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT zf.[ZoneFortressId], zf.[WorldId], zf.[ZoneIndex], zf.[HitPoints], zf.[Attack], zf.[Defense], zf.[Finished], zf.[ClanId], c.[Name], 
			zf.[PlayerId], zf.[Data]
	FROM [dbo].[ZoneFortress] AS zf
	LEFT JOIN [dbo].[Clan] AS c ON c.[ClanId] = zf.[ClanId]
	WHERE zf.[ZoneFortressId] = @id;

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END