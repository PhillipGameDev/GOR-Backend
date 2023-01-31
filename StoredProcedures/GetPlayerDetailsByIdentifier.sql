USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerDetailsByIdentifier]    Script Date: 1/18/2023 5:48:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetPlayerDetailsByIdentifier]
	@Identifier VARCHAR(1000)
AS
BEGIN
	DECLARE @tIdentifier VARCHAR(1000) = LTRIM(RTRIM(@Identifier))
	DECLARE @id INT = NULL;
	SELECT @id = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerIdentifier] = @tIdentifier;
	EXEC [dbo].[GetPlayerDetailsById] @id
END