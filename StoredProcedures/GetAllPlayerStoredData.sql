USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllPlayerStoredData]    Script Date: 8/14/2022 3:02:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetAllPlayerStoredData]
	@PlayerId INT,
	@StructureLocationId INT = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @userId INT = @PlayerId;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

    DECLARE @sLocId INT = @StructureLocationId;

	SET @case = 100;
	SET @message = 'All stored data'

	IF @sLocId IS NULL OR @sLocId = -1
		SELECT p.[StructureLocationId], p.[DataTypeId], p.[ValueId], p.[Value] FROM [dbo].[StoredData] AS p
		WHERE p.[PlayerId] = @userId AND p.[Value] IS NOT NULL AND p.[Value] > 0;
	ELSE
		SELECT p.[StructureLocationId], p.[DataTypeId], p.[ValueId], p.[Value] FROM [dbo].[StoredData] AS p
		WHERE p.[PlayerId] = @userId AND p.[StructureLocationId] = @sLocId AND p.[Value] IS NOT NULL AND p.[Value] > 0;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
