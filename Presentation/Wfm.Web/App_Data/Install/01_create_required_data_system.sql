SET IDENTITY_INSERT [dbo].[Franchise] ON
INSERT [dbo].[Franchise]([Id],[FranchiseGuid],[FranchiseId],[FranchiseName],[BusinessNumber],[PrimaryContactName],[Email],[WebSite],[IsSslEnabled],[Description],[ReasonForDisabled],[Note],[IsHot],[IsActive],[IsDeleted],[OwnerId],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
                 VALUES (1, '8394FCD9-A374-4472-AF02-CA7EEC78B2E5', 'GCFR13-0001', 'GC Head Office', '416-850-5060', '123456789', 'GC-employment@gmail.com', 'http://www.gc-employment.com', 0, null, null, null, 1, 1, 0, 1, 1, 1, GETUTCDATE(), GETUTCDATE())
SET IDENTITY_INSERT [dbo].[Franchise] OFF
GO


SET IDENTITY_INSERT [dbo].[Company] ON
INSERT [dbo].[Company]([Id],[CompanyGuid],[CompanyName],[WebSite],[KeyTechnology],[Note],[IsHot],[IsActive],[IsDeleted],[IsAdminCompany],[OwnerId],[EnteredBy],[FranchiseId],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
        VALUES (1, '{31D5B5ED-515F-47B8-B449-6DDB6E9057BB}', 'GC Employment Services', 'http://www.gc-employment.com', 'Toronto based Staffing Company', null, 1, 1, 0, 1, 1, 0, 1, 0, GETUTCDATE(), GETUTCDATE())
SET IDENTITY_INSERT [dbo].[Company] OFF
GO


SET IDENTITY_INSERT [dbo].[CompanyLocation] ON
INSERT [dbo].[CompanyLocation]([Id]
      ,[CompanyId],[LocationName],[PrimaryPhone],[PrimaryPhoneExtension],[SecondaryPhone],[SecondaryPhoneExtension],[FaxNumber]
      ,[UnitNumber],[AddressLine1],[AddressLine2],[AddressLine3],[CityId],[StateProvinceId],[CountryId],[PostalCode],[MajorIntersection1],[MajorIntersection2]
      ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
       VALUES (1, 1, N'GC Toronto', N'(416) 850-5060', null, null, null, null, 
        null, N'28 Madison Ave.', null, null, 1204, 71, 2, N'M5R 2S1', null, null,
        null, 1, 0, 1, 0, GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[CompanyLocation]([Id]
      ,[CompanyId],[LocationName],[PrimaryPhone],[PrimaryPhoneExtension],[SecondaryPhone],[SecondaryPhoneExtension],[FaxNumber]
      ,[UnitNumber],[AddressLine1],[AddressLine2],[AddressLine3],[CityId],[StateProvinceId],[CountryId],[PostalCode],[MajorIntersection1],[MajorIntersection2]
      ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
       VALUES (2, 1, N'GC North York', N'(416) 850-5060', null, null, null, null, 
        N'109', N'5050 Dufferin St.', null, null, 1166, 71, 2, N'M3H 5T5', null, null,
        null, 1, 0, 1, 0, GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[CompanyLocation]([Id]
      ,[CompanyId],[LocationName],[PrimaryPhone],[PrimaryPhoneExtension],[SecondaryPhone],[SecondaryPhoneExtension],[FaxNumber]
      ,[UnitNumber],[AddressLine1],[AddressLine2],[AddressLine3],[CityId],[StateProvinceId],[CountryId],[PostalCode],[MajorIntersection1],[MajorIntersection2]
      ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
       VALUES (3, 1, N'GC Mississauga', N'(416) 299-0789', null, null, null, null, 
        N'10', N'7025 Tomken Rd.', null, null, 1219, 71, 2, N'L5S 1R6', null, null,
        null, 1, 0, 1, 0, GETUTCDATE(), GETUTCDATE())
SET IDENTITY_INSERT [dbo].[CompanyLocation] OFF
GO



--------------------------------------------------------------------------------------------------


SET IDENTITY_INSERT [dbo].[Account] ON
--------------------------------------------------------------------------------------------------
-- Admin Account
--------------------------------------------------------------------------------------------------
-- Account "Admin"
INSERT [dbo].[Account]([Id],[AccountGuid],[Username],[Password],[PasswordFormatId],[PasswordSalt],[Email],[FirstName],[MiddleName],[LastName],[SalutationId],[GenderId],[HomePhone]
           ,[MobilePhone],[WorkPhone],[EmergencyPhone],[BirthDate],[SocialInsuranceNumber],[Note],[CompanyId],[CompanyLocationId],[CompanyDepartmentId],[ManagerId],[Title],[ReportTo],[IsLeftCompany],[IsActive],[IsDeleted]
           ,[IsClientAccount],[ClientName],[IsSystemAccount],[SystemName],[LastIpAddress],[EnteredBy],[OwnerId] ,[FranchiseId],[IsLimitedToFranchises],[CreatedOnUtc],[UpdatedOnUtc])
           VALUES(1, 'B6C6B3C0-7F20-4DDB-A40A-EE6618FDF0CA', 'Admin', '2F831F2E00B26B8EA8624EA9F8071196DE615B75', 1, 'LFWXYhY=', 'admin@gc-employment.com', 'Admin', null, 'Super', 3, 3, 
                                '416-456-7898', '416-456-7898', '416-456-7898', '416-456-7898', 
                                GETUTCDATE(), null, null, 1,0,0,0, 'Super Admin', null, 0, 1, 0, 0, null, 0, null, null, 1, 0, 1, 0, GETUTCDATE(), GETUTCDATE())


--------------------------------------------------------------------------------------------------
-- Staff Account
--------------------------------------------------------------------------------------------------
-- "joey"
INSERT [dbo].[Account]([Id],[AccountGuid],[Username],[Password],[PasswordFormatId],[PasswordSalt],[Email],[FirstName],[MiddleName],[LastName],[SalutationId],[GenderId],[HomePhone]
           ,[MobilePhone],[WorkPhone],[EmergencyPhone],[BirthDate],[SocialInsuranceNumber],[Note],[CompanyId],[CompanyLocationId],[CompanyDepartmentId],[ManagerId],[Title],[ReportTo],[IsLeftCompany],[IsActive],[IsDeleted]
           ,[IsClientAccount],[ClientName],[IsSystemAccount],[SystemName],[LastIpAddress],[EnteredBy],[OwnerId] ,[FranchiseId],[IsLimitedToFranchises],[CreatedOnUtc],[UpdatedOnUtc])
           VALUES(2, '1B0D2088-4BC4-427F-AB56-2D8AC211A610', 'Joey', '2F831F2E00B26B8EA8624EA9F8071196DE615B75', 1, 'LFWXYhY=', 'joey.he@gc-employment.com', 'Joey', null, 'He', 3, 3, 
                 '647-400-7568', '647-400-7568', '416-850-5060(221)', '647-400-7568', 
                 GETUTCDATE(), null, null, 1,0,0,0, 'CEO', null, 0, 1, 0, 0, null, 0, null, null, 1, 0, 1, 0, GETUTCDATE(), GETUTCDATE())


---"Shaun Levy"
INSERT [dbo].[Account]([Id],[AccountGuid],[Username],[Password],[PasswordFormatId],[PasswordSalt],[Email],[FirstName],[MiddleName],[LastName],[SalutationId],[GenderId],[HomePhone]
           ,[MobilePhone],[WorkPhone],[EmergencyPhone],[BirthDate],[SocialInsuranceNumber],[Note],[CompanyId],[CompanyLocationId],[CompanyDepartmentId],[ManagerId],[Title],[ReportTo],[IsLeftCompany],[IsActive],[IsDeleted]
           ,[IsClientAccount],[ClientName],[IsSystemAccount],[SystemName],[LastIpAddress],[EnteredBy],[OwnerId] ,[FranchiseId],[IsLimitedToFranchises],[CreatedOnUtc],[UpdatedOnUtc])
           VALUES(3, '1B0D2088-4BC4-427F-AB56-2D8AC211A610', 'Shaun', '2F831F2E00B26B8EA8624EA9F8071196DE615B75', 1, 'LFWXYhY=', 'shaunlevy@gc-employment.com', 'Shaun', null, 'Levy', 3, 3, 
                 '416-456-6717', '416-456-6717', '416-850-5060(222)', '416-456-6717', 
                 GETUTCDATE(), null, null, 1,0,0,0, 'CEO', null, 0, 1, 0, 0, null, 0, null, null, 1, 0, 1, 0, GETUTCDATE(), GETUTCDATE())

SET IDENTITY_INSERT [dbo].[Account] OFF
GO


--