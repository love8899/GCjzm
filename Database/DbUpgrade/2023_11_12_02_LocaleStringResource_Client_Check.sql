-- 2023_11_12_LocaleStringResource_Client_Check
USE [Workforce3]
GO

Select [LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc]
from [dbo].[LocaleStringResource]
 where [ResourceName] in (
	'Client.Account.Signature.Menu',
	'Client.Account.Signature.Clear',
	'Client.Account.Signature.Accept',
	'Client.Account.Signature.DrawSignature',
	'Client.Account.Signature.AcceptedSignature',
	'Client.Account.Signature.SavedSignature',
	'Client.Account.Signature.Title'
	)

GO


