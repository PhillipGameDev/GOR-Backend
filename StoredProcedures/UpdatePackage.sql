USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpdatePackage]    Script Date: 8/14/2022 3:13:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[UpdatePackage]
	@PackageId INT,
	@Cost INT,
	@Active BIT
AS
BEGIN
	DECLARE @case INT = 100, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = 'Package updated';
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	BEGIN TRY
		IF EXISTS (SELECT 1 FROM [dbo].[PackageQuestRel] WHERE [PackageId] = @PackageId) AND (@Cost >= 0)
			UPDATE [dbo].[PackageQuestRel] SET [Cost] = @Cost, [Active] = @Active WHERE [PackageId] = @PackageId;
		ELSE
			BEGIN
				SET @case = 200;
				SET @message = 'Invalid params';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
