USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllInventoryData]    Script Date: 11/4/2023 9:08:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[GetAllInventoryData]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	BEGIN TRY
		SET @case = 100;
		SET @message = 'fetch all inventory data';
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT i.[Id], i.InventoryId, i.InventoryLevel, i.Requirements, i.TimeToUpgrade FROM [dbo].[InventoryData] AS i

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
