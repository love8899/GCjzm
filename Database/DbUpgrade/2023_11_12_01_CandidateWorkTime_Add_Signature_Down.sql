-- 2023_11_12_CandidateWorkTime_Add_Signature_Down
USE [Workforce3]
GO

alter table [Workforce3].[dbo].[CandidateWorkTime] drop constraint DF_CandidateWorkTime_SignatureBy;

ALTER TABLE [Workforce3].[dbo].[CandidateWorkTime] 
DROP COLUMN [SignatureBy], [SignatureByName], [SignatureOnUtc];


GO


