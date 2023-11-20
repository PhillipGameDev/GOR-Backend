USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpdateClanCapacity]    Script Date: 11/20/2023 9:50:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[UpdateClanCapacity]
	@ClanId INT,
	@Capacity INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @tempCId INT = @ClanId;
	DECLARE @tempCapacity BIT = @Capacity;

	BEGIN TRY

		DECLARE @existingClanId INT = NULL;
		SELECT @existingClanId = c.[ClanId] FROM [dbo].[Clan] AS c WHERE c.[ClanId] = @tempCId;

		IF (@existingClanId IS NULL)
			BEGIN
				SET @case = 201;
				SET @message = 'Clan does not exists';
			END
		ELSE
			BEGIN
				UPDATE [dbo].[Clan] SET [Capacity] = @tempCapacity WHERE [ClanId] = @tempCId;
				SET @case = 100;
				SET @message = 'Update clan capacity successfully.';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END