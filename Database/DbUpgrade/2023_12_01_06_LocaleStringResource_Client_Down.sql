-- 2023_12_01__06_LocaleStringResource_Client_Down
USE [Workforce3]
GO


delete from [dbo].[LocaleStringResource] where [ResourceName] in ('Admin.Common.ExportToPDF.Selected');

GO


