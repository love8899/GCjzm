---
--- Drop all indexes
---
declare @qry nvarchar(max);
select @qry = 
    (SELECT  'DROP INDEX ' + ix.name + ' ON ' + OBJECT_NAME(ID) + '; '
     FROM  sysindexes ix
     WHERE   ix.Name IS NOT null and ix.Name like '_dta_index_%'
     for xml path(''));
exec sp_executesql @qry




---
--- Drop all statistics
---
WHILE EXISTS (SELECT * FROM sys.stats i WHERE OBJECTPROPERTY(i.[object_id],'IsUserTable') = 1 AND i.[name] LIKE '_dta_%' and user_created = 1)
BEGIN
    DECLARE @sql varchar(max)
    SELECT @sql = 'drop statistics [' + object_name(i.[object_id]) + '].['+ i.[name] + ']'
    FROM sys.stats i
    WHERE
    OBJECTPROPERTY(i.[object_id],'IsUserTable') = 1 AND
    i.[name] LIKE '_dta_%' and user_created = 1
    SELECT @sql
    EXEC (@sql)
END







--CREATE NONCLUSTERED INDEX [IX_LocaleStringResource] ON [LocaleStringResource] ([ResourceName] ASC,  [LanguageId] ASC)
--GO


--CREATE NONCLUSTERED INDEX [IX_Company_CompanyName] ON [Company] ([CompanyName] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_CompanyBillingRates_CompanyId] ON [CompanyBillingRates] ([CompanyId] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_CompanyLocation_CompanyId] ON [CompanyLocation] ([CompanyId] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_CompanyDepartment_CompanyId] ON [CompanyDepartment] ([CompanyId] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_CompanyClockDevice_CompanyLocationId] ON [CompanyClockDevice] ([CompanyLocationId] ASC)
--GO


--CREATE NONCLUSTERED INDEX [IX_Account_Username] ON [Account] ([Username] ASC)
--GO


--CREATE NONCLUSTERED INDEX [IX_Candidate_Email] ON [Candidate] ([Email] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_Candidate_Username] ON [Candidate] ([Username] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_Candidate_FirstName] ON [Candidate] ([FirstName] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_Candidate_LastName] ON [Candidate] ([LastName] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_Candidate_MobilePhone] ON [Candidate] ([MobilePhone] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_Candidate_HomePhone] ON [Candidate] ([HomePhone] ASC)
--GO


--CREATE NONCLUSTERED INDEX [IX_CandidateKeySkills_CandidateId] ON [CandidateKeySkills] ([CandidateId] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_CandidateKeySkills_KeySkill] ON [CandidateKeySkills] ([KeySkill] ASC)
--GO


--CREATE NONCLUSTERED INDEX [IX_CandidateAttachment_CandidateId] ON [CandidateAttachment] ([CandidateId] ASC)
--GO


--CREATE NONCLUSTERED INDEX [IX_CandidateAddress_CandidateId] ON [CandidateAddress] ([CandidateId] ASC)
--GO


--CREATE NONCLUSTERED INDEX [IX_JobOrder_CompanyId] ON [JobOrder] ([CompanyId] ASC)
--GO


--CREATE NONCLUSTERED INDEX [IX_CandidateJobOrder] ON [CandidateJobOrder] ([CandidateId] ASC,  [JobOrderId] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_CandidateJobOrderStatusHistory] ON [CandidateJobOrderStatusHistory] ([CandidateId] ASC,  [JobOrderId] ASC)
--GO


--CREATE NONCLUSTERED INDEX [IX_GenericAttribute_EntityId_and_KeyGroup] ON [GenericAttribute] ([EntityId] ASC, [KeyGroup] ASC)
--GO


--CREATE NONCLUSTERED INDEX [IX_QueuedEmail_CreatedOnUtc] ON [QueuedEmail] ([CreatedOnUtc] ASC)
--GO



--CREATE NONCLUSTERED INDEX [IX_Language_DisplayOrder] ON [Language] ([DisplayOrder] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_BlogPost_LanguageId] ON [BlogPost] ([LanguageId] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_BlogComment_BlogPostId] ON [BlogComment] ([BlogPostId] ASC)
--GO


--CREATE NONCLUSTERED INDEX [IX_Forums_Group_DisplayOrder] ON [Forums_Group] ([DisplayOrder] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_Forums_Forum_DisplayOrder] ON [Forums_Forum] ([DisplayOrder] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_Forums_Forum_ForumGroupId] ON [Forums_Forum] ([ForumGroupId] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_Forums_Topic_ForumId] ON [Forums_Topic] ([ForumId] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_Forums_Post_TopicId] ON [Forums_Post] ([TopicId] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_Forums_Post_AccountId] ON [Forums_Post] ([AccountId] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_ForumId] ON [Forums_Subscription] ([ForumId] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_TopicId] ON [Forums_Subscription] ([TopicId] ASC)
--GO



--CREATE NONCLUSTERED INDEX [IX_AccessLog_CreatedOnUtc] ON [AccessLog] ([CreatedOnUtc] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_ActivityLog_CreatedOnUtc] ON [ActivityLog] ([CreatedOnUtc] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_Log_CreatedOnUtc] ON [Log] ([CreatedOnUtc] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_MessageHistory_CreatedOnUtc] ON [MessageHistory] ([CreatedOnUtc] ASC)
--GO



--CREATE NONCLUSTERED INDEX [IX_Country_DisplayOrder] ON [Country] ([DisplayOrder] ASC)
--GO
--CREATE NONCLUSTERED INDEX [IX_StateProvince_CountryId] ON [StateProvince] ([CountryId]) INCLUDE ([DisplayOrder])
--GO
--CREATE NONCLUSTERED INDEX [IX_City_StateProvinceId] ON [City] ([StateProvinceId]) INCLUDE ([DisplayOrder])
--GO


