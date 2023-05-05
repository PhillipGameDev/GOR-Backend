USE [GameOfRevenge]
GO
/****** Object:  Trigger [dbo].[TRG_Player_Insert]    Script Date: 5/1/2023 8:58:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   TRIGGER [dbo].[TRG_Player_Insert] ON [dbo].[Player]
	INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
	DECLARE @PlayerIdentifier VARCHAR(1000);
    DECLARE @FirebaseId VARCHAR(1000);
	DECLARE @Name NVARCHAR(200);
	DECLARE @Terms BIT;
	DECLARE @IsAdmin BIT;
	DECLARE @IsDeveloper BIT;
	DECLARE @VIPPoints INT;
	DECLARE @Version INT;

    SELECT @PlayerIdentifier = i.[PlayerIdentifier], @FirebaseId = i.[FirebaseId], @Name = i.[Name], 
			@Terms = i.[AcceptedTermAndCondition], @IsAdmin = i.[IsAdmin], @IsDeveloper = i.[IsDeveloper], 
			@VIPPoints = i.[VIPPoints], @Version = i.[Version] FROM inserted AS i;

	IF (@FirebaseId IS NULL)
		BEGIN
			INSERT INTO [dbo].[Player] (PlayerIdentifier, FirebaseId, Name, AcceptedTermAndCondition, IsAdmin, IsDeveloper, VIPPoints, Version)
			VALUES (@PlayerIdentifier, @FirebaseId, @Name, @Terms, @IsAdmin, @IsDeveloper, @VIPPoints, @Version)
		END
	ELSE
		BEGIN
			DECLARE @existingId INT = NULL;
			SELECT @existingId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[FirebaseId] = @FirebaseId;

			IF (@existingId IS NULL)
				BEGIN
					INSERT INTO [dbo].[Player] (PlayerIdentifier, FirebaseId, Name, AcceptedTermAndCondition, IsAdmin, IsDeveloper, VIPPoints, Version)
					VALUES (@PlayerIdentifier, @FirebaseId, @Name, @Terms, @IsAdmin, @IsDeveloper, @VIPPoints, @Version)
				END
			ELSE
				BEGIN
					RAISERROR('The unique constraint applies on FirebaseId %d', 16, 1, @FirebaseId);
					ROLLBACK TRANSACTION;
				END
		END
END