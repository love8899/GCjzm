SET IDENTITY_INSERT [dbo].[Franchise] OFF
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
	--SET @InsertDate = DATEADD(dd, @Random, GETUTCDATE())
	SET @InsertDate = DateAdd(d, ROUND(DateDiff(d, '2011-01-01', '2013-10-24') * RAND(), 0), '2011-01-01')

	INSERT INTO [dbo].[Franchise] 
		([FranchiseGuid]
		,[FranchiseId]
		,[FranchiseName]
		,[BusinessNumber]
		,[PrimaryContactName]
		,[Email]
		,[WebSite]
		,[Description]
		,[ReasonForDisabled]
		,[Note]
		,[IsHot]
		,[IsActive]
		,[IsDeleted]
		,[OwnerId]
		,[EnteredBy]
		,[DisplayOrder]
		,[CreatedOnUtc]
		,[UpdatedOnUtc]
		) 
	VALUES 
		(NEWID()
		,'GCFR13-'+ REPLICATE('0', 4 - DATALENGTH(@RowString)) + @RowString
		,'Franchise_' + REPLICATE('0', 7 - DATALENGTH(@RowString)) + @RowString
		,'416-850-5060'
		,1
		,'sales@fr' + CAST( @RowCount as varchar(7) ) + '.com'
		,'http://www.fr' + REPLICATE('0', 7 - DATALENGTH(@RowString)) + @RowString + '.com'
		,NULL
		,NULL
		,NULL
		,0
		,1
		,0
		,1
		,1
		,1
		,@InsertDate
		,@InsertDate
		)

	SET @RowCount = @RowCount + 1 
END
SET IDENTITY_INSERT [dbo].[Franchise] OFF
GO