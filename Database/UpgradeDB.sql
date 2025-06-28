--- DO NOT CHANGE THIS PART

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID (N'DB_Version', N'U') IS  NULL
Begin
	CREATE TABLE [DB_Version](
		[VersionNumber] [decimal](18, 2) NOT NULL,
		[CommitTimestamp] [smalldatetime] NOT NULL
	) ON [PRIMARY]


	ALTER TABLE DB_Version
	ADD CONSTRAINT UQ_VersionNumber UNIQUE (VersionNumber); 
End

IF OBJECT_ID ( 'NewDBStep', 'P' ) IS NULL 
begin
	Exec ('
	-- ============================================
	-- Description:	Applies a new DB upgrade step to the current DB
	-- =============================================
	CREATE PROCEDURE NewDBStep 
		@dbVersion [decimal](18, 2),
		@script    varchar (max)
	AS
	BEGIN
		If not exists (select 1 from DB_Version Where VersionNumber = @dbVersion)
		Begin
			-- SET NOCOUNT ON added to prevent extra result sets from
			-- interfering with SELECT statements.
			SET NOCOUNT ON;
		
			BEGIN TRY
				Begin tran
				Exec (@script)

				Insert into DB_Version (VersionNumber, CommitTimestamp) Values (@dbVersion, CURRENT_TIMESTAMP);
				Commit tran

				Print ''Applied upgrade step '' + Cast ( @dbVersion as nvarchar(20))
			END TRY
			BEGIN CATCH
				Rollback tran
				Print ''Failed to apply step '' + Cast ( @dbVersion as nvarchar(20))
				Select ERROR_NUMBER() AS ErrorNumber
				,ERROR_SEVERITY() AS ErrorSeverity
				,ERROR_STATE() AS ErrorState
				,ERROR_PROCEDURE() AS ErrorProcedure
				,ERROR_LINE() AS ErrorLine
				,ERROR_MESSAGE() AS ErrorMessage;
			END CATCH
		End
	END ') ;
End

----------------  Add the new steps here
-- Step: 0.01 
-- TFS: NA 
-- Adding the Payroll_Users table if it does not exist. This is to sunc up the older existing DBs
Exec NewDBStep 0.01, '
IF OBJECT_ID (N''Payroll_Users'', N''U'') IS  NULL
Begin
	CREATE TABLE [Payroll_Users](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[UserType] [nvarchar](20) NULL,
		[UserName] [nvarchar](255) NULL,
		[Password] [nvarchar](255) NULL,
		[Encrypted_UserName] [nvarchar](255) NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
End
'

-- Step: 0.02
-- TFS: NA 
-- Applying the old scripts. This is to sunc up the older existing DBs
Exec NewDBStep 0.02, '
IF OBJECT_ID (N''PayPeriodBillingChart_Approved_Processed'', N''U'') IS  NULL
Begin
	CREATE TABLE [PayPeriodBillingChart_Approved_Processed](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[PayPeriodBillingChartId] [int] NULL,
		[IsApproved] [char](1) NULL,
		[ApprovedBy] [nvarchar](255) NULL,
		[IsProcessed] [char](1) NULL,
		[ProcessedBy] [nvarchar](255) NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]	
End

IF OBJECT_ID (N''Candidate_Other_Payments_Approved_Processed'', N''U'') IS  NULL
Begin
	CREATE TABLE [Candidate_Other_Payments_Approved_Processed](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[CandidateOtherPaymentsId] [int] NULL,
		[IsApproved] [char](1) NULL,
		[ApprovedBy] [nvarchar](255) NULL,
		[IsProcessed] [char](1) NULL,
		[ProcessedBy] [nvarchar](255) NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
End

IF OBJECT_ID (N''Candidate_Payment_History'', N''U'') IS  NULL
Begin
	CREATE TABLE [Candidate_Payment_History](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[CandidateId] [int] NULL,
		[CPP] [decimal](18, 2) NULL,
		[EI] [decimal](18, 2) NULL,
		[Year] [char](4) NULL,
		[Federal_Tax] [decimal](18, 2) NULL,
		[Provincial_Tax] [decimal](18, 2) NULL,
		[Bank_Charges] [decimal](18, 2) NULL,
		[NetPay] [decimal](18, 2) NULL,
		[YTD_CPP] [decimal](18, 2) NULL,
		[YTD_EI] [decimal](18, 2) NULL,
		[YTD_Federal_Tax] [decimal](18, 2) NULL,
		[YTD_Provincial_Tax] [decimal](18, 2) NULL,
		[YTD_NetPay] [decimal](18, 2) NULL,
		[YTD_Bank_Charges] [decimal](18, 2) NULL,
		[GrossPay] [decimal](18, 2) NULL,
		[YTD_GrossPay] [decimal](18, 2) NULL,
		[Payment_Date] [datetime] NULL,
		[Cheque_Number] [nvarchar](20) NULL,
		[Direct_Deposit_Number] [nvarchar](20) NULL
	) ON [PRIMARY]

	SET ANSI_PADDING ON
	ALTER TABLE [Candidate_Payment_History] ADD [Week_Start] [char](2) NULL
	ALTER TABLE [Candidate_Payment_History] ADD [Week_End] [char](2) NULL
	ALTER TABLE [Candidate_Payment_History] ADD [YTD_Hours] [decimal](18, 2) NULL
	ALTER TABLE [Candidate_Payment_History] ADD [QPIP] [decimal](18, 2) NULL
	ALTER TABLE [Candidate_Payment_History] ADD [YTD_QPIP] [decimal](18, 2) NULL
	ALTER TABLE [Candidate_Payment_History] ADD [PayPeriodStartDate] [datetime] NULL
	ALTER TABLE [Candidate_Payment_History] ADD [PayPeriodEndDate] [datetime] NULL
	ALTER TABLE [Candidate_Payment_History] ADD [IsEmailed] [char](1) NULL
	ALTER TABLE [Candidate_Payment_History] ADD [CompanyId] [int] NULL
	ALTER TABLE [Candidate_Payment_History] ADD [EmployeeType] [int] NULL
	ALTER TABLE [Candidate_Payment_History] ADD [PayrollType] [nvarchar](12) NULL
	ALTER TABLE [Candidate_Payment_History] ADD [IsPrinted] [char](1) NULL
	ALTER TABLE [Candidate_Payment_History] ADD [QHSF] [decimal](18, 2) NULL
	ALTER TABLE [Candidate_Payment_History] ADD [YTD_QHSF] [decimal](18, 2) NULL
	ALTER TABLE [Candidate_Payment_History] ADD [YTD_CNT] [decimal](18, 2) NULL
	ALTER TABLE [Candidate_Payment_History] ADD [CNT] [decimal](18, 2) NULL
	 CONSTRAINT [PK_Candidate_YTD_CPP_EI] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
End
'

-- Step: 0.03
-- TFS: NA 
-- Applying the old scripts. This is to sunc up the older existing DBs
Exec NewDBStep 0.03, '
IF OBJECT_ID (N''BackupFiles'', N''U'') IS  NULL
Begin
	CREATE TABLE [[BackupFiles](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[FileTitle] [varchar](max) NULL,
		[File] [varbinary](max) NULL,
	 CONSTRAINT [PK_BackupFiles] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]	
End

IF OBJECT_ID (N''Candidate_T4'', N''U'') IS  NULL
Begin
	CREATE TABLE [Candidate_T4](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[CandidateId] [int] NULL,
		[Year] [char](4) NULL,
		[SIN] [nvarchar](max) NULL,
		[ProvinceCode] [char](2) NULL,
		[YTD_GrossPay] [decimal](18, 2) NULL,
		[YTD_Tax] [decimal](18, 2) NULL,
		[Maximum_Pensionable_Earnings] [decimal](18, 2) NULL,
		[YTD_CPP] [decimal](18, 2) NULL,
		[Maximum_Insurable_Earnings] [decimal](18, 2) NULL,
		[YTD_EI] [decimal](18, 2) NULL,
		[LastName] [nvarchar](255) NULL,
		[FirstName] [nvarchar](255) NULL,
		[AddressLine1] [nvarchar](max) NULL,
		[AddressLine2] [nvarchar](max) NULL,
		[City] [nvarchar](max) NULL,
		[Postalcode] [nvarchar](max) NULL,
	 CONSTRAINT [PK_Candidate_T4] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]		
End

IF OBJECT_ID (N''Company_Contact_Information'', N''U'') IS  NULL
Begin
	CREATE TABLE [Company_Contact_Information](
		[CompanyId] [int] NOT NULL,
		[Email] [nvarchar](255) NULL,
		[ContactPerson] [nvarchar](max) NULL,
		[TermOfPayment] [int] NULL
	) 	
End

IF OBJECT_ID (N''Candidate_Hire_Data'', N''U'') IS  NULL
Begin
	CREATE TABLE [Candidate_Hire_Data](
	[CandidateId] [int] NOT NULL,
	[FirstHireDate] [datetime] NULL,
	[LastHireDate] [datetime] NULL,
	[TerminationDate] [datetime] NULL
) ON [PRIMARY]
End

IF OBJECT_ID (N''WSIB'', N''U'') IS  NULL
Begin
	CREATE TABLE [WSIB](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[StartDate] [datetime] NULL,
		[EndDate] [datetime] NULL,
		[JobOrderId] [int] NULL,
		[JobDescription] [nvarchar](max) NULL,
		[GrossAmount] [decimal](18, 2) NULL,
		[WSIBRate] [decimal](18, 2) NULL,
		[WSIBAmount] [decimal](18, 2) NULL,
		[JobOrderCategory] [nvarchar](255) NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
End


IF OBJECT_ID (N''Invoice_Bridge'', N''U'') IS  NULL
Begin
	CREATE TABLE [Invoice_Bridge](
		[InvoiceId] [int] NOT NULL,
		[PayPeriodBillingChartId] [int] NOT NULL
	) ON [PRIMARY]
End

IF OBJECT_ID (N''SalesTaxRates'', N''U'') IS  NULL
Begin
	CREATE TABLE [SalesTaxRates](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Province] [char](2) NULL,
		[Type] [nvarchar](10) NULL,
		[ProvinceRate] [decimal](18, 3) NULL,
		[TotalTax] [decimal](18, 3) NULL,
	 CONSTRAINT [PK_SalesTaxRates] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
End
'

-- Step: 0.04
-- TFS: NA 
-- Applying the old scripts. This is to sunc up the older existing DBs
Exec NewDBStep 0.04, '
IF OBJECT_ID (N''Invoice'', N''U'') IS  NULL
Begin
	CREATE TABLE [Invoice](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[CompanyId] [int] NULL,
		[Date] [datetime] NULL,
		[Amount] [decimal](18, 2) NULL,
		[Tax] [decimal](18, 2) NULL,
		[InvoiceTotal] [decimal](18, 2) NULL,
		[Comments] [nvarchar](200) NULL,
	 CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]	
End

IF OBJECT_ID (N''Candidate_Payment_History'', N''U'') IS  NULL
Begin
	CREATE TABLE [Candidate_Payment_History](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[CandidateId] [int] NULL,
		[CPP] [decimal](18, 2) NULL,
		[EI] [decimal](18, 2) NULL,
		[Year] [char](4) NULL,
		[Federal_Tax] [decimal](18, 2) NULL,
		[Provincial_Tax] [decimal](18, 2) NULL,
		[Bank_Charges] [decimal](18, 2) NULL,
		[NetPay] [decimal](18, 2) NULL,
		[YTD_CPP] [decimal](18, 2) NULL,
		[YTD_EI] [decimal](18, 2) NULL,
		[YTD_Federal_Tax] [decimal](18, 2) NULL,
		[YTD_Provincial_Tax] [decimal](18, 2) NULL,
		[YTD_NetPay] [decimal](18, 2) NULL,
		[YTD_Bank_Charges] [decimal](18, 2) NULL,
		[GrossPay] [decimal](18, 2) NULL,
		[YTD_GrossPay] [decimal](18, 2) NULL,
		[Payment_Date] [datetime] NULL,
		[Cheque_Number] [nvarchar](20) NULL,
		[Direct_Deposit_Number] [nvarchar](20) NULL
	) ON [PRIMARY]
	
	SET ANSI_PADDING ON

	ALTER TABLE [Candidate_Payment_History] ADD [Week_Start] [char](2) NULL
	ALTER TABLE [Candidate_Payment_History] ADD [Week_End] [char](2) NULL
	ALTER TABLE [Candidate_Payment_History] ADD [YTD_Hours] [decimal](18, 2) NULL
	ALTER TABLE [Candidate_Payment_History] ADD [PayPeriod] [nvarchar](50) NULL
	ALTER TABLE [Candidate_Payment_History] ADD [QPIP] [decimal](18, 2) NULL
	ALTER TABLE [Candidate_Payment_History] ADD [YTD_QPIP] [decimal](18, 2) NULL
	 CONSTRAINT [PK_Candidate_YTD_CPP_EI] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

End

'
-- Step: 1.00 
-- TFS: 2 
Exec NewDBStep 1.00, 'ALTER TABLE Payroll_Users ADD CONSTRAINT UQ_UserName UNIQUE (UserName) '

-- Step: 2.00 
-- TFS: 7
-- Adding PayFrequency table to be used in Pay Group Feature
Exec NewDBStep 2.00, 'CREATE TABLE [PayFrequencyType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NOT NULL CONSTRAINT UQ_PayFrequencyType_Code UNIQUE,
	[Description] [nvarchar](100) NOT NULL,
	[Frequency] [int] NOT NULL,
 CONSTRAINT [PK_PayFrequencyType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)  '

-- Step: 3.00 
-- TFS: 7
-- Adding the pre-defined Pay Frequency types
Exec NewDBStep 3.00, 'INSERT INTO [PayFrequencyType] ([Code] ,[Description] ,[Frequency]) VALUES(''MONTHLY'' ,''Monthly (12 pay periods a year)'' ,12)
INSERT INTO [PayFrequencyType] ([Code] ,[Description] ,[Frequency]) VALUES(''SEMI_MONTHLY'' ,''Semi-monthly (24 pay periods a year)'' ,24)
INSERT INTO [PayFrequencyType] ([Code] ,[Description] ,[Frequency]) VALUES(''WEEKLY52'' ,''Weekly (52 pay periods a year)'' ,52)
INSERT INTO [PayFrequencyType] ([Code] ,[Description] ,[Frequency]) VALUES(''WEEKLY53'' ,''Weekly (53 pay periods a year)'' ,53)
INSERT INTO [PayFrequencyType] ([Code] ,[Description] ,[Frequency]) VALUES(''BIWEEKLY26'' ,''Biweekly (26 pay periods a year)'' ,26)
INSERT INTO [PayFrequencyType] ([Code] ,[Description] ,[Frequency]) VALUES(''BIWEEKLY27'' ,''Biweekly (27 pay periods a year)'' ,27) '

-- Step: 4.00 
-- TFS: 7
-- Adding PayGroup table to be used in Pay Group Feature
Exec NewDBStep 4.00, 'CREATE TABLE [PayGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NOT NULL CONSTRAINT UQ_PayGRoup_Code UNIQUE,
	[Name] [nvarchar](60) NOT NULL,
	[PayFrequencyTypeId] [int] NOT NULL,
	[Year] [int] NOT NULL,
 CONSTRAINT [PK_PayGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [PayGroup]  WITH CHECK ADD  CONSTRAINT [FK_PayGroup_PayFrequencyType] FOREIGN KEY([PayFrequencyTypeId])
REFERENCES [PayFrequencyType] ([Id])

ALTER TABLE [PayGroup] CHECK CONSTRAINT [FK_PayGroup_PayFrequencyType] 
'
-- Step: 5.00 
-- TFS: 37
-- Adding a column named "Issued Date" to Table "CandidateT4"
Exec NewDBStep 5.00, 'ALTER TABLE [Candidate_T4]
ADD Issued_Date DateTime;
'
-- Step: 6.00 
Exec NewDBStep 6.00, 'INSERT  [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.SupervisorUpdateWorkTime'', N''Supervisor Update'', GETUTCDATE(), GETUTCDATE())
INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.TimeSheet.SupervisorUpdateApproval'', N''Supervisor Update/Approve Worktime'', GETUTCDATE(), GETUTCDATE());'

-- Step: 7.00 
-- TFS: 7
-- Adding a new table for Payroll Calendar
Exec NewDBStep 7.00, '
CREATE TABLE [Payroll_Calendar](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PayGroupId] [int] NOT NULL,
	[PayPeriodNumber] [int] NOT NULL,
	[PayPeriodStartDate] [datetime] NOT NULL,
	[PayPeriodEndDate] [datetime] NOT NULL,
	[PayPeriodPayDate] [datetime] NOT NULL,
	[PayPeriodCommitDate] [datetime] NULL,
 CONSTRAINT [PK_Payroll_Calendar] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [Payroll_Calendar]  WITH CHECK ADD  CONSTRAINT [FK_Payroll_Calendar_PayGroup] FOREIGN KEY([PayGroupId])
REFERENCES [PayGroup] ([Id])
ON DELETE CASCADE


ALTER TABLE [Payroll_Calendar] CHECK CONSTRAINT [FK_Payroll_Calendar_PayGroup]
'


-- Step: 8.00 
-- TFS: 11
-- Adding a new table for Candidates' TD1 data
Exec NewDBStep 8.00, '
IF OBJECT_ID (N''Candidate_TD1_Federal'', N''U'') IS  NOT NULL
Begin
	ALTER TABLE [Candidate_TD1_Federal] DROP CONSTRAINT [PK_Candidate_TD1]
End


CREATE TABLE [Candidate_TD1](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CandidateId] [int] NOT NULL,
	[Year] [int] NOT NULL,
	[Basic_Amount] [decimal](10, 2) NOT NULL,
	[Child_Amount] [decimal](10, 2) NULL,
	[Age_Amount] [decimal](10, 2) NULL,
	[Pension_Income_Amount] [decimal](10, 2) NULL,
	[Tuition_Amounts] [decimal](10, 2) NULL,
	[Disablility_Amount] [decimal](10, 2) NULL,
	[Spouse_Amount] [decimal](10, 2) NULL,
	[Eligible_Dependant_Amount] [decimal](10, 2) NULL,
	[Caregiver_Amount] [decimal](10, 2) NULL,
	[Infirm_Dependant_Amount] [decimal](10, 2) NULL,
	[Amount_Transferred_From_Spouse] [decimal](10, 2) NULL,
	[Amount_Transferred_From_Dependant] [decimal](10, 2) NULL,
	[Family_Tax_Benefit] [decimal](10, 2) NULL,
	[Senior_Supplementary_Amount] [decimal](10, 2) NULL,
 CONSTRAINT [PK_Candidate_TD1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 


ALTER TABLE [Candidate_TD1]  WITH CHECK ADD  CONSTRAINT [FK_Candidate_TD1_Candidate] FOREIGN KEY([CandidateId])
REFERENCES [Candidate] ([Id])


ALTER TABLE [Candidate_TD1] CHECK CONSTRAINT [FK_Candidate_TD1_Candidate]
'


-- Step: 9.00 
-- TFS: 11
-- Drop table Candidate_TD1_Federal and Candidate_TD1_Provincial
Exec NewDBStep 9.00, '
IF OBJECT_ID (N''Candidate_TD1_Federal'', N''U'') IS  NOT NULL
Begin
	DROP TABLE [Candidate_TD1_Federal]
End

IF OBJECT_ID (N''Candidate_TD1_Provincial'', N''U'') IS  NOT NULL
Begin
	DROP TABLE [Candidate_TD1_Provincial]
End

--'

-- Step: 10.00 
-- TFS: 11
-- Add the province code to Candidate_TD1 table
Exec NewDBStep 10.00, '
Alter TABLE [Candidate_TD1] Add [Province_Code] [varchar](2) NOT NULL
'
-- Step: 11.00 
-- TFS: 65
-- Adding new tables to support payroll items 
Exec NewDBStep 11.00, '
CREATE TABLE [Payroll_Item_Type](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](30) NOT NULL CONSTRAINT UQ_Payroll_Item_Type_Name UNIQUE,
	[Code] [nvarchar](30) NOT NULL CONSTRAINT UQ_Payroll_Item_Type_Code UNIQUE,
	[IsInternal] [bit] NOT NULL,
 CONSTRAINT [PK_Payroll_Item_Type] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



CREATE TABLE [Payroll_Item_SubType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](30) NOT NULL CONSTRAINT UQ_Payroll_Item_SubType_Name UNIQUE,
	[UnitName] [nvarchar](20) NULL,
	[Code] [nvarchar](30) NOT NULL CONSTRAINT UQ_Payroll_Item_SubType_Code UNIQUE,
	[IsInternal] [bit] NOT NULL,
 CONSTRAINT [PK_Payroll_Item_SubType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



CREATE TABLE [Payroll_Item](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](4) NULL,
	[Description] [varchar](30) NULL,
	[TypeID] [int] NOT NULL,
	[SubTypeId] [int] NOT NULL,
	[State_Code] [varchar](3) NULL,
	[PrintOnPayStub] [bit] NOT NULL DEFAULT (1),
	[IsReadOnly] [bit] NOT NULL DEFAULT (0),
	[IsTaxable] [bit] NOT NULL DEFAULT (0),
	[IsPensionable] [bit] NOT NULL DEFAULT (0),
	[IsInsurable] [bit] NOT NULL DEFAULT (0),
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

'

-- Step: 12.00 
-- TFS: 65
-- Adding new FK constraints to support payroll items 
Exec NewDBStep 12.00, '
ALTER TABLE [Payroll_Item]  WITH CHECK ADD  CONSTRAINT [FK_Payroll_Item_Payroll_Item_SubType] FOREIGN KEY([SubTypeId])
REFERENCES [Payroll_Item_SubType] ([Id])

ALTER TABLE [Payroll_Item] CHECK CONSTRAINT [FK_Payroll_Item_Payroll_Item_SubType]



ALTER TABLE [Payroll_Item]  WITH CHECK ADD  CONSTRAINT [FK_Payroll_Item_Payroll_Item_Type] FOREIGN KEY([TypeID])
REFERENCES [Payroll_Item_Type] ([Id])

ALTER TABLE [Payroll_Item] CHECK CONSTRAINT [FK_Payroll_Item_Payroll_Item_Type]
'


-- Step: 13.00 
-- TFS: 65
-- Inserting the pre-defined rows for payroll item types and subtypes
Exec NewDBStep 13.00, '
-- Payroll item types
Insert Into Payroll_Item_Type Values (''Earning'',''EARNING'',0)
Insert Into Payroll_Item_Type Values (''Deduction'',''DEDUCTION'',0)
Insert Into Payroll_Item_Type Values (''Benefit'',''BENEFIT'',0)
Insert Into Payroll_Item_Type Values (''Tax'',''TAX'',0)
Insert Into Payroll_Item_Type Values (''CPP/QPP'',''CPP_QPP'',1)
Insert Into Payroll_Item_Type Values (''EI'',''EI'',1)
Insert Into Payroll_Item_Type Values (''Parental Insurance'',''PIP'',1)
Insert Into Payroll_Item_Type Values (''Epmloyer Tax'',''ER_TAX'',0)
Insert Into Payroll_Item_Type Values (''Internal'',''INTERNAL'',1)

-- Payroll item sub-types
Insert into Payroll_Item_subType Values (''Fix Amount'', ''Dollar'', ''FIX_AMOUNT'',0)
Insert into Payroll_Item_subType Values (''Hourly'', ''Hours'', ''HOURLY'',0)
Insert into Payroll_Item_subType Values (''Percentage'', ''Dollar'', ''PERCENT'',0)
Insert into Payroll_Item_subType Values (''Bonus'', ''Dollar'', ''BONUS'',0)
Insert into Payroll_Item_subType Values (''Vacation Pay'', ''Dollar'', ''VACPAY'',1)
Insert into Payroll_Item_subType Values (''Federal Income Tax'', ''Dollar'', ''FED_TAX'',1)
Insert into Payroll_Item_subType Values (''Provincial Income Tax'', ''Dollar'', ''PROV_TAX'',1)
Insert into Payroll_Item_subType Values (''CPP'', ''Dollar'', ''CPP'',1)
Insert into Payroll_Item_subType Values (''QPP'', ''Dollar'', ''QPP'',1)
Insert into Payroll_Item_subType Values (''QPIP'', ''Dollar'', ''QPIP'',1)
Insert into Payroll_Item_subType Values (''EI'', ''Dollar'', ''FED_EI'',1)
Insert into Payroll_Item_subType Values (''EI (QC)'', ''Dollar'', ''QC_EI'',1)
Insert into Payroll_Item_subType Values (''Total Payments'', ''Dollar'', ''GROSS_PAY'',1)
Insert into Payroll_Item_subType Values (''Total Deductions'', ''Dollar'', ''TOTAL_DED'',1)
Insert into Payroll_Item_subType Values (''Net Pay'', ''Dollar'', ''NET_PAY'',1)

'
-- Step: 14.00 
-- TFS: 65
-- Inserting the pre-defined rows for payroll items 
Exec NewDBStep 14.00, '
declare @typeId int
declare @subtypeId int

Select @typeId = Id from Payroll_Item_Type where Code = ''EARNING''

select @subtypeId = Id from Payroll_Item_Subtype where Code = ''FIX_AMOUNT''
Insert Into [Payroll_Item] Values( ''101'', ''Salary'',	@TypeID, @SubTypeId, Null, 1, 0, 1, 1, 1)
Insert Into [Payroll_Item] Values( ''110'', ''Adjustment'',	@TypeID, @SubTypeId, Null, 1, 0, 1, 1, 1)

select @subtypeId = Id from Payroll_Item_Subtype where Code = ''HOURLY''
Insert Into [Payroll_Item] Values( ''102'', ''Hourle Wages'', @TypeID, @SubTypeId, Null, 1, 0, 1, 1, 1)
Insert Into [Payroll_Item] Values( ''103'', ''Overtime'', @TypeID, @SubTypeId, Null, 1, 0, 1, 1, 1)

select @subtypeId = Id from Payroll_Item_Subtype where Code = ''VACPAY''
Insert Into [Payroll_Item] Values( ''104'', ''Vacation Pay'', @TypeID, @SubTypeId, Null, 1, 1, 1, 1, 1)

select @subtypeId = Id from Payroll_Item_Subtype where Code = ''BONUS''
Insert Into [Payroll_Item] Values( ''105'', ''Bonus'', @TypeID, @SubTypeId, Null, 1, 0, 1, 1, 1)

Select @typeId = Id from Payroll_Item_Type where Code = ''TAX''

select @subtypeId = Id from Payroll_Item_Subtype where Code = ''FED_TAX''
Insert Into [Payroll_Item] Values( ''301'', ''Federal Income Tax'',	@TypeID, @SubTypeId,''FED'', 1, 1, 0, 0, 0)

select @subtypeId = Id from Payroll_Item_Subtype where Code = ''PROV_TAX''
Insert Into [Payroll_Item] Values( ''302'', ''Provincial Income Tax'', @TypeID, @SubTypeId, Null, 1, 1, 0, 0, 0)

select @subtypeId = Id from Payroll_Item_Subtype where Code = ''FIX_AMOUNT''
Insert Into [Payroll_Item] Values( ''303'', ''Extra Income Tax'', @TypeID, @SubTypeId, Null, 1, 1, 0, 0, 0)


Select @typeId = Id from Payroll_Item_Type where Code = ''CPP_QPP''
select @subtypeId = Id from Payroll_Item_Subtype where Code = ''CPP''
Insert Into [Payroll_Item] Values( ''304'', ''CPP'', @TypeID, @SubTypeId,''FED'', 1, 1, 0, 0, 0)
select @subtypeId = Id from Payroll_Item_Subtype where Code = ''QPP''
Insert Into [Payroll_Item] Values( ''305'', ''QPP'', @TypeID, @SubTypeId,''QC'', 1, 1, 0, 0, 0)


Select @typeId = Id from Payroll_Item_Type where Code = ''EI''
select @subtypeId = Id from Payroll_Item_Subtype where Code = ''FED_EI''
Insert Into [Payroll_Item] Values( ''306'', ''EI'',	@TypeID, @SubTypeId,''FED'', 1, 1, 0, 0, 0)

select @subtypeId = Id from Payroll_Item_Subtype where Code = ''QC_EI''
Insert Into [Payroll_Item] Values( ''307'', ''EI(QC)'',	@TypeID, @SubTypeId, ''QC'', 1, 1, 0, 0, 0)


Select @typeId = Id from Payroll_Item_Type where Code = ''PIP''
select @subtypeId = Id from Payroll_Item_Subtype where Code = ''QPIP''
Insert Into [Payroll_Item] Values( ''308'', ''QPIP'', @TypeID, @SubTypeId, ''QC'', 1, 1, 0, 0, 0)

Select @typeId = Id from Payroll_Item_Type where Code = ''DEDUCTION''

select @subtypeId = Id from Payroll_Item_Subtype where Code = ''FIX_AMOUNT''
Insert Into [Payroll_Item] Values( ''401'', ''Misc. Deduction'', @TypeID, @SubTypeId, Null, 1, 1, 0, 0, 0)
Insert Into [Payroll_Item] Values( ''402'', ''Deposit Fee'', @TypeID, @SubTypeId, Null, 1, 1, 0, 0, 0)


Select @typeId = Id from Payroll_Item_Type where Code = ''ER_TAX''

select @subtypeId = Id from Payroll_Item_Subtype where Code = ''CPP''
Insert Into [Payroll_Item] Values( ''502'', ''CPP (Employer Contribution)'', @TypeID, @SubTypeId, ''FED'', 1, 1, 0, 0, 0)

select @subtypeId = Id from Payroll_Item_Subtype where Code = ''QPP''
Insert Into [Payroll_Item] Values( ''503'', ''QPP (Employer Contribution)'', @TypeID, @SubTypeId, ''QC'',  1, 1, 0, 0, 0)

select @subtypeId = Id from Payroll_Item_Subtype where Code = ''FED_EI''
Insert Into [Payroll_Item] Values( ''501'', ''EI (Employer Contribution)'',	@TypeID, @SubTypeId, ''FED'', 1, 1, 0, 0, 0)

Select @typeId = Id from Payroll_Item_Type where Code = ''INTERNAL''

select @subtypeId = Id from Payroll_Item_Subtype where Code = ''GROSS_PAY''
Insert Into [Payroll_Item] Values( ''900'', ''Total Payments'',	@TypeID, @SubTypeId, Null, 1, 1, 0, 0, 0)
select @subtypeId = Id from Payroll_Item_Subtype where Code = ''TOTAL_DED''
Insert Into [Payroll_Item] Values( ''901'', ''Total Deductions'', @TypeID, @SubTypeId, Null, 1, 1, 0, 0, 0)
select @subtypeId = Id from Payroll_Item_Subtype where Code = ''NET_PAY''
Insert Into [Payroll_Item] Values( ''902'', ''Net Pay'', @TypeID, @SubTypeId, Null, 1, 1, 0, 0, 0)
'

-- Step: 15.00 
-- TFS: 10
-- For each candidate in candidate Table, we create 14 rows which stand for Federal and 13 provinces in Candidate_TD1.
Exec NewDBStep 15.00, '
Declare _cursor CURSOR FOR
Select distinct Id
from Candidate
order by Id;

Declare @_id int;

Open _cursor

Fetch Next from _cursor
Into @_id

While @@FETCH_STATUS = 0
Begin 
	Insert Into Candidate_TD1 (CandidateId,Year,Province_Code,Basic_Amount)
	Values (@_id,2015,''CA'',11327),
		    (@_id,2015,''ON'',9863),
			(@_id,2015,''QC'',11425),
			(@_id,2015,''AB'',18214),
			(@_id,2015,''BC'',9938),
			(@_id,2015,''MB'',9134),
			(@_id,2015,''NB'',9633),
			(@_id,2015,''NL'',8767),
			(@_id,2015,''NS'',8481),
			(@_id,2015,''NT'',13900),
			(@_id,2015,''NU'',12781),
			(@_id,2015,''PE'',7708),
			(@_id,2015,''SK'',15639),
			(@_id,2015,''YT'',11327)	
	fetch next from _cursor
	into @_id
End
Close _cursor;
Deallocate _cursor;
'
-- Step: 16.00 
-- Add additional columns to JobOrders
Exec NewDBStep 16.00, '
alter table dbo.joborder
add AllowSuperVisorModifyWorkTime bit not null default(0)

alter table dbo.joborder
add AllowDailyApproval bit not null default(0)
'

-- Step: 17.00 
-- Change Abbreviation for Yukon from YU to YT
Exec NewDBStep 17.00, '
UPDATE [StateProvince]
   SET [Abbreviation] = ''YT''
 WHERE [StateProvinceName]=''Yukon Territory''
'

-- Step: 18.00 
-- Adding new items to resource strings
Exec NewDBStep 18.00, '
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.SupervisorUpdateWorkTime'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.SupervisorUpdateWorkTime'', N''Supervisor Update'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.TimeSheet.SupervisorUpdateApproval'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.TimeSheet.SupervisorUpdateApproval'', N''Supervisor Update/Approve Worktime'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.JobOrder.JobOrder.Fields.AllowSuperVisorModifyWorkTime'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.JobOrder.JobOrder.Fields.AllowSuperVisorModifyWorkTime'', N''Allow Supervisor to Modify Worktime'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.JobOrder.JobOrder.Fields.AllowDailyApproval'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.JobOrder.JobOrder.Fields.AllowDailyApproval'', N''Allow Daily Approval'', GETUTCDATE(), GETUTCDATE()) 
'

-- Step: 19.00 
-- Create supporting tables: PayPeriodBillingChartType, PayPeriodBillingChartStatus 
Exec NewDBStep 19.00, '
CREATE TABLE [PayPeriodBillingChartType](
	[Id] [int] NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PayPeriodBillingChartType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [PayPeriodBillingChartStatus](
	[Id] [int] NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PayPeriodBillingChartStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]		


Insert Into PayPeriodBillingChartType Values (27, ''CONTRACTOR'', ''Contractor'')
Insert Into PayPeriodBillingChartType Values (40, ''TEMP'', ''Temp'')

Insert Into PayPeriodBillingChartStatus Values (31, ''SUBMITTED'', ''Submitted'')
Insert Into PayPeriodBillingChartStatus Values (33, ''PENDINGAPPROVAL'', ''PendingApproval'')
Insert Into PayPeriodBillingChartStatus Values (35, ''PAID'', ''Paid'')
Insert Into PayPeriodBillingChartStatus Values (37, ''APPROVED'', ''Approved'')
Insert Into PayPeriodBillingChartStatus Values (39, ''REJECTED'', ''Rejected'')
'
-- Step: 20.00 
-- Create a Table called FranchiseBankAccount
Exec NewDBStep 20.00, '
CREATE TABLE [FranchiseBankAccount](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FranchiseId] [nvarchar](max) NULL,
	[BankAccount] [nvarchar](255) NOT NULL,
	[FileCreationNumber] [nvarchar](4) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

Insert Into FranchiseBankAccount (FranchiseId,BankAccount,FileCreationNumber) Values (''GCFR13-0001'',''3402820000'',''0001'')

'
-- Step: 21.00 
Exec NewDBStep 21.00, '
CREATE TABLE  [CandidateWorkTimeDepartmentBreakdown](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CandidateWorkTimeId] [int] NOT NULL,
	[CompanyDepartmentId] [int] NOT NULL,
	[AllocationTimeInHours] [decimal](18, 2) NOT NULL,
	[CreatedOnUtc] [datetime] null,
	[UpdatedOnUtc] [datetime] null,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
'
-- Step: 22.00 
Exec NewDBStep 22.00, '
ALTER TABLE  [CandidateWorkTimeDepartmentBreakdown]  WITH CHECK ADD  CONSTRAINT [CandidateWorkTimeDepartmentBreakdown_CandidateWorkTime] FOREIGN KEY([CandidateWorkTimeId])
REFERENCES  [CandidateWorkTime] ([Id])
ON DELETE NO ACTION

ALTER TABLE  [CandidateWorkTimeDepartmentBreakdown] CHECK CONSTRAINT [CandidateWorkTimeDepartmentBreakdown_CandidateWorkTime]

ALTER TABLE  [CandidateWorkTimeDepartmentBreakdown]  WITH CHECK ADD  CONSTRAINT [CandidateWorkTimeDepartmentBreakdown_CompanyDepartment] FOREIGN KEY([CompanyDepartmentId])
REFERENCES  [CompanyDepartment] ([Id])
ON DELETE NO ACTION

ALTER TABLE  [CandidateWorkTimeDepartmentBreakdown] CHECK CONSTRAINT [CandidateWorkTimeDepartmentBreakdown_CompanyDepartment]
'
-- Step: 23.00 
-- Adding new items to resource strings
Exec NewDBStep 23.00, '
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Common.DepartmentBreakdown'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Common.DepartmentBreakdown'', N''Department Breakdown'', GETUTCDATE(), GETUTCDATE())
'
-- Step: 24.00 
-- Creating a new table called GLAccount 
Exec NewDBStep 24.00,'
CREATE TABLE [GLAccount](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Payroll_ItemId] [int] NOT NULL,
	[DebitAccount] [nvarchar](80) NULL,
	[CreditAccount] [nvarchar](80) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [GLAccount]  WITH CHECK ADD  CONSTRAINT [GLAccount_PayrollItem] FOREIGN KEY([Payroll_ItemId])
REFERENCES [Payroll_Item] ([ID])
ON DELETE CASCADE

ALTER TABLE [GLAccount] CHECK CONSTRAINT [GLAccount_PayrollItem]
'
-- Step: 24.50
Exec NewDBStep 24.50,'
IF OBJECT_ID (N''Candidate_Other_Payments'', N''U'') IS NULL
Begin
	CREATE TABLE [Candidate_Other_Payments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NULL,
	[CandidateId] [int] NOT NULL,
	[JobOrderId] [int] NULL,
	[Year] [int] NOT NULL,
	[WeekOfYear] [int] NOT NULL,
	[RegularHours] [decimal](18, 2) NOT NULL,
	[RegularPayRate] [decimal](18, 2) NOT NULL,
	[RegularBillingRate] [decimal](18, 2) NOT NULL,
	[OvertimeHours] [decimal](18, 2) NOT NULL,
	[OvertimePayRate] [decimal](18, 2) NOT NULL,
	[OvertimeBillingRate] [decimal](18, 2) NOT NULL,
	[GrossPay] [decimal](18, 2) NOT NULL,
	[CreatedOnUtc] [datetime] NULL,
	[PayPeriodBillingChartTypeId] [int] NULL,
	[StatusId] [int] NULL,
	[StatHolidayDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
End
'

-- Step: 25.00 
-- TFS: 71
-- Create new tables, to be used by Payroll Process module
Exec NewDBStep 25.00, '
CREATE TABLE [Payroll_Batch](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Status] [char](1) NOT NULL,
	[LastUpdateDate] [datetime] NOT NULL,
	[Payroll_CalendarId] [int] NOT NULL,
	CONSTRAINT [PK_Payroll_Batch] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [Payroll_Batch]  WITH CHECK ADD  CONSTRAINT [FK_Payroll_Batch_Payroll_Calendar] FOREIGN KEY([Payroll_CalendarId])
REFERENCES [Payroll_Calendar] ([ID])

ALTER TABLE [Payroll_Batch] CHECK CONSTRAINT [FK_Payroll_Batch_Payroll_Calendar]



CREATE TABLE [Candidate_Payment_History_Detail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Payment_HistoryId] [int] NOT NULL,
	[Payroll_ItemId] [int] NOT NULL,
	[Unit] [decimal](18, 4) NULL,
	[Rate] [decimal](18, 4) NULL,
	[Amount] [decimal](18, 4) NULL,
	[YTD_Unit] [decimal](18, 4) NOT NULL CONSTRAINT [DF_Candidate_Payment_History_Detail_YTD_Unit]  DEFAULT ((0)),
	[YTD_Amount] [decimal](18, 4) NOT NULL CONSTRAINT [DF_Candidate_Payment_History_Detail_YTD_Amount]  DEFAULT ((0)),
 CONSTRAINT [PK_Candidate_Payment_History_Detail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [Candidate_Payment_History_Detail]  WITH CHECK ADD  CONSTRAINT [FK_Candidate_Payment_History_Detail_Candidate_Payment_History] FOREIGN KEY([Payment_HistoryId])
REFERENCES [Candidate_Payment_History] ([Id])
ON DELETE CASCADE

ALTER TABLE [Candidate_Payment_History_Detail] CHECK CONSTRAINT [FK_Candidate_Payment_History_Detail_Candidate_Payment_History]

ALTER TABLE [Candidate_Payment_History_Detail]  WITH CHECK ADD  CONSTRAINT [FK_Candidate_Payment_History_Detail_Payroll_Item] FOREIGN KEY([Payroll_ItemId])
REFERENCES [Payroll_Item] ([ID])

ALTER TABLE [Candidate_Payment_History_Detail] CHECK CONSTRAINT [FK_Candidate_Payment_History_Detail_Payroll_Item]


-- Add a new column for Batch Id to these tables
Alter TABLE [Candidate_Payment_History] Add [PayrollBatchId] [int] NULL
Alter TABLE [PayPeriodBillingChart] Add [PayrollBatchId] [int] NULL
Alter TABLE [Candidate_Other_Payments] Add [PayrollBatchId] [int] NULL 
' 

-- Step: 26.00  
-- TFS: 71
-- Adding a new calculated column to TD1, to be used by payroll processing, New columns to Payroll batch and Candidate_Payment_History
Exec NewDBStep 26.00, '
Alter table Payroll_Batch Add [Year] int NOT NULL

ALTER TABLE Candidate_TD1 ADD TotalCredit AS (Basic_Amount +  IsNull(Child_Amount, 0) + IsNull(Age_Amount, 0) +
                                              IsNull(Pension_Income_Amount, 0) + IsNull(Tuition_Amounts, 0) + IsNull(Disablility_Amount, 0) + 
											  IsNull(Spouse_Amount, 0) + IsNull(Eligible_Dependant_Amount, 0) + IsNull(Caregiver_Amount, 0) + 
											  IsNull(Infirm_Dependant_Amount, 0) + IsNull(Amount_Transferred_From_Spouse, 0) + IsNull(Amount_Transferred_From_Dependant, 0) +
											  IsNull(Family_Tax_Benefit, 0) + IsNull(Senior_Supplementary_Amount, 0)  ) 

Alter TABLE [Candidate_Payment_History] Add [ProvinceCode] [nvarchar](2) NULL
Alter TABLE [Candidate_Payment_History] Add [TotalDeductions] [decimal](10, 2) NULL
'

-- Step: 26.50 
-- TFS: 71
-- Adding a new tables for payroll processing
Exec NewDBStep 26.50, '
CREATE TABLE [Payroll_InProgress](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CandidateId] [int] NULL,
	[EmployeeType] [int] NULL,
	[PayrollType] [nvarchar](12) NULL,
	[Payroll_BatchId] [int] NULL,
	[ProvinceCode] [nvarchar](2) NULL,
	[Cheque_Number] [nvarchar](20) NULL,
	[Direct_Deposit_Number] [nvarchar](20) NULL,
	[IsDirectDeposit] [bit] NULL,
 CONSTRAINT [PK_Payroll_InProgress] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [Payroll_InProgress]  WITH CHECK ADD  CONSTRAINT [FK_Payroll_InProgress_Payroll_Batch] FOREIGN KEY([Payroll_BatchId])
REFERENCES [Payroll_Batch] ([Id])

ALTER TABLE [Payroll_InProgress] CHECK CONSTRAINT [FK_Payroll_InProgress_Payroll_Batch]



CREATE TABLE  [Payroll_InProgress_Detail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Payroll_InProgressId] [int] NOT NULL,
	[Payroll_ItemId] [int] NOT NULL,
	[Unit] [decimal](18, 4) NULL,
	[Rate] [decimal](18, 4) NULL,
	[Amount] [decimal](18, 4) NULL,
	[YTD_Unit] [decimal](18, 4) NOT NULL CONSTRAINT [DF_Payroll_InProgress_Detail_YTD_Unit]  DEFAULT ((0)),
	[YTD_Amount] [decimal](18, 4) NOT NULL CONSTRAINT [DF_Payroll_InProgress_Detail__YTD_Amount]  DEFAULT ((0)),
 CONSTRAINT [PK_Payroll_InProgress_Detail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE  [Payroll_InProgress_Detail]  WITH CHECK ADD  CONSTRAINT [FK_Payroll_InProgress_Detail_Payroll_InProgress] FOREIGN KEY([Payroll_InProgressId])
REFERENCES  [Payroll_InProgress] ([Id])
ON DELETE CASCADE

ALTER TABLE  [Payroll_InProgress_Detail] CHECK CONSTRAINT [FK_Payroll_InProgress_Detail_Payroll_InProgress]

ALTER TABLE  [Payroll_InProgress_Detail]  WITH CHECK ADD  CONSTRAINT [FK_Payroll_InProgress_Detail_Payroll_Item] FOREIGN KEY([Payroll_ItemId])
REFERENCES  [Payroll_Item] ([ID])

ALTER TABLE  [Payroll_InProgress_Detail] CHECK CONSTRAINT [FK_Payroll_InProgress_Detail_Payroll_Item]
'
-- Step: 27.00 
-- Drop the table GLAccount and add two columns to Payroll_Item table
Exec NewDBStep 27.00,'
DROP TABLE [GLAccount]

ALTER TABLE Payroll_Item
ADD CreditAccount nvarchar(18),
	DebitAccount nvarchar(18)
'

-- Step: 28.00 
-- Drop two columns (BankId and BankAccountTypeId) from CandidateBankAccount table and alter the data type for institution number, 
-- transit number and account number
Exec NewDBStep 28.00, '
ALTER Table CandidateBankAccount
DROP COLUMN BankId,BankAccountTypeId

ALTER TABLE CandidateBankAccount
ALTER COLUMN InstitutionNumber varchar(4)

ALTER TABLE CandidateBankAccount
ALTER COLUMN TransitNumber varchar(5)

ALTER TABLE CandidateBankAccount
ALTER COLUMN AccountNumber varchar(17)
'
-- Step: 29.00 
-- TFS 82: Drop one duplicate FK Franchise_Id
Exec NewDBStep 29.00, '
ALTER TABLE [FranchiseAddress] 
DROP CONSTRAINT [Franchise_FranchiseAddresses]

ALTER TABLE [FranchiseAddress]
DROP COLUMN FRANCHISE_Id
 '
-- Step: 30.00 
Exec NewDBStep 30.00, '
ALTER TABLE [CompanyLocation] ADD [LastPunchClockFileUploadDateTimeUtc] DateTime NULL
ALTER TABLE [CompanyLocation] ADD [LastWorkTimeCalculationDateTimeUtc] DateTime NULL
'
-- Step: 31.00 
-- Improve and adjust the Payroll items and their predefined types
Exec NewDBStep 31.00,'
Update Payroll_item  Set TypeId = (Select Id from Payroll_Item_Type Where code = ''TAX'') Where Code like ''3%''
Update Payroll_Item_Type Set Name = ''Source Deduction'' Where Code = ''TAX''
Delete Payroll_Item_Type Where  Code In (''CPP_QPP'', ''EI'', ''PIP'')

Delete from Payroll_Item Where Description = ''EI(QC)''
Delete from Payroll_Item_SubType Where Code = ''QC_EI''
Update Payroll_Item_SubType Set Code = ''EI'' Where Code = ''FED_EI''
'
-- Step: 32.00 
-- After migration from Clarity to GC, we need to update Candidate_TD1 and change the colunm Province_Code = 'FA' to Province_Code = 'CA'
Exec NewDBStep 32.00,'
UPDATE [Candidate_TD1]
   SET [Province_Code] = ''CA''
 WHERE [Province_Code]=''FA''
'
-- Step: 33.00 
-- Adding new items to resource strings for Admin.Attendance
Exec NewDBStep 33.00, '
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance'', N''Attendance'', GETUTCDATE(), GETUTCDATE())
'

-- Step: 34.00 
-- TFS 90
-- Create new tables for WCB types and rates
Exec NewDBStep 34.00,'
CREATE TABLE [WCB_Type](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_WCB_Types] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_WCB_Types_Code] UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



CREATE TABLE  [WCB_Rate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WCB_Type_Id] [int] NOT NULL,
	[Province_Id] [int] NOT NULL,
	[Rate] [decimal](10, 2) NOT NULL,
	[Maximum] [decimal](18, 2) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
 CONSTRAINT [PK_WCB_Rate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE  [WCB_Rate]  WITH CHECK ADD  CONSTRAINT [FK_WCB_Rate_StateProvince] FOREIGN KEY([Province_Id])
REFERENCES  [StateProvince] ([Id])

ALTER TABLE  [WCB_Rate] CHECK CONSTRAINT [FK_WCB_Rate_StateProvince]

ALTER TABLE  [WCB_Rate]  WITH CHECK ADD  CONSTRAINT [FK_WCB_Rate_WCB_Type] FOREIGN KEY([WCB_Type_Id])
REFERENCES  [WCB_Type] ([Id])

ALTER TABLE  [WCB_Rate] CHECK CONSTRAINT [FK_WCB_Rate_WCB_Type]
'

-- Step: 35.00 
-- TFS 98
-- Changing the schema for Paycards
Exec NewDBStep 35.00,'
Alter Table Payroll_InProgress Drop Column PayrollType
Alter Table Payroll_InProgress Add IsPrinted bit CONSTRAINT [DF_Payroll_InProgress_IsPrinted] DEFAULT ((0))
Alter Table Payroll_InProgress Add IsEmailed bit CONSTRAINT [DF_Payroll_InProgress_IsEmailed] DEFAULT ((0))

ALter Table Payroll_InProgress_Detail Add JobOrder_Id int
'
-- Step: 36.00 
Exec NewDBStep 36.00, '
CREATE TABLE [dbo].[JobOrderRenewSchedule](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobOrderId] [int] NOT NULL,
	[RenewStartDate] [datetime] NOT NULL,
	[RenewEndDate] [datetime] NULL,
	[Note] [nvarchar](255) NULL,
	[CreatedOnUtc] [datetime] NULL,
	[UpdatedOnUtc] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

ALTER TABLE [dbo].[JobOrderRenewSchedule]  WITH CHECK ADD  CONSTRAINT [JobOrderRenewSchedule_JobOrder] FOREIGN KEY([JobOrderId])
REFERENCES [dbo].[JobOrder] ([Id])
ON DELETE CASCADE'
-- Step: 37.00 
Exec NewDBStep 37.00, '
CREATE TABLE [dbo].[JobOrderRenewLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobOrderRenewScheduleId] [int] NOT NULL,
	[JobOrderId] [int] NOT NULL,
	[CreatedOnUtc] [datetime] NULL,
	[UpdatedOnUtc] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

ALTER TABLE [dbo].[JobOrderRenewLog]  WITH CHECK ADD  CONSTRAINT [JobOrderRenewLog_JobOrderRenewSchedule] FOREIGN KEY([JobOrderRenewScheduleId])
REFERENCES [dbo].[JobOrderRenewSchedule] ([Id])
ON DELETE CASCADE

ALTER TABLE [dbo].[JobOrderRenewLog]  WITH CHECK ADD  CONSTRAINT [JobOrderRenewLog_JobOrder] FOREIGN KEY([JobOrderId])
REFERENCES [dbo].[JobOrder] ([Id])
ON DELETE NO ACTION'
-- Step: 38.00 
-- Add a new column to Table Invoice
Exec NewDBStep 38.00, '
ALTER TABLE Invoice
ADD InvoiceFile varbinary(max)
'
-- Step: 39.00 
-- TFS 98
-- Enhancements in Payroll items 
Exec NewDBStep 39.00,'
Alter Table Payroll_Item_Type Add Description nvarchar(30) ;

Update Payroll_Item Set Description = ''Hourly Wages'' where Description = ''Hourle Wages''

Insert into Payroll_Item (Code, Description, TypeId, SubTypeId, PrintOnPayStub, IsReadOnly, IsTaxable, IsPensionable, IsInsurable) 
                  Values (''111'', ''Previouse Missing Hours'', 1, 2, 1, 0, 1, 1, 1)

Insert into  Payroll_Item_SubType (Code, Name, UnitName, IsInternal) Values (''QHSF'', ''QHSF'', ''Dollar'', 1)
declare @newId int = @@IDENTITY 
Insert into Payroll_Item (Code, Description, TypeId, SubTypeId, PrintOnPayStub, IsReadOnly, IsTaxable, IsPensionable, IsInsurable, State_Code) 
                  Values (''503'', ''QHSF'', 8, @newId, 0, 0, 0, 0, 0, ''QC'')

Insert into  Payroll_Item_SubType (Code, Name, UnitName, IsInternal) Values (''CNT'', ''CNT'', ''Dollar'', 1)
Set @newId = @@IDENTITY 
Insert into Payroll_Item (Code, Description, TypeId, SubTypeId, PrintOnPayStub, IsReadOnly, IsTaxable, IsPensionable, IsInsurable, State_Code) 
                  Values (''504'', ''CNT'', 8, @newId, 0, 0, 0, 0, 0, ''QC'')
'

-- Step: 40.00 
-- TFS 98
-- Enhancements in Payroll items 
Exec NewDBStep 40.00,'
Update Payroll_Item_Type set Description = ''Earnings'' where Code = ''EARNING''
Update Payroll_Item_Type set Description = ''Deductions'' where Code = ''DEDUCTION''
Update Payroll_Item_Type set Description = ''Benefits'' where Code = ''BENEFIT''
Update Payroll_Item_Type set Description = ''Taxes'' where Code = ''TAX''
Update Payroll_Item_Type set Description = ''Taxes (Employer)'' where Code = ''ER_TAX''
Update Payroll_Item_Type set Description = ''Totals'' where Code = ''INTERNAL''
'
-- Step: 41.00 
-- Add string source for candidate attendance
Exec NewDBStep 41.00, '

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance'', N''Candidate Attendance'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.LastWeek'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.LastWeek'', N''Last Week'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.ThisWeek'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.ThisWeek'', N''This Week'', GETUTCDATE(), GETUTCDATE())


if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.JobOrderId'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.JobOrderId'', N''Job Order Id'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.JobTitle'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.JobTitle'', N''Title'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.JobShift'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.JobShift'', N''Shift'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.JobStartDate'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.JobStartDate'', N''Start Date'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.JobEndDate'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.JobEndDate'', N''End Date'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.CompanyId'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.CompanyId'', N''Company Id'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.CompanyName'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.CompanyName'', N''Company'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.CompanyLocationId'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.CompanyLocationId'', N''Location Id'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.LocationName'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.LocationName'', N''Location'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.CompanyDepartmentId'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.CompanyDepartmentId'', N''Department Id'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.DepartmentName'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.DepartmentName'', N''Department'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.CompanyContactId'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.CompanyContactId'', N''Contact Id'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.ContactName'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.ContactName'', N''Contact'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.Placed'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.Placed'', N''Placed'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.SundayPunched'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.SundayPunched'', N''Sun I/O'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.SundayValid'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.SundayValid'', N''Sun OK'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.SundayMissing'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.SundayMissing'', N''Sun NOK'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.MondayPunched'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.MondayPunched'', N''Mon I/O'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.MondayValid'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.MondayValid'', N''Mon OK'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.MondayMissing'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.MondayMissing'', N''Mon NOK'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.TuesdayPunched'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.TuesdayPunched'', N''Tue I/O'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.TuesdayValid'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.TuesdayValid'', N''Tue OK'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.TuesdayMissing'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.TuesdayMissing'', N''Tue NOK'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.WednesdayPunched'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.WednesdayPunched'', N''Wed I/O'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.WednesdayValid'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.WednesdayValid'', N''Wed OK'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.WednesdayMissing'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.WednesdayMissing'', N''Wed NOK'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.ThursdayPunched'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.ThursdayPunched'', N''Thu I/O'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.ThursdayValid'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.ThursdayValid'', N''Thu OK'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.ThursdayMissing'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.ThursdayMissing'', N''Thu NOK'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.FridayPunched'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.FridayPunched'', N''Fri I/O'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.FridayValid'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.FridayValid'', N''Fri OK'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.FridayMissing'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.FridayMissing'', N''Fri NOK'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.SaturdayPunched'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.SaturdayPunched'', N''Sat I/O'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.SaturdayValid'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.SaturdayValid'', N''Sat OK'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Attendance.CandidateAttendance.Fields.SaturdayMissing'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Attendance.CandidateAttendance.Fields.SaturdayMissing'', N''Sat NOK'', GETUTCDATE(), GETUTCDATE())

'

-- Step: 42.00 
Exec NewDBStep 42.00, '
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.JobOrder.JobOrder.RenewSchedule'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.JobOrder.JobOrder.RenewSchedule'', N''Renew Schedule'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.JobOrder.Renew.StartDate'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.JobOrder.AutoRenew.StartDate'', N''Start Date'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.JobOrder.Renew.EndDate'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.JobOrder.AutoRenew.EndDate'', N''End Date'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.JobOrder.Renew.Note'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.JobOrder.AutoRenew.Note'', N''Note'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.JobOrder.JobOrderRenewScheduleModel.Fields.RenewStartDate.Required'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.JobOrder.JobOrderRenewScheduleModel.Fields.RenewStartDate.Required'', N''Renew schedule start date is required'', GETUTCDATE(), GETUTCDATE())

'
Exec NewDBStep 42.01, '
INSERT INTO [dbo].[ScheduleTask]
           ([Name]
           ,[Seconds]
           ,[Type]
           ,[IsActive]
           ,[StopOnError]
           ,[LastStartUtc]
           ,[LastEndUtc]
           ,[LastSuccessUtc]
           ,[Note]
           ,[EnteredBy]
           ,[CreatedOnUtc]
           ,[UpdatedOnUtc])
     VALUES
           (''Job Order Auto Renewal''
           ,28800
           ,''Wfm.Services.JobOrders.JobOrderAutoRenewTask, Wfm.Services''
           ,1
           ,0
           ,null
           ,null
           ,null
           ,null
           ,1
           ,GETDATE()
           ,null)'

-- Step: 43.00 
-- Adding missing columns to Candidate_TD1 table
Exec NewDBStep 43.00,'
Alter TABLE [Candidate_TD1] Add [Amount_For_Workers_65_Or_Older] decimal (10,2), QC_Deductions decimal (10,2)

' 
-- Step: 44.00 
-- tfs 101: Add two more columns to FranchiseBankAccount table
Exec NewDBstep 44.00, '
Alter table [FranchiseBankAccount] Add [Client_Number] nvarchar(10), [Transmission_Header] nvarchar(50) 
'
-- Step: 45.00 
Exec NewDBStep 45.00, '
INSERT INTO [dbo].[MessageTemplate]
           ([EmailAccountId]
           ,[TagName]
           ,[CCEmailAddresses]
           ,[BccEmailAddresses]
           ,[Subject]
           ,[Body]
           ,[PossibleVariables]
           ,[AllowSubstitution]
           ,[Note]
           ,[IsActive]
           ,[IsDeleted]
           ,[EnteredBy]
           ,[FranchiseId]
           ,[DisplayOrder]
           ,[CreatedOnUtc]
           ,[UpdatedOnUtc])
     VALUES
           (0
           ,''JobOrder.AutoRenewal.Completed''
           ,null
           ,null
           ,''%Franchise.Name%. Job Order is renewed by system based on schedule''
           ,''<p><a href="%Franchise.URL%">%Franchise.Name%</a> <br /><br />System has automatically renewed below job order based on schedule. Please logon the web site to check.<br /><br />Job id : %JobOrder.Id%<br />Job title : %JobOrder.Title%</p><p>Start Date&nbsp;: %JobOrder.StartDate%<br />End Date&nbsp;: %JobOrder.EndDate%</p><p><br />%Franchise.Name%</p>''
           ,null
           ,0
           ,null
           ,1
           ,0
           ,0
           ,0
           ,0
           ,GETDATE()
           ,null)'

-- Step: 46.00 
Exec NewDBStep 46.00, '
 Insert into Payroll_Item_SubType ([Name], [UnitName] ,[Code] ,[IsInternal]) Values (''Regular Hours'', ''Hours'', ''REG_HOURS'', 1)
 Update Payroll_Item Set SubTypeId = @@IDENTITY Where Code = ''102''

 Insert into Payroll_Item_SubType ([Name], [UnitName] ,[Code] ,[IsInternal]) Values (''Overtime Hours'', ''Hours'', ''OT_HOURS'', 1)
 Update Payroll_Item Set SubTypeId = @@IDENTITY Where Code = ''103''
'
-- Step: 47.00 
-- tfs 120: drop unused tables in Database
Exec NewDBStep 47.00, '
IF OBJECT_ID (N''BC_TaxReduction'', N''U'') IS NOT NULL
   DROP TABLE BC_TaxReduction

IF OBJECT_ID (N''Candidate_Cheque'', N''U'') IS NOT NULL
   DROP TABLE Candidate_Cheque

IF OBJECT_ID (N''Candidate_DirectDeposit'', N''U'') IS NOT NULL
   DROP TABLE Candidate_DirectDeposit

IF OBJECT_ID (N''Candidate_RRSP'', N''U'') IS NOT NULL
   DROP TABLE Candidate_RRSP

IF OBJECT_ID (N''CPP_Rate'', N''U'') IS NOT NULL
   DROP TABLE CPP_Rate

IF OBJECT_ID (N''EI_Rate'', N''U'') IS NOT NULL
   DROP TABLE EI_Rate

IF OBJECT_ID (N''FederalTaxRates'', N''U'') IS NOT NULL
   DROP TABLE FederalTaxRates

IF OBJECT_ID (N''Ontario_SurTax'', N''U'') IS NOT NULL
   DROP TABLE Ontario_SurTax

IF OBJECT_ID (N''Ontario_TaxReduction'', N''U'') IS NOT NULL
   DROP TABLE Ontario_TaxReduction

IF OBJECT_ID (N''OntarioHealthPremium'', N''U'') IS NOT NULL
   DROP TABLE OntarioHealthPremium

IF OBJECT_ID (N''PE_SurTax'', N''U'') IS NOT NULL
   DROP TABLE PE_SurTax

IF OBJECT_ID (N''ProvincialTaxRates'', N''U'') IS NOT NULL
   DROP TABLE ProvincialTaxRates

IF OBJECT_ID (N''QPIP_Rate'', N''U'') IS NOT NULL
   DROP TABLE QPIP_Rate

IF OBJECT_ID (N''QuebecHealthPremium'', N''U'') IS NOT NULL
   DROP TABLE QuebecHealthPremium

IF OBJECT_ID (N''YT_SurTax'', N''U'') IS NOT NULL
   DROP TABLE YT_SurTax

IF OBJECT_ID (N''Yukon_SurTax'', N''U'') IS NOT NULL
   DROP TABLE Yukon_SurTax
'

-- Step: 48.00 
-- add QPIP (ER) to Table Payroll_Item
Exec NewDBStep 48.00, '
INSERT INTO [Payroll_Item]
           ([Code]
           ,[Description]
           ,[TypeID]
           ,[SubTypeId]
           ,[State_Code]
           ,[PrintOnPayStub]
           ,[IsReadOnly]
           ,[IsTaxable]
           ,[IsPensionable]
           ,[IsInsurable])
     VALUES
           (504
           ,''QPIP (Employer Contribution)''
           ,8
           ,10
           ,''QC''
           ,0
           ,1
           ,0
           ,0
           ,0)
'

-- Step: 49.00 
-- Change the decimal point to 2 for payroll data
Exec NewDBStep 49.00, '
Alter TABLE [Candidate_Payment_History_Detail] Alter column [Unit] [decimal](18, 2)
Alter TABLE [Candidate_Payment_History_Detail] Alter column [Rate] [decimal](18, 2)
Alter TABLE [Candidate_Payment_History_Detail] Alter column [Amount] [decimal](18, 2)
Alter TABLE [Candidate_Payment_History_Detail] Alter column [YTD_Unit] [decimal](18, 2)
Alter TABLE [Candidate_Payment_History_Detail] Alter column [YTD_Amount] [decimal](18, 2)

Alter TABLE [Payroll_InProgress_Detail] Alter column [Unit] [decimal](18, 2)
Alter TABLE [Payroll_InProgress_Detail] Alter column [Rate] [decimal](18, 2)
Alter TABLE [Payroll_InProgress_Detail] Alter column [Amount] [decimal](18, 2)
Alter TABLE [Payroll_InProgress_Detail] Alter column [YTD_Unit] [decimal](18, 2)
Alter TABLE [Payroll_InProgress_Detail] Alter column [YTD_Amount] [decimal](18, 2)
'
-- Step: 50.00 
-- Payroll_Item table shouldn't have duplicated codes. Also, employer contributions shouldn't get printed on pay stub
Exec NewDBStep 50.00, '
Update [Payroll_Item] Set Code = ''505'' Where Description = ''QPIP (Employer Contribution)''
Update [Payroll_Item] Set Code = ''506'' Where Description = ''QHSF''

Alter table Payroll_Item Alter column Code Varchar(4) Not Null
Alter table Payroll_Item  ADD CONSTRAINT UQ_Code UNIQUE (Code) 

Update Payroll_Item Set PrintOnPayStub = 0 Where Description in (''EI (Employer Contribution)'', ''CPP (Employer Contribution)'', ''QPP (Employer Contribution)'')
'
-- Step: 51.00 
-- TFS 91 - Create EmployeeType, to be added by the templates
Exec NewDBStep 51.00, '
CREATE TABLE [Employee_Type](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Employee_Type] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

Alter table Employee_Type  ADD CONSTRAINT UQ_Employee_Type_Code UNIQUE (Code) 

Insert into Employee_Type  ([Name], [Code]) Values (''Temporary'', ''TEMP'')
Insert into Employee_Type  ([Name], [Code]) Values (''Contractor'', ''CONTRACTOR'')
Insert into Employee_Type  ([Name], [Code]) Values (''Regular'', ''REG'')

Alter table [Candidate] Add [Employee_TypeId] int
Alter table [PayGroup] Add [RegularEmployees] bit
'
-- Step: 52.00 
-- TFS 91 - Create EmployeeType, to be added by the templates
Exec NewDBStep 52.00, '
CREATE TABLE [Payroll_Template](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Employee_TypeId] [int] NOT NULL,
	[Payroll_ItemId] [int] NOT NULL,
	[Unit] [decimal](10,2) NULL,
	[Amount] [decimal](10,2) NULL,
	[Maximum_Per_PayPeriod] [decimal](10,2) NULL,
 CONSTRAINT [PK_Payroll_Template] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

-- Now populate some defualt data
Declare @payItemId int
Select  @payItemId = Id from Payroll_Item Where Description = ''Deposit Fee''
INSERT INTO Payroll_Template ( Employee_TypeId, Payroll_ItemId, Unit, Amount, Maximum_Per_PayPeriod)
 SELECT empType.Id , @payItemId , 0 , 2.00 , 2.00 
 From Employee_Type empType Where empType.Code <> ''REG'' -- Regular employees do not pay deposit fee

Select  @payItemId = Id from Payroll_Item Where Description = ''Vacation Pay''
INSERT INTO Payroll_Template ( Employee_TypeId, Payroll_ItemId, Unit, Amount, Maximum_Per_PayPeriod)
 SELECT empType.Id , @payItemId , 0 , 0 , 0 
 From Employee_Type empType Where empType.Code <> ''CONTRACTOR'' -- Contractors do not get vacation pay
 '

-- Step: 53.00 
-- TFS 136 -  Employee - Payroll Information tab - Add 'Employee Type' drop down to the page
Exec NewDBStep 53.00, '
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = ''EmployeeTypeId'' AND object_id = OBJECT_ID(''Candidate_Hire_Data''))
BEGIN
   ALTER TABLE Candidate_Hire_Data
   ADD EmployeeTypeId [int] NULL
END
'

-- Step: 54.00 
-- TFS 150 - Adding a sort order column to payroll_item table
Exec NewDBStep 54.00, '
Alter table Payroll_Item_Type Add Sort_Order int
'

-- Step: 54.50 
-- TFS 150 - Assigning the value of sort order column in payroll_item table
Exec NewDBStep 54.50, '
Update Payroll_Item_Type Set Sort_Order = 1 where code = ''EARNING''
Update Payroll_Item_Type Set Sort_Order = 2 where code = ''BENEFIT''
Update Payroll_Item_Type Set Sort_Order = 3 where code = ''TAX''
Update Payroll_Item_Type Set Sort_Order = 4 where code = ''DEDUCTION''
Update Payroll_Item_Type Set Sort_Order = 5 where code = ''ER_TAX''
Update Payroll_Item_Type Set Sort_Order = 6 where code = ''INTERNAL''
'

-- Step: 55.00 
-- TFS 143: Configuration - Franchise tab - Add a new group box for payroll information
Exec NewDBStep 55.00, '
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = ''EIRate'' AND object_id = OBJECT_ID(''Franchise''))
BEGIN
  ALTER TABLE Franchise
  ADD EIRate [decimal](10, 2) NULL
END
'

-- Step: 56.00 
Exec NewDBStep 56.00, ' 
ALTER TABLE [Payroll_InProgress]
ADD PayCardFile varbinary(max)
'
-- Step: 57.00 
Exec NewDBStep 57.00, ' 
ALTER TABLE [Candidate_Payment_History]
ADD Paystub varbinary(max)
'

-- Step: 58.00 
-- TFS 162 - Add the companyId to the batch and copy it into the payment history during the commit. Also, clean up the Candidate_Payment_History table
Exec NewDBStep 58.00, ' 
ALTER TABLE [Payroll_Batch] ADD CompanyId int

Alter table Candidate_Payment_History Drop Column  [CPP]  ,[EI]  ,[Federal_Tax] ,[Provincial_Tax] ,[Bank_Charges] ,[NetPay]
      ,[YTD_CPP] ,[YTD_EI] ,[YTD_Federal_Tax] ,[YTD_Provincial_Tax] ,[YTD_NetPay] ,[YTD_Bank_Charges] ,[GrossPay] ,[YTD_GrossPay]
      ,[Week_Start] ,[Week_End] ,[YTD_Hours] ,[QPIP] ,[YTD_QPIP] ,[QHSF] ,[YTD_QHSF] ,[YTD_CNT] ,[CNT] ,[TotalDeductions]
	  ,[PayPeriodStartDate] ,[PayPeriodEndDate]
'

-- Step: 59.00 
-- TFS 166 - Adding default data to DB
Exec NewDBStep 59.00, ' 
  If not exists (select 1 from Payroll_Users where UserType = ''Admin'')
    Insert into Payroll_Users (UserType, UserName, [Password], Encrypted_UserName) 
	values (''Admin'', ''admin'', ''C8qiotBAbGg='', ''C8qiotBAbGg='')

  Update Franchise Set EIRate = 1.4 Where EIRate is null
'
-- Step: 60.00 
-- TFS 172 - Fix the DB schema for provincial sales tax rates
Exec NewDBStep 60.00, ' 
Drop table SalesTaxRates 

CREATE TABLE [SalesTaxRate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProvinceCode] [char](2) NULL,
	[Year] [int] NOT NULL,
	[GST] [decimal](10, 4) NOT NULL CONSTRAINT DF_SalesTaxRate_GST Default (0),
	[PST] [decimal](10, 4) NOT NULL CONSTRAINT DF_SalesTaxRate_PST Default (0),
	[HST] [decimal](10, 4) NOT NULL CONSTRAINT DF_SalesTaxRate_HST Default (0),
 CONSTRAINT [PK_SalesTaxRate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



 Alter table Invoice Add InvoiceNumber [int] null;
'
-- Step: 61.00 
-- TFS 172 - populate the provincial sales tax rates for 2015
Exec NewDBStep 61.00, ' 
Insert into SalesTaxRate (ProvinceCode, [Year], GST, PST, HST) Values (''BC'', 2015, 5, 7, 0)
Insert into SalesTaxRate (ProvinceCode, [Year], GST, PST, HST) Values (''AB'', 2015, 5, 0, 0)
Insert into SalesTaxRate (ProvinceCode, [Year], GST, PST, HST) Values (''SK'', 2015, 5, 5, 0)
Insert into SalesTaxRate (ProvinceCode, [Year], GST, PST, HST) Values (''MB'', 2015, 5, 8, 0)
Insert into SalesTaxRate (ProvinceCode, [Year], GST, PST, HST) Values (''ON'', 2015, 0, 0, 13)
Insert into SalesTaxRate (ProvinceCode, [Year], GST, PST, HST) Values (''QC'', 2015, 5, 9.975, 0)
Insert into SalesTaxRate (ProvinceCode, [Year], GST, PST, HST) Values (''NL'', 2015, 0, 0, 13)
Insert into SalesTaxRate (ProvinceCode, [Year], GST, PST, HST) Values (''NS'', 2015, 0, 0, 15)
Insert into SalesTaxRate (ProvinceCode, [Year], GST, PST, HST) Values (''NB'', 2015, 0, 0, 13)
Insert into SalesTaxRate (ProvinceCode, [Year], GST, PST, HST) Values (''PE'', 2015, 0, 0, 14)
Insert into SalesTaxRate (ProvinceCode, [Year], GST, PST, HST) Values (''NT'', 2015, 5, 0, 0)
Insert into SalesTaxRate (ProvinceCode, [Year], GST, PST, HST) Values (''NU'', 2015, 5, 0, 0)
Insert into SalesTaxRate (ProvinceCode, [Year], GST, PST, HST) Values (''YT'', 2015, 5, 0, 0) 
'
-- Step: 62.00 
-- TFS 172 - Fixing the Invoice table 
Exec NewDBStep 62.00, ' 
 Update Invoice set InvoiceNumber = id;
 Alter table Invoice Alter column InvoiceNumber [int] not null
'
-- Step: 63.00 
-- tfs 508 - update Table LocalStringResource and set open available sentence
-- Made by Harry
Exec NewDBStep 63.00,'
  Update LocaleStringResource
  Set ResourceValue=''Since {0} is {1}.''
  where ResourceName=''Admin.JobOrder.JobOrder.Fields.OpeningChanges''
'
-- Step: 64.00 
-- Step: 65.00 
-- Step: 66.00 
-- Step: 67.00 
-- Step: 68.00 
-- Step: 69.00 
-- Strp: 70.00
-- Strp: 71.00
-- Strp: 72.00
-- Strp: 73.00
-- Strp: 74.00
-- Strp: 75.00
-- Strp: 76.00
-- Strp: 77.00
-- Strp: 78.00
-- Strp: 79.00
-- Strp: 80.00

--step 100.00
Exec NewDBStep 100.00, ' 
 Alter table dbo.JobOrder Alter column EndDate [DateTime] null
 Alter table dbo.JobOrder Add JobOrderCloseReason [int] null
'
--step 101.00
Exec NewDBStep 101.00, ' 
 Alter table dbo.CandidateJobOrder Add StartDate [DateTime] not null default getdate()
 Alter table dbo.CandidateJobOrder Add EndDate [DateTime] null
'
--step 102.00
Exec NewDBStep 102.00, ' 
 update dbo.CandidateJobOrder set StartDate = (select StartDate from JobOrder where Id = dbo.CandidateJobOrder.JobOrderId)
'

--step 103.00
Exec NewDBStep 103.00, ' 
CREATE TABLE [dbo].[JobOrderOpening](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobOrderId] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[OpeningNumber] [int] NOT NULL,
	[CreatedOnUtc] [datetime] NULL,
	[UpdatedOnUtc] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

ALTER TABLE [dbo].[JobOrderOpening]  WITH CHECK ADD  CONSTRAINT [JobOrderOpening_JobOrder] FOREIGN KEY([JobOrderId])
REFERENCES [dbo].[JobOrder] ([Id])
ON DELETE CASCADE'

--step 104.00
Exec NewDBStep 104.00, ' 
insert into [dbo].[JobOrderOpening](
	[JobOrderId],
	[StartDate],
	[EndDate],
	[OpeningNumber],
	[CreatedOnUtc])
select Id, StartDate, EndDate, OpeningNumber, GETDATE() from dbo.JobOrder

DECLARE @sql NVARCHAR(MAX)
WHILE 1=1
BEGIN
    SELECT TOP 1 @sql = N''alter table [dbo].[JobOrder] drop constraint [''+dc.NAME+N'']''
    from sys.default_constraints dc
    JOIN sys.columns c
        ON c.default_object_id = dc.object_id
    WHERE 
        dc.parent_object_id = OBJECT_ID(''[dbo].[JobOrder]'')
    AND c.name = N''OpeningNumber''
    IF @@ROWCOUNT = 0 BREAK
    EXEC (@sql)
END
'
--step 105.00
Exec NewDBStep 105.00, ' 
/****** Object:  Index [_dta_index_JobOrder_8_946102411__K40D_1_2_3_4_5_6_7_8_9_10_11_12_13_14_15_16_17_18_19_20_21_22_23_24_25_26_27_28_29_30_31_32_]    Script Date: 2015-06-22 9:43:09 PM ******/
DROP INDEX [_dta_index_JobOrder_8_946102411__K40D_1_2_3_4_5_6_7_8_9_10_11_12_13_14_15_16_17_18_19_20_21_22_23_24_25_26_27_28_29_30_31_32_] ON [dbo].[JobOrder]

/****** Object:  Index [_dta_index_JobOrder_8_946102411__K40D_1_2_3_4_5_6_7_8_9_10_11_12_13_14_15_16_17_18_19_20_21_22_23_24_25_26_27_28_29_30_31_32_]    Script Date: 2015-06-22 9:43:09 PM ******/
CREATE NONCLUSTERED INDEX [_dta_index_JobOrder_8_946102411__K40D_1_2_3_4_5_6_7_8_9_10_11_12_13_14_15_16_17_18_19_20_21_22_23_24_25_26_27_28_29_30_31_32_] ON [dbo].[JobOrder]
(
	[UpdatedOnUtc] DESC
)
INCLUDE ( 	[Id],
	[JobOrderGuid],
	[CompanyId],
	[CompanyLocationId],
	[CompanyDepartmentId],
	[CompanyContactId],
	[CompanyJobNumber],
	[JobTitle],
	[JobDescription],
	[HiringDurationExpiredDate],
	[EstimatedFinishingDate],
	[EstimatedMargin],
	[StartDate],
	[EndDate],
	[StartTime],
	[EndTime],
	[SchedulePolicyId],
	[JobOrderTypeId],
	[Salary],
	[JobOrderStatusId],
	[JobOrderCategoryId],
	[CompanyBillingRateId],
	[ShiftId],
	[ShiftSchedule],
	[Supervisor],
	[HoursPerWeek],
	[Note],
	[RequireSafeEquipment],
	[RequireSafetyShoe],
	[IsInternalPosting],
	[IsPublished],
	[IsHot],
	[RecruiterId],
	[OwnerId],
	[EnteredBy],
	[FranchiseId],
	[IsDeleted],
	[CreatedOnUtc]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

alter table dbo.joborder drop column OpeningNumber
'
--step 106.00
Exec NewDBStep 106.00, ' 
CREATE TABLE [dbo].[CompanyCandidate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[CandidateId] [int] NOT NULL,
	[Position] [nvarchar](20) NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[ReasonForLeave] [nvarchar](40) NULL,
	[Note] [nvarchar](255) NULL,
	[CreatedOnUtc] [datetime] NULL,
	[UpdatedOnUtc] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

ALTER TABLE [dbo].[CompanyCandidate]  WITH CHECK ADD  CONSTRAINT [CompanyCandidate_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[CompanyCandidate]  WITH CHECK ADD  CONSTRAINT [CompanyCandidate_Candidate] FOREIGN KEY([CandidateId])
REFERENCES [dbo].[Candidate] ([Id])
ON DELETE CASCADE'
--step 107.00
Exec NewDBStep 107.00, ' 
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.Company.Candidate'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.Company.Candidate'', N''Candidate'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.Company.Candidate.Import'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.Company.Candidate.Import'', N''Import'', GETUTCDATE(), GETUTCDATE())
'
--step 108.00
Exec NewDBStep 108.00, ' 
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.CompanyCandidate.Fields.CompanyId'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.CompanyCandidate.Fields.CompanyId'', N''Company Id'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.CompanyCandidate.Fields.CompanyId.Required'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.CompanyCandidate.Fields.CompanyId'', N''Company Id is required.'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.CompanyCandidate.Fields.CandidateId'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.CompanyCandidate.Fields.CandidateId'', N''Candidate Id'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.CompanyCandidate.Fields.CandidateId.Required'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.CompanyCandidate.Fields.CandidateId'', N''Candidate Id is required.'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.CompanyCandidate.Fields.Position'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.CompanyCandidate.Fields.Position'', N''Position'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.CompanyCandidate.Fields.StartDate'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.CompanyCandidate.Fields.StartDate'', N''Start Date'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.CompanyCandidate.Fields.StartDate.Required'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.CompanyCandidate.Fields.StartDate'', N''Start Date is required.'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.CompanyCandidate.Fields.EndDate'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.CompanyCandidate.Fields.EndDate'', N''End Date'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.CompanyCandidate.Fields.ReasonForLeave'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.CompanyCandidate.Fields.ReasonForLeave'', N''Reason for Leave'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.CompanyCandidate.Fields.Note'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.CompanyCandidate.Fields.Note'', N''Note'', GETUTCDATE(), GETUTCDATE())
'

--step 108.01
Exec NewDBStep 108.01, ' 
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.CompanyCandidate.Fields.FirstName'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.CompanyCandidate.Fields.FirstName'', N''First Name'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.CompanyCandidate.Fields.LastName'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.CompanyCandidate.Fields.LastName'', N''Last name'', GETUTCDATE(), GETUTCDATE())
'

--step 108.02
Exec NewDBStep 108.02, ' 
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.JobOrders.Pipeline.ActiveTitle'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.JobOrders.Pipeline.ActiveTitle'', N''Active Candidates'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.JobOrders.Pipeline.PoolTitle'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.JobOrders.Pipeline.PoolTitle'', N''Candidate Pool'', GETUTCDATE(), GETUTCDATE())
'

--step 108.03
Exec NewDBStep 108.03, ' 
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Company.ImportCandidate.Imported'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Company.ImportCandidate.Imported'', N''Company candidates imported successfully.'', GETUTCDATE(), GETUTCDATE())
'
--step 108.04
Exec NewDBStep 108.04, ' 
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.CompanyCandidate.Fields.JobDuration'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.CompanyCandidate.Fields.JobDuration'', N''Job Dur. (hrs)'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.CompanyCandidate.Fields.IsArrivedToday'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.CompanyCandidate.Fields.IsArrivedToday'', N''Attended'', GETUTCDATE(), GETUTCDATE())
'

--step 108.05
Exec NewDBStep 108.05, ' 
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.Company.NewCandidate'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.Company.NewCandidate'', N''Add Selected to Pool'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.Company.Candidate.Added.Successful'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.Company.Candidate.Added.Successful'', N''Candidates added to pool successfully'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.Company.Candidate.Added.Duplicated'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.Company.Candidate.Added.Duplicated'', N''Candidate exists in the pool'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''ActivityLog.AddCandidateToCompanyPool'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''ActivityLog.AddCandidateToCompanyPool'', N''Add candidate into company pool'', GETUTCDATE(), GETUTCDATE())
'

--step 108.06
Exec NewDBStep 108.06, ' 
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.Companies.CompanyCandidate.Fields.ReasonForLeave.Required'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.Companies.Company.NewCandidate'', N''Reason for Leave is required.'', GETUTCDATE(), GETUTCDATE())
'
--step 108.07
Exec NewDBStep 108.07, ' 
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.JobOrder.JobOrder.Fields.OpeningChanges'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.JobOrder.JobOrder.Fields.OpeningChanges'', N''Since {0} is {1} (changed at {2})'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.JobOrder.JobOrder.Fields.From'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.JobOrder.JobOrder.Fields.From'', N''From:'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.JobOrder.JobOrder.Fields.Shortage'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.JobOrder.JobOrder.Fields.Shortage'', N''Shortage:'', GETUTCDATE(), GETUTCDATE())
'

--step 109.00
Exec NewDBStep 109.00, ' 
	ALTER TABLE [CompanyCandidate] ADD [RatingValue] [int] NULL
	ALTER TABLE [CompanyCandidate] ADD [RatingComment] [nvarchar](2048) NULL
	ALTER TABLE [CompanyCandidate] ADD [RatedBy] [nvarchar](128) NULL
'

--step 110.00
Exec NewDBStep 110.00, ' 
ALTER TABLE [Candidate] ADD [SinExpirationDate] [datetime] NULL

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Web.Candidate.Candidate.Fields.SinExpirationDate'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Web.Candidate.Candidate.Fields.SinExpirationDate'', N''SIN Expiration Date'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Web.Candidate.Candidate.Fields.SinExpirationDate.Required'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Web.Candidate.Candidate.Fields.SinExpirationDate.Required'', N''Expiration Date is required for Temporary SIN.'', GETUTCDATE(), GETUTCDATE())

if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Web.Candidate.Candidate.Fields.SocialInsuranceNumber.Required'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Web.Candidate.Candidate.Fields.SocialInsuranceNumber.Required'', N''Social Insurance Number (SIN) is required.'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Web.Candidate.Candidate.Fields.SocialInsuranceNumber.Invalid'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Web.Candidate.Candidate.Fields.SocialInsuranceNumber.Invalid'', N''Social Insurance Number (SIN) is invalid.'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Web.Candidate.Candidate.Fields.SocialInsuranceNumber.Used'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Web.Candidate.Candidate.Fields.SocialInsuranceNumber.Used'', N''Social Insurance Number (SIN) is used.'', GETUTCDATE(), GETUTCDATE())
'

--step 111.00
Exec NewDBStep 111.00, ' 
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.JobOrder.Pipeline.WeeklyWorkTime'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.JobOrder.Pipeline.WeeklyWorkTime'', N''Weekly WorkTime'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.JobOrder.Pipeline.WeeklyWorkTime.Attach'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.JobOrder.Pipeline.WeeklyWorkTime.Attach'', N''Attach Document'', GETUTCDATE(), GETUTCDATE())
if not exists (select 1 from LocaleStringResource where LanguageId = 1 and ResourceName = N''Admin.JobOrder.Pipeline.WeeklyWorkTime.Total'')
  INSERT [LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, N''Admin.JobOrder.Pipeline.WeeklyWorkTime.Total'', N''Total'', GETUTCDATE(), GETUTCDATE())
'
        
--step 112.00
Exec NewDBStep 112.00, '
CREATE TABLE [ClientTimeSheetDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobOrderId] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[FileType] [int] NOT NULL,
	[FileName] [nvarchar](50) NOT NULL,
	[Stream] [varbinary](max) NOT NULL,
	[Source] [int] NOT NULL,
	[CreatedOnUtc] [datetime] NULL,
	[UpdatedOnUtc] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

ALTER TABLE [dbo].[ClientTimeSheetDocument]  WITH CHECK ADD  CONSTRAINT [ClientTimeSheetDocument_JobOrder] FOREIGN KEY([JobOrderId])
REFERENCES [dbo].[JobOrder] ([Id])
ON DELETE CASCADE'

--step 112.10
Exec NewDBStep 112.10, '
ALTER TABLE [dbo].[ClientTimeSheetDocument] ALTER COLUMN [FileName] [nvarchar](100) NOT NULL
'
