USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpdateUserInventoryOrder]    Script Date: 11/5/2023 9:41:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[UpdateUserInventoryOrder]
	@PlayerId INT,
	@ItemId INT,
	@Order INT
AS
BEGIN
	DECLARE @case INT = 100, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = 'User inventory data order change';
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	BEGIN TRY
		UPDATE [dbo].[InventoryUserData] SET [Order] = -1 WHERE [Order] = @Order and [PlayerId] = @PlayerId;
		UPDATE [dbo].[InventoryUserData] SET [Order] = @Order WHERE [Id] = @ItemId;
	END TRY

	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
