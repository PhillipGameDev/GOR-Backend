USE [GameOfRevenge]
GO


CREATE OR ALTER PROCEDURE [dbo].[GetAllResources]
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
	DECLARE @tempuserId INT = NULL;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
	DECLARE @userId INT = NULL;

	SET @case = 100;
	SET @message = 'Resource list fetched succesfully';

	SELECT r.[ResourceId], rr.[Code] AS 'Rarity', r.[Name], r.[Code] FROM [dbo].[Resource] AS r
	INNER JOIN [dbo].[Rarity] AS rr ON rr.[RarityId] = r.[RarityId]

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END
GO

--CREATE OR ALTER PROCEDURE [dbo].[AddResource]
--	@Name VARCHAR(1000),
--	@Code VARCHAR(100),
--	@RarityCode VARCHAR(100)
--AS
--BEGIN
--	DECLARE @case INT = 1, @error INT = 0;
--	DECLARE @tempuserId INT = NULL;
--	DECLARE @message NVARCHAR(MAX) = NULL;
--	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
--	DECLARE @userId INT = NULL;

--	DECLARE @tempName VARCHAR(1000) = LTRIM(RTRIM(@Name));
--	DECLARE @tempCode VARCHAR(100) = LTRIM(RTRIM(@Code));
--	DECLARE @tempRarityCode VARCHAR(100) = LTRIM(RTRIM(@RarityCode))

--	BEGIN TRY
--		IF(@tempName IS NULL OR @tempCode IS NULL OR @tempCode = '' OR @tempName = '')
--			BEGIN
--				SET @case = 200;
--				SET @message = 'Invalid data was provided';
--			END
--		ELSE
--			BEGIN
--				DECLARE @currentId INT = NULL;
--				SELECT @currentId = s.[ResourceId] FROM [dbo].[Resource] AS s WHERE s.[Code] = @tempCode;
--				DECLARE @rarityId INT = NULL
--				SELECT @rarityId = r.[RarityId] FROM [dbo].[Rarity] AS r WHERE r.[Code] = @tempRarityCode;
--				IF(@currentId IS NULL)
--					BEGIN
--						IF(@rarityId IS NULL)
--							SELECT TOP 1 @rarityId = r.[RarityId] FROM [dbo].[Rarity] AS r
--						INSERT INTO [dbo].[Resource] VALUES (@tempName, @tempCode, @rarityId)
--						SET @case = 100;
--						SET @message = 'Add Resource succesfully';
--					END
--				ELSE
--					BEGIN
--						IF(@rarityId IS NULL)
--							UPDATE [dbo].[Resource] SET Name = @tempName WHERE Code = @tempCode;
--						ELSE 
--							UPDATE [dbo].[Resource] SET Name = @tempName, RarityId = @rarityId
--							WHERE Code = @tempCode;
--						SET @case = 101;
--						SET @message = 'Updated Resource succesfully';
--					END
--			END
--	END TRY
--	BEGIN CATCH
--		SET @case = 0;
--		SET @error = 1;
--		SET @message = ERROR_MESSAGE();
--	END CATCH

--	SELECT r.[ResourceId], rr.[Code] AS 'Rarity', r.[Name], r.[Code] FROM [dbo].[Resource] AS r 
--	INNER JOIN [dbo].[Rarity] AS rr ON rr.[RarityId] = r.[RarityId]
--	WHERE r.[Code] = @tempCode

--	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
--END
--GO

--CREATE OR ALTER PROCEDURE [dbo].[RemoveResourceById]
--	@ResourceId INT
--AS
--BEGIN
--	DECLARE @case INT = 1, @error INT = 0;
--	DECLARE @tempuserId INT = NULL;
--	DECLARE @message NVARCHAR(MAX) = NULL;
--	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
--	DECLARE @userId INT = NULL;

--	DECLARE @tempResId INT = @ResourceId;

--	BEGIN TRY
--		DECLARE @currentId INT = NULL;
--		SELECT @currentId = s.[ResourceId] FROM [dbo].[Resource] AS s WHERE s.[ResourceId] = @tempResId;
--		IF(@currentId IS NULL)
--			BEGIN
--				SET @case = 200;
--				SET @message = 'Resource does not exists';
--			END
--		ELSE
--			BEGIN
--				DELETE FROM [dbo].[Resource] WHERE [ResourceId] = @currentId;
--				SET @case = 100;
--				SET @message = 'Removed Resource succesfully';
--			END
--	END TRY
--	BEGIN CATCH
--		SET @case = 0;
--		SET @error = 1;
--		SET @message = ERROR_MESSAGE();
--	END CATCH

--	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
--END
--GO

--CREATE OR ALTER PROCEDURE [dbo].[RemoveResourceByCode]
--	@Code VARCHAR(100)
--AS
--BEGIN
--	DECLARE @tempCode VARCHAR(100) = LTRIM(RTRIM(@Code));
--	DECLARE @currentId INT = NULL;
--	SELECT @currentId = r.[ResourceId] FROM [dbo].[Resource] AS r WHERE r.[Code] = @tempCode;
--	EXECUTE [dbo].[RemoveResourceById] @currentId
--END
--GO



--CREATE OR ALTER PROCEDURE [dbo].[GetResourceById]
--	@ResourceId INT
--AS
--BEGIN
--	DECLARE @case INT = 1, @error INT = 0;
--	DECLARE @tempuserId INT = NULL;
--	DECLARE @message NVARCHAR(MAX) = NULL;
--	DECLARE @time DATETIME = CURRENT_TIMESTAMP;
--	DECLARE @userId INT = NULL;
--	DECLARE @currentId INT = @ResourceId;
--	DECLARE @finalId INT = NULL;

--	BEGIN TRY
--		IF(@currentId IS NULL OR @currentId = 0)
--			BEGIN
--				SET @case = 200;
--				SET @message = 'Invalid data was provided';
--			END
--		ELSE
--			BEGIN
--				SELECT @finalId = s.[ResourceId] FROM [dbo].[Resource] AS s WHERE s.[ResourceId] = @currentId;
--				IF(@finalId IS NULL)
--					BEGIN
--						SET @case = 201;
--						SET @message = 'Resource does not exists';
--					END
--				ELSE
--					BEGIN
--						SET @case = 101;
--						SET @message = 'Resource fetched succesfully';
--					END
--			END
--	END TRY
--	BEGIN CATCH
--		SET @case = 0;
--		SET @error = 1;
--		SET @message = ERROR_MESSAGE();
--	END CATCH

--	SELECT r.[ResourceId], rr.[Code] AS 'Rarity', r.[Name], r.[Code] FROM [dbo].[Resource] AS r
--	INNER JOIN [dbo].[Rarity] AS rr ON rr.[RarityId] = r.[RarityId]
--	WHERE r.[ResourceId] = @finalId

--	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
--END
--GO

--CREATE OR ALTER PROCEDURE [dbo].[GetResourceByCode]
--	@Code VARCHAR(100)
--AS
--BEGIN
--	DECLARE @tempCode VARCHAR(100) = LTRIM(RTRIM(@Code));
--	DECLARE @currentId INT = NULL;
--	SELECT @currentId = s.[ResourceId] FROM [dbo].[Resource] AS s WHERE s.[Code] = @tempCode;
--	EXEC [dbo].[GetResourceById] @currentId
--END
--GO
