USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerAllInventory]    Script Date: 11/4/2023 9:35:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[GetPlayerAllInventory]
	@PlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	DECLARE @currentId INT = NULL;

	BEGIN TRY
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
	END TRY
	BEGIN CATCH
		SET @currentId = NULL;
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	IF (@currentId IS NOT NULL)
		BEGIN
			SET @case = 100;
			SELECT p.Id AS [DataId], p.InventoryId, p.[Level], p.[Order], p.[UpgradeDate], p.[Duration]
			FROM [dbo].[InventoryUserData] AS p 
			WHERE p.[PlayerId] = @currentId

			SET @message = 'Fetched player game data';
		END

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
