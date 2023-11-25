USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetBattleHistory]    Script Date: 11/25/2023 1:07:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetBattleHistory]
	@Id INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	SET @case = 100;
	SET @message = 'fetched a battle history';

	BEGIN TRY
		SELECT * FROM [dbo].[BattleHistory] WHERE [Id] = @Id;
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] null, @message, @case, @error, @time, 1, 1;
END