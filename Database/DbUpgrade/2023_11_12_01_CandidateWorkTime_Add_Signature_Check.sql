-- 2023_11_12_CandidateWorkTime_Add_Signature_Check
USE [Workforce3]
GO

Select top 1 [SignatureBy], [SignatureByName], [SignatureOnUtc]
From [Workforce3].[dbo].[CandidateWorkTime] 


GO


