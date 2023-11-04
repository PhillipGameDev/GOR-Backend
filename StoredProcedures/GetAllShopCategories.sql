USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllShopCategories]    Script Date: 11/4/2023 9:33:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetAllShopCategories]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	SET @case = 100;
	SET @message = 'GetAllShopCategory';

	SELECT c.[Id], c.[Name] FROM [dbo].[ShopCategory] as c;

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
