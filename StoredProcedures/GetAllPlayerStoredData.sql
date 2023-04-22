USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllPlayerStoredData]    Script Date: 4/22/2023 5:39:39 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetAllPlayerStoredData]
	@PlayerId INT,
	@DataTypeId INT = NULL,
	@LocationId INT = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @userId INT = @PlayerId;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @currentId INT = NULL;
	DECLARE @vDataTypeId INT = @DataTypeId;

    DECLARE @sLocId INT = @LocationId;

	BEGIN TRY
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
		ELSE
			BEGIN
				SET @case = 100;
				SET @message = 'All stored data'

				SELECT p.[StoredDataId], p.[StructureLocationId], p.[DataTypeId], p.[ValueId], p.[Value]
				FROM [dbo].[StoredData] AS p
				WHERE p.[PlayerId] = @currentId
				AND p.[Value] IS NOT NULL AND p.[Value] > 0
				AND (@vDataTypeId IS NULL OR p.[DataTypeId] = @vDataTypeId)
				AND (@sLocId IS NULL OR p.[StructureLocationId] = @sLocId);
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END