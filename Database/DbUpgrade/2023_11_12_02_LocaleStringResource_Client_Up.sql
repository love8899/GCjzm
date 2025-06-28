-- 2023_11_12_LocaleStringResource_Client_Up
USE [Workforce3]
GO

Declare @currentUtcDatetime Datetime = GETUTCDATE();

INSERT INTO [dbo].[LocaleStringResource] ([LanguageId],[ResourceName],[ResourceValue],[CreatedOnUtc],[UpdatedOnUtc])
	VALUES 
	(1,'Client.Account.Signature.Menu','Signature',@currentUtcDatetime,@currentUtcDatetime),
	(1,'Client.Account.Signature.Clear','Clear',@currentUtcDatetime,@currentUtcDatetime),
	(1,'Client.Account.Signature.Accept','Accept',@currentUtcDatetime,@currentUtcDatetime),
	(1,'Client.Account.Signature.DrawSignature','Draw Signature',@currentUtcDatetime,@currentUtcDatetime),
	(1,'Client.Account.Signature.AcceptedSignature','Accetped Signature',@currentUtcDatetime,@currentUtcDatetime),
	(1,'Client.Account.Signature.SavedSignature','Saved Signature',@currentUtcDatetime,@currentUtcDatetime),
	(1,'Client.Account.Signature.Title','Signature Pad',@currentUtcDatetime,@currentUtcDatetime)

GO


