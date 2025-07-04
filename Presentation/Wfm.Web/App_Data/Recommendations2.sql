use [Workforce3]
go

CREATE NONCLUSTERED INDEX [_dta_index_Candidate_8_981578535__K29_K28] ON [dbo].[Candidate]
(
	[IsDeleted] ASC,
	[IsBanned] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

CREATE NONCLUSTERED INDEX [_dta_index_Candidate_8_981578535__K29] ON [dbo].[Candidate]
(
	[IsDeleted] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

CREATE STATISTICS [_dta_stat_946102411_31] ON [dbo].[JobOrder]([IsInternalPosting])
go

