use [Workforce3]
go

SET ANSI_PADDING ON

go

CREATE NONCLUSTERED INDEX [_dta_index_CandidateAttachment_8_1109578991__K15D_1_2_3_4_5_6_7_8_9_10_11_12_13_14] ON [dbo].[CandidateAttachment]
(
	[UpdatedOnUtc] DESC
)
INCLUDE ( 	[Id],
	[CandidateId],
	[AttachmentTypeId],
	[AttachmentName],
	[OriginalFileName],
	[StoredFileName],
	[StoredPath],
	[ContentType],
	[ContentText],
	[FileSizeInKB],
	[Note],
	[IsActive],
	[IsDeleted],
	[CreatedOnUtc]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

CREATE NONCLUSTERED INDEX [_dta_index_CandidateAttachment_8_1109578991__K12] ON [dbo].[CandidateAttachment]
(
	[IsActive] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

SET ANSI_PADDING ON

go

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
	[OpeningNumber],
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
	[CreatedOnUtc]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

CREATE NONCLUSTERED INDEX [_dta_index_JobOrder_8_946102411__K31] ON [dbo].[JobOrder]
(
	[IsInternalPosting] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

CREATE NONCLUSTERED INDEX [_dta_index_CandidateKeySkills_8_1429580131__K6] ON [dbo].[CandidateKeySkills]
(
	[IsDeleted] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

CREATE NONCLUSTERED INDEX [_dta_index_CandidateAddress_8_1045578763__K16] ON [dbo].[CandidateAddress]
(
	[IsActive] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

