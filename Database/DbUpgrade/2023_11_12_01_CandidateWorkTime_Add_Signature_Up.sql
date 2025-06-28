-- 2023_11_12_CandidateWorkTime_Add_Signature_Up
USE [Workforce3]
GO

ALTER TABLE [Workforce3].[dbo].[CandidateWorkTime] 
ADD [SignatureBy] Int Not NULL, 
	CONSTRAINT DF_CandidateWorkTime_SignatureBy DEFAULT 0 FOR SignatureBy;

ALTER TABLE [Workforce3].[dbo].[CandidateWorkTime] 
ADD [SignatureByName] nvarchar(max) null, [SignatureOnUtc] datetime null;

GO
