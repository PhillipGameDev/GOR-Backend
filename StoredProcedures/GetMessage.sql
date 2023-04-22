USE [GameOfRevenge]
GO
/****** Object:  StoredProcedure [dbo].[GetMessage]    Script Date: 4/21/2023 12:03:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[GetMessage]
	@UserId INT, 
	@Message NVARCHAR(MAX),
	@Case INT,
	@IsError BIT,
	@Time DATETIME = NULL,
	@Print BIT = 1,
	@Store BIT = 0
AS
BEGIN  
	DECLARE @tempTime DATETIME = ISNULL(@Time, CURRENT_TIMESTAMP), 
			@tempUserId INT = @UserId, 
			@tempMessage NVARCHAR(MAX) = LTRIM(RTRIM(ISNULL(@Message,''))), 
			@tempCase INT = ISNULL(@Case,0), 
			@tempIsError INT = ISNULL(@IsError,0), 
			@tempPrint INT = ISNULL(@Print,0), 
			@tempStoreLog INT = ISNULL(@Store,0);

	IF (@tempPrint = 1)
		SELECT (CASE WHEN @tempIsError = 0 THEN @tempMessage 
					ELSE 'Unexpected database error has occured, please contact support team' 
				END) AS 'Message', @tempCase AS 'Case';
	IF (@Store = 1 OR @IsError = 1)
		INSERT INTO [dbo].[TransactionLog] VALUES (@tempUserId, @tempMessage, @tempCase, @tempIsError, @tempTime, CURRENT_TIMESTAMP);
END;