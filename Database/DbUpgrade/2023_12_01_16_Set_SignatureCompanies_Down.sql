-- 2023_12_01_16_Set_SignatureCompanies_Down
USE [Workforce3]
GO

Declare @currentUtcDatetime Datetime = GETUTCDATE();

update [dbo].[Setting] set [Value] = '495', [UpdatedOnUtc]= @currentUtcDatetime where name = 'SignatureCompanies';


GO


