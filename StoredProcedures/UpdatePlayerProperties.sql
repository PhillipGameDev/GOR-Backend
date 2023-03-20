USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpdatePlayerProperties]    Script Date: 3/19/2023 2:42:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[UpdatePlayerProperties]
	@PlayerId INT,
	@FirebaseId VARCHAR(1000) = NULL,
	@Terms BIT = NULL,
	@WorldTileId INT = NULL,
	@Name VARCHAR(1000) = NULL,
	@VIPPoints INT = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;

	DECLARE @tFirebaseId VARCHAR(1000) = NULL;
	DECLARE @tTerms BIT = NULL;
	DECLARE @tTileId INT = NULL;
	DECLARE @tName VARCHAR(1000) = NULL;
	DECLARE @tPts INT = NULL;

	DECLARE @validUserId INT = NULL;

	BEGIN TRY
		SELECT @validUserId = p.[PlayerId], @tFirebaseId = p.[FirebaseId], @tName = p.[Name], @tPts = p.[VIPPoints], @tTerms = p.[AcceptedTermAndCondition], @tTileId = p.[WorldTileId]
		FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

		IF (@validUserId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
		ELSE
			BEGIN
				IF ((@FirebaseId IS NOT NULL) AND (@FirebaseId <> @tFirebaseId)) BEGIN
					IF (@tFirebaseId IS NULL)
						SET @tFirebaseId = @FirebaseId;
					ELSE BEGIN
						SET @case = 201;
						SET @message = 'Account already linked with another account';
					END
				END
				IF (@case <> 201) BEGIN
					IF (@Terms IS NOT NULL) SET @tTerms = @Terms;
					IF (@WorldTileId IS NOT NULL) SET @tTileId = @WorldTileId;
					IF (@Name IS NOT NULL) SET @tName = @Name;
					IF (@VIPPoints IS NOT NULL) SET @tPts = @VIPPoints;
					UPDATE [dbo].[Player] SET [FirebaseId] = @FirebaseId, [AcceptedTermAndCondition] = @tTerms, [WorldTileId] = @tTileId, [Name] = @tName, [VIPPoints] = @tPts
					WHERE [PlayerId] = @userId;

					SET @case = 100;
					SET @message = 'Updated player properties';
				END
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT p.[PlayerId], p.[PlayerIdentifier], p.[FirebaseId], p.[AcceptedTermAndCondition], p.[IsAdmin], p.[IsDeveloper], p.[WorldTileId], p.[Name], p.[VIPPoints], 'Info' = NULL
	FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;


	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
