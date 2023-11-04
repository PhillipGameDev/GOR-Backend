USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllPackages]    Script Date: 11/4/2023 9:13:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetAllPackages]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	SET @case = 100;
	SET @message = 'GetAllPackages'

	SELECT * FROM [dbo].[PackageList];

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
