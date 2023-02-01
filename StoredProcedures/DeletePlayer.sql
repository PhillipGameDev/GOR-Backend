USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[DeletePlayer]    Script Date: 1/31/2023 9:15:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[DeletePlayer]
	@PlayerId INT,
	@WorldTileId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	DECLARE @userId INT = @PlayerId;
	DECLARE @tileId INT = @WorldTileId;

	DECLARE @currentId INT = NULL;
	DECLARE @tilePlayerId INT = NULL;

/*	SET NOCOUNT ON;*/
	BEGIN TRY
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;
		IF (@currentId IS NOT NULL)
			BEGIN
				DECLARE @json VARCHAR(MAX) = NULL;

				SELECT @json = c.[TileData] FROM [dbo].[WorldTileData] as c WHERE c.[WorldTileDataId] = @tileId;
				IF (@json IS NOT NULL)
					BEGIN TRY
						SELECT
							@tilePlayerId = PlayerId
						FROM OPENJSON (@json)
						WITH (PlayerId INT);
					END TRY
					BEGIN CATCH
					END CATCH

				IF (@tilePlayerId = @currentId)
					BEGIN TRY
						BEGIN TRAN
						DELETE FROM [dbo].[ChapterUserDataRel] WHERE [PlayerId] = @currentId;
						DELETE FROM [dbo].[ClanInvite] WHERE [FromPlayerId] = @currentId OR [ToPlayerId] = @currentId;
						DELETE FROM [dbo].[ClanJoinRequest] WHERE [PlayerId] = @currentId;
						DELETE FROM [dbo].[ClanMember] WHERE [PlayerId] = @currentId;
						DELETE FROM [dbo].[Mail] WHERE [PlayerId] = @currentId;
						DELETE FROM [dbo].[MarketPlayerData] WHERE [FromPlayerId] = @currentId OR [ToPlayerId] = @currentId;
						DELETE FROM [dbo].[MarketTransactionLog] WHERE [PlayerId] = @currentId;
						DELETE FROM [dbo].[PlayerTutorial] WHERE [PlayerId] = @currentId;
						DELETE FROM [dbo].[PlayerTutorialObjectiveRel] WHERE [PlayerId] = @currentId;
						DELETE FROM [dbo].[QuestUserDataRel] WHERE [PlayerId] = @currentId;
						DELETE FROM [dbo].[WorldTileData] WHERE [WorldTileDataId] = @tileId;
						DELETE FROM [dbo].[PlayerData] WHERE [PlayerId] = @currentId;
						DELETE FROM [dbo].[Player] WHERE [PlayerId] = @currentId;
						COMMIT TRAN

						SET @case = 100;
						SET @message ='Player removed';
					END TRY
					BEGIN CATCH
						IF (@@TRANCOUNT > 0) ROLLBACK TRAN;
						THROW;
					END CATCH
				ELSE
					BEGIN
						SET @case = 201;
						SET @message = 'Player does not exists in world map';
					END
			END
		ELSE
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
