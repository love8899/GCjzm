-- 2023_11_29_Set_SignatureCompanies_Down
USE [Workforce3]
GO

Declare @currentUtcDatetime Datetime = GETUTCDATE();

Delete from [dbo].[Setting] where name like 'SignatureCompanies';

GO


