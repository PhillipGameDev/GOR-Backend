USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[CreateWorld]    Script Date: 9/12/2023 12:41:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[CreateWorld]
	@Name VARCHAR(1000),
	@Code VARCHAR(100),
	@ZoneX SMALLINT,
	@ZoneY SMALLINT,
	@ZoneSize SMALLINT
AS
BEGIN
	DECLARE @tname VARCHAR(1000) = LTRIM(RTRIm(@Name));
	DECLARE @tcode VARCHAR(100) = LTRIM(RTRIm(@Code));
	DECLARE @tzonex SMALLINT = ISNULL(@ZoneX, 1);
	DECLARE @tzoney SMALLINT = ISNULL(@ZoneY, 1);
	DECLARE @tzoneSize SMALLINT = ISNULL(@ZoneSize, 50);

	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;
	DECLARE @case INT = 0;
	DECLARE @error INT = 0;

	BEGIN TRY
		SET @case = 100;
		SET @message = 'Success';
		INSERT INTO [dbo].[World] VALUES (@tname, @tcode, @tzonex, @tzoney, @tzoneSize, -1);
	END TRY
	BEGIN CATCH
		SET @case = 200;
		SET @error = 0;
		SET @message = 'World already exist with that code';
	END CATCH

	SELECT w.[WorldId], w.[Name], w.[Code], w.[ZoneX], w.[ZoneY], w.[ZoneSize], w.[CurrentZone]
	FROM [dbo].[World] AS w WHERE w.[Code] = @Code;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END