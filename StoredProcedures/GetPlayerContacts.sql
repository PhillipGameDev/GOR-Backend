USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerContacts]    Script Date: 5/1/2023 8:28:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetPlayerContacts]
	@PlayerId INT,
	@TargetPlayerId INT = NULL,
	@ContactId BIGINT = NULL,
	@Status TINYINT = NULL,
	@Log BIT = 1
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @BLOCKED TINYINT = 1;

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
				SELECT c.[ContactId], 
					(CASE WHEN c.[PlayerId] = @currentId THEN c.[Player2Id] ELSE c.[PlayerId] END) AS 'PlayerId', 
				    p1.[Name], 
				    CAST(p1.Rank1 AS INT) AS 'Rank1', 
				    cl.[ClanId], 
				    cl.[Name], 
				    (CASE WHEN c.[Player2Id] = @currentId THEN CAST((c.[Status] + 1) AS TINYINT) ELSE c.[Status] END) AS 'Status'
				FROM [dbo].[Contacts] AS c
				    INNER JOIN (
				        SELECT p.PlayerId, p.Name, ROW_NUMBER() OVER (ORDER BY Overall DESC) AS Rank1
				        FROM [dbo].[Player] AS p 
				        LEFT JOIN OverallRankings AS r ON r.[PlayerId] = p.[PlayerId]
				    ) AS p1 
					    ON ((c.[Player2Id] <> @currentId) AND (p1.[PlayerId] = c.[Player2Id]))
				        OR ((c.[PlayerId] <> @currentId) AND (p1.[PlayerId] = c.[PlayerId]))
				    LEFT JOIN [dbo].[ClanMember] AS cm ON cm.[PlayerId] = (CASE WHEN c.[PlayerId] = @currentId THEN c.[Player2Id] ELSE c.[PlayerId] END)
				    LEFT JOIN [dbo].[Clan] AS cl ON cl.[ClanId] = cm.[ClanId]
				WHERE ((c.[PlayerId] = @currentId) OR (c.[Player2Id] = @currentId))
				AND (@TargetPlayerId IS NULL OR ((c.[PlayerId] = @TargetPlayerId) OR (c.[Player2Id] = @TargetPlayerId)))
				AND ((@ContactId IS NULL AND (c.[Status] <> 0)) OR (c.[ContactId] = @ContactId))
				AND ((c.[PlayerId] = @currentId) OR ((c.[Status] <> @BLOCKED) OR @ContactID IS NOT NULL))
				AND ((CASE WHEN @Status IS NULL THEN 
					(CASE WHEN c.[Player2Id] = @currentId THEN (c.[Status] + 1) ELSE c.[Status] END)
					ELSE @Status END) = 
					(SELECT CASE WHEN c.[Player2Id] = @currentId THEN (c.[Status] + 1) ELSE c.[Status] END))

				SET @case = 100;
				SET @message = 'Player Contacts';
			END TRY
			BEGIN CATCH
				SET @case = 201;
				SET @message = 'Account does not exist';
			END CATCH
		END

	IF (@Log = 1) EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END