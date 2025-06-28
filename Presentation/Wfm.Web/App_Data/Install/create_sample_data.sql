---- Demo Franchise
--SET IDENTITY_INSERT [dbo].[Franchise] ON
--INSERT [dbo].[Franchise]([Id], [FranchiseGuid], [FranchiseId], [FranchiseName], [BusinessNumber], [PrimaryContactName], [Email], [WebSite], [Description], [ReasonForDisabled], [Note], [IsHot], [IsActive], [IsDeleted], [OwnerId], [EnteredBy], [DisplayOrder], [CreatedOnUtc], [UpdatedOnUtc])
--                 VALUES (7, '8394FCD9-A374-4472-AF02-CA7EEC78B2E5', 'GCFR13-0010', 'Demo Franchise', '416-850-5060', '123456789', 'DemoFranchise@Franchise.com', 'http://www.demofranchise.com', null, null, null, 1, 1, 0, 1, 1, 1, GETUTCDATE(), GETUTCDATE())
--SET IDENTITY_INSERT [dbo].[Franchise] OFF
--GO

-- Demo Company
SET IDENTITY_INSERT [dbo].[Company] ON
INSERT [dbo].[Company]([Id], [CompanyGuid], [CompanyName], [WebSite], [KeyTechnology], [Note], [IsHot], [IsActive], [IsDeleted],[IsAdmin], [OwnerId], [EnteredBy], [FranchiseId], [DisplayOrder], [CreatedOnUtc], [UpdatedOnUtc])
			   VALUES (10, '{31D5B5ED-515F-47B8-B449-6DDB6E9057BB}', 'Demo Company', 'http://www.democompany.com', 'Demo business', null, 1, 1, 0, 0, 1, 1, 1, 1, GETUTCDATE(), GETUTCDATE())
SET IDENTITY_INSERT [dbo].[Company] OFF
GO


SET IDENTITY_INSERT [dbo].[Account] ON
-- Account "DemoR"
INSERT [dbo].[Account](
			[Id]
		   ,[AccountGuid]
           ,[Username]
           ,[Password]
           ,[PasswordFormatId]
           ,[PasswordSalt]
		   ,[PasswordResetToken]
		   ,[TokenExpiryDate]
           ,[Email]
           ,[FirstName]
           ,[MiddleName]
           ,[LastName]
           ,[Salutation]
		   ,[Gender]
           ,[HomePhone]
           ,[MobilePhone]
           ,[WorkPhone]
           ,[EmergencyPhone]
           ,[BirthDate]
           ,[SocialInsuranceNumber]
           ,[Note]
           ,[CompanyId]
           ,[Title]
           ,[ReportTo]
           ,[IsLeftCompany]
           ,[IsActive]
           ,[IsDeleted]
           ,[IsSystemAccount]
           ,[SystemName]
           ,[LastIpAddress]
           ,[EnteredBy]
           ,[OwnerId]
           ,[FranchiseId]
           ,[IsLimitedToFranchises]
           ,[CreatedOnUtc]
           ,[UpdatedOnUtc]) VALUES(10, '1B0D2088-4BC4-427F-AB56-2D8AC211A610', 'DemoR', '2F831F2E00B26B8EA8624EA9F8071196DE615B75', 1, 'LFWXYhY=', 'c00b57bb-c7ad-4baa-8d61-db521605eab6', null, 'recruiter@gc-employment.com', 'Demo Recruiter', null, 'for Testing', 'Mr.', 'Male', 
								'416-456-7898', '416-456-7898', '416-456-7898', '416-456-7898', 
								GETUTCDATE(), null, null, 1, 'Recruiter', null, 0, 1, 0, 0,  null, null, 1, 1, 1, 0, GETUTCDATE(), GETUTCDATE())

-- Account "DemoC"
INSERT [dbo].[Account](
			[Id]
		   ,[AccountGuid]
           ,[Username]
           ,[Password]
           ,[PasswordFormatId]
           ,[PasswordSalt]
		   ,[PasswordResetToken]
		   ,[TokenExpiryDate]
           ,[Email]
           ,[FirstName]
           ,[MiddleName]
           ,[LastName]
           ,[Salutation]
		   ,[Gender]
           ,[HomePhone]
           ,[MobilePhone]
           ,[WorkPhone]
           ,[EmergencyPhone]
           ,[BirthDate]
           ,[SocialInsuranceNumber]
           ,[Note]
           ,[CompanyId]
           ,[Title]
           ,[ReportTo]
           ,[IsLeftCompany]
           ,[IsActive]
           ,[IsDeleted]
           ,[IsSystemAccount]
           ,[SystemName]
           ,[LastIpAddress]
           ,[EnteredBy]
           ,[OwnerId]
           ,[FranchiseId]
           ,[IsLimitedToFranchises]
           ,[CreatedOnUtc]
           ,[UpdatedOnUtc]) VALUES(11, '1B0D2088-4BC4-427F-AB56-2D8AC211A610', 'DemoC', '2F831F2E00B26B8EA8624EA9F8071196DE615B75', 1, 'LFWXYhY=', 'c00b57bb-c7ad-4baa-8d61-db521605eab6', null, 'company@company.com', 'Demo Contact', null, 'for Testing', 'Mr.', 'Male', 
								'416-456-7898', '416-456-7898', '416-456-7898', '416-456-7898', 
								GETUTCDATE(), null, null, 10, 'Company System Administrator', null, 0, 1, 0, 0,  null, null, 1, 1, 7, 0, GETUTCDATE(), GETUTCDATE())
SET IDENTITY_INSERT [dbo].[Account] OFF
GO

--------------------------------------------------------------------------------------------------
-- Assign User to Role
--------------------------------------------------------------------------------------------------

-- Assign User "DemoR" as Recruiters Role
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id], [AccountRole_Id]) VALUES (10, 3)
GO

-- Assign User "DemoC" as Recruiters Role
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id], [AccountRole_Id]) VALUES (11, 4)
GO