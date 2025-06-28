-- 2023_12_01_06_LocaleStringResource_Client_Check
USE [Workforce3]
GO

select [LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]
	from [dbo].[LocaleStringResource]
	where [ResourceName] in ('Admin.Common.ExportToPDF.Selected');

GO


