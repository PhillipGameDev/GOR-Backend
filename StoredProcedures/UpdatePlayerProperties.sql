USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpdatePlayerProperties]    Script Date: 3/14/2023 8:46:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[UpdatePlayerProperties]
	@PlayerId INT,
	@Name VARCHAR(1000) = NULL,
	@VIPPoints INT = NULL,
	@Terms BIT = NULL,
	@WorldTileId INT = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;

	DECLARE @tName VARCHAR(1000) = NULL;
	DECLARE @tPts INT = NULL;
	DECLARE @tTerms BIT = NULL;
	DECLARE @tTileId INT = NULL;

	DECLARE @validUserId INT = NULL;

	BEGIN TRY
		SELECT @validUserId = p.[PlayerId], @tName = p.[Name], @tPts = p.[VIPPoints], @tTerms = p.[AcceptedTermAndCondition], @tTileId = p.[WorldTileId]
		FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

		IF (@validUserId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
		ELSE
			BEGIN
				IF (@Name IS NOT NULL) SET @tName = @Name;
				IF (@VIPPoints IS NOT NULL) SET @tPts = @VIPPoints;
				IF (@Terms IS NOT NULL) SET @tTerms = @Terms;
				IF (@WorldTileId IS NOT NULL) SET @tTileId = @WorldTileId;
				UPDATE [dbo].[Player] SET [Name] = @tName, [VIPPoints] = @tPts, [AcceptedTermAndCondition] = @tTerms, [WorldTileId] = @tTileId
				WHERE [PlayerId] = @userId;

				SET @case = 100;
				SET @message = 'Updated player properties';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT p.[PlayerId], p.[PlayerIdentifier], p.[RavasAccountId], p.[Name], p.[AcceptedTermAndCondition], p.[IsAdmin], p.[IsDeveloper], p.[VIPPoints], p.[WorldTileId], 'Info' = NULL
	FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;


	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
