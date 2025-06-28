--------------------------------------------
-- update from one Table to another based on a ID match
--------------------------------------------


--------------------------------------------
UPDATE
    CandidateClockTime
SET
    CandidateClockTime.CompanyLocationId = CD.CompanyLocationId
FROM
    CandidateClockTime ct
INNER JOIN
    CompanyClockDevice CD
ON 
    ct.ClockDeviceUid = CD.ClockDeviceUid

GO

--------------------------------------------
UPDATE
    CandidateClockTime
SET
    CandidateClockTime.CompanyId = CL.CompanyId
FROM
    CandidateClockTime ct
INNER JOIN
    CompanyLocation CL
ON 
    ct.CompanyLocationId = CL.Id

GO

--------------------------------------------
UPDATE
    CandidateClockTime
SET
    CandidateClockTime.CompanyName = C.CompanyName
FROM
    CandidateClockTime ct
INNER JOIN
    Company C
ON 
    ct.CompanyId = C.Id

GO

--------------------------------------------
UPDATE
    CandidateClockTime
SET
    CandidateClockTime.CandidateId = SC.CandidateId
FROM
    CandidateClockTime ct
INNER JOIN
    CandidateSmartCard SC
ON 
    ct.SmartCardUid = SC.SmartCardUid
    
GO

--------------------------------------------
UPDATE
    CandidateClockTime
SET
    CandidateClockTime.CandidateFirstName = C.FirstName
FROM
    CandidateClockTime ct
INNER JOIN
    Candidate C
ON 
    ct.CandidateId = C.Id
    
GO

--------------------------------------------
UPDATE
    CandidateClockTime
SET
    CandidateClockTime.CandidateLastName = C.LastName
FROM
    CandidateClockTime ct
INNER JOIN
    Candidate C
ON 
    ct.CandidateId = C.Id
    
GO

--------------------------------------------
-- [JobOrder] - Reset Job order start/end date
--------------------------------------------
SELECT * from JobOrder where cast(StartTime as time) < cast(EndDate as time)
SELECT DATEADD(dd, 0, DATEDIFF(dd, 0, StartDate)), DATEADD(dd, 1, DATEDIFF(dd, 0, StartDate)) from JobOrder

update JobOrder set EndDate = DATEADD(dd, 0, DATEDIFF(dd, 0, StartDate)) where cast(StartTime as time) <= cast(EndDate as time)
update JobOrder set EndDate = DATEADD(dd, 1, DATEDIFF(dd, 0, StartDate)) where cast(StartTime as time) > cast(EndDate as time)


--------------------------------------------
-- [Candidate] - Reset Employee ID
--------------------------------------------
UPDATE [dbo].[Candidate]
    SET EmployeeId = 'GCEMP' + CONVERT(VARCHAR(2),[CreatedOnUtc],11)+'-' + REPLICATE('0', 9 - DATALENGTH(CAST(Id AS VARCHAR(10)) )) + CAST(Id AS VARCHAR(10))

--------------------------------------------
-- [Candidate] - Update Username
--------------------------------------------
update Candidate set Username = Email
GO

-- capitalize first letter only
--------------------------------------------
CREATE FUNCTION [dbo].[InitCap] ( @InputString varchar(4000) )
RETURNS VARCHAR(4000)
AS
BEGIN

DECLARE @Index          INT
DECLARE @Char           CHAR(1)
DECLARE @PrevChar       CHAR(1)
DECLARE @OutputString   VARCHAR(255)

SET @OutputString = LOWER(@InputString)
SET @Index = 1

WHILE @Index <= LEN(@InputString)
BEGIN
    SET @Char     = SUBSTRING(@InputString, @Index, 1)
    SET @PrevChar = CASE WHEN @Index = 1 THEN ' '
                         ELSE SUBSTRING(@InputString, @Index - 1, 1)
                    END

    IF @PrevChar IN (' ', ';', ':', '!', '?', ',', '.', '_', '-', '/', '&', '''', '(')
    BEGIN
        IF @PrevChar != '''' OR UPPER(@Char) != 'S'
            SET @OutputString = STUFF(@OutputString, @Index, 1, UPPER(@Char))
    END

    SET @Index = @Index + 1
END

RETURN @OutputString

END
GO

--------------------------------------------
update Candidate set [FirstName] = dbo.InitCap([FirstName])
GO
update Candidate set [LastName] = dbo.InitCap([LastName])
GO



drop function dbo.InitCap
GO

--------------------------------------------
-- [Candidate] - Extract Numbers
----------------------------------------------------------------------------------
drop function dbo.ExtractNumbers
GO
----------------------------------------------------------------------------------
-- Extract only numbers from a String
create function dbo.ExtractNumbers (@s varchar(max)) returns varchar(max)
   --with schemabinding
begin
	declare @result varchar(max)
	set @result=''
	select
		@result=@result+case when number like '[0-9]' then number else '' end from
		(
			 select substring(@s,number,1) as number from
			(
				select number from master..spt_values where type='p' and number between 1 and len(@s)
			) as t
		) as t

	return @result
end
GO

----------------------------------------------------------------------------------
update Candidate set [HomePhone] = dbo.ExtractNumbers([HomePhone])
GO
update Candidate set [MobilePhone] = dbo.ExtractNumbers([MobilePhone])
GO
update Candidate set [EmergencyPhone] = dbo.ExtractNumbers([EmergencyPhone])
GO
update Candidate set [SocialInsuranceNumber] = dbo.ExtractNumbers([SocialInsuranceNumber])
GO


-- Format SocialInsuranceNumber
--
--update Candidate set [SocialInsuranceNumber] = SUBSTRING([SocialInsuranceNumber], 1, 3) + ' ' + SUBSTRING([SocialInsuranceNumber], 4, 3) + ' ' + SUBSTRING([SocialInsuranceNumber], 7, 3)
--GO
----------------------------------------------------------------------------------
drop function dbo.ExtractNumbers
GO
----------------------------------------------------------------------------------



--------------------------------------------
-- [Candidate] - Clean up ContentText
--------------------------------------------

----------------------------------------------------------------------------------
drop function dbo.RemoveSpecialChars
GO
----------------------------------------------------------------------------------

-- Removes special characters from a string value.
-- All characters except space, 0-9, a-z and A-Z are removed and
-- the remaining characters are returned.
-- Author: Christian d'Heureuse, www.source-code.biz
create function dbo.RemoveSpecialChars (@s varchar(max)) returns varchar(max)
   with schemabinding
begin
   if @s is null
      return null
   declare @s2 varchar(max)
   set @s2 = ''
   declare @l int
   set @l = len(@s)
   declare @p int
   set @p = 1
   while @p <= @l
   begin
      declare @c int
      set @c = ascii(substring(@s, @p, 1))
      if @c between 48 and 57 or @c between 65 and 90 or @c between 97 and 122 or @c = 32
         set @s2 = @s2 + char(@c)
      else
         set @s2 = @s2 + char(32)
      set @p = @p + 1
   end
   if len(@s2) = 0
      return null

   -- trim spaces
   set @s2 = LTRIM(RTRIM(@s2))
   -- Replace duplicate spaces with a single space
   set @s2 = replace(replace(replace(@s2,' ','<>'),'><',''),'<>',' ')

   return @s2
end

GO

------------------------------------------------------------------------------------
update CandidateAttachment set ContentText = dbo.RemoveSpecialChars(ContentText)
GO
------------------------------------------------------------------------------------
drop function dbo.RemoveSpecialChars
GO
------------------------------------------------------------------------------------
