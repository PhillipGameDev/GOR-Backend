USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[StoreGameData]    Script Date: 1/4/2023 1:59:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[StoreGameData]
	@Type INT,
	@Data NVARCHAR(MAX)
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();
	DECLARE @tType INT = @Type;
	DECLARE @tData NVARCHAR(MAX) = @Data;

	DECLARE @vId INT = NULL;
	SELECT @vId = [Id] FROM [dbo].[GameData] WHERE [Type] = @tType;

	BEGIN TRY
		IF (@vId IS NULL)
			INSERT INTO [dbo].[GameData] ([Type], [Data]) VALUES (@tType, @tData)
		ELSE
			UPDATE [dbo].[GameData] SET [Data] = @tData WHERE [Id] = @vId;

		SET @case = 100;
		SET @message = 'Data stored';
	END TRY
	BEGIN CATCH
		SET @case = 200;
		SET @message = 'Error storing data'
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END