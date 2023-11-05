USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpgradeUserInventory]    Script Date: 11/5/2023 9:42:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[UpgradeUserInventory]
	@PlayerId INT,
	@InventoryId INT,
	@Level INT,
	@StartTime DATETIME,
	@Duration INT
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
					UPDATE [dbo].[InventoryUserData] SET [Level]=@Level, [UpgradeDate]=@StartTime, [Duration]=@Duration 
					WHERE [PlayerId]=@PlayerId and [InventoryId]=@InventoryId;

					SET @case = 100;

					SELECT p.Id AS [DataId], p.InventoryId, p.[Level], p.[Order], p.[UpgradeDate], p.[Duration]
					FROM [dbo].[InventoryUserData] AS p 
					WHERE p.[PlayerId] = @vuserId and p.[InventoryId] = @InventoryId

					SET @message = 'Update inventory user data successfully';
				END TRY
				BEGIN CATCH
					SET @case = 0;
					SET @error = 1;
					SET @message = ERROR_MESSAGE();
				END CATCH
	END

	

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
