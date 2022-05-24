DELETE FROM [dbo].[ClanMember]
GO

DELETE FROM [dbo].[ClanInvite]
GO

DELETE FROM [dbo].[Clan]
GO

DELETE FROM [dbo].[Mail]
GO

DELETE FROM [dbo].[WorldTileData]
GO

DELETE FROM [dbo].[World]
GO

DELETE FROM [dbo].[PlayerData]
GO

DELETE FROM [dbo].[Player]
GO

DELETE FROM [dbo].[TransactionLog]
GO


DBCC CHECKIDENT ('[ClanMember]', RESEED, 0);
GO
DBCC CHECKIDENT ('[ClanInvite]', RESEED, 0);
GO
DBCC CHECKIDENT ('[Clan]', RESEED, 0);
GO

DBCC CHECKIDENT ('[Mail]', RESEED, 0);
GO
DBCC CHECKIDENT ('[WorldTileData]', RESEED, 0);
GO
DBCC CHECKIDENT ('[World]', RESEED, 0);
GO

DBCC CHECKIDENT ('[PlayerData]', RESEED, 0);
GO
DBCC CHECKIDENT ('[Player]', RESEED, 0);
GO
DBCC CHECKIDENT ('[TransactionLog]', RESEED, 0);
GO