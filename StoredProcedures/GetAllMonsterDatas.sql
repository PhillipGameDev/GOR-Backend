USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllMonsterDatas]    Script Date: 11/25/2023 1:06:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetAllMonsterDatas]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	BEGIN TRY
		SELECT * FROM [dbo].[MonsterData]
		SET @case = 100;
		SET @message = 'Fetched all monster datas';
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] null, @message, @case, @error, @time, 1, 1;
END