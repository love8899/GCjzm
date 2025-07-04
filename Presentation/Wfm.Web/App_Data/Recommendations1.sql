use [Workforce3]
go

SET ANSI_PADDING ON

go

CREATE NONCLUSTERED INDEX [_dta_index_Candidate_8_981578535__K29_K53_1_2_3_4_5_6_7_8_9_10_11_12_13_14_15_16_17_18_19_20_21_22_23_24_25_26_27_28_30_31_32_] ON [dbo].[Candidate]
(
	[IsDeleted] ASC,
	[UpdatedOnUtc] ASC
)
INCLUDE ( 	[Id],
	[CandidateGuid],
	[Username],
	[Password],
	[PasswordFormatId],
	[PasswordSalt],
	[EmployeeId],
	[Email],
	[Email2],
	[SalutationId],
	[GenderId],
	[EthnicTypeId],
	[VetranTypeId],
	[SourceId],
	[FirstName],
	[LastName],
	[MiddleName],
	[HomePhone],
	[MobilePhone],
	[EmergencyPhone],
	[BirthDate],
	[SocialInsuranceNumber],
	[WebSite],
	[BestTimetoCall],
	[DisabilityStatus],
	[IsHot],
	[IsActive],
	[IsBanned],
	[Entitled],
	[CanRelocate],
	[JobTitle],
	[Education],
	[Education2],
	[DateAvailable],
	[CurrentEmployer],
	[CurrentPay],
	[DesiredPay],
	[ShiftId],
	[TransportationId],
	[MajorIntersection1],
	[MajorIntersection2],
	[PreferredWorkLocation],
	[Note],
	[LastIpAddress],
	[LastLoginDateUtc],
	[LastActivityDateUtc],
	[EnteredBy],
	[OwnerId],
	[FranchiseId],
	[SearchKeys],
	[CreatedOnUtc]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

CREATE STATISTICS [_dta_stat_981578535_53_29] ON [dbo].[Candidate]([UpdatedOnUtc], [IsDeleted])
go

CREATE STATISTICS [_dta_stat_981578535_28_29_53] ON [dbo].[Candidate]([IsBanned], [IsDeleted], [UpdatedOnUtc])
go

SET ANSI_PADDING ON

go

CREATE NONCLUSTERED INDEX [_dta_index_CandidateAddress_8_1045578763__K21D_1_2_3_4_5_6_7_8_9_10_11_12_13_14_15_16_17_18_19_20] ON [dbo].[CandidateAddress]
(
	[UpdatedOnUtc] DESC
)
INCLUDE ( 	[Id],
	[CandidateId],
	[AddressTypeId],
	[AddressName],
	[UnitNumber],
	[AddressLine1],
	[AddressLine2],
	[AddressLine3],
	[AddressLine4],
	[AddressLine5],
	[City],
	[StateProvince],
	[Country],
	[PostalCode],
	[Note],
	[IsActive],
	[IsDeleted],
	[EnteredBy],
	[DisplayOrder],
	[CreatedOnUtc]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

CREATE STATISTICS [_dta_stat_1045578763_16] ON [dbo].[CandidateAddress]([IsActive])
go

SET ANSI_PADDING ON

go

CREATE NONCLUSTERED INDEX [_dta_index_CandidateKeySkills_8_1429580131__K8D_K5D_1_2_3_4_6_7_9] ON [dbo].[CandidateKeySkills]
(
	[CreatedOnUtc] DESC,
	[LastUsedDate] DESC
)
INCLUDE ( 	[Id],
	[CandidateId],
	[KeySkill],
	[YearsOfExperience],
	[IsDeleted],
	[Note],
	[UpdatedOnUtc]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

CREATE STATISTICS [_dta_stat_1429580131_6] ON [dbo].[CandidateKeySkills]([IsDeleted])
go

