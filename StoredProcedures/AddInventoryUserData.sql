USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddInventoryUserData]    Script Date: 11/4/2023 9:05:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[AddInventoryUserData]
	@PlayerId INT,
	@InventoryId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;

	DECLARE @vuserId INT = NULL;
	DECLARE @vInventoryId INT = NULL;

	BEGIN
		SELECT @vuserId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @userid;
		IF (@vuserId IS NULL OR @vuserId = 0)
			BEGIN
				SET @case = 201;
				SET @message = 'Invalid user id';
			END
		ELSE
			SELECT @vInventoryId = [InventoryId] FROM [dbo].[Inventory] WHERE [InventoryId] = @InventoryId;

			IF (@vInventoryId IS NULL OR @vInventoryId = 0)
				BEGIN
					SET @case = 201;
					SET @message = 'Invalid inventory id';
				END
			ELSE
				BEGIN TRY
					INSERT INTO [dbo].[InventoryUserData] ([InventoryId], [PlayerId], [Level], [Order]) VALUES (@vInventoryid, @vuserId, 1, -1);
					SET @case = 100;
					SET @message = 'Add inventory user data successfully';
				END TRY
				BEGIN CATCH
					SET @case = 0;
					SET @error = 1;
					SET @message = ERROR_MESSAGE();
				END CATCH
	END

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
