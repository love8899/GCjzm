-- 2023_12_01_16_Set_SignatureCompanies_Check
USE [Workforce3]
GO

Declare @currentUtcDatetime Datetime = GETUTCDATE();

select * from [dbo].[Setting] 
where name like 'SignatureCompanies';

GO


