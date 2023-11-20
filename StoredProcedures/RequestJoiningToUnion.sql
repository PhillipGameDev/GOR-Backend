USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[RequestJoiningToUnion]    Script Date: 11/20/2023 9:49:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[RequestJoiningToUnion]
	@FromClanId INT,
	@ToClanId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	BEGIN TRY
		DECLARE @existingFromClanId INT = NULL;
		SELECT @existingFromClanId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @FromClanId;

		DECLARE @existingToClanId INT = NULL;
		SELECT @existingToClanId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @ToClanId;

		IF (@existingFromClanId IS NULL OR @existingToClanId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Clan does not exists';
			END
		ELSE
			BEGIN
				INSERT INTO [dbo].[Union] VALUES (@FromClanId, @ToClanId, 0);

				SET @case = 100;
				SET @message = 'Sent join request to clan successfully!';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END