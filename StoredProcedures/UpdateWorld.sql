USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpdateWorld]    Script Date: 9/12/2023 7:42:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[UpdateWorld]
	@WorldId INT,
	@CurrentZone SMALLINT = NULL
AS
BEGIN
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;
	DECLARE @case INT = 0;
	DECLARE @error INT = 0;

	DECLARE @tid INT = NULL;
	DECLARE @tcurrZone SMALLINT = NULL;
	DECLARE @twid INT = @WorldId;

	BEGIN TRY
		SELECT @tid = w.[WorldId], @tcurrZone = w.[CurrentZone] FROM [dbo].[World] AS w WHERE w.[WorldId] = @twid;
		IF (@tid IS NULL)
			BEGIN
				Set @case = 200;
				SET @message = 'World does not exist';
			END
		ELSE
			BEGIN
				IF (@CurrentZone IS NOT NULL) SET @tcurrZone = @CurrentZone;
				UPDATE [dbo].[World] SET [CurrentZone] = @tcurrZone WHERE [WorldId] = @twid;

				Set @case = 100;
				SET @message = 'World updated';
			END

	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT w.[WorldId], w.[Name], w.[Code], w.[ZoneX], w.[ZoneY], w.[ZoneSize], w.[CurrentZone]
	FROM [dbo].[World] AS w WHERE w.[WorldId] = @tid;

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END