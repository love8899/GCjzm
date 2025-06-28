using AutoMapper;
using Wfm.Shared.Models.JobPosting;
using Wfm.Core.Domain.JobPosting;
using Wfm.Core.Domain.Policies;
using Wfm.Shared.Models.Policies;
using Wfm.Core.Domain.Accounts;
using Wfm.Shared.Models.Accounts;
using Wfm.Core.Domain.Candidates;
using Wfm.Shared.Models.Candidate;
using System;



namespace Wfm.Shared.Extensions
{
    public static class MappingExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }


        #region JobPost

        public static JobPostingModel ToModel(this JobPosting entity)
        {
            var model = Mapper.Map<JobPosting, JobPostingModel>(entity);

            model.CompanyGuid = entity.Company.CompanyGuid;
            if (entity.CompanyLocation != null)
                model.LocationName = entity.CompanyLocation.LocationName;
            if (entity.CompanyDepartment != null)
                model.DepartmentName = entity.CompanyDepartment.DepartmentName;
            if (entity.Account != null)
                model.SubmittedByName = String.Concat(entity.Account.FirstName, " ", entity.Account.LastName);

            return model;
        }
        
        public static JobPosting ToEntity(this JobPostingModel model)
        {
            return Mapper.Map<JobPostingModel, JobPosting>(model);
        }

        public static JobPostingEditModel ToEditModel(this JobPosting entity)
        {
            var model = Mapper.Map<JobPosting, JobPostingEditModel>(entity);
            //if (entity.CompanyLocationId == null)
            //    model.CompanyLocationId = 0;
            if (entity.CompanyDepartmentId == null)
                model.CompanyDepartmentId = 0;
            return model;
        }
        
        public static JobPosting ToEntity(this JobPostingEditModel model)
        {
            var entity =  Mapper.Map<JobPostingEditModel, JobPosting>(model);
            //if (model.CompanyLocationId == 0)
            //    entity.CompanyLocationId = null;
            if (model.CompanyDepartmentId == 0)
                entity.CompanyDepartmentId = null;
            return entity;
        }
        
        public static JobPosting ToEntity(this JobPostingEditModel model, JobPosting destination)
        {
            var entity = Mapper.Map(model, destination);
            //if (model.CompanyLocationId == 0)
            //    entity.CompanyLocationId = null;
            if (model.CompanyDepartmentId == 0)
                entity.CompanyDepartmentId = null;
            return entity;
        }
        
        #endregion
        #region Password Policy
        public static PasswordPolicyModel ToModel(this PasswordPolicy entity)
        {
            return Mapper.Map<PasswordPolicy, PasswordPolicyModel>(entity);
        }

        public static PasswordPolicy ToEntity(this PasswordPolicyModel model)
        {
            return Mapper.Map<PasswordPolicyModel, PasswordPolicy>(model);
        }

        public static PasswordPolicy ToEntity(this PasswordPolicyModel model, PasswordPolicy destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region AccoountPasswordHistory
        public static AccountPasswordHistoryModel ToModel(this AccountPasswordHistory entity)
        {
            return Mapper.Map<AccountPasswordHistory, AccountPasswordHistoryModel>(entity);
        }

        public static AccountPasswordHistory ToEntity(this AccountPasswordHistoryModel model)
        {
            return Mapper.Map<AccountPasswordHistoryModel, AccountPasswordHistory>(model);
        }

        public static AccountPasswordHistory ToEntity(this AccountPasswordHistoryModel model, AccountPasswordHistory destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion

        #region CandidatePasswordHistory
        public static CandidatePasswordHistoryModel ToModel(this CandidatePasswordHistory entity)
        {
            return Mapper.Map<CandidatePasswordHistory, CandidatePasswordHistoryModel>(entity);
        }

        public static CandidatePasswordHistory ToEntity(this CandidatePasswordHistoryModel model)
        {
            return Mapper.Map<CandidatePasswordHistoryModel, CandidatePasswordHistory>(model);
        }

        public static CandidatePasswordHistory ToEntity(this CandidatePasswordHistoryModel model, CandidatePasswordHistory destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion
    }
}
