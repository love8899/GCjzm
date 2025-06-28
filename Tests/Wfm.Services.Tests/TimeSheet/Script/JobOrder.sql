-- Tradition Fine Foods

SET IDENTITY_INSERT [dbo].[JobOrder] ON
INSERT INTO [dbo].[JobOrder] ([Id],[JobOrderGuid],[CompanyId],[CompanyLocationId],[CompanyDepartmentId],[CompanyContactId],[CompanyJobNumber]
        ,[JobTitle],[JobDescription],[HiringDurationExpiredDate],[EstimatedFinishingDate],[EstimatedMargin],[StartDate],[EndDate],[StartTime],[EndTime]
        ,[SchedulePolicyId],[JobOrderTypeId],[Salary],[JobOrderStatusId],[JobOrderCategoryId],[CompanyBillingRateId]
        ,[OpeningNumber],[ShiftId],[ShiftSchedule],[Supervisor],[HoursPerWeek],[Note],[RequireSafeEquipment],[RequireSafetyShoe]
        ,[IsInternalPosting],[IsPublished],[IsHot],[IsDeleted],[RecruiterId],[OwnerId],[FranchiseId],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (1,NEWID(),88113,20,10,8832,NULL
        ,'Shipping','Shipping workers for morning shift (Strict schedule)',NULL,NULL,NULL,'2014-06-01 00:00:00.000','2014-06-05 00:00:00.000','2014-06-01 09:00:00.000','2014-06-01 17:00:00.000'
        ,14,1,NULL,1,16,15
        ,10,2,NULL,NULL,NULL,NULL,1,0
        ,0,1,0,0,1,1,1,1,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')


INSERT INTO [dbo].[JobOrder] ([Id],[JobOrderGuid],[CompanyId],[CompanyLocationId],[CompanyDepartmentId],[CompanyContactId],[CompanyJobNumber]
        ,[JobTitle],[JobDescription],[HiringDurationExpiredDate],[EstimatedFinishingDate],[EstimatedMargin],[StartDate],[EndDate],[StartTime],[EndTime]
        ,[SchedulePolicyId],[JobOrderTypeId],[Salary],[JobOrderStatusId],[JobOrderCategoryId],[CompanyBillingRateId]
        ,[OpeningNumber],[ShiftId],[ShiftSchedule],[Supervisor],[HoursPerWeek],[Note],[RequireSafeEquipment],[RequireSafetyShoe]
        ,[IsInternalPosting],[IsPublished],[IsHot],[IsDeleted],[RecruiterId],[OwnerId],[FranchiseId],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (2,NEWID(),88113,20,11,8833,NULL
        ,'Assemblying','Assemblying workers for afternoon shift (Strict schedule)',NULL,NULL,NULL,'2014-06-06 00:00:00.000','2014-06-06 00:00:00.000','2014-06-06 15:00:00.000','2014-06-06 23:00:00.000'
        ,14,1,NULL,1,16,15
        ,10,2,NULL,NULL,NULL,NULL,1,0
        ,0,1,0,0,1,1,1,1,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')



--INSERT INTO [dbo].[JobOrder] ([Id],[JobOrderGuid],[CompanyId],[CompanyLocationId],[CompanyDepartmentId],[CompanyContactId],[CompanyJobNumber]
--        ,[JobTitle],[JobDescription],[HiringDurationExpiredDate],[EstimatedFinishingDate],[EstimatedMargin],[StartDate],[EndDate],[StartTime],[EndTime]
--        ,[SchedulePolicyId],[JobOrderTypeId],[Salary],[JobOrderStatusId],[JobOrderCategoryId],[CompanyBillingRateId]
--        ,[OpeningNumber],[ShiftId],[ShiftSchedule],[Supervisor],[HoursPerWeek],[Note],[RequireSafeEquipment],[RequireSafetyShoe]
--        ,[IsInternalPosting],[IsPublished],[IsHot],[IsDeleted],[RecruiterId],[OwnerId],[FranchiseId],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
--     VALUES
--        (3,NEWID(),88113,20,10,25,NULL
--        ,'Test Schedule 3','Test Schedule 3',NULL,NULL,NULL,'2014-06-03 00:00:00.000','2014-06-03 00:00:00.000','2014-06-03 09:00:00.000','2014-06-03 17:00:00.000'
--        ,3,1,NULL,1,16,15
--        ,10,2,NULL,NULL,NULL,NULL,1,0
--        ,1,1,1,0,1,1,1,1,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')



--INSERT INTO [dbo].[JobOrder] ([Id],[JobOrderGuid],[CompanyId],[CompanyLocationId],[CompanyDepartmentId],[CompanyContactId],[CompanyJobNumber]
--        ,[JobTitle],[JobDescription],[HiringDurationExpiredDate],[EstimatedFinishingDate],[EstimatedMargin],[StartDate],[EndDate],[StartTime],[EndTime]
--        ,[SchedulePolicyId],[JobOrderTypeId],[Salary],[JobOrderStatusId],[JobOrderCategoryId],[CompanyBillingRateId]
--        ,[OpeningNumber],[ShiftId],[ShiftSchedule],[Supervisor],[HoursPerWeek],[Note],[RequireSafeEquipment],[RequireSafetyShoe]
--        ,[IsInternalPosting],[IsPublished],[IsHot],[IsDeleted],[RecruiterId],[OwnerId],[FranchiseId],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
--     VALUES
--        (4,NEWID(),88113,20,10,25,NULL
--        ,'Test Schedule 4','Test Schedule 4',NULL,NULL,NULL,'2014-06-03 00:00:00.000','2014-06-03 00:00:00.000','2014-06-03 09:00:00.000','2014-06-03 17:00:00.000'
--        ,14,1,NULL,1,16,15
--        ,10,2,NULL,NULL,NULL,NULL,1,0
--        ,1,1,1,0,1,1,1,1,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')


SET IDENTITY_INSERT [dbo].[JobOrder] OFF
GO



-- Night shift company


SET IDENTITY_INSERT [dbo].[JobOrder] ON

INSERT INTO [dbo].[JobOrder] ([Id],[JobOrderGuid],[CompanyId],[CompanyLocationId],[CompanyDepartmentId],[CompanyContactId],[CompanyJobNumber]
        ,[JobTitle],[JobDescription],[HiringDurationExpiredDate],[EstimatedFinishingDate],[EstimatedMargin],[StartDate],[EndDate],[StartTime],[EndTime]
        ,[SchedulePolicyId],[JobOrderTypeId],[Salary],[JobOrderStatusId],[JobOrderCategoryId],[CompanyBillingRateId]
        ,[OpeningNumber],[ShiftId],[ShiftSchedule],[Supervisor],[HoursPerWeek],[Note],[RequireSafeEquipment],[RequireSafetyShoe]
        ,[IsInternalPosting],[IsPublished],[IsHot],[IsDeleted],[RecruiterId],[OwnerId],[FranchiseId],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
     VALUES
        (11,NEWID(),88210,30,31,8845,NULL
        ,'Overnight','Overnight Shift 19:00 - 05:00',NULL,NULL,NULL,'2014-06-07 00:00:00.000','2014-06-08 00:00:00.000','2014-06-05 21:00:00.000','2014-06-03 05:00:00.000'
        ,22,1,NULL,1,16,16
        ,10,2,NULL,NULL,NULL,NULL,1,0
        ,1,1,1,0,1,1,1,1,'2014-06-03 16:09:20.533','2014-06-03 16:09:20.533')

SET IDENTITY_INSERT [dbo].[JobOrder] OFF
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------------

-- Tradition Fine Foods

SET IDENTITY_INSERT [dbo].[CandidateJobOrder] ON

INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (100, 67226, 1, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (101, 67227, 1, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (102, 67228, 1, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (103, 67229, 1, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (104, 67230, 1, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (105, 67231, 1, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (106, 67232, 1, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (107, 67234, 1, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (108, 67235, 1, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (109, 67236, 1, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (110, 67238, 1, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))


INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (120, 67226, 2, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (121, 67227, 2, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (122, 67228, 2, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (123, 67229, 2, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (124, 67230, 2, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (125, 67231, 2, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (126, 67232, 2, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (127, 67234, 2, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (128, 67235, 2, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (129, 67236, 2, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (130, 67238, 2, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))



SET IDENTITY_INSERT [dbo].[CandidateJobOrder] OFF


-- Night shift company


SET IDENTITY_INSERT [dbo].[CandidateJobOrder] ON

INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (200, 67226, 11, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (201, 67227, 11, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (202, 67228, 11, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (203, 67229, 11, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (204, 67230, 11, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (205, 67231, 11, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (206, 67232, 11, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (208, 67234, 11, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (209, 67235, 11, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (210, 67236, 11, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))
INSERT [dbo].[CandidateJobOrder] ([Id],[CandidateId],[JobOrderId],[CandidateJobOrderStatusId],[RatingValue],[RatingComment],[HelpfulYesTotal],[HelpfulNoTotal],[RatedBy],[EnteredBy],[Note],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (211, 67238, 11, 12, 0, NULL, 0, 0, NULL, 1, NULL, CAST(0x0000A1950126A51A AS DateTime), CAST(0x0000A1950126ADF7 AS DateTime))


SET IDENTITY_INSERT [dbo].[CandidateJobOrder] OFF
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------------

-- Tradition Fine Foods

SET IDENTITY_INSERT [dbo].[CandidateJobOrderStatusHistory] ON

INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (100, 67226, 1, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (101, 67227, 1, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (102, 67228, 1, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (103, 67229, 1, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (104, 67230, 1, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (105, 67231, 1, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (106, 67232, 1, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (107, 67234, 1, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (108, 67235, 1, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (109, 67236, 1, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (110, 67238, 1, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')

INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (120, 67226, 2, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (121, 67227, 2, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (122, 67228, 2, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (123, 67229, 2, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (124, 67230, 2, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (125, 67231, 2, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (126, 67232, 2, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (127, 67234, 2, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (128, 67235, 2, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (129, 67236, 2, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (130, 67238, 2, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')


SET IDENTITY_INSERT [dbo].[CandidateJobOrderStatusHistory] OFF
GO


-- Night shift company


SET IDENTITY_INSERT [dbo].[CandidateJobOrderStatusHistory] ON

INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (200, 67226, 11, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (201, 67227, 11, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (202, 67228, 11, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (203, 67229, 11, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (204, 67230, 11, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (205, 67231, 11, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (206, 67232, 11, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (208, 67234, 11, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (209, 67235, 11, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (210, 67236, 11, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')
INSERT [dbo].[CandidateJobOrderStatusHistory] ([Id],[CandidateId],[JobOrderId],[StatusFrom],[StatusTo],[Note],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
VALUES (211, 67238, 11, 1, 12, NULL, 1, '2014-06-03 00:00:00.000', '2014-06-03 00:00:00.000')


SET IDENTITY_INSERT [dbo].[CandidateJobOrderStatusHistory] OFF
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------------

