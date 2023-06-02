USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetRankings]    Script Date: 4/2/2023 6:27:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[GetRankings]
  @RankId BIGINT = NULL,
  @Length INT = NULL
AS
BEGIN
  DECLARE @case INT = 1, @error INT = 0;
  DECLARE @message NVARCHAR(MAX) = NULL;
  DECLARE @time DATETIME = GETUTCDATE();

  DECLARE @trankId BIGINT = ISNULL(@RankId, 0);
  DECLARE @tlen INT = @Length;

/*  IF OBJECT_ID('dbo.vw_RankingOverall', 'V') IS NULL
  BEGIN
     CREATE VIEW [dbo].[vw_RankingOverall] AS
     SELECT VIPoints FROM [dbo].[PlayerInfo]
  END*/

/*  IF (@tchatId = 0)
    BEGIN
      SELECT @tchatId = (ISNULL(MAX([RankId]), 0) + 1) FROM [dbo].[vw_RankingOverall];
      IF (@tlen IS NULL) SET @tlen = 10;
    END
  ELSE
    IF (@tlen IS NULL) SET @tlen = 5;*/


  SET @case = 100;
  SET @message = 'Rankings';


  WITH Rankings AS (
    SELECT pd.[PlayerId],
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
    CROSS APPLY (
      SELECT (CASE WHEN (jsonValues.Count - ISNULL(jsonValues.Wounded, 0) < 0) THEN 0 ELSE (jsonValues.Count - ISNULL(jsonValues.Wounded, 0)) END) AS TroopCount
    ) AS t
    WHERE pd.[DataTypeId] = 3
    GROUP BY pd.[PlayerId]
  )

  SELECT TOP 50 p1.PlayerId, p1.Name,
       CAST(p1.Rank1 AS INT) AS Rank1, CAST(ISNULL(p1.Overall, 0) AS BIGINT) AS Overall,
       CAST(p2.Rank2 AS INT) AS Rank2, CAST(ISNULL(p1.Attack, 0) AS BIGINT) AS Attack,
       CAST(p3.Rank3 AS INT) AS Rank3, CAST(ISNULL(p1.Defense, 0) AS BIGINT) AS Defense
  FROM (
    SELECT p.PlayerId, p.Name, Overall, Attack, Defense,
      ROW_NUMBER() OVER (ORDER BY Overall DESC) AS Rank1
    FROM [dbo].[Player] as p
    LEFT JOIN Rankings as r ON r.[PlayerId] = p.[PlayerId]
  ) p1
  FULL OUTER JOIN (
    SELECT p.PlayerId, Overall, Attack, Defense,
      ROW_NUMBER() OVER (ORDER BY Attack DESC) AS Rank2
    FROM [dbo].[Player] as p
    LEFT JOIN Rankings as r ON r.[PlayerId] = p.[PlayerId]
  ) p2 ON (p1.PlayerId = p2.PlayerId)
  FULL OUTER JOIN (
    SELECT p.PlayerId, Overall, Attack, Defense,
      ROW_NUMBER() OVER (ORDER BY Defense DESC) AS Rank3
    FROM [dbo].[Player] as p
    LEFT JOIN Rankings as r ON r.[PlayerId] = p.[PlayerId]
  ) p3 ON (p1.PlayerId = p3.PlayerId) OR (p2.PlayerId = p3.PlayerId)
  WHERE COALESCE(p1.Rank1, p2.Rank2, p3.Rank3) >= @trankId
  ORDER BY COALESCE(p1.Rank1, p2.Rank2, p3.Rank3)


  EXEC [dbo].[GetMessage] NULL, @message, @case, @error, @time, 1, 1;
END