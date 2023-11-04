USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllItems]    Script Date: 11/4/2023 9:11:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetAllItems]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'Items list fetched succesfully';

	SELECT s.[Id], s.[DataTypeId], s.[ValueId], s.[Value], s.[IsTimeItem], s.[CanBuy] FROM [dbo].[Item] AS s

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
