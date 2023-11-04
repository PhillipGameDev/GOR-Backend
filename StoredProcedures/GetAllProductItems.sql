USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllProductItems]    Script Date: 11/4/2023 9:14:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetAllProductItems]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	SET @case = 100;
	SET @message = 'GetAllProductItems';

	SELECT * FROM [dbo].[PackageItem];

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
