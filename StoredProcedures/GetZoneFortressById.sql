USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetZoneFortressById]    Script Date: 9/12/2023 7:42:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetZoneFortressById]
	@ZoneFortressId INT
AS
BEGIN
	DECLARE @id INT = NULL;
	DECLARE @tid INT = @ZoneFortressId;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;
	DECLARE @case INT = 0;
	DECLARE @error INT = 0;

	BEGIN TRY
		SELECT @id = zf.[ZoneFortressId] FROM [dbo].[ZoneFortress] AS zf WHERE zf.[ZoneFortressId] = @tid;
		IF (@id IS NULL)
			BEGIN
				Set @case = 200;
				SET @message = 'Zone fortress does not exist';
			END
		ELSE
			BEGIN
				Set @case = 100;
				SET @message = 'Zone fortress data';
			END

	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT zf.[ZoneFortressId], zf.[WorldId], zf.[ZoneIndex], zf.[HitPoints], zf.[Attack], zf.[Defense], zf.[ClanId], c.[Name], 
			zf.[PlayerId], zf.[Data]
	FROM [dbo].[ZoneFortress] AS zf
	LEFT JOIN [dbo].[Clan] AS c ON c.[ClanId] = zf.[ClanId]
	WHERE zf.[ZoneFortressId] = @id;

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END