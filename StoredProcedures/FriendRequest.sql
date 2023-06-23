USE [GameOfRevenge]
GO

/****** Object:  Table [dbo].[FriendRequest]    Script Date: 6/5/2023 2:45:49 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[FriendRequest](
	[RequestId] [BIGINT] IDENTITY(1,1) NOT NULL,
	[FromPlayerId] [int] NOT NULL,
	[ToPlayerId] [int] NOT NULL,
	[Flags] [TINYINT] NOT NULL,
	[RequestDate] [datetime] NOT NULL,
 CONSTRAINT [PK_RequestId] PRIMARY KEY CLUSTERED 
(
	[RequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_FromPlayerId_ToPlayerId] UNIQUE NONCLUSTERED 
(
	[FromPlayerId] ASC,
	[ToPlayerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FriendRequest]  WITH CHECK ADD  CONSTRAINT [FK_FromPlayerId_Player_PlayerId] FOREIGN KEY([FromPlayerId])
REFERENCES [dbo].[Player] ([PlayerId])
GO

ALTER TABLE [dbo].[FriendRequest] CHECK CONSTRAINT [FK_FromPlayerId_Player_PlayerId]
GO

ALTER TABLE [dbo].[FriendRequest]  WITH CHECK ADD  CONSTRAINT [FK_ToPlayerId_Player_PlayerId] FOREIGN KEY([ToPlayerId])
REFERENCES [dbo].[Player] ([PlayerId])
GO

ALTER TABLE [dbo].[FriendRequest] CHECK CONSTRAINT [FK_ToPlayerId_Player_PlayerId]
GO


