
-- add MSP
if not exists (select 1 from Franchise where FranchiseName = 'Tempus workforce')
	insert into Franchise (FranchiseGuid, FranchiseName, IsHot, IsActive, IsDeleted, OwnerId, EnteredBy, DisplayOrder, CreatedOnUtc, UpdatedOnUtc, IsSslEnabled, IsDefaultManagedServiceProvider, EnableStandAloneJobOrders)
		values (NEWID(), 'Tempus workforce', 0, 1, 0, 63, 63, 0, GETUTCDATE(), GETUTCDATE(), 0, 1, 0)


-- change GC to vendor
update Franchise set IsDefaultManagedServiceProvider = 0, UpdatedOnUtc = GETUTCDATE() where FranchiseName = 'Great Connections Employment Services Inc.'


-- move admin & payroll to MSP; move recuiters to vendor GC

declare @MspFranchiseId int
declare @GcFranchiseId int

select top 1 @MspFranchiseId = Id from Franchise where IsDefaultManagedServiceProvider = 1 and Id != 1
select top 1 @GcFranchiseId = Id from Franchise where FranchiseName = 'Great Connections Employment Services Inc.'

declare @AdministratorRoleId int
declare @PayrollAdministratorRoleId int
declare @RecruiterSupervisorsRoleId int
declare @RecruitersRoleId int
declare @VendorRecruiterSupervisorsRoleId int
declare @VendorRecruitersRoleId int

select top 1 @AdministratorRoleId = Id from AccountRole where SystemName = 'Administrators'
select top 1 @PayrollAdministratorRoleId = Id from AccountRole where SystemName = 'PayrollAdministrators'
select top 1 @RecruiterSupervisorsRoleId = Id from AccountRole where SystemName = 'RecruiterSupervisors'
select top 1 @RecruitersRoleId = Id from AccountRole where SystemName = 'Recruiters'
select top 1 @VendorRecruiterSupervisorsRoleId = Id from AccountRole where SystemName = 'VendorRecruiterSupervisors'
select top 1 @VendorRecruitersRoleId = Id from AccountRole where SystemName = 'VendorRecruiters'


delete Account where FirstName is null or LastName is null

delete a from Account_AccountRole_Mapping as a
inner join (
	select Account_Id, min(AccountRole_Id) as MinAccountRoleId from Account_AccountRole_Mapping
	group by Account_Id
	having count(1) > 1
) as c
on a.Account_Id = c.Account_Id and a.AccountRole_Id != c.MinAccountRoleId


update account set FranchiseId = @MspFranchiseId, UpdatedOnUtc = GETUTCDATE()
where FranchiseId = @GcFranchiseId
and Id in (select distinct Account_Id from Account_AccountRole_Mapping where AccountRole_Id in (@AdministratorRoleId, @PayrollAdministratorRoleId))

update account set IsLimitedToFranchises = 1, UpdatedOnUtc = GETUTCDATE()
where FranchiseId = @GcFranchiseId
and Id in (select distinct Account_Id from Account_AccountRole_Mapping where AccountRole_Id in (@RecruiterSupervisorsRoleId, @RecruitersRoleId))


update aam set AccountRole_Id = @VendorRecruiterSupervisorsRoleId
from Account_AccountRole_Mapping as aam
inner join account as a on a.Id = Account_Id
where a.FranchiseId = @GcFranchiseId and AccountRole_Id = @RecruiterSupervisorsRoleId

update aam set AccountRole_Id = @VendorRecruitersRoleId
from Account_AccountRole_Mapping as aam
inner join account as a on a.Id = Account_Id
where a.FranchiseId = @GcFranchiseId and AccountRole_Id = @RecruitersRoleId
