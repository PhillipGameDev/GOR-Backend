USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllPackageItems]    Script Date: 11/4/2023 9:12:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetAllPackageItems]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	SET @case = 100;
	SET @message = 'GetAllPackageItems';

	SELECT t.[Id], t.[PackageId], t.ItemId, i.DataTypeId, i.[ValueId], i.[Value], t.[Count] 
	FROM [dbo].[PackageItem] as t 
	INNER JOIN [dbo].[Item] as i ON t.ItemId = i.Id
	ORDER BY t.[PackageId];

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
