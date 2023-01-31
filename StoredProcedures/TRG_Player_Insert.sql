USE [GameOfRevenge]
GO
/****** Object:  Trigger [dbo].[TRG_Player_Insert]    Script Date: 1/16/2023 12:04:17 AM ******/
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
    DECLARE @RavasAccountId INT;
	DECLARE @Name VARCHAR(1000);
	DECLARE @AcceptedTermAndCondition BIT;
	DECLARE @IsAdmin BIT;
	DECLARE @IsDeveloper BIT;
	DECLARE @Version INT;

    SELECT @PlayerIdentifier = i.[PlayerIdentifier], @RavasAccountId = i.[RavasAccountId], @Name = i.[Name], @AcceptedTermAndCondition = i.[AcceptedTermAndCondition], @IsAdmin = i.[IsAdmin], @IsDeveloper = i.[IsDeveloper], @Version = i.[Version] FROM inserted AS i;

	IF (@RavasAccountId IS NULL)
		BEGIN
			INSERT INTO [dbo].[Player] (PlayerIdentifier, RavasAccountId, Name, AcceptedTermAndCondition, IsAdmin, IsDeveloper, Version)
			VALUES (@PlayerIdentifier, @RavasAccountId, @Name, @AcceptedTermAndCondition, @IsAdmin, @IsDeveloper, @Version)
		END
	ELSE
		BEGIN
			DECLARE @existingId INT = NULL;
			SELECT @existingId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[RavasAccountId] = @RavasAccountId;

			IF (@existingId IS NULL)
				BEGIN
					INSERT INTO [dbo].[Player] (PlayerIdentifier, RavasAccountId, Name, AcceptedTermAndCondition, IsAdmin, IsDeveloper, Version)
					VALUES (@PlayerIdentifier, @RavasAccountId, @Name, @AcceptedTermAndCondition, @IsAdmin, @IsDeveloper, @Version)
				END
			ELSE
				BEGIN
					RAISERROR('The unique constraint applies on RavasAccountId %d', 16, 1, @RavasAccountId);
					ROLLBACK TRANSACTION;
				END
		END
END