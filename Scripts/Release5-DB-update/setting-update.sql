update Setting set Value = 'Hashed', UpdatedOnUtc = GETUTCDATE()
where Name = 'CandidateSettings.DefaultPasswordFormat'
