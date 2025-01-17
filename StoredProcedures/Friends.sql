USE [GameOfRevenge]
GO

/****** Object:  Table [dbo].[Contacts]    Script Date: 6/5/2023 2:45:49 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[Contacts](
	[ContactId] [BIGINT] IDENTITY(1,1) NOT NULL,
	[PlayerId] [int] NOT NULL,
	[OtherPlayerId] [int] NOT NULL,
	[Status] [TINYINT] NOT NULL,/* 0 none, 1 follow, 12 block */
	[CreationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ContactId] PRIMARY KEY CLUSTERED 
(
	[ContactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_PlayerId_OtherPlayerId] UNIQUE NONCLUSTERED 
(
	[PlayerId] ASC,
	[OtherPlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Contacts]  WITH CHECK ADD  CONSTRAINT [FK_Contacts_PlayerId_Player_PlayerId] FOREIGN KEY([PlayerId])
REFERENCES [dbo].[Player] ([PlayerId])
GO

ALTER TABLE [dbo].[Contacts] CHECK CONSTRAINT [FK_Contacts_PlayerId_Player_PlayerId]
GO

ALTER TABLE [dbo].[Contacts] ADD  DEFAULT (CURRENT_TIMESTAMP) FOR [CreationDate]
GO
