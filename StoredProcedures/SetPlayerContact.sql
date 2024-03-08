USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[SetPlayerContact]    Script Date: 3/18/2023 3:35:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[SetPlayerContact]
	@PlayerId INT,
	@TargetId INT,
	@Status TINYINT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @contactId BIGINT = 0;

	DECLARE @userId INT = NULL;
	DECLARE @user2Id INT = NULL;

	IF (@PlayerId = @TargetId)
		BEGIN
			SET @case = 200;
			SET @message = 'Invalid account Id';
		END
	ELSE
		BEGIN
			BEGIN TRY
				SELECT @userId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @PlayerId;
				SELECT @user2Id = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @TargetId;
			END TRY
			BEGIN CATCH
			END CATCH

			IF ((@userId IS NULL) OR (@user2Id IS NULL))
				BEGIN
					SET @case = 201;
					SET @message = 'Account does not exist';
				END
			ELSE
				BEGIN
					BEGIN TRY
						DECLARE @currStatus TINYINT = NULL;
						SELECT @contactId = [ContactId], @currStatus = [Status] FROM [dbo].[Contacts] 
						WHERE ([PlayerId] = @userId) AND ([Player2Id] = @user2Id);

						IF (@currStatus = @Status)
							BEGIN
								SET @case = 101;
								SET @message = 'Status already changed';
							END
						ELSE
							BEGIN
								IF (@contactId = 0)
									BEGIN
										DECLARE @tempTable table (ContactId BIGINT);

										INSERT INTO [dbo].[Contacts] ([PlayerId], [Player2Id], [Status])
										OUTPUT INSERTED.[ContactId] INTO @tempTable
										VALUES (@userId, @user2Id, @Status);

										SELECT @contactId = [ContactId] FROM @tempTable;
									END
								ELSE
									UPDATE [dbo].[Contacts] SET [Status] = @Status WHERE [ContactId] = @contactId;

								SET @case = 100;
								SET @message = 'Status changed';
							END
					END TRY
					BEGIN CATCH
						SET @case = 300;
						SET @message = 'An error occurred';
					END CATCH
				END
		END

	EXEC [dbo].[GetPlayerContacts] @userId, NULL, @contactId, NULL, 0;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END