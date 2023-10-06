USE [GameOfRevenge]
GO

/****** Object:  Table [dbo].[ZoneFortress]    Script Date: 9/12/2023 6:36:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ZoneFortress](
	[ZoneFortressId] [int] IDENTITY(1,1) NOT NULL,
	[WorldId] [int] NOT NULL,
	[ZoneIndex] [smallint] NOT NULL,
	[HitPoints] [int] NOT NULL,
	[Attack] [int] NOT NULL,
	[Defense] [int] NOT NULL,
	[ClanId] [int] NULL,
	[PlayerId] [int] NULL,
	[Data] [varchar](max) NULL
 CONSTRAINT [PK_ZoneFortress_ZoneFortressId] PRIMARY KEY CLUSTERED 
(
	[ZoneFortressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_ZoneFortress_UniqueCode] UNIQUE NONCLUSTERED 
(
	[WorldId] ASC,
	[ZoneIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ZoneFortress]  WITH CHECK ADD  CONSTRAINT [FK_ZoneFortress_WorldId] FOREIGN KEY([WorldId])
REFERENCES [dbo].[World] ([WorldId])
GO

ALTER TABLE [dbo].[ZoneFortress] CHECK CONSTRAINT [FK_ZoneFortress_WorldId]
GO



{"playerTroops":[{"troops":[{"troopType":1,"troopData":[{"level":1,"count":1000},{"level":2,"count":1000}]},{"troopType":4,"troopData":[{"level":1,"count":1000}]},{"troopType":3,"troopData":[{"level":2,"count":1000}]},{"troopType":2,"troopData":[{"level":3,"count":1000}]}]}]}


{"playerTroops":[{"troops":[{"troopType":1,"troopData":[{"level":1,"count":100000}]}]}],"startTime":"2023-10-04T15:26:00Z","duration":86400}



{"FirstCapturedTime":"2023-10-04T00:55:46.1898632Z","PlayerTroops":[{"PlayerId":1071,"Recalled":false,"Troops":[{"Id":10647,"TroopType":1,"TroopData":[{"Level":1,"Count":60}]},{"Id":44390,"TroopType":2,"TroopData":[{"Level":1,"Count":200}]}]},{"PlayerId":1145,"Recalled":true,"Troops":[{"Id":11495,"TroopType":1,"TroopData":[{"Level":2,"Count":200},{"Level":1,"Count":50}]},{"Id":11624,"TroopType":4,"TroopData":[{"Level":1,"Count":100},{"Level":2,"Count":25}]},{"Id":11623,"TroopType":2,"TroopData":[{"Level":1,"Count":25},{"Level":2,"Count":50}]}]}],"StartTime":"2023-10-04T00:55:46.1898632Z","Duration":600}




{"PlayerTroops":[{"PlayerId":0,"Troops":[{"Id":0,"TroopType":1,"TroopData":[{"Level":1,"Count":1000},{"Level":2,"Count":1000}]},{"Id":0,"TroopType":4,"TroopData":[{"Level":1,"Count":1000}]},{"Id":0,"TroopType":3,"TroopData":[{"Level":2,"Count":1000}]},{"Id":0,"TroopType":2,"TroopData":[{"Level":3,"Count":1000}]}]},{"PlayerId":1,"Troops":[{"Id":0,"TroopType":4,"TroopData":[{"Level":1,"Count":1000}]},{"Id":0,"TroopType":3,"TroopData":[{"Level":2,"Count":1000}]},{"Id":0,"TroopType":2,"TroopData":[{"Level":3,"Count":1000}]},{"Id":0,"TroopType":1,"TroopData":[{"Level":1,"Count":1000},{"Level":2,"Count":1000}]}]}]}


{"PlayerTroops":[{"PlayerId":0,"Troops":[{"Id":0,"TroopType":1,"TroopData":[{"Level":1,"Count":805},{"Level":2,"Count":1000}]},{"Id":0,"TroopType":4,"TroopData":[{"Level":1,"Count":1000}]},{"Id":0,"TroopType":3,"TroopData":[{"Level":2,"Count":1000}]},{"Id":0,"TroopType":2,"TroopData":[{"Level":3,"Count":1000}]}]},{"PlayerId":1,"Troops":[{"Id":0,"TroopType":4,"TroopData":[{"Level":1,"Count":1000}]},{"Id":0,"TroopType":3,"TroopData":[{"Level":2,"Count":1000}]},{"Id":0,"TroopType":2,"TroopData":[{"Level":3,"Count":1000}]},{"Id":0,"TroopType":1,"TroopData":[{"Level":1,"Count":806},{"Level":2,"Count":1000}]}]}]}
