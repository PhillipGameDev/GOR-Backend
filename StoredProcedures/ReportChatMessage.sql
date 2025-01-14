USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[ReportChatMessage]    Script Date: 3/18/2023 3:35:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[ReportChatMessage]
	@PlayerId INT,
	@ChatId BIGINT,
	@AllianceId INT = NULL,
	@ReportType TINYINT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	DECLARE @userId INT = @PlayerId;
	DECLARE @clanId INT = ISNULL(@AllianceId, 0);
	DECLARE @flags TINYINT = NULL;

	IF (@clanID <> 0)
		SELECT @flags = [Flags] FROM [dbo].[ClanChat] WHERE [ChatId] = @ChatId AND [ClanId] = @clanID;
	ELSE
		SELECT @flags = [Flags] FROM [dbo].[Chat] WHERE [ChatId] = @ChatId;

	IF (@flags IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'Message not found';
		END
	ELSE IF ((@flags & @ReportType) = 0)
		BEGIN
			BEGIN TRY
				IF (@clanID <> 0)
					UPDATE [dbo].[ClanChat] SET [Flags] = (@flags | @ReportType) WHERE [ChatId] = @ChatId;
				ELSE
					UPDATE [dbo].[Chat] SET [Flags] = (@flags | @ReportType) WHERE [ChatId] = @ChatId;
				SET @case = 100;
				SET @message = 'Message reported';
			END TRY
			BEGIN CATCH
				SET @case = 200;
				SET @message = 'Message not found';
			END CATCH
		END
	ELSE
		BEGIN
			SET @case = 101;
			SET @message = 'Message already reported';
		END

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END