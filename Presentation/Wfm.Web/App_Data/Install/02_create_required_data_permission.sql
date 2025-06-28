-- Permission
-- 
-- Uncomment below to reset Permissions and Roles
-- 
--DELETE FROM [Account_AccountRole_Mapping]
--DELETE FROM [PermissionRecord_Role_Mapping]
--DELETE FROM [AccountRole]
--DELETE FROM [PermissionRecord]
--GO
--

-- Clean up as basic PermissionRecord
--DELETE FROM [PermissionRecord] WHERE Id NOT IN (101, 501)


--   Define Permissions
-- ======================

SET IDENTITY_INSERT [dbo].[PermissionRecord] ON
----------------------------
-- Admin area
----------------------------
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (101, 'Access admin area', 'AccessAdminPanel', 'Standard', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (102, 'Admin area. Manage Companies', 'ManageCompanies', 'Company', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (103, 'Admin area. Manage Contacts', 'ManageContacts', 'Contact', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (104, 'Admin area. Manage Job Orders', 'ManageJobOrders', 'JobOrder', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (105, 'Admin area. Manage Franchises', 'ManageFranchises', 'Franchise', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (106, 'Admin area. Manage Candidates', 'ManageCandidates', 'Candidate', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (107, 'Admin area. Manage Dashboard', 'ManageDashboard', 'Dashboard', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (108, 'Admin area. Manage Calendar Events', 'ManageCalendarEvents', 'Calender', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (109, 'Admin area. Manage Profile', 'ManageProfile', 'Profile', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (110, 'Admin area. Manage Blog', 'ManageBlog', 'Blog', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (111, 'Admin area. Manage Configuration', 'ManageConfiguration', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (112, 'Admin area. Manage Accounts', 'ManageAccounts', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (113, 'Admin area. Manage System Log', 'ManageSystemLog', 'Logging', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (114, 'Admin area. Manage Countries', 'ManageCountries', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (115, 'Admin area. Manage Provinces', 'ManageStateProvinces', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (116, 'Admin area. Manage Cities', 'ManageCities', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (117, 'Admin area. Manage Local String Resource', 'ManageLocalStringResource', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (118, 'Admin area. Manage Candidate Smart Cards', 'ManageCandidateSmartCards', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (119, 'Admin area. Manage Company Clock Devices', 'ManageCompanyClockDevices', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (120, 'Admin area. Manage Message History', 'ManageMessageHistory', 'Logging', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (121, 'Admin area. Manage Languages', 'ManageLanguages', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (122, 'Admin area. Manage Job Categories', 'ManageJobCategories', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (123, 'Admin area. Manage Maintenance', 'ManageMaintenance', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (124, 'Admin area. Manage Access Log', 'ManageAccessLog', 'Logging', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (125, 'Admin area. Manage Activity Log', 'ManageActivityLog', 'Logging', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (126, 'Admin area. Manage ACL', 'ManageAcl', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (127, 'Admin area. Manage Tests', 'ManageTests', 'Test', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (128, 'Admin area. Manage Attachments', 'ManageAttachments', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (129, 'Admin area. Manage Plugins', 'ManagePlugins', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (130, 'Admin area. Manage Intersections', 'ManageIntersections', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (131, 'Admin area. Manage Message Templates', 'ManageMessageTemplates', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (132, 'Admin area. Manage Settings', 'ManageSettings', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (133, 'Admin area. Manage Shifts', 'ManageShifts', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (134, 'Admin area. Manage Skills', 'ManageSkills', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (135, 'Admin area. Manage Clock Times', 'ManageClockTimes', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (136, 'Admin area. Manage Candidate Work Time', 'ManageCandidateWorkTime', 'CandidateWorkTime', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (137, 'Admin area. Manage Candidate Time Sheet', 'ManageCandidateTimeSheet', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (138, 'Admin area. Manage Company Billings', 'ManageCompanyBillings', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (139, 'Admin area. Manage Policies', 'ManagePolicies', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (140, 'Admin area. Manage Payroll', 'ManagePayroll', 'Payroll', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (141, 'Admin area. Manage Company Locations', 'ManageCompanyLocations', 'Company', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (142, 'Admin area. Manage Company Departments', 'ManageCompanyDepartments', 'Company', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (143, 'Admin area. Manage Forums', 'ManageForums', 'Forum', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (144, 'Admin area. Manage Message Queue', 'ManageMessageQueue', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (145, 'Admin area. Manage Candidate Activity Log', 'ManageCandidateActivityLog', 'Logging', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (146, 'Admin area. Manage Banks', 'ManageBanks', 'Configuration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (147, 'Admin area. Manage Candidate Bank Accounts', 'ManageCandidateBankAccounts', 'Candidate', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (148, 'Admin area. Manage Widgets', 'ManageWidgets', 'Content Management', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (149, 'Admin area. Manage HTML Editor. Manage pictures', 'HtmlEditor.ManagePictures', 'Configuration', GETUTCDATE(), GETUTCDATE())

-- View only permission
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (301, 'Admin area. View Companies', 'ViewCompanies', 'Company', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (302, 'Admin area. View Contacts', 'ViewContacts', 'Contact', GETUTCDATE(), GETUTCDATE())


----------------------------
-- Client area
----------------------------
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (501, 'Access client area', 'AccessClientPanel', 'Standard', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (502, 'Client area. Manage Client Companies', 'ManageClientCompanies', 'ClientAdministration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (503, 'Client area. Manage Client Company Contacts', 'ManageClientCompanyContacts', 'ClientAdministration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (504, 'Client area. Manage Client Company Locations', 'ManageClientCompanyLocations', 'ClientAdministration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (505, 'Client area. Manage Client Company Departments', 'ManageClientCompanyDepartments', 'ClientAdministration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (506, 'Client area. Manage Client Profile', 'ManageClientProfile', 'ClientAdministration', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (507, 'Client area. Manage Client Company Job Orders', 'ManageClientCompanyJobOrders', 'ClientJobOrder', GETUTCDATE(), GETUTCDATE())
INSERT [dbo].[PermissionRecord]([Id],[Name],[SystemName],[Category],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (508, 'Client area. Manage Client Company Time Sheets', 'ManageClientCompanyTimeSheets', 'ClientTimeSheet', GETUTCDATE(), GETUTCDATE())

SET IDENTITY_INSERT [dbo].[PermissionRecord] OFF
GO


--   Define Roles
-- ==================
SET IDENTITY_INSERT [dbo].[AccountRole] ON
-- Admin Role
INSERT [dbo].[AccountRole] ([Id],[AccountRoleName],[Description],[SystemName],[IsClientRole],[ClientName],[Note],[IsActive],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (1, 'Administrators', 'Super Users who have full access to the system', 'Administrators', 0, NULL, NULL, 1, 1, 1, CAST(0x0000A20500A89FB0 AS DateTime), CAST(0x0000A20500A89FB0 AS DateTime))
INSERT [dbo].[AccountRole] ([Id],[AccountRoleName],[Description],[SystemName],[IsClientRole],[ClientName],[Note],[IsActive],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (2, 'Payroll Administrators', 'Payroll Administrators who have payroll access', 'PayrollAdministrators', 0, NULL, NULL, 1, 1, 1, CAST(0x0000A20500A89FB0 AS DateTime), CAST(0x0000A20500A89FB0 AS DateTime))
INSERT [dbo].[AccountRole] ([Id],[AccountRoleName],[Description],[SystemName],[IsClientRole],[ClientName],[Note],[IsActive],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (3, 'Recruiter Supervisors', 'Recruiter Supervisors who can also manage smart card', 'RecruiterSupervisors', 0, NULL, NULL, 1, 1, 1, CAST(0x0000A20500A89FB0 AS DateTime), CAST(0x0000A20500A89FB0 AS DateTime))
INSERT [dbo].[AccountRole] ([Id],[AccountRoleName],[Description],[SystemName],[IsClientRole],[ClientName],[Note],[IsActive],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (4, 'Recruiters', 'Recruiters', 'Recruiters', 0, NULL, NULL, 1, 1, 1, CAST(0x0000A20500A89FB0 AS DateTime), CAST(0x0000A20500A89FB0 AS DateTime))
-- Client Role
INSERT [dbo].[AccountRole] ([Id],[AccountRoleName],[Description],[SystemName],[IsClientRole],[ClientName],[Note],[IsActive],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (51, 'Company Administrators', 'Company Administrators', 'CompanyAdministrators', 1, N'ClientCompany', NULL, 1, 1, 1, CAST(0x0000A20500A89FB0 AS DateTime), CAST(0x0000A20500A89FB0 AS DateTime))
INSERT [dbo].[AccountRole] ([Id],[AccountRoleName],[Description],[SystemName],[IsClientRole],[ClientName],[Note],[IsActive],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (52, 'Location Managers', 'Company Location Managers', 'CompanyLocationManagers', 1, N'ClientCompany', NULL, 1, 1, 1, CAST(0x0000A20500A89FB0 AS DateTime), CAST(0x0000A20500A89FB0 AS DateTime))
INSERT [dbo].[AccountRole] ([Id],[AccountRoleName],[Description],[SystemName],[IsClientRole],[ClientName],[Note],[IsActive],[EnteredBy],[DisplayOrder],[CreatedOnUtc],[UpdatedOnUtc]) VALUES (53, 'Department Supervisors', 'Company Department Supervisors', 'CompanyDepartmentSupervisors', 1, N'ClientCompany', NULL, 1, 1, 1, CAST(0x0000A20500A89FB0 AS DateTime), CAST(0x0000A20500A89FB0 AS DateTime))

SET IDENTITY_INSERT [dbo].[AccountRole] OFF
GO



--------------------------------------------------------------------------------------------------
-- Reset User Role access
--------------------------------------------------------------------------------------------------
-- Clean up as basic permission
-- DELETE FROM PermissionRecord_Role_Mapping WHERE PermissionRecord_Id NOT IN (101, 501)

-- Admin Role
-- ==============================
-- Administrators Role Permission
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (101, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (102, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (103, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (104, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (105, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (106, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (107, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (108, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (109, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (110, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (111, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (112, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (113, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (114, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (115, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (116, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (117, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (118, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (119, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (120, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (121, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (122, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (123, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (124, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (125, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (126, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (127, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (128, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (129, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (130, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (131, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (132, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (133, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (134, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (135, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (136, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (137, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (138, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (139, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (140, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (141, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (142, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (143, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (144, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (145, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (146, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (147, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (148, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (149, 1)

INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (301, 1)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (302, 1)
GO

-- PayrollAdministrators Role
-- ==============================
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (101, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (102, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (103, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (104, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (105, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (106, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (107, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (108, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (109, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (118, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (135, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (136, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (137, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (138, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (139, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (140, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (141, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (142, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (146, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (147, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (301, 2)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (302, 2)
GO

-- Recruiters Supervisors Role
-- ==============================
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (101, 3)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (102, 3)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (103, 3)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (104, 3)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (106, 3)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (107, 3)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (108, 3)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (109, 3)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (118, 3)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (135, 3)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (136, 3)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (137, 3)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (141, 3)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (142, 3)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (301, 3)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (302, 3)
GO

-- Recruiters Role
-- ==============================
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (101, 4)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (104, 4)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (106, 4)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (107, 4)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (108, 4)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (109, 4)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (135, 4)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (136, 4)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (137, 4)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (301, 4)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (302, 4)
GO



-- Client Role
-- ==============================
-- CompanyAdministrators Role Permission
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (501, 51)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (502, 51)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (503, 51)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (504, 51)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (505, 51)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (506, 51)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (507, 51)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (508, 51)
GO

-- CompanyLocationManagers Role Permission
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (501, 52)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (502, 52)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (503, 52)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (504, 52)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (505, 52)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (506, 52)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (507, 52)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (508, 52)
GO

-- CompanyDepartmentSupervisors Role Permission
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (501, 53)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (502, 53)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (503, 53)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (504, 53)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (505, 53)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (506, 53)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (507, 53)
INSERT [dbo].[PermissionRecord_Role_Mapping]([PermissionRecord_Id],[AccountRole_Id]) VALUES (508, 53)
GO



--------------------------------------------------------------------------------------------------
-- Assign User Role
--------------------------------------------------------------------------------------------------

-- Assign User "Admin" as Administrators, PayrollAdministrators, Recruiters
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id],[AccountRole_Id]) VALUES (1, 1)
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id],[AccountRole_Id]) VALUES (1, 2)
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id],[AccountRole_Id]) VALUES (1, 3)
Go

-- Assign User "joey" as Recruiters Role
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id],[AccountRole_Id]) VALUES (2, 1)
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id],[AccountRole_Id]) VALUES (2, 2)
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id],[AccountRole_Id]) VALUES (2, 3)
GO

-- Assign User "Shaun" as Recruiters Role
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id],[AccountRole_Id]) VALUES (3, 1)
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id],[AccountRole_Id]) VALUES (3, 2)
INSERT [dbo].[Account_AccountRole_Mapping]([Account_Id],[AccountRole_Id]) VALUES (3, 3)
GO


-- 