SET IDENTITY_INSERT [dbo].[Company] OFF
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

	INSERT INTO [dbo].[Company] 
		([CompanyGuid]
		,[CompanyName]
		,[WebSite]
		,[KeyTechnology]
		,[Note]
		,[IsHot]
		,[IsActive]
		,[IsDeleted]
		,[IsAdmin]
		,[OwnerId]
		,[EnteredBy]
		,[FranchiseId]
		,[DisplayOrder]
		,[CreatedOnUtc]
		,[UpdatedOnUtc]
		) 
	VALUES 
		(NEWID()
		,'Company_' + CAST( @RowCount as varchar(7) )
		,'http://www.comp' + REPLICATE('0', 7 - DATALENGTH(@RowString)) + @RowString + '.com'
		,'staffing ... ' + CAST( @RowCount as varchar(7) )
		,NULL
		,0
		,1
		,0
		,0
		,1
		,1
		,1
		,1
		,@InsertDate
		,@InsertDate
		)

	SET @RowCount = @RowCount + 1 
END
SET IDENTITY_INSERT [dbo].[Company] OFF
GO