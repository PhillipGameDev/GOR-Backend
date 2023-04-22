USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[AddOrUpdatePlayerStoredData]    Script Date: 4/21/2023 10:33:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[AddOrUpdatePlayerStoredData]
	@PlayerId INT,
	@LocationId INT,
    @DataTypeId INT,
    @ValueId INT,
    @Value INT
AS
BEGIN
	DECLARE @case INT = 1, @error INT = 0;
    DECLARE @userId INT = @PlayerId;
	DECLARE @message NVARCHAR(MAX) = NULL;
	DECLARE @time DATETIME = GETUTCDATE();

    DECLARE @strucLocId INT = @LocationId;
    DECLARE @dTypeId INT = @DataTypeId;
    DECLARE @valId INT = @ValueId;
	DECLARE @val INT = ISNULL(@Value, 0);

    DECLARE @vPlayerId INT = NULL;
    SELECT @vPlayerId = p.[PlayerId] FROM [dbo].[Player] AS p WHERE p.[PlayerId] = @userId;

    DECLARE @RESOURCES INT = 1;

/*    IF (@val = 0)
        BEGIN
            SET @case = 101;
            SET @message = 'No updates was needed';
        END*/
    IF (@dTypeId <> @RESOURCES)
        BEGIN
            SET @case = 200;
            SET @message = 'Invalid data type';
        END
    ELSE IF ((@valId < 1) OR (@valId > 3))
        BEGIN
            SET @case = 201;
            SET @message = 'Invalid resource type';
        END
    ELSE IF (@vPlayerId IS NULL)
        BEGIN
            SET @case = 202;
            SET @message = 'No existing account found';
        END
    ELSE
        BEGIN TRY
            DECLARE @vPlaceId INT = NULL;
    --        SELECT @vPlaceId = p.[StructureLocationId] FROM [dbo].[StructureLocation] AS p WHERE p.[StructureLocationId] = @strucLocId;
            SET @vPlaceId = @strucLocId;

            DECLARE @vValId INT = NULL;
            SELECT @vValId = p.[ResourceId] FROM [dbo].[Resource] AS p WHERE p.[ResourceId] = @valId;

            IF (@vPlaceId IS NULL)
                BEGIN
                    SET @case = 203;
                    SET @message = 'No location found';
                END
            ELSE IF (@vValId IS NULL)
                BEGIN
                    SET @case = 204;
                    SET @message = 'Unknown resource type';
                END
            ELSE
                BEGIN
                    DECLARE @tempDataTable TABLE
                    (
                        StoredDataId BIGINT NOT NULL,
                        PlayerId INT NOT NULL,
                        StructureLocationId INT NOT NULL,
                        DataTypeId INT NOT NULL,
                        ValueId INT NOT NULL,
                        Value INT NOT NULL
                    );
--                    DECLARE @foodId INT, @resDataTypeId INT;

                    INSERT INTO @tempDataTable
                    SELECT p.[StoredDataId], p.[PlayerId], p.[StructureLocationId], p.[DataTypeId], p.[ValueId], p.[Value] 
                    FROM [dbo].[StoredData] AS p WHERE p.[PlayerId] = @vPlayerId AND p.[StructureLocationId] = @vPlaceId;

                    DECLARE @prevStoredId BIGINT = NULL;
                    DECLARE @prevStoredValue INT = NULL;

                    SELECT @prevStoredId = p.[StoredDataId], @prevStoredValue = p.[Value]
                    FROM @tempDataTable AS p 
                    WHERE p.[DataTypeId] = @dTypeId AND p.[ValueId] = @vValId;
                    IF (@prevStoredValue IS NULL) SET @prevStoredValue = 0;

                    DECLARE @dif INT = @val - @prevStoredValue;
                    IF (@dif <> 0)
                        BEGIN
                            IF (@prevStoredId IS NULL)
                                BEGIN
                                    SET NOCOUNT ON;

                                    INSERT INTO [dbo].[StoredData]
                                    OUTPUT INSERTED.StoredDataId
                                    VALUES (@vPlayerId, @vPlaceId, @dTypeId, @vValId, @val);

                                    SET @prevStoredId = SCOPE_IDENTITY();
                                    SET NOCOUNT OFF;
                                END
                            ELSE
                                UPDATE [dbo].[StoredData] SET [Value] = @val WHERE [StoredDataId] = @prevStoredId;

                            IF (@dif > 0)
                                BEGIN
                                    IF (@dTypeId = @RESOURCES)
                                        BEGIN
                                            IF (@vValId = 1)
                                                EXEC [RemovePlayerResourceData] @vPlayerId, @dif, 0, 0, 0, 0;
                                            ELSE IF (@vValId = 2)
                                                EXEC [RemovePlayerResourceData] @vPlayerId, 0, @dif, 0, 0, 0;
                                            ELSE IF (@vValId = 3)
                                                EXEC [RemovePlayerResourceData] @vPlayerId, 0, 0, @dif, 0, 0;
                                        END
                                END
                            ELSE
                                BEGIN
                                    SET @dif = -@dif;
                                    IF (@dTypeId = @RESOURCES)
                                        BEGIN
                                            IF (@vValId = 1)
                                                EXEC [AddPlayerResourceData] @vPlayerId, @dif, 0, 0, 0, 0;
                                            ELSE IF (@vValId = 2)
                                                EXEC [AddPlayerResourceData] @vPlayerId, 0, @dif, 0, 0, 0;
                                            ELSE IF (@vValId = 3)
                                                EXEC [AddPlayerResourceData] @vPlayerId, 0, 0, @dif, 0, 0;
                                        END
                                END

                            SET @case = 100;
                            SET @message = 'success';
                        END
                    ELSE
                        BEGIN
                            SET @case = 101;
                            SET @message = 'No updates was needed';
                        END

                    SELECT @prevStoredId AS 'StoredId', @val AS 'StoredValue', p.[PlayerDataId] AS 'DataId', p.[Value] AS 'DataValue' FROM [dbo].[PlayerData] AS p 
                    WHERE p.[PlayerId] = @vPlayerId AND p.[DataTypeId] = @dTypeId AND p.[ValueId] = @valId;
                END
        END TRY
        BEGIN CATCH
            SET @case = 0;
            SET @error = 1;
            SET @message = ERROR_MESSAGE();
        END CATCH

	EXEC [dbo].[GetMessage] @userId, @message, @case, @error, @time, 1, 1;
END