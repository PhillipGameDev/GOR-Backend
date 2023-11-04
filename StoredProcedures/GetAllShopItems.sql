USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllShopItems]    Script Date: 11/4/2023 9:33:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetAllShopItems]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	SET @case = 100;
	SET @message = 'GetAllShopItems';

	SELECT s.Id, s.CategoryId, s.ItemId, i.DataTypeId, i.ValueId, i.Value, s.Cost FROM [dbo].[ShopItem] as s INNER JOIN [dbo].[Item] as i on s.ItemId = i.Id ORDER BY [CategoryId];

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
