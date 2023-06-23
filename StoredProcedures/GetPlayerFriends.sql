USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerFriends]    Script Date: 6/5/2023 3:09:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetPlayerFriends]
	@PlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

	DECLARE @userId INT = @PlayerId;
	DECLARE @currentId INT = NULL;

	SELECT @currentId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @userId;

	IF (@currentId IS NULL)
		BEGIN
			SET @case = 200;
			SET @message = 'Account does not exist';
		END
	ELSE
		BEGIN
			BEGIN TRY
				WITH OverallRankings AS (
				    SELECT pd.[PlayerId],
				    SUM(t.TroopCount * (td.AttackDamage + td.Defence)) AS 'Overall'
				    FROM [dbo].[PlayerData] AS pd
				    CROSS APPLY OPENJSON(pd.[Value]) WITH (
				    [Level] INT,
				    [Count] INT,
				    [Wounded] INT
				    ) AS jsonValues
				    INNER JOIN [dbo].[TroopData] AS td ON (pd.[ValueId] = td.[TroopId]) AND (jsonValues.[Level] = td.[TroopLevel])
				    CROSS APPLY (
				    SELECT (CASE WHEN (jsonValues.Count - ISNULL(jsonValues.Wounded, 0) < 0) THEN 0 ELSE (jsonValues.Count - ISNULL(jsonValues.Wounded, 0)) END) AS TroopCount
				    ) AS t
				    WHERE pd.[DataTypeId] = 3
				    GROUP BY pd.[PlayerId]
				)
				SELECT f.[FriendId], f.[FriendPlayerId] AS 'PlayerId', p1.[Name], CAST(p1.Rank1 AS INT) AS Rank1, c.[ClanId], c.[Name] FROM [dbo].[Friends] as f
				INNER JOIN (
				  SELECT p.PlayerId, p.Name, ROW_NUMBER() OVER (ORDER BY Overall DESC) AS Rank1
				  FROM [dbo].[Player] as p
				  LEFT JOIN OverallRankings as r ON r.[PlayerId] = p.[PlayerId]
				) as p1 ON p1.[PlayerId] = f.[FriendPlayerId]
				INNER JOIN [dbo].[ClanMember] as cm ON cm.[PlayerId] = f.[FriendPlayerId]
				INNER JOIN [dbo].[Clan] as c ON c.[ClanId] = cm.[ClanId]
				WHERE f.[PlayerId] = @currentId;

				SET @case = 100;
				SET @message = 'Player friends';
			END TRY
			BEGIN CATCH
				SET @case = 200;
				SET @message = 'Account does not exist';
			END CATCH
		END

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END