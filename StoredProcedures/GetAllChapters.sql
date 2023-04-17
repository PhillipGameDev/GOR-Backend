USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllChapters]    Script Date: 4/10/2023 11:51:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetAllChapters]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	SET @case = 100;
	SET @message = 'All chapters'

	SELECT [ChapterId], [Name], [Description] FROM [dbo].[Chapter]
	ORDER BY [ChapterId]

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END