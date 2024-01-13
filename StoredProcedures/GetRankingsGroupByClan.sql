USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetRankingsGroupByClan]    Script Date: 12/22/2023 3:36:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Rs301,,Name>
-- Create date: <2023-12-18,,>
-- Description:	<Get Clan OverAll Rankings,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetRankingsGroupByClan] 
	-- Add the parameters for the stored procedure here
	@RankId BIGINT = NULL
AS
BEGIN
DECLARE @case INT = 1, @error INT = 0;
  DECLARE @message NVARCHAR(MAX) = NULL;
  DECLARE @time DATETIME = GETUTCDATE();

  DECLARE @trankId BIGINT = ISNULL(@RankId, 0);

  SET @case = 100;
  SET @message = 'Rankings';


  WITH Rankings AS (
    SELECT c.[ClanId],
    SUM(t.TroopCount * (td.AttackDamage + td.Defence)) AS 'Overall',
    SUM(t.TroopCount * td.AttackDamage) AS 'Attack', 
    SUM(t.TroopCount * td.Defence) AS 'Defense'
    FROM [dbo].[PlayerData] AS pd
    CROSS APPLY OPENJSON(pd.[Value]) WITH (
      [Level] INT,
      [Count] INT,
      [Wounded] INT
    ) AS jsonValues
    INNER JOIN [dbo].[TroopData] AS td ON (pd.[ValueId] = td.[TroopId]) AND (jsonValues.[Level] = td.[TroopLevel])
	INNER JOIN [dbo].[ClanMember] AS c ON (c.[PlayerId] = pd.[PlayerId])
    CROSS APPLY (
      SELECT (CASE WHEN (jsonValues.Count - ISNULL(jsonValues.Wounded, 0) < 0) THEN 0 ELSE (jsonValues.Count - ISNULL(jsonValues.Wounded, 0)) END) AS TroopCount
    ) AS t
    WHERE pd.[DataTypeId] = 3
    GROUP BY c.[ClanId]
  )

  SELECT TOP 50 c1.ClanId, c1.Name, c1.LeaderName,
       CAST(c1.Rank1 AS INT) AS Rank1, CAST(ISNULL(c1.Overall, 0) AS BIGINT) AS Overall,
       CAST(c2.Rank2 AS INT) AS Rank2, CAST(ISNULL(c1.Attack, 0) AS BIGINT) AS Attack,
       CAST(c3.Rank3 AS INT) AS Rank3, CAST(ISNULL(c1.Defense, 0) AS BIGINT) AS Defense
  FROM (
    SELECT c.ClanId, c.Name, p.Name as LeaderName, Overall, Attack, Defense,
      ROW_NUMBER() OVER (ORDER BY Overall DESC) AS Rank1
    FROM [dbo].[Clan] as c
	LEFT JOIN [dbo].[ClanMember] AS m ON (m.[ClanId] = c.[ClanId] AND m.[ClanRoleId] = 1)
	LEFT JOIN [dbo].[Player] AS p ON m.PlayerId = p.PlayerId
    LEFT JOIN Rankings as r ON r.[ClanId] = c.[ClanId]
  ) c1
  FULL OUTER JOIN (
    SELECT c.ClanId, Overall, Attack, Defense,
      ROW_NUMBER() OVER (ORDER BY Attack DESC) AS Rank2
    FROM [dbo].[Clan] as c
    LEFT JOIN Rankings as r ON r.[ClanId] = c.[ClanId]
  ) c2 ON (c1.ClanId = c2.ClanId)
  FULL OUTER JOIN (
	SELECT c.ClanId, Overall, Attack, Defense,
      ROW_NUMBER() OVER (ORDER BY Defense DESC) AS Rank3
    FROM [dbo].[Clan] as c
    LEFT JOIN Rankings as r ON r.[ClanId] = c.[ClanId]
  ) c3 ON (c1.ClanId = c3.ClanId) OR (c2.ClanId = c3.ClanId)
  WHERE COALESCE(c1.Rank1, c2.Rank2, c3.Rank3) >= @trankId
  ORDER BY COALESCE(c1.Rank1, c2.Rank2, c3.Rank3)


  EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END
