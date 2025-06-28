-- 2023_11_12_LocaleStringResource_Client_Down
USE [Workforce3]
GO

Delete from [dbo].[LocaleStringResource]
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


