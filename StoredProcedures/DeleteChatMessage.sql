USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[DeleteChatMessage]    Script Date: 3/18/2023 3:35:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[DeleteChatMessage]
	@PlayerId INT,
	@ChatId BIGINT,
	@AllianceId INT = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	DECLARE @userId INT = @PlayerId;
	DECLARE @clanId INT = ISNULL(@AllianceId, 0);
	DECLARE @ownerId INT = NULL;
	DECLARE @flags TINYINT = NULL;

	IF (@clanID <> 0)
		SELECT @ownerId = [PlayerId], @flags = [Flags] FROM [dbo].[ClanChat] WHERE [ChatId] = @ChatId AND [ClanId] = @clanID;
	ELSE
		SELECT @ownerId = [PlayerId], @flags = [Flags] FROM [dbo].[Chat] WHERE [ChatId] = @ChatId;

	IF (@ownerId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'Message not found';
		END
	ELSE IF (@PlayerId <> @ownerId)
		BEGIN
			SET @case = 201;
			SET @message = 'Player is not the owner';
		END
	ELSE IF ((@flags & 128) = 0)
		BEGIN
			BEGIN TRY
				SET @flags = 128;/*@flags | 128;*/
				IF (@clanID <> 0)
					UPDATE [dbo].[ClanChat] SET [Flags] = @flags WHERE [ChatId] = @ChatId;
				ELSE
					UPDATE [dbo].[Chat] SET [Flags] = @flags WHERE [ChatId] = @ChatId;
				SET @case = 100;
				SET @message = 'Message marked as deleted';
			END TRY
			BEGIN CATCH
				SET @case = 200;
				SET @message = 'Message not found';
			END CATCH
		END
	ELSE
		BEGIN
			SET @case = 101;
			SET @message = 'Message already deleted';
		END

	SELECT @flags AS 'Flags';

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END