USE [GameOfRevenge]
GO

CREATE TABLE [dbo].[MarketPlayerData]
(
	MarketPlayerDataId BIGINT IDENTITY(1,1) NOT NULL,
	FromPlayerId INT NOT NULL,
	ToPlayerId INT NOT NULL,
	StartTime DATETIME NOT NULL,
	IsRedeemed BIT NOT NULL,
	Food INT NULL,
	Wood INT NULL,
	Ore INT NULL,
	CONSTRAINT [PK_MarketPlayerData_MarketPlayerDataId] PRIMARY KEY CLUSTERED (MarketPlayerDataId ASC),
	CONSTRAINT [FK_MarketPlayerData_Player_FromPlayerId] FOREIGN KEY (FromPlayerId) REFERENCES [dbo].[Player] (PlayerId),
	CONSTRAINT [FK_MarketPlayerData_Player_ToPlayerId] FOREIGN KEY (ToPlayerId) REFERENCES [dbo].[Player] (PlayerId)
)
GO

CREATE OR ALTER PROCEDURE [dbo].[GiftResource]
	@FromPlayerId INT,
	@TargetPlayerId INT,
	@Food INT = NULL,
	@Wood INT = NULL,
	@Ore INT = NULL
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @FromPlayerId;

	DECLARE @fuserid INT = @FromPlayerId, 
			@tuserid INT = @TargetPlayerId, 
			@vFood INT = ISNULL(@Food,0), 
			@vWood INT = ISNULL(@Wood,0),
			@vOre INT = ISNULL(@Ore,0);

	DECLARE @vfuserId INT = NULL, @vtuserId INT = NULL;
	SELECT @vfuserId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @fuserid;

	IF (@vFood = 0 AND @vWood = 0 AND @vOre = 0)
		BEGIN
			SET @case = 100;
			SET @message = 'No resource are selected to be send';
		END
	ELSE IF (@vfuserId IS NULL OR @vfuserId = 0)
		BEGIN
			SET @case = 200;
			SET @message = 'Invalid from user id';
		END
	ELSE
		BEGIN
			SELECT @vtuserId = [PlayerId] FROM [dbo].[Player] WHERE [PlayerId] = @tuserid;
			IF (@vtuserId IS NULL OR @vtuserId = 0)
				BEGIN
					SET @case = 201;
					SET @message = 'Invalid to user id';
				END
			ELSE
				BEGIN TRY
					EXEC [dbo].[RemovePlayerResourceData] @vfuserId, @vFood, @vWood, @vOre, 0, 0;
					INSERT INTO [dbo].[MarketPlayerData] VALUES (@vfuserId, @vtuserId, @time, 0, @vFood, @vWood, @vOre);
					SET @case = 100;
					SET @message = 'Sended resource sucessfully';
				END TRY
				BEGIN CATCH
					SET @case = 0;
					SET @error = 1;
					SET @message = ERROR_MESSAGE();
				END CATCH
		END

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[RedeemGiftResource]
	@MarketPlayerDataId BIGINT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	DECLARE @tMarketPlayerDataId BIGINT = @MarketPlayerDataId;
	DECLARE @food INT, @wood INT, @ore INT;
	SELECT @userId = [ToPlayerId], @food = [Food], @wood = [Wood], @ore = [Ore] FROM [dbo].[MarketPlayerData] WHERE [MarketPlayerDataId] = @tMarketPlayerDataId AND [IsRedeemed] = 0;
	
	IF (@tMarketPlayerDataId IS NULL OR @tMarketPlayerDataId = 0)
		BEGIN
			SET @case = 200;
			SET @message = 'Already redemeed id';
		END
	ELSE IF (@userId IS NULL OR @userId = 0)
		BEGIN
			SET @case = 200;
			SET @message = 'Already redemeed';
		END
	ELSE
		BEGIN TRY
			EXEC [dbo].[AddPlayerResourceData] @userId, @food, @wood, @ore, 0, 0
		
			UPDATE [dbo].[MarketPlayerData]
			SET [IsRedeemed] = 1
			WHERE [MarketPlayerDataId] = @tMarketPlayerDataId;

			SET @case = 100;
			SET @message = 'Sended redemeed sucessfully';
		END TRY
		BEGIN CATCH
			SET @case = 0;
			SET @error = 1;
			SET @message = ERROR_MESSAGE();
		END CATCH

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[GetAllGiftResource]
	@PlayerId INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = @PlayerId;

	DECLARE @tExtId INT = NULL;

	SELECT @tExtId = [PlayerId] 
	FROM [dbo].[Player] 
	WHERE [PlayerId] = @userId;
	
	IF (@tExtId IS NULL OR @tExtId = 0)
		BEGIN
			SET @case = 200;
			SET @message = 'Account doest not exist';
		END
	ELSE
		BEGIN
			SET @case = 100;
			SET @message = 'All market gifts';
		END

	SELECT m.[MarketPlayerDataId], m.[FromPlayerId], p.[Name] AS 'From Player Name', m.[ToPlayerId], m.[StartTime], m.[IsRedeemed], m.[Food], m.[Wood], m.[Ore] FROM [dbo].[MarketPlayerData] AS m
	INNER JOIN [dbo].[Player] AS p ON p.[PlayerId] = m.[FromPlayerId]
	WHERE m.[ToPlayerId] = @tExtId AND m.[IsRedeemed] = 0;

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO


--[dbo].[GiftResource] 17, 29, 1000, 1000, 1000

--delete from [MarketPlayerData]

--SELECT * from [MarketPlayerData]
--go
--[dbo].[RedeemGiftResource] 5

--select * from playerdata where playerid = 18

--update playerdata
--set value = 0
--where playerdataid = 299

--update playerdata
--set value = 0
--where playerdataid = 300

--update playerdata
--set value = 0
--where playerdataid = 301

--update playerdata
--set value = 0
--where playerdataid = 302

--update playerdata
--set value = 10000
--where playerdataid = 166

--update playerdata
--set value = 10000
--where playerdataid = 167

--update playerdata
--set value = 10000
--where playerdataid = 168

--update playerdata
--set value = 10000
--where playerdataid = 169

--select * from playerdata where playerid = 18 and datatypeid = 1
--select * from playerdata where playerid = 29 and datatypeid = 1
--SELECT * from [MarketPlayerData]

--select * from player where playerid = 29