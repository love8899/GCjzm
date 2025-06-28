SET IDENTITY_INSERT [dbo].[ActivityLog] OFF
DECLARE @RowCount 
INT DECLARE @RowString VARCHAR(10) 
DECLARE @Random INT 
DECLARE @Upper INT 
DECLARE @Lower INT 
DECLARE @InsertDate DATETIME 
SET @Lower = -730 
SET @Upper = -1 
SET @RowCount = 1 

WHILE @RowCount <= 1000000
BEGIN 
	SET @RowString = CAST(@RowCount AS VARCHAR(10)) 
	SELECT @Random = ROUND(((@Upper - @Lower -1) * RAND() + @Lower), 0) 
	-- SET @InsertDate = DATEADD(dd, @Random, GETUTCDATE()) 
	SET @InsertDate = DateAdd(d, ROUND(DateDiff(d, '2011-01-01', '2013-10-24') * RAND(), 0), '2011-01-01')

	INSERT INTO [dbo].[ActivityLog] 
		([ActivityLogTypeId]
		,[JobOrderId]
		,[JobTitle]
		,[CandidateId]
		,[CandidateName]
		,[AccountId]
		,[AccountName]
		,[FranchiseId]
		,[FranchiseName]
		,[ActivityLogDetail]
		,[CreatedOnUtc]
		,[UpdatedOnUtc]
		) 
	VALUES 
		(1
		,1
		,'JOb ... ' + REPLICATE('0', 7 - DATALENGTH(@RowString)) + @RowString
		,1
		,'CAN ... ' + REPLICATE('0', 7 - DATALENGTH(@RowString)) + @RowString
		,1
		,'ACC ... ' + REPLICATE('0', 7 - DATALENGTH(@RowString)) + @RowString
		,1
		,'FRA ... ' + REPLICATE('0', 7 - DATALENGTH(@RowString)) + @RowString
		,'This is sample log ... ' + CAST( @RowCount as varchar(7) )
		,GETUTCDATE()
		,GETUTCDATE()
		)

	SET @RowCount = @RowCount + 1 
END
SET IDENTITY_INSERT [dbo].[ActivityLog] OFF
GO