USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetGameData]    Script Date: 1/4/2023 4:59:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetGameData]
	@Type INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @tType INT = @Type;

	DECLARE @vId INT = NULL;
	SELECT @vId = [Id] FROM [dbo].[GameData] WHERE [Type] = @tType;

	IF (@vId IS NOT NULL)
		BEGIN
			SET @case = 100;
			SET @message = 'Data retrieved';
			SELECT [Data] FROM [dbo].[GameData] WHERE [Id] = @vId;
		END
	ELSE
		BEGIN
			SET @case = 200;
			SET @message = 'Data not found'
		END

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
