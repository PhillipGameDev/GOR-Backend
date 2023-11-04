USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllInventoryItems]    Script Date: 11/4/2023 9:09:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetAllInventoryItems]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	BEGIN TRY
		SET @case = 100;
		SET @message = 'Inventory list';
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	SELECT i.[InventoryId], i.[Code], i.[Name], r.[Code] AS 'Rarity' FROM [dbo].[Inventory] AS i
	INNER JOIN [dbo].[Rarity] AS r ON r.[RarityId] = i.[RarityId]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END