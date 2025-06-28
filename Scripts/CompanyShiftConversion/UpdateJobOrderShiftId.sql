declare @CompanyId int = 230
declare @Map table (FromShiftId int, ToShiftId int)

-- identify all the distinct ShiftId, from JobOrder
--select distinct jo.Shiftid, s.ShiftName from JobOrder as jo
--join Shift as s on s.Id = jo.ShiftId
--where jo.CompanyId = @CompanyId

-- identify all distinct ShiftId, from Shift
--select * from Shift where CompanyId = @CompanyId

-- add mapping, for Day shift
declare @DayShiftId int = (select Id from Shift where CompanyId = @CompanyId and ShiftName = 'Day' and IsActive = 1)
if @DayShiftId is not null
begin
	insert into @Map
		select Id, @DayShiftId from Shift where CompanyId = 0 and ShiftName in ('Any', 'Morning', 'D', 'Day')
	--or add one by one
	--insert into @Map values(?, @DayShiftId)
end

-- add mapping, for Afteroon shift
declare @AfternoonShiftId int = (select Id from Shift where CompanyId = @CompanyId and ShiftName = 'Afternoon' and IsActive = 1)
if @AfternoonShiftId is not null
begin
	insert into @Map
		select Id, @AfternoonShiftId from shift where CompanyId = 0 and ShiftName in ('Afternoon', 'A', 'Evening')
	--or add one by one
	--insert into @Map values(?, @AfternoonShiftId)
end

-- add mapping, for Night shift
declare @NightShiftId int = (select Id from Shift where CompanyId = @CompanyId and ShiftName = 'Night' and IsActive = 1)
if @NightShiftId is not null
begin
	insert into @Map
		select Id, @NightShiftId from shift where CompanyId = 0 and ShiftName in ('Night', 'N')
	--or add one by one
	--insert into @Map values(?, @NightShiftId)
end

-- any other shifts???
--

-- check all mapping
--select * from @Map

-- update JobOrder
update JobOrder set ShiftId = ToShiftId--, Note = FromShiftId
from JobOrder as jo
join @Map as m on jo.ShiftId = m.FromShiftId
Where CompanyId = @CompanyId
