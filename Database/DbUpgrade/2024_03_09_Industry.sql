
IF OBJECT_ID('dbo.Industry') IS NULL
Begin
	CREATE TABLE [dbo].[Industry](
		[IndustryId] [int] IDENTITY(1,1) NOT NULL,
		[IndustryName] [nvarchar](255) NOT NULL,
		[IsActive] [bit] NOT NULL,
		[IsDeleted] [bit] NOT NULL,
		[EnteredBy] [int] NOT NULL,
		[CreatedOnUtc] [datetime] NULL,
		[UpdatedOnUtc] [datetime] NULL,
		PRIMARY KEY CLUSTERED 
		(
			[IndustryId] ASC
		)
	) ON [PRIMARY]
End
Else 
Begin
	PRINT 'Table dbo.Industry exists'
End

IF COL_LENGTH('[dbo].[Company]', 'IndustryId') IS NULL
Begin
	ALTER TABLE  [dbo].[Company]
	ADD IndustryId int NULL,
	FOREIGN KEY(IndustryId) REFERENCES [dbo].[Industry](IndustryId);
End
Else
Begin
	PRINT 'IndustryId exists in [dbo].[Company]'
End


IF COL_LENGTH('[dbo].[TestCategory]', 'IndustryId') IS NULL
Begin
	ALTER TABLE  [dbo].[TestCategory]
	ADD IndustryId int NULL,
	FOREIGN KEY(IndustryId) REFERENCES [dbo].[Industry](IndustryId);
End
Else
Begin
	PRINT 'IndustryId exists in [dbo].[TestCategory]'
End

/*
SET IDENTITY_INSERT [dbo].[Industry] ON
INSERT INTO [dbo].[Industry]
           (IndustryID
		   ,[IndustryName]
           ,[IsActive]
           ,[IsDeleted]
           ,[EnteredBy]
           ,[CreatedOnUtc]
           ,[UpdatedOnUtc])
     VALUES
           (1, 'Service', 1, 0, 0, GETUTCDATE(), GETUTCDATE()),
           (2, 'Manuafcture', 1, 0, 0, GETUTCDATE(), GETUTCDATE()),
           (3, 'Transportation', 1, 0, 0, GETUTCDATE(), GETUTCDATE())
SET IDENTITY_INSERT [dbo].[Industry] OFF
*/

-- select * from [dbo].[Industry];

-- update company set IndustryID = null;
-- update company set IndustryID = 1 where Id = 460
-- update company set IndustryID = 1 where Id = 489 and CompanyName like 'Test_company2'
-- select Id, CompanyName, IndustryID from company where Id in (460,489, 494)

-- update [dbo].[TestCategory] set IndustryID = 1 where Id = 2 and TestCategoryName like 'WHMIS Test';
-- update [dbo].[TestCategory] set CompanyId = 2 where Id = 3 
-- update [dbo].[TestCategory] set CompanyId = 2, IndustryID = 3 where Id = 4 
-- update [dbo].[TestCategory] set IndustryID = 2 where Id = 5
-- select Id, TestCategoryName, CompanyId, IndustryID from [dbo].[TestCategory] where IsActive = 1 and IsDeleted = 0;

-- ALTER TABLE [dbo].[TestCategory] DROP COLUMN IndustryId;
-- ALTER TABLE [dbo].[Company] DROP COLUMN IndustryId;
-- drop table [dbo].[Industry];


--select * from [dbo].[CandidateAppliedJobs] where CandidateId = 208539;
--select * from Candidate where Id = 208539;

--- Password='703F5D6B36EB53B58A428B7E769350FC6A9D7639': Password@123
--update [Candidate] set Password='703F5D6B36EB53B58A428B7E769350FC6A9D7639', PasswordSalt='ZSb7f3U=' where Id = 208539
