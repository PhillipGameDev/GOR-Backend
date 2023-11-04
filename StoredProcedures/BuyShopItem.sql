USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[BuyShopItem]    Script Date: 11/4/2023 9:07:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[BuyShopItem]
	@PlayerId INT,
	@TypeId INT,
	@ItemId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;

	DECLARE @vItemId INT = @ItemId;
	DECLARE @vTypeId INT = @TypeId;

	DECLARE @vuserId INT = NULL;

	BEGIN
		SELECT @vuserId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @userid;
		IF (@vuserId IS NULL OR @vuserId = 0)
			BEGIN
				SET @case = 201;
				SET @message = 'Invalid user id';
			END
		ELSE
			BEGIN TRY
				INSERT INTO [dbo].[ShopPlayerData] VALUES (@vuserId, @time, @vTypeId, @vItemId);
				SET @case = 100;
				SET @message = 'Purchase shop item sucessfully';
			END TRY
			BEGIN CATCH
				SET @case = 0;
				SET @error = 1;
				SET @message = ERROR_MESSAGE();
			END CATCH
	END

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
