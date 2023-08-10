USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetStorePackages]    Script Date: 8/14/2022 3:13:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[GetStorePackages]
	@Active BIT = NULL
AS
BEGIN
	DECLARE @case INT = 100, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = 'All Store Packages';
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	IF (@Active IS NULL)
		SELECT [PackageId], [QuestId], [ProductId], [Cost], [Active] FROM [dbo].[PackageQuestRel];
	ELSE
		SELECT [PackageId], [QuestId], [ProductId], [Cost], [Active] FROM [dbo].[PackageQuestRel] WHERE [Active] = @Active;

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
