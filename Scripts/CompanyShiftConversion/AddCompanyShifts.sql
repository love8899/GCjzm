declare @CompanyId int = 230
declare @DefaultMinStartTime time = '07:00'
declare @DefaultMaxEndTime time = '18:00'

-- cleanup
--delete Shift where companyId = @CompanyId

-- insert shifts, per ShiftCode from billing rates
INSERT INTO [dbo].[Shift]
           ([ShiftName]
           ,[Description]
           ,[IsActive]
           ,[IsDeleted]
           ,[EnteredBy]
           ,[DisplayOrder]
           ,[CreatedOnUtc]
           ,[UpdatedOnUtc]
           ,[StartTimeOfDayTicks]
           ,[LengthInHours]
           ,[EnableInRegistration]
           ,[CompanyId]
           ,[MinStartTime]
           ,[MaxEndTime])
     select distinct ShiftCode
           ,null
           ,1
           ,0
           ,0
           ,0
           ,GETUTCDATE()
           ,GETUTCDATE()
           ,null
           ,null
           ,0
           ,@CompanyId
           ,@DefaultMinStartTime
           ,@DefaultMaxEndTime
	from CompanyBillingRates where CompanyId = @CompanyId

--
-- Update all shifts to make sure the start/end times are correct
--
