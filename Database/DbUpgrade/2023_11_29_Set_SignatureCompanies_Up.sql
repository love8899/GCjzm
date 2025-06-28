-- 2023_11_29_Set_SignatureCompanies_Up
USE [Workforce3]
GO

Declare @currentUtcDatetime Datetime = GETUTCDATE();

INSERT INTO [dbo].[Setting] ([Name],[Value],[FranchiseId],[EnteredBy],[CreatedOnUtc],[UpdatedOnUtc])
   VALUES ('SignatureCompanies', '495', 1, 63, @currentUtcDatetime, @currentUtcDatetime);

GO


