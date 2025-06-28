
SET IDENTITY_INSERT [dbo].[Account] ON

-- Account "Recruiter"
INSERT [dbo].[Account]([Id],[AccountGuid],[Username],[Password],[PasswordFormatId],[PasswordSalt],[Email],[FirstName],[MiddleName],[LastName],[Salutation],[Gender],[HomePhone]
           ,[MobilePhone],[WorkPhone],[EmergencyPhone],[BirthDate],[SocialInsuranceNumber],[Note],[CompanyId],[CompanyLocationId],[CompanyDepartmentId],[ManagerId],[Title],[ReportTo],[IsLeftCompany],[IsActive],[IsDeleted]
           ,[IsClientAccount],[ClientName],[IsSystemAccount],[SystemName],[LastIpAddress],[EnteredBy],[OwnerId] ,[FranchiseId],[IsLimitedToFranchises],[CreatedOnUtc],[UpdatedOnUtc])
           VALUES(8810, 'B6C6B3C0-7F20-4DDB-A40A-EE6618FDF0CA', 'Recruiter', '4D1C47E0A070DDFFFE411DCFAF127247C6FFEB1F', 1, 'cqxK4qE=', 'recruiter@gc-employment.com', 'Recruiter', null, 'I', 'Mr.', 'Male',
                                '416-456-7898', '416-456-7898', '416-456-7898', '416-456-7898',
                                GETUTCDATE(), null, null, 1,0,0,0, 'I Recruiter', null, 0, 1, 0, 0, null, 0, null, null, 1, 0, 1, 0, GETUTCDATE(), GETUTCDATE())
-- Role
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id],[AccountRole_Id]) VALUES (8810, 4)


-- Account "Payroll"
INSERT [dbo].[Account]([Id],[AccountGuid],[Username],[Password],[PasswordFormatId],[PasswordSalt],[Email],[FirstName],[MiddleName],[LastName],[Salutation],[Gender],[HomePhone]
           ,[MobilePhone],[WorkPhone],[EmergencyPhone],[BirthDate],[SocialInsuranceNumber],[Note],[CompanyId],[CompanyLocationId],[CompanyDepartmentId],[ManagerId],[Title],[ReportTo],[IsLeftCompany],[IsActive],[IsDeleted]
           ,[IsClientAccount],[ClientName],[IsSystemAccount],[SystemName],[LastIpAddress],[EnteredBy],[OwnerId] ,[FranchiseId],[IsLimitedToFranchises],[CreatedOnUtc],[UpdatedOnUtc])
           VALUES(8811, 'B6C6B3C0-7F20-4DDB-A40A-EE6618FDF0CA', 'Payroll', '4D1C47E0A070DDFFFE411DCFAF127247C6FFEB1F', 1, 'cqxK4qE=', 'payroll@gc-employment.com', 'Payroll', null, 'I', 'Mr.', 'Male',
                                '416-456-7898', '416-456-7898', '416-456-7898', '416-456-7898',
                                GETUTCDATE(), null, null, 1,0,0,0, 'I Payroll', null, 0, 1, 0, 0, null, 0, null, null, 1, 0, 1, 0, GETUTCDATE(), GETUTCDATE())
-- Role
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id],[AccountRole_Id]) VALUES (8811, 2)

SET IDENTITY_INSERT [dbo].[Account] OFF
GO

/****** Company ******/

-- Polybrite

SET IDENTITY_INSERT [dbo].[Company] ON
INSERT [dbo].[Company] ([Id],[CompanyGuid],[CompanyName],[WebSite],[KeyTechnology],[Note],[IsHot],[IsActive],[IsDeleted],[IsAdminCompany],[OwnerId],[EnteredBy],[FranchiseId],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc],[AdminName])
VALUES (88113, N'1d4d0ef2-dd45-4014-8878-d7f8a4fedaaf', N'Polybrite', N'http://www.Polybrite.ca/ ', N'Tech', NULL, 0, 1, 0, 0, 1, 1, 1, 1, CAST(0x0000A0C200E25504 AS DateTime), CAST(0x0000A161014AC157 AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[Company] OFF
GO

SET IDENTITY_INSERT [dbo].[CompanyLocation] ON
INSERT [dbo].[CompanyLocation] ([Id],[CompanyId],[LocationName],[PrimaryPhone],[PrimaryPhoneExtension],[SecondaryPhone],[SecondaryPhoneExtension],[FaxNumber],[UnitNumber],[AddressLine1],[AddressLine2],[AddressLine3],[City],[StateProvince],[Country],[PostalCode],[MajorIntersection1],[MajorIntersection2],[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (20, 88113, N'Mississauga Office', N'416-444-4777', NULL, NULL, NULL, N'416-444-7084', NULL, N'663 Mississauga Rd', NULL, NULL,  N'Mississauga', N'Ontario', N'Canada', N'M1L 3Z5', NULL, NULL, NULL, 1, 0, 1, 1, CAST(0x0000A0C200E25585 AS DateTime), CAST(0x0000A0C200E25585 AS DateTime))
SET IDENTITY_INSERT [dbo].[CompanyLocation] OFF
GO


SET IDENTITY_INSERT [dbo].[CompanyDepartment] ON
INSERT [dbo].[CompanyDepartment] ([Id],[CompanyId],[DepartmentName],[CompanyLocationId],[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (10, 88113, N'Shipping Dept.', 20, NULL, 1, 0, 16, 0, CAST(0x0000A2E300F78640 AS DateTime), CAST(0x0000A2E300F78640 AS DateTime))
SET IDENTITY_INSERT [dbo].[CompanyDepartment] OFF
GO

SET IDENTITY_INSERT [dbo].[CompanyDepartment] ON
INSERT [dbo].[CompanyDepartment] ([Id],[CompanyId],[DepartmentName],[CompanyLocationId],[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (11, 88113, N'Assemblying Dept.', 20, NULL, 1, 0, 16, 0, CAST(0x0000A2E300F78640 AS DateTime), CAST(0x0000A2E300F78640 AS DateTime))
SET IDENTITY_INSERT [dbo].[CompanyDepartment] OFF
GO

-- Night shift company

SET IDENTITY_INSERT [dbo].[Company] ON
INSERT [dbo].[Company] ([Id],[CompanyGuid],[CompanyName],[WebSite],[KeyTechnology],[Note],[IsHot],[IsActive],[IsDeleted],[IsAdminCompany],[OwnerId],[EnteredBy],[FranchiseId],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc],[AdminName])
VALUES (88210, N'1d4d0ef2-dd45-4014-8878-d7f8a4fedaaf', N'Night Shift', N'http://www.Night.ca/ ', N'Night Shift', NULL, 0, 1, 0, 0, 1, 1, 1, 1, CAST(0x0000A0C200E25504 AS DateTime), CAST(0x0000A161014AC157 AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[Company] OFF
GO

SET IDENTITY_INSERT [dbo].[CompanyLocation] ON
INSERT [dbo].[CompanyLocation] ([Id],[CompanyId],[LocationName],[PrimaryPhone],[PrimaryPhoneExtension],[SecondaryPhone],[SecondaryPhoneExtension],[FaxNumber],[UnitNumber],[AddressLine1],[AddressLine2],[AddressLine3],[City],[StateProvince],[Country],[PostalCode],[MajorIntersection1],[MajorIntersection2],[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (30, 88210, N'Toronto Office', N'416-444-4777', NULL, NULL, NULL, N'416-444-7084', NULL, N'999 Yonge Street', NULL, NULL,  N'Toronto', N'Ontario', N'Canada', N'M1L 3Z5', NULL, NULL, NULL, 1, 0, 1, 1, CAST(0x0000A0C200E25585 AS DateTime), CAST(0x0000A0C200E25585 AS DateTime))
SET IDENTITY_INSERT [dbo].[CompanyLocation] OFF
GO


SET IDENTITY_INSERT [dbo].[CompanyDepartment] ON
INSERT [dbo].[CompanyDepartment] ([Id],[CompanyId],[DepartmentName],[CompanyLocationId],[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (31, 88210, N'Night shift', 30, NULL, 1, 0, 16, 0, CAST(0x0000A2E300F78640 AS DateTime), CAST(0x0000A2E300F78640 AS DateTime))
SET IDENTITY_INSERT [dbo].[CompanyDepartment] OFF
GO


----------------------------------------------------------------------------------------------------------------------------------------------------------------------

-- Polybrite Contact

SET IDENTITY_INSERT [dbo].[Account] ON
INSERT [dbo].[Account] ([Id],[AccountGuid],[Username],[Password],[PasswordFormatId],[PasswordSalt],[Email],[FirstName],[MiddleName],[LastName],[Salutation],[Gender],[HomePhone],[MobilePhone],[WorkPhone],[EmergencyPhone],[BirthDate],[SocialInsuranceNumber],[Note],
                        [CompanyId],[CompanyLocationId],[CompanyDepartmentId],[ManagerId],[Title],[ReportTo],[IsLeftCompany],[IsActive],[IsDeleted],[IsSystemAccount],[SystemName],[LastIpAddress],[EnteredBy],[OwnerId],[FranchiseId],[IsLimitedToFranchises],[CreatedOnUtc],[UpdatedOnUtc],[IsClientAccount],[ClientName])
VALUES (8831, N'08c2772a-a884-470e-a3e4-308512968d50', N'admin@Polybrite.ca', N'4D1C47E0A070DDFFFE411DCFAF127247C6FFEB1F', 1, N'cqxK4qE=', N'admin@Polybrite.ca', N'Polybrite', NULL, N'Administrator', N'Mr.', NULL, NULL, NULL, N'1234567890', NULL, NULL, NULL, NULL,
        88113, 20, 10, 0, N'Polybrite Administrator', NULL, 0, 1, 0, 0, NULL, NULL, 0, 0, 1, 1, CAST(0x0000A351002534A1 AS DateTime), CAST(0x0000A351002534A1 AS DateTime), 1, N'Company')
SET IDENTITY_INSERT [dbo].[Account] OFF


SET IDENTITY_INSERT [dbo].[Account] ON
INSERT [dbo].[Account] ([Id],[AccountGuid],[Username],[Password],[PasswordFormatId],[PasswordSalt],[Email],[FirstName],[MiddleName],[LastName],[Salutation],[Gender],[HomePhone],[MobilePhone],[WorkPhone],[EmergencyPhone],[BirthDate],[SocialInsuranceNumber],[Note],
                        [CompanyId],[CompanyLocationId],[CompanyDepartmentId],[ManagerId],[Title],[ReportTo],[IsLeftCompany],[IsActive],[IsDeleted],[IsSystemAccount],[SystemName],[LastIpAddress],[EnteredBy],[OwnerId],[FranchiseId],[IsLimitedToFranchises],[CreatedOnUtc],[UpdatedOnUtc],[IsClientAccount],[ClientName])
VALUES (8832, N'08c2772a-a884-470e-a3e4-308512968d50', N'shipping@Polybrite.ca', N'4D1C47E0A070DDFFFE411DCFAF127247C6FFEB1F', 1, N'cqxK4qE=', N'shipping@Polybrite.ca', N'Polybrite', NULL, N'Shipping', N'Mr.', NULL, NULL, NULL, N'1234567890', NULL, NULL, NULL, NULL,
        88113, 20, 10, 0, N'Shipping Supervisor', NULL, 0, 1, 0, 0, NULL, NULL, 0, 0, 1, 1, CAST(0x0000A351002534A1 AS DateTime), CAST(0x0000A351002534A1 AS DateTime), 1, N'Company')
SET IDENTITY_INSERT [dbo].[Account] OFF


SET IDENTITY_INSERT [dbo].[Account] ON
INSERT [dbo].[Account] ([Id],[AccountGuid],[Username],[Password],[PasswordFormatId],[PasswordSalt],[Email],[FirstName],[MiddleName],[LastName],[Salutation],[Gender],[HomePhone],[MobilePhone],[WorkPhone],[EmergencyPhone],[BirthDate],[SocialInsuranceNumber],[Note],
                        [CompanyId],[CompanyLocationId],[CompanyDepartmentId],[ManagerId],[Title],[ReportTo],[IsLeftCompany],[IsActive],[IsDeleted],[IsSystemAccount],[SystemName],[LastIpAddress],[EnteredBy],[OwnerId],[FranchiseId],[IsLimitedToFranchises],[CreatedOnUtc],[UpdatedOnUtc],[IsClientAccount],[ClientName])
VALUES (8833, N'08c2772a-a884-470e-a3e4-308512968d50', N'assemblying@Polybrite.ca', N'4D1C47E0A070DDFFFE411DCFAF127247C6FFEB1F', 1, N'cqxK4qE=', N'assemblying@Polybrite.ca', N'Polybrite', NULL, N'Assemblying', N'Mrs.', NULL, NULL, NULL, N'1234567890', NULL, NULL, NULL, NULL,
        88113, 20, 11, 0, N'Assemblying Supervisor', NULL, 0, 1, 0, 0, NULL, NULL, 0, 0, 1, 1, CAST(0x0000A351002534A1 AS DateTime), CAST(0x0000A351002534A1 AS DateTime), 1, N'Company')
SET IDENTITY_INSERT [dbo].[Account] OFF
GO

-- Role
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id],[AccountRole_Id]) VALUES (8831, 51)
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id],[AccountRole_Id]) VALUES (8832, 53)
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id],[AccountRole_Id]) VALUES (8833, 53)
GO


-- -- Night shift company Contact

SET IDENTITY_INSERT [dbo].[Account] ON
INSERT [dbo].[Account] ([Id],[AccountGuid],[Username],[Password],[PasswordFormatId],[PasswordSalt],[Email],[FirstName],[MiddleName],[LastName],[Salutation],[Gender],[HomePhone],[MobilePhone],[WorkPhone],[EmergencyPhone],[BirthDate],[SocialInsuranceNumber],[Note],
                        [CompanyId],[CompanyLocationId],[CompanyDepartmentId],[ManagerId],[Title],[ReportTo],[IsLeftCompany],[IsActive],[IsDeleted],[IsSystemAccount],[SystemName],[LastIpAddress],[EnteredBy],[OwnerId],[FranchiseId],[IsLimitedToFranchises],[CreatedOnUtc],[UpdatedOnUtc],[IsClientAccount],[ClientName])
VALUES (8845, N'08c2772a-a884-470e-a3e4-308512968d50', N'demo@night.ca', N'4D1C47E0A070DDFFFE411DCFAF127247C6FFEB1F', 1, N'cqxK4qE=', N'demo@night.ca', N'Demo', NULL, N'Night', N'Mrs.', NULL, NULL, NULL, N'1111111111', NULL, NULL, NULL, NULL,
        88210, 30, 31, 0, N'Supervisor', NULL, 0, 1, 0, 0, NULL, NULL, 0, 0, 1, 1, CAST(0x0000A351002534A1 AS DateTime), CAST(0x0000A351002534A1 AS DateTime), 1, N'Company')
SET IDENTITY_INSERT [dbo].[Account] OFF
GO
-- Role
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id],[AccountRole_Id]) VALUES (8845, 51)
GO


----------------------------------------------------------------------------------------------------------------------------------------------------------------------

-- Polybrite ClockDevice / BillingRates

SET IDENTITY_INSERT [dbo].[CompanyClockDevice] ON
INSERT [dbo].[CompanyClockDevice] ([Id],[CompanyLocationId],[ClockDeviceUid],[MacAddress],[DongleName],[DongleModel],[SIMCardCarrier],[IsActive],[IsDeleted],[ActivatedDate],[DeactivatedDate],[ReasonForDeactivation],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (10, 20, N'PC000002', NULL, NULL, NULL, NULL, 1, 0, NULL, NULL, NULL, NULL, 0, NULL, CAST(0x0000A2F300155313 AS DateTime))
SET IDENTITY_INSERT [dbo].[CompanyClockDevice] OFF
GO

SET IDENTITY_INSERT [dbo].[CompanyBillingRates] ON
INSERT [dbo].[CompanyBillingRates] ([Id],[CompanyId],[RateCode],[PositionCode],[ShiftCode],[RegularBillingRate],[RegularPayRate],[OvertimeBillingRate],[OvertimePayRate],[BillingTaxRate],[MaxRate],[WeeklyWorkHours],[AveragingWorkHoursPeriod],[EffectiveDate],[DeactivatedDate],[ReasonForDeactivation],[Note],[IsActive],[EnteredBy],[DisplayOrder],[IsDeleted],[DeletedBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (15, 88113, N'GL / A', N'GL', N'A', CAST(14.7800 AS Decimal(18, 4)), CAST(10.3400 AS Decimal(18, 4)), CAST(22.1700 AS Decimal(18, 4)), CAST(15.5100 AS Decimal(18, 4)),CAST(0.1300 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), CAST(44.00 AS Decimal(18, 2)), 0, CAST(0x0000A30300000000 AS DateTime), NULL, NULL, NULL, 1, 0, 0, 0, 0, CAST(0x0000A303014576ED AS DateTime), CAST(0x0000A303014576ED AS DateTime))
SET IDENTITY_INSERT [dbo].[CompanyBillingRates] OFF
GO


-- Night shift company ClockDevice / BillingRates

SET IDENTITY_INSERT [dbo].[CompanyClockDevice] ON
INSERT [dbo].[CompanyClockDevice] ([Id],[CompanyLocationId],[ClockDeviceUid],[MacAddress],[DongleName],[DongleModel],[SIMCardCarrier],[IsActive],[IsDeleted],[ActivatedDate],[DeactivatedDate],[ReasonForDeactivation],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (11, 30, N'PC000003', NULL, NULL, NULL, NULL, 1, 0, NULL, NULL, NULL, NULL, 0, NULL, CAST(0x0000A2F300155313 AS DateTime))
SET IDENTITY_INSERT [dbo].[CompanyClockDevice] OFF
GO

SET IDENTITY_INSERT [dbo].[CompanyBillingRates] ON
INSERT [dbo].[CompanyBillingRates] ([Id],[CompanyId],[RateCode],[PositionCode],[ShiftCode],[RegularBillingRate],[RegularPayRate],[OvertimeBillingRate],[OvertimePayRate],[BillingTaxRate],[MaxRate],[WeeklyWorkHours],[AveragingWorkHoursPeriod],[EffectiveDate],[DeactivatedDate],[ReasonForDeactivation],[Note],[IsActive],[EnteredBy],[DisplayOrder],[IsDeleted],[DeletedBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (16, 88210, N'FL / D', N'FL', N'D', CAST(15.7800 AS Decimal(18, 4)), CAST(11.3400 AS Decimal(18, 4)), CAST(23.1700 AS Decimal(18, 4)), CAST(16.5100 AS Decimal(18, 4)),CAST(0.1300 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), CAST(44.00 AS Decimal(18, 2)), 0, CAST(0x0000A30300000000 AS DateTime), NULL, NULL, NULL, 1, 0, 0, 0, 0, CAST(0x0000A303014576ED AS DateTime), CAST(0x0000A303014576ED AS DateTime))
SET IDENTITY_INSERT [dbo].[CompanyBillingRates] OFF
GO



------****** Policies ******-------------------------------------------------------------------------------------------------------------------------------------------

-- Polybrite

SET IDENTITY_INSERT [dbo].[MealPolicy] ON
INSERT INTO [dbo].[MealPolicy]([Id],[CompanyId],[Name]
        ,[MealTimeInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (1,88113,'M20'
        ,20
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')
SET IDENTITY_INSERT [dbo].[MealPolicy] OFF
GO

SET IDENTITY_INSERT [dbo].[BreakPolicy] ON
INSERT INTO [dbo].[BreakPolicy]  ([Id],[CompanyId],[Name]
        ,[BreakTimeInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (1,88113,'B15'
        ,15
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')
SET IDENTITY_INSERT [dbo].[BreakPolicy] OFF
GO

SET IDENTITY_INSERT [dbo].[RoundingPolicy] ON
INSERT INTO [dbo].[RoundingPolicy]([Id],[CompanyId],[Name]
        ,[IntervalInMinutes],[GracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (1,88113,'A15'
        ,15,8
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')
SET IDENTITY_INSERT [dbo].[RoundingPolicy] OFF
GO


-- Night shift company


SET IDENTITY_INSERT [dbo].[MealPolicy] ON
INSERT INTO [dbo].[MealPolicy]([Id],[CompanyId],[Name]
        ,[MealTimeInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (2,88210,'M15'
        ,15
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')
SET IDENTITY_INSERT [dbo].[MealPolicy] OFF
GO

SET IDENTITY_INSERT [dbo].[BreakPolicy] ON
INSERT INTO [dbo].[BreakPolicy]  ([Id],[CompanyId],[Name]
        ,[BreakTimeInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (2,88210,'B15'
        ,15
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')
SET IDENTITY_INSERT [dbo].[BreakPolicy] OFF
GO

SET IDENTITY_INSERT [dbo].[RoundingPolicy] ON
INSERT INTO [dbo].[RoundingPolicy]([Id],[CompanyId],[Name]
        ,[IntervalInMinutes],[GracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (2,88210,'A15'
        ,15,5
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')
SET IDENTITY_INSERT [dbo].[RoundingPolicy] OFF
GO


------****** Schedule Policies ******-----------------------------------------------------------------------------------------------------------------------------------


-- Polybrite


SET IDENTITY_INSERT [dbo].[SchedulePolicy] ON

-- Job order standard time
INSERT INTO [dbo].[SchedulePolicy] ([Id],[CompanyId],[Name]
        ,[MealPolicyId],[BreakPolicyId],[RoundingPolicyId],[IsStrictSchedule],[OvertimeGracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
       (1,88113,'M20-B00-A00'
       ,1,0,0,0,15
       ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')

-- Use punch time
INSERT INTO [dbo].[SchedulePolicy] ([Id],[CompanyId],[Name]
        ,[MealPolicyId],[BreakPolicyId],[RoundingPolicyId],[IsStrictSchedule],[OvertimeGracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
       (2,88113,'M20-B00-A00-S'
       ,1,0,0,1,15
       ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')


-- Job order standard time
INSERT INTO [dbo].[SchedulePolicy] ([Id],[CompanyId],[Name]
        ,[MealPolicyId],[BreakPolicyId],[RoundingPolicyId],[IsStrictSchedule],[OvertimeGracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
       (3,88113,'M00-B15-A00'
       ,0,1,0,0,15
       ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')


-- Use punch time
INSERT INTO [dbo].[SchedulePolicy] ([Id],[CompanyId],[Name]
        ,[MealPolicyId],[BreakPolicyId],[RoundingPolicyId],[IsStrictSchedule],[OvertimeGracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
       (4,88113,'M00-B15-A00-S'
       ,0,1,0,1,15
       ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')



-- Job order standard time
INSERT INTO [dbo].[SchedulePolicy] ([Id],[CompanyId],[Name]
        ,[MealPolicyId],[BreakPolicyId],[RoundingPolicyId],[IsStrictSchedule],[OvertimeGracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
       (5,88113,'M00-B00-A15'
       ,0,0,1,0,15
       ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')


-- Use punch time
INSERT INTO [dbo].[SchedulePolicy] ([Id],[CompanyId],[Name]
        ,[MealPolicyId],[BreakPolicyId],[RoundingPolicyId],[IsStrictSchedule],[OvertimeGracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (6,88113,'M00-B00-A15-S'
        ,0,0,1,1,15
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')



INSERT INTO [dbo].[SchedulePolicy] ([Id],[CompanyId],[Name]
        ,[MealPolicyId],[BreakPolicyId],[RoundingPolicyId],[IsStrictSchedule],[OvertimeGracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (7,88113,'M20-B15-A00'
        ,1,1,0,0,15
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')


INSERT INTO [dbo].[SchedulePolicy] ([Id],[CompanyId],[Name]
        ,[MealPolicyId],[BreakPolicyId],[RoundingPolicyId],[IsStrictSchedule],[OvertimeGracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (8,88113,'M20-B15-A00-S'
        ,1,1,0,1,15
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')




INSERT INTO [dbo].[SchedulePolicy] ([Id],[CompanyId],[Name]
        ,[MealPolicyId],[BreakPolicyId],[RoundingPolicyId],[IsStrictSchedule],[OvertimeGracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (9,88113,'M20-B00-A15'
        ,1,0,1,0,15
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')


INSERT INTO [dbo].[SchedulePolicy] ([Id],[CompanyId],[Name]
        ,[MealPolicyId],[BreakPolicyId],[RoundingPolicyId],[IsStrictSchedule],[OvertimeGracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (10,88113,'M20-B00-A15-S'
        ,1,0,1,1,15
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')



INSERT INTO [dbo].[SchedulePolicy] ([Id],[CompanyId],[Name]
        ,[MealPolicyId],[BreakPolicyId],[RoundingPolicyId],[IsStrictSchedule],[OvertimeGracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (11,88113,'M00-B15-A15'
        ,0,1,1,0,15
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')


INSERT INTO [dbo].[SchedulePolicy] ([Id],[CompanyId],[Name]
        ,[MealPolicyId],[BreakPolicyId],[RoundingPolicyId],[IsStrictSchedule],[OvertimeGracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (12,88113,'M00-B15-A15-S'
        ,0,1,1,1,15
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')




INSERT INTO [dbo].[SchedulePolicy] ([Id],[CompanyId],[Name]
        ,[MealPolicyId],[BreakPolicyId],[RoundingPolicyId],[IsStrictSchedule],[OvertimeGracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (13,88113,'M20-B15-A15'
        ,1,1,1,0,15
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')


INSERT INTO [dbo].[SchedulePolicy] ([Id],[CompanyId],[Name]
        ,[MealPolicyId],[BreakPolicyId],[RoundingPolicyId],[IsStrictSchedule],[OvertimeGracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (14,88113,'M20-B15-A15-S'
        ,1,1,1,1,15
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')



SET IDENTITY_INSERT [dbo].[SchedulePolicy] OFF
GO


-- Night shift company

SET IDENTITY_INSERT [dbo].[SchedulePolicy] ON

INSERT INTO [dbo].[SchedulePolicy] ([Id],[CompanyId],[Name]
        ,[MealPolicyId],[BreakPolicyId],[RoundingPolicyId],[IsStrictSchedule],[OvertimeGracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (21,88210,'M15-B15-A15'
        ,2,2,2,0,15
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')


INSERT INTO [dbo].[SchedulePolicy] ([Id],[CompanyId],[Name]
        ,[MealPolicyId],[BreakPolicyId],[RoundingPolicyId],[IsStrictSchedule],[OvertimeGracePeriodInMinutes]
        ,[Note],[IsActive],[IsDeleted],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (22,88210,'M15-B15-A15-S'
        ,2,2,2,1,15
        ,NULL,1,0,1,0,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')

SET IDENTITY_INSERT [dbo].[SchedulePolicy] OFF
GO
