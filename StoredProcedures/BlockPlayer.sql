USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[BlockPlayer]    Script Date: 3/18/2023 3:35:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[BlockPlayer]
	@PlayerId INT,
	@BlockPlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	DECLARE @userId INT = NULL;
	DECLARE @blockUserId INT = NULL;

	BEGIN TRY
		SELECT @userId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @PlayerId;
		SELECT @blockUserId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @BlockPlayerId;
	END TRY
	BEGIN CATCH
	END CATCH

	IF ((@userId IS NULL) OR (@blockUserId IS NULL))
		BEGIN
			SET @case = 200;
			SET @message = 'Account does not exist';
		END
	ELSE
		BEGIN TRY
			DECLARE @contactId BIGINT = NULL;
			DECLARE @status TINYINT = NULL;
			SELECT @contactId = [ContactId], @status = [Status] FROM [dbo].[Contacts] 
			WHERE ([PlayerId] = @PlayerId) AND ([Player2Id] = @BlockPlayerId);

			IF (@status = 2)
				BEGIN
					SET @case = 101;
					SET @message = 'Player already blocked';
				END
			ELSE
				BEGIN
					IF (@contactId IS NULL)
						INSERT INTO [dbo].[Contacts] ([PlayerId], [Player2Id], [Status])
						VALUES (@userId, @blockUserId, 2);
					ELSE
						UPDATE [dbo].[Contacts] SET [Status] = 2 WHERE [ContactId] = @contactId;

					SET @case = 100;
					SET @message = 'Player blocked';
				END
		END TRY
		BEGIN CATCH
			BEGIN
				SET @case = 300;
				SET @message = 'An error occurred'
			END
		END CATCH

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END