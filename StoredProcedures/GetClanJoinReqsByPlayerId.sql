USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetClanJoinReqsByPlayerId]    Script Date: 11/20/2023 9:39:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetClanJoinReqsByPlayerId]
	@PlayerId INT,
	@IsNew BIT = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;

	BEGIN TRY
		DECLARE @currentId INT = NULL;
		SELECT @currentId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @PlayerId;

		IF (@currentId IS NULL)
			BEGIN
				SET @case = 200;
				SET @message = 'Player does not exists';
			END
		ELSE
			BEGIN
				SELECT c.*, p.Name FROM [dbo].[ClanJoinRequest] as c 
				INNER JOIN [dbo].[Player] as p ON p.[PlayerId] = c.[PlayerId]
				WHERE c.[PlayerId] = @PlayerId AND (@IsNew IS NULL OR [IsNew] = @IsNew);
				SET @case = 100;
				SET @message = 'Fetched Clan join requests Successfully!';
			END
	END TRY
	BEGIN CATCH
		SET @case = 0;
		SET @error = 1;
		SET @message = ERROR_MESSAGE();
	END CATCH

	EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END