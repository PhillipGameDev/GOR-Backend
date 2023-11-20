USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[UpdateJoiningToUnion]    Script Date: 11/20/2023 9:52:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[UpdateJoiningToUnion]
	@FromClanId INT,
	@ToClanId INT,
	@Accepted BIT = NULL
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

		DECLARE @existingUnionId INT = NULL;
		SELECT @existingUnionId = c.[Id] FROM [dbo].[Union] AS c WHERE c.[FromClanId] = @FromClanId AND c.[ToClanId] = @ToClanId;

		IF (@existingUnionId IS NULL)
			BEGIN
				SELECT @existingUnionId = c.[Id] FROM [dbo].[Union] AS c WHERE c.[FromClanId] = @ToClanId AND c.[ToClanId] = @FromClanId;
			END

		IF (@existingFromClanId IS NULL OR @existingToClanId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Clan does not exist';
			END
		ELSE IF (@existingUnionId IS NULL)
			BEGIN
				SET @case = 201;
				SET @message = 'Union request does not exist';
			END
		ELSE
			BEGIN

				IF (@Accepted IS NULL)
					BEGIN
						DELETE FROM [dbo].[Union] WHERE [Id] = @existingUnionId;
					END
				ELSE
					BEGIN
						UPDATE [dbo].[Union] SET [Accepted] = @Accepted WHERE [Id] = @existingUnionId;
					END

				SET @case = 100;
				SET @message = 'Join request replied successfully!';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END