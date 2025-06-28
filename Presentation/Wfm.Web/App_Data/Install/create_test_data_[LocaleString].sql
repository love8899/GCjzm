SET IDENTITY_INSERT [dbo].[LocaleStringResource] OFF
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
	SET @InsertDate = DATEADD(dd, @Random, GETUTCDATE()) 

	INSERT INTO [dbo].[LocaleStringResource] 
		([LanguageId]
		,[ResourceName]
		,[ResourceValue]
		,[CreatedOnUtc]
		,[UpdatedOnUtc]
		) 
	VALUES 
		(1
		,'Admin.Common.SAMPLE.' + REPLICATE('0', 7 - DATALENGTH(@RowString)) + @RowString
		,'This is sample string ... ' + CAST( @RowCount as varchar(7) )
		,GETUTCDATE()
		,GETUTCDATE()
		)

	SET @RowCount = @RowCount + 1 
END
SET IDENTITY_INSERT [dbo].[LocaleStringResource] OFF
GO