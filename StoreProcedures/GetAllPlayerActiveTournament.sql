USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetAllPlayerActiveTournament]    Script Date: 8/14/2022 3:13:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[GetAllPlayerActiveTournament]
	@PlayerId INT
AS
BEGIN
	DECLARE @case INT = 100, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = 'Active tournaments';
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;
	
	SELECT td.[TournamentId], td.[Description], td.[EntryFee], td.[EntryFeeUnitId], td.[StartTime], td.[EndTime], td.[AddedOn], td.[Code], td.[IsActive]
	FROM [dbo].[Tournament] AS td
	INNER JOIN [dbo].[TournamentTicket] AS t ON t.[TournamentId] = td.[TournamentId]
	WHERE td.[IsActive] = 1 AND t.[PlayerId] = @userId AND td.[EndTime] < @time;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
