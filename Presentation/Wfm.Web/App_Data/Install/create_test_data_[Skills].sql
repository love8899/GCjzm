SET IDENTITY_INSERT [dbo].[Skill] OFF
DECLARE @RowCount 
INT DECLARE @RowString VARCHAR(10) 
DECLARE @Random INT 
DECLARE @Upper INT 
DECLARE @Lower INT 
DECLARE @InsertDate DATETIME 
SET @Lower = -730 
SET @Upper = -1 
SET @RowCount = 1 

WHILE @RowCount <= 5000
BEGIN 
	SET @RowString = CAST(@RowCount AS VARCHAR(10)) 
	SELECT @Random = ROUND(((@Upper - @Lower -1) * RAND() + @Lower), 0) 
	SET @InsertDate = DATEADD(dd, @Random, GETUTCDATE()) 

	INSERT INTO [dbo].[Skill] 
		([SkillName]
		,[Description]
		,[IsActive]
		,[IsDeleted]
		,[EnteredBy]
		,[DisplayOrder]
		,[CreatedOnUtc]
		,[UpdatedOnUtc]
		) 
	VALUES 
		('Skill_' + REPLICATE('0', 7 - DATALENGTH(@RowString)) + @RowString
		,'This is a test skill ... ' + CAST( @RowCount as varchar(7) )
		,1
		,0
		,1
		,0
		,GETUTCDATE()
		,GETUTCDATE()
		)

	SET @RowCount = @RowCount + 1 
END
SET IDENTITY_INSERT [dbo].[Skill] OFF
GO