USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllTechnology]    Script Date: 8/19/2022 5:57:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetAllTechnology]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'All technology types';

	SELECT * FROM [dbo].[Technology]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
