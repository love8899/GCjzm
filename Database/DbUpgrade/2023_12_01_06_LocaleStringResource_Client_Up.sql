-- 2023_12_01_06_LocaleStringResource_Client_Up
USE [Workforce3]
GO

Declare @currentUtcDatetime Datetime = GETUTCDATE();

INSERT INTO [dbo].[LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc])
	VALUES 
	(1,'Admin.Common.ExportToPDF.Selected','Export To PDF (Selected)',@currentUtcDatetime,@currentUtcDatetime)

GO


