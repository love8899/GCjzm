SET IDENTITY_INSERT [dbo].[Candidate] OFF
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

	INSERT INTO [dbo].[Candidate] 
		([CandidateGuid]
		,[UserName]
		,[Password]
		,[PasswordFormatId]
		,[PasswordSalt]
		,[PasswordResetToken]
		,[TokenExpiryDate]
		,[EmployeeId]
		,[Email]
		,[Email2]
		,[SalutationId]
		,[GenderId]
		,[EthnicTypeId]
		,[VetranTypeId]
		,[SourceId]
		,[FirstName]
		,[LastName]
		,[MiddleName]
		,[HomePhone]
		,[MobilePhone]
		,[EmergencyPhone]
		,[WebSite]
		,[BestTimetoCall]
		,[DisabilityStatus]
		,[IsHot]
		,[IsActive]
		,[IsBanned]
		,[IsDeleted]
		,[Entitled]
		,[CanRelocate]
		,[JobTitle]
		,[Education]
		,[Education2]
		,[DateAvailable]
		,[CurrentEmployer]
		,[CurrentPay]
		,[DesiredPay]
		,[ShiftId]
		,[TransportationId]
		,[MajorIntersection1]
		,[MajorIntersection2]
		,[PreferredWorkLocation]
		,[BirthDate]
		,[SocialInsuranceNumber]
		,[Note]
		,[EnteredBy]
		,[OwnerId]
		,[FranchiseId]
		,[CreatedOnUtc]
		,[UpdatedOnUtc]
		) 
	VALUES 
		(NEWID()
		,'Can_' + CAST( @RowCount as varchar(7) )
		,'pass'
		,0
		,null
		,NEWID()
		,null
		,'GC13_' + REPLICATE('0', 7 - DATALENGTH(@RowString)) + @RowString
		,'hotmail_' + CAST( @RowCount as varchar(7) ) + '@hotmail.com'
		,NULL
		,1
		,1
		,NULL
		,NULL
		,NULL
		,'Fname_' + CAST( @RowCount as varchar(7) )
		,'Lname_' + CAST( @RowCount as varchar(7) )
		,NULL
		,'416-888-9999'
		,NULL
		,NULL
		,NULL
		,NULL
		,0
		,0
		,1
		,0
		,0
		,1
		,0
		,'GL' + REPLICATE('0', 7 - DATALENGTH(@RowString)) + @RowString
		,NULL
		,NULL
		,NULL
		,NULL
		,NULL
		,NULL
		,1
		,2
		,'Int 1' + CAST( @RowCount as varchar(7) )
		,'Int 2' + CAST( @RowCount as varchar(7) )
		,NULL
		,NULL
		,NULL
		,NULL
		,1
		,1
		,1
		,@InsertDate
		,@InsertDate
		)

	SET @RowCount = @RowCount + 1 
END
SET IDENTITY_INSERT [dbo].[Candidate] OFF
GO