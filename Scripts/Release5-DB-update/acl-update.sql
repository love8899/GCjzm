
-- remove some permissions for all acocunts

delete PermissionRecord_Role_Mapping
where PermissionRecord_Id in (
	select Id
	from PermissionRecord
	where SystemName in
	(
		'ManageCandidateBankAccounts',
		'ManageClientJobPosting',
		'ManageClientScheduling'
	)
)



-- others

declare @AdministratorRoleId int
declare @PayrollAdministratorRoleId int
declare @RecruiterSupervisorsRoleId int
declare @RecruitersRoleId int
declare @VendorAdminRoleId int
declare @VendorRecruiterSupervisorsRoleId int
declare @VendorRecruitersRoleId int
declare @HrManagersRoleId int

select top 1 @AdministratorRoleId = Id from AccountRole where SystemName = 'Administrators'
select top 1 @PayrollAdministratorRoleId = Id from AccountRole where SystemName = 'PayrollAdministrators'
select top 1 @RecruiterSupervisorsRoleId = Id from AccountRole where SystemName = 'RecruiterSupervisors'
select top 1 @RecruitersRoleId = Id from AccountRole where SystemName = 'Recruiters'
select top 1 @VendorAdminRoleId = Id from AccountRole where SystemName = 'VendorAdministrators'
select top 1 @VendorRecruiterSupervisorsRoleId = Id from AccountRole where SystemName = 'VendorRecruiterSupervisors'
select top 1 @VendorRecruitersRoleId = Id from AccountRole where SystemName = 'VendorRecruiters'
select top 1 @HrManagersRoleId = Id from AccountRole where SystemName = 'HrManagers'


declare @AdminReportsPermissionId int
select top 1 @AdminReportsPermissionId = Id from PermissionRecord where SystemName = 'AdminReports'

--insert into PermissionRecord_Role_Mapping
--select @AdminReportsPermissionId, Id from AccountRole where Id in (@RecruitersRoleId, @VendorAdminRoleId, @VendorRecruiterSupervisorsRoleId, @VendorRecruitersRoleId)

if not exists (select 1 from PermissionRecord_Role_Mapping where PermissionRecord_Id = @AdminReportsPermissionId and AccountRole_Id = @RecruitersRoleId)
	insert into PermissionRecord_Role_Mapping
		select @AdminReportsPermissionId, Id from AccountRole where Id = @RecruitersRoleId

if not exists (select 1 from PermissionRecord_Role_Mapping where PermissionRecord_Id = @AdminReportsPermissionId and AccountRole_Id = @VendorAdminRoleId)
	insert into PermissionRecord_Role_Mapping
		select @AdminReportsPermissionId, Id from AccountRole where Id = @VendorAdminRoleId

if not exists (select 1 from PermissionRecord_Role_Mapping where PermissionRecord_Id = @AdminReportsPermissionId and AccountRole_Id = @VendorRecruiterSupervisorsRoleId)
	insert into PermissionRecord_Role_Mapping
		select @AdminReportsPermissionId, Id from AccountRole where Id = @VendorRecruiterSupervisorsRoleId

if not exists (select 1 from PermissionRecord_Role_Mapping where PermissionRecord_Id = @AdminReportsPermissionId and AccountRole_Id = @VendorRecruitersRoleId)
	insert into PermissionRecord_Role_Mapping
		select @AdminReportsPermissionId, Id from AccountRole where Id = @VendorRecruitersRoleId


declare @AllowResetCandidatePasswordPermissionId int
select top 1 @AllowResetCandidatePasswordPermissionId = Id from PermissionRecord where SystemName = 'AllowResetCandidatePassword'

--insert into PermissionRecord_Role_Mapping
--select @AllowResetCandidatePasswordPermissionId, Id from AccountRole where Id in (@RecruitersRoleId, @VendorAdminRoleId, @VendorRecruiterSupervisorsRoleId, @VendorRecruitersRoleId)

if not exists (select 1 from PermissionRecord_Role_Mapping where PermissionRecord_Id = @AllowResetCandidatePasswordPermissionId and AccountRole_Id = @RecruitersRoleId)
	insert into PermissionRecord_Role_Mapping
		select @AllowResetCandidatePasswordPermissionId, Id from AccountRole where Id = @RecruitersRoleId

if not exists (select 1 from PermissionRecord_Role_Mapping where PermissionRecord_Id = @AllowResetCandidatePasswordPermissionId and AccountRole_Id = @VendorAdminRoleId)
	insert into PermissionRecord_Role_Mapping
		select @AllowResetCandidatePasswordPermissionId, Id from AccountRole where Id = @VendorAdminRoleId

if not exists (select 1 from PermissionRecord_Role_Mapping where PermissionRecord_Id = @AllowResetCandidatePasswordPermissionId and AccountRole_Id = @VendorRecruiterSupervisorsRoleId)
	insert into PermissionRecord_Role_Mapping
		select @AllowResetCandidatePasswordPermissionId, Id from AccountRole where Id = @VendorRecruiterSupervisorsRoleId

if not exists (select 1 from PermissionRecord_Role_Mapping where PermissionRecord_Id = @AllowResetCandidatePasswordPermissionId and AccountRole_Id = @VendorRecruitersRoleId)
	insert into PermissionRecord_Role_Mapping
		select @AllowResetCandidatePasswordPermissionId, Id from AccountRole where Id = @VendorRecruitersRoleId


declare @ViewContactsPermissionId int
select top 1 @ViewContactsPermissionId = Id from PermissionRecord where SystemName = 'ViewContacts'

--insert into PermissionRecord_Role_Mapping
--select @ViewContactsPermissionId, Id from AccountRole where Id in (@VendorAdminRoleId, @VendorRecruiterSupervisorsRoleId, @VendorRecruitersRoleId)

if not exists (select 1 from PermissionRecord_Role_Mapping where PermissionRecord_Id = @ViewContactsPermissionId and AccountRole_Id = @VendorAdminRoleId)
	insert into PermissionRecord_Role_Mapping
		select @ViewContactsPermissionId, Id from AccountRole where Id = @VendorAdminRoleId

if not exists (select 1 from PermissionRecord_Role_Mapping where PermissionRecord_Id = @ViewContactsPermissionId and AccountRole_Id = @VendorRecruiterSupervisorsRoleId)
	insert into PermissionRecord_Role_Mapping
		select @ViewContactsPermissionId, Id from AccountRole where Id = @VendorRecruiterSupervisorsRoleId

if not exists (select 1 from PermissionRecord_Role_Mapping where PermissionRecord_Id = @ViewContactsPermissionId and AccountRole_Id = @VendorRecruitersRoleId)
	insert into PermissionRecord_Role_Mapping
		select @ViewContactsPermissionId, Id from AccountRole where Id = @VendorRecruitersRoleId


declare @ViewCandidateSINPermissionId int
select top 1 @ViewCandidateSINPermissionId = Id from PermissionRecord where SystemName = 'ViewCandidateSIN'

--insert into PermissionRecord_Role_Mapping
--select @ViewCandidateSINPermissionId, Id from AccountRole where Id in (@VendorAdminRoleId, @VendorRecruiterSupervisorsRoleId)

if not exists (select 1 from PermissionRecord_Role_Mapping where PermissionRecord_Id = @ViewCandidateSINPermissionId and AccountRole_Id = @VendorAdminRoleId)
	insert into PermissionRecord_Role_Mapping
		select @ViewCandidateSINPermissionId, Id from AccountRole where Id = @VendorAdminRoleId

if not exists (select 1 from PermissionRecord_Role_Mapping where PermissionRecord_Id = @ViewCandidateSINPermissionId and AccountRole_Id = @VendorRecruiterSupervisorsRoleId)
	insert into PermissionRecord_Role_Mapping
		select @ViewCandidateSINPermissionId, Id from AccountRole where Id = @VendorRecruiterSupervisorsRoleId


declare @ClientReportsPermissionId int
select top 1 @ClientReportsPermissionId = Id from PermissionRecord where SystemName = 'ClientReports'

if not exists (select 1 from PermissionRecord_Role_Mapping where PermissionRecord_Id = @ClientReportsPermissionId and AccountRole_Id = @HrManagersRoleId)
	insert into PermissionRecord_Role_Mapping
		select @ClientReportsPermissionId, Id from AccountRole where Id in (@HrManagersRoleId)
