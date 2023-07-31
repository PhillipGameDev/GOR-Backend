USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerIDs]    Script Date: 3/18/2023 3:35:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetPlayerIDs]
	@PlayerId BIGINT = NULL,
	@Length INT = NULL,
	@Log BIT = 1
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	DECLARE @tplayerId BIGINT = ISNULL(@PlayerId, 0);
	DECLARE @tlen INT = ISNULL(@Length, 10);

	SET @case = 100;
	SET @message = 'Players';

	IF (@tlen = 0)
		SELECT [PlayerId], (CASE WHEN DATEDIFF(DAY, [LastLogin], @time) > (30 * 6) THEN 1 ELSE 0 END) AS 'Invaded'
		FROM [dbo].[Player] WHERE [PlayerId] > @tplayerId;
/*		SELECT [PlayerId], [LastLogin] FROM [dbo].[Player] WHERE [PlayerId] > @tplayerId;*/
	ELSE
		SELECT TOP (@tlen) [PlayerId], (CASE WHEN DATEDIFF(DAY, [LastLogin], @time) > (30 * 6) THEN 1 ELSE 0 END) AS 'Invaded'
		FROM [dbo].[Player] WHERE [PlayerId] > @tplayerId;
/*	ORDER BY [PlayerId] DESC;*/

	IF (@Log = 1) EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END