------------------------
-- [Candidate]
------------------------
SELECT Id,FirstName,LastName,Email,HomePhone,MobilePhone,EmergencyPhone,SocialInsuranceNumber,BirthDate,CreatedOnUtc FROM Candidate WHERE Email in (
    SELECT Email FROM Candidate GROUP BY Email HAVING (COUNT(*) > 1)
) ORDER BY Email


SELECT Id,FirstName,LastName,Email,HomePhone,MobilePhone,EmergencyPhone,SocialInsuranceNumber,BirthDate,CreatedOnUtc FROM Candidate WHERE FirstName+LastName+HomePhone in (
    SELECT FirstName+LastName+HomePhone FROM Candidate GROUP BY FirstName+LastName+HomePhone HAVING (COUNT(*) > 1)
)   ORDER BY FirstName, LastName, CreatedOnUtc


SELECT Id,FirstName,LastName,Email,HomePhone,MobilePhone,EmergencyPhone,SocialInsuranceNumber,BirthDate,CreatedOnUtc FROM Candidate WHERE SocialInsuranceNumber in (
    SELECT SocialInsuranceNumber FROM Candidate WHERE LEN(SocialInsuranceNumber) > 2 GROUP BY SocialInsuranceNumber HAVING (COUNT(*) > 1)
)   ORDER BY SocialInsuranceNumber


SELECT Id,FirstName,LastName,Email,HomePhone,MobilePhone,EmergencyPhone,SocialInsuranceNumber,BirthDate,CreatedOnUtc FROM Candidate WHERE FirstName+LastName+BirthDate in (
    SELECT FirstName+LastName+BirthDate FROM Candidate GROUP BY FirstName+LastName+BirthDate HAVING (COUNT(*) > 1)
) ORDER BY FirstName+LastName+BirthDate


SELECT Id,FirstName,LastName,Email,HomePhone,MobilePhone,EmergencyPhone,SocialInsuranceNumber,BirthDate,CreatedOnUtc FROM Candidate 
WHERE FirstName+LastName+MobilePhone in (
    SELECT FirstName+LastName+MobilePhone FROM Candidate WHERE LEN(MobilePhone) > 2 GROUP BY FirstName+LastName+MobilePhone HAVING (COUNT(*) > 1)
) ORDER BY FirstName+LastName+MobilePhone

SELECT Id,FirstName,LastName,Email,HomePhone,MobilePhone,EmergencyPhone,SocialInsuranceNumber,BirthDate,CreatedOnUtc FROM Candidate 
WHERE FirstName+LastName+EmergencyPhone in (
    SELECT FirstName+LastName+EmergencyPhone FROM Candidate WHERE LEN(EmergencyPhone) > 2 GROUP BY FirstName+LastName+EmergencyPhone HAVING (COUNT(*) > 1)
) ORDER BY FirstName+LastName+EmergencyPhone


-----------------------------------------------------------------------------------
-- clean up
-----------------------------------------------------------------------------------
DECLARE @idToDelete int; set @idToDelete = 4550
DECLARE @idToKeep int; set @idToKeep = 5433

-- Checking...
SELECT * FROM CandidateTestResult WHERE CandidateId in ( @idToDelete )
SELECT * FROM CandidateJobOrder WHERE CandidateId in ( @idToDelete )
SELECT * FROM CandidateJobOrderStatusHistory WHERE CandidateId in ( @idToDelete )
SELECT * FROM CandidateSmartCard WHERE CandidateId in ( @idToDelete )

-- Purging...
UPDATE CandidateTestResult set CandidateId = @idToKeep WHERE CandidateId = @idToDelete
UPDATE CandidateJobOrder set CandidateId = @idToKeep WHERE CandidateId = @idToDelete
UPDATE CandidateSmartCard set CandidateId = @idToKeep WHERE CandidateId = @idToDelete


DELETE FROM Candidate WHERE Id IN ( @idToDelete )



