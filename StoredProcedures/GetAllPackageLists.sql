USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllPackageLists]    Script Date: 11/4/2023 9:13:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetAllPackageLists]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	SET @case = 100;
	SET @message = 'GetAllPackageLists';

	SELECT t.[Id], t.[Name], t.[Cost] FROM [dbo].[PackageList] as t;

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
