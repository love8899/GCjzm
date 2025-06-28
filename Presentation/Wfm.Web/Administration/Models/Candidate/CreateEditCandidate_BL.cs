using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Web.Framework;
using Wfm.Services.Candidates;
using Wfm.Services.Logging;
using Wfm.Core;
using Wfm.Services.Localization;
using Wfm.Admin.Extensions;
using Kendo.Mvc.UI;
using Wfm.Services.Accounts;
using Wfm.Services.Common;
using Wfm.Core.Domain.Media;
using Wfm.Core.Domain.Candidates;
using Wfm.Services.DirectoryLocation;
using Wfm.Core.Domain.Accounts;
using Wfm.Services.Security;
using Wfm.Services.Franchises;
using Wfm.Services.Messages;
using Wfm.Services.Companies;
using Wfm.Admin.Infrastructure.WcfHelper;
using Wfm.Admin.Models.Common;
using Wfm.Shared.Extensions;
using System.Text;


namespace Wfm.Admin.Models.Candidate
{
    public class CreateEditCandidate_BL
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly ISalutationService _salutationService;
        private readonly IShiftService _shiftService;
        private readonly ISkillService _skillService;
        private readonly IPositionService _positionService;
        private readonly IPermissionService _permissionService;
        private readonly IFranchiseService _franchiseService;
        private readonly IAccountService _accountService;
        private readonly ICandidateService _candidateService;
        private readonly ICandidateBlacklistService _candidateBlacklistService;
        private readonly ICandidateKeySkillService _candidateKeySkillsService;
        private readonly ICandidateAddressService _candidateAddressService;
        private readonly ICandidatePictureService _candidatePictureService;
        private readonly ICompanyCandidateService _companyCandidateService;
        private readonly ICandidateJobOrderService _candidateJobOrderService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IActivityLogService _activityLogService;
        private readonly ILocalizationService _localizationService;
        private readonly CandidateMassEmailSettings _candidateMassEmailSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IAccountPasswordPolicyService _accountPasswordPolicyService;
        private readonly ITextMessageSender _textMessageSender;

        #endregion

        #region Ctor

        public CreateEditCandidate_BL(
            IWorkContext workContext,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ICityService cityService,
            ISalutationService salutationService,
            IShiftService shiftService,
            ISkillService skillService,
            IPositionService positionService,
            IPermissionService permissionService,
            IFranchiseService franchiseService,
            IAccountService accountService,
            ICompanyCandidateService companyCandidateService,
            ICandidateJobOrderService candidateJobOrderService,
            ICandidateService candidateService,
            ICandidateBlacklistService candidateBlacklistService,
            ICandidateKeySkillService candidateKeySkillsService,
            ICandidateAddressService candidateAddressService,
            ICandidatePictureService candidatePictureService,
            IEmailAccountService emailAccountService,
            IActivityLogService activityLogService,
            ILocalizationService localizationService,
            CandidateMassEmailSettings candidateMassEmailSettings,
            MediaSettings mediaSettings,
            IAccountPasswordPolicyService accountPasswordPolicyService,
            ITextMessageSender textMessageSender)
        {
            _workContext = workContext;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _cityService = cityService;
            _salutationService = salutationService;
            _shiftService = shiftService;
            _skillService = skillService;
            _positionService = positionService;
            _permissionService = permissionService;
            _franchiseService = franchiseService;
            _accountService = accountService;
            _companyCandidateService = companyCandidateService;
            _candidateJobOrderService = candidateJobOrderService;
            _candidateService = candidateService;
            _candidateBlacklistService = candidateBlacklistService;
            _candidateKeySkillsService = candidateKeySkillsService;
            _candidateAddressService = candidateAddressService;
            _candidatePictureService = candidatePictureService;
            _emailAccountService = emailAccountService;
            _activityLogService = activityLogService;
            _localizationService = localizationService;
            _candidateMassEmailSettings = candidateMassEmailSettings;
            _mediaSettings = mediaSettings;
            _accountPasswordPolicyService = accountPasswordPolicyService;
            _textMessageSender = textMessageSender;
        }

        #endregion


        public bool RegisterCandidateFromAdmin(CreateEditCandidateModel candidateData, out string errorMessage)
        {
            // Check RePassword
            if (candidateData.Password != candidateData.RePassword)
            {
                errorMessage = _localizationService.GetResource("Common.EnteredPasswordsDoNotMatch");
                return false;
            }

            Wfm.Core.Domain.Candidates.Candidate candidate = candidateData.ToEntity();

            string duplicate = string.Empty;
            // Ensure the candidate is unique
            // -----------------------------------------------------
            if (_candidateService.IsDuplicate(candidate, out duplicate))
            {
                errorMessage = String.Format("The following candidate(s) ({0}) have similar information as the new one. Please double check it.", duplicate);
                return false;
            }

            // Save new Candidate
            // -----------------------------------------------------
            candidate.GenderId = _salutationService.GetSalutationById(candidate.SalutationId).GenderId;
            candidate.OwnerId = _workContext.CurrentAccount.Id;
            candidate.FranchiseId = _workContext.CurrentFranchise.Id;
            candidate.EnteredBy = _workContext.CurrentAccount.Id;

            string candidateRegisterResult = _candidateService.RegisterCandidate(candidate, false, true);
            if (!String.IsNullOrWhiteSpace(candidateRegisterResult))
            {
                errorMessage = candidateRegisterResult; // _localizationService.GetResource("Admin.Candidate.Candidate.Added.Fail");
                return false;
            }

            //activity log
            _activityLogService.InsertActivityLog("AddNewCandidate", _localizationService.GetResource("ActivityLog.AddNewCandidate"), candidate.GetFullName());

            var candidateHomeAddress = new Wfm.Core.Domain.Candidates.CandidateAddress
            {
                CandidateId = candidate.Id,
                AddressTypeId = 1,
                UnitNumber = candidateData.CandidateAddressModel.UnitNumber,
                AddressLine1 = candidateData.CandidateAddressModel.AddressLine1.Trim(),
                AddressLine2 = candidateData.CandidateAddressModel.AddressLine2,
                AddressLine3 = candidateData.CandidateAddressModel.AddressLine3,
                CityId = candidateData.CandidateAddressModel.CityId,
                StateProvinceId = candidateData.CandidateAddressModel.StateProvinceId,
                CountryId = candidateData.CandidateAddressModel.CountryId,
                PostalCode = candidateData.CandidateAddressModel.PostalCode.Replace(" ", ""),
                IsActive = true,
                EnteredBy = _workContext.CurrentAccount.Id
            };

            _candidateAddressService.InsertCandidateAddress(candidateHomeAddress);

            var skill1 = new CandidateKeySkill()
            {
                CandidateId = candidate.Id,
                KeySkill = candidateData.KeySkill1,
                YearsOfExperience = candidateData.YearsOfExperience1,
                LastUsedDate = candidateData.LastUsedDate1
            };

            _candidateKeySkillsService.InsertCandidateKeySkill(skill1);

            // Update search keys
            UpdateCandidateSearchKeys(candidate.Id);

            errorMessage = null;

            return true;
        }

        public void UpdateCandidateSearchKeys(int candidateId)
        {
            Wfm.Core.Domain.Candidates.Candidate candidateUpdateSearchKeys = _candidateService.GetCandidateById(candidateId);
            IList<CandidateKeySkill> candidateKeySkills = _candidateKeySkillsService.GetCandidateKeySkillsByCandidateId(candidateId);
            CandidateAddress candidateAddress = _candidateAddressService.GetAllCandidateAddressesByCandidateId(candidateId).FirstOrDefault();
            candidateUpdateSearchKeys.SearchKeys = GetCandidateSearchKeysSet(candidateKeySkills, candidateAddress);

            _candidateService.UpdateCandidate(candidateUpdateSearchKeys);
        }

        private string GetCandidateSearchKeysSet(IList<CandidateKeySkill> s, CandidateAddress address)
        {
            IList<String> ltStr = new List<String>();

            if (s != null)
            {
                foreach (var item in s)
                {
                    ltStr.Add(item.KeySkill);
                }
            }

            return string.Join(", ", ltStr);
        }

        public bool UpdateCandidateData(CandidateModel candidateData, out string errorMessage)
        {
            bool result = true;
            errorMessage = null;

            Wfm.Core.Domain.Candidates.Candidate candidate = _candidateService.GetCandidateByGuid(candidateData.CandidateGuid);
            if (candidate == null)
            {
                //No candidate found with the specified id
                errorMessage = _localizationService.GetResource("Admin.Candidate.Candidate.CandidateDoesntExist");
                return false;
            }

            // Ensure the candidate is unique
            // -----------------------------------------------------
            if (_candidateService.IsDuplicateWhenUpdate(candidateData.ToEntity()))
            {
                errorMessage = _localizationService.GetResource("Admin.Candidate.Candidate.Added.Fail.DuplicateCandidate");
                return false;
            }

            // Is candidate banned?
            if (candidateData.IsBanned)
            {
                if (candidateData.IsActive)
                {
                    candidateData.IsActive = false;
                }
                candidateData.IsHot = false;
            }

            if (!candidateData.IsActive && String.IsNullOrWhiteSpace(candidateData.InactiveReason))
                candidateData.InactiveReason = "Other";

            candidateData.IsEmployee = candidate.IsEmployee;
            candidateData.CreatedOnUtc = candidate.CreatedOnUtc;

            candidate = candidateData.ToEntity(candidate);
            candidate.GenderId = _salutationService.GetSalutationById(candidate.SalutationId).GenderId;

            UpdateCandidate(candidate);

            return result;
        }


        public DataSourceResult GetCandidateList(bool isSortingRequired, DataSourceRequest request)
        {
            var candidates = _candidateService.GetAllCandidatesWithAddressAsQueryable(_workContext.CurrentAccount, true, true, false, isSortingRequired).PagedForCommand(request);
            return GetCandidateModelList(candidates);
        }


        private DataSourceResult GetCandidateModelList(IPagedList<CandidateWithAddress> candidates)
        {
            var candidateModelList = new List<CandidateWithAddressModel>();
            foreach (var item in candidates)
            {
                var candidateModel = item.ToModel();
                if (!_permissionService.Authorize(StandardPermissionProvider.ViewCandidateSIN))
                    candidateModel.SocialInsuranceNumber = candidateModel.SocialInsuranceNumber.ToMaskedSocialInsuranceNumber();
                candidateModelList.Add(candidateModel);
            }
            var result = new DataSourceResult()
            {
                Data = candidateModelList, // Process data (paging and sorting applied)
                Total = candidates.TotalCount, // Total number of records
            };
            return result;
        }


        public IQueryable<CandidateBlacklistModel> GetCandidateBlacklist()
        {
            var candidates = _candidateBlacklistService.GetAllCandidateBlacklistsAsQueryable(_workContext.CurrentAccount);

            return candidates.ProjectTo<CandidateBlacklistModel>();
        }


        public DataSourceResult GetCandidateListByPicture([DataSourceRequest] DataSourceRequest request)
        {
            var candidates = _candidateService.GetAllCandidatesAsQueryable(_workContext.CurrentAccount, true, true).PagedForCommand(request);

            var candidateModelList = new List<CandidateModel>();
            foreach (var x in candidates)
            {
                var candidateModel = x.ToModel();

                // candidate picture
                var defaultCandidatePicture = _candidatePictureService.GetCandidatePicturesByCandidateId(x.Id, 1).FirstOrDefault();
                candidateModel.PictureThumbnailUrl = _candidatePictureService.GetCandidatePictureUrl(defaultCandidatePicture, _mediaSettings.CandidateDetailsPictureSize, true);

                CandidateAddress candidateAddress = _candidateAddressService.GetCandidateHomeAddressByCandidateId(x.Id);
                if (candidateAddress != null)
                {
                    candidateModel.CandidateAddressModel = candidateAddress.ToModel();
                }
                else
                {
                    candidateModel.CandidateAddressModel = new AddressModel();
                }

                candidateModel.GenderModel = x.Gender.ToModel();

                candidateModelList.Add(candidateModel);
            }

            var result = new DataSourceResult()
            {
                Data = candidateModelList, // Process data (paging and sorting applied)
                Total = candidates.TotalCount, // Total number of records
            };

            return result;
        }


        public DataSourceResult GetCandidateListBySkills([DataSourceRequest] DataSourceRequest request)
        {
            var skills = _candidateKeySkillsService.GetAllCandidateKeySkillsAsQueryable().PagedForCommand(request);

            var candidateSkillsList = new List<CandidateKeySkillModel>();
            foreach (var x in skills)
            {
                var model = new CandidateKeySkillModel()
                {
                    CandidateGuid = x.CandidateGuid,
                    CandidateId = x.CandidateId,
                    EmployeeId = x.EmployeeId,
                    KeySkill = x.KeySkill,
                    CreatedOnUtc = x.CreatedOnUtc,
                    Id = x.Id,
                    LastUsedDate = x.LastUsedDate,
                    Note = x.Note,
                    UpdatedOnUtc = x.UpdatedOnUtc,
                    YearsOfExperience = x.YearsOfExperience,
                    CandidateName = x.CandidateName
                };

                candidateSkillsList.Add(model);
            }

            var result = new DataSourceResult()
            {
                Data = candidateSkillsList,
                Total = skills.TotalCount
            };

            return result;
        }

        public DataSourceResult GetCandidateListByAddress([DataSourceRequest] DataSourceRequest request)
        {
            var addresses = _candidateAddressService.GetAllCandidateAddressesForListAsQueryable().PagedForCommand(request);

            var candidateAddressModelList = new List<CandidateAddressModel>();
            foreach (var x in addresses)
            {
                var model = x.ToModel();
                model.CandidateGuid = x.Candidate.CandidateGuid;
                model.CityName = model.CityId > 0 ? _cityService.GetCityById(model.CityId).CityName : "";
                model.StateProvinceName = model.StateProvinceId > 0 ? _stateProvinceService.GetStateProvinceById(model.StateProvinceId).StateProvinceName : "";
                model.CountryName = model.CountryId > 0 ? _countryService.GetCountryById(model.CountryId).CountryName : "";
                model.FranchiseId = x.Candidate.FranchiseId;
                candidateAddressModelList.Add(model);
            }

            var result = new DataSourceResult()
            {
                //Data = addresses.Select(x => x.ToModel()),
                Data = candidateAddressModelList,
                Total = addresses.TotalCount, // Total number of records
            };

            return result;
        }

        public CandidateModel GetCandidateBasicDataById(Guid guid)
        {
            Wfm.Core.Domain.Candidates.Candidate candidate = _candidateService.GetCandidateByGuid(guid);
            if (candidate == null)
                return null; // RedirectToAction("Index");

            var postalCode = "M3H 5T5";
            var homeAddress = _candidateAddressService.GetCandidateHomeAddressByCandidateId(candidate.Id);
            if (homeAddress != null)
                postalCode = homeAddress.PostalCode;

            CandidateModel candidateModel = candidate.ToModel();
            candidateModel.RePassword = candidateModel.Password = "******"; //to pass validation
            candidateModel.PasswordPolicyModel = _accountPasswordPolicyService.Retrieve("Candidate").PasswordPolicy.ToModel();
            //CreateEditCandidateModel model = new CreateEditCandidateModel()
            //{
            //    CandidateModel = candidateModel,

            //    AddressLine1 = "for validation",
            //    CountryId = 0,
            //    StateProvinceId = 0,
            //    CityId = 0,
            //    PostalCode = postalCode,
            //    KeySkill1 = "for validation",
            //    YearsOfExperience1 = 1,
            //    LastUsedDate1 = DateTime.Now
            //};

            return candidateModel;
        }

        public CandidateModel GetCandidateDetailDataById(Guid guid)
        {
            Wfm.Core.Domain.Candidates.Candidate candidate = _candidateService.GetCandidateByGuid(guid);
            if (candidate == null)
                return null;

            var account = _workContext.CurrentAccount;
            if (account.IsVendor() && account.FranchiseId != candidate.FranchiseId)
                return null;

            CandidateModel model = candidate.ToModel();
            var enterAccount = _accountService.GetAccountById(model.EnteredBy);
            if (enterAccount != null)
                model.EnteredName = enterAccount.FullName;
            if (!_permissionService.Authorize(StandardPermissionProvider.ViewCandidateSIN))
                model.SocialInsuranceNumber = model.SocialInsuranceNumber.ToMaskedSocialInsuranceNumber();
            if (model.BirthDate != null)
            {
                model.Age = model.BirthDate.Value.CalculateAge().ToString();
            }
            model.ShiftModel = candidate.Shift.ToModel();
            model.GenderModel = candidate.Gender.ToModel();
            model.VetranTypeModel = candidate.VetranType.ToModel();
            model.TransportationModel = candidate.Transportation.ToModel();
            model.SourceModel = candidate.Source.ToModel();
            model.SalutationModel = candidate.Salutation.ToModel();
            model.EthnicTypeModel = candidate.EthnicType.ToModel();
            var franchise = _franchiseService.GetFranchiseById(candidate.FranchiseId);
            model.FranchiseName = franchise == null ? "" : franchise.FranchiseName;

            var homeAddress = _candidateAddressService.GetCandidateHomeAddressByCandidateId(candidate.Id);
            model.CandidateAddressModel = homeAddress != null ? homeAddress.ToModel() : null;

            // candidate picture
            var defaultCandidatePicture = _candidatePictureService.GetCandidatePicturesByCandidateId(candidate.Id, 1).FirstOrDefault();
            model.PictureThumbnailUrl = _candidatePictureService.GetCandidatePictureUrl(defaultCandidatePicture, _mediaSettings.CandidateDetailsPictureSize, true);

            //model.OnboardingStatus = _candidateOnboardingStatusRepository.GetById(candidate.CandidateOnboardingStatusId).StatusName;
            model.OnboardingStatus = ((CandidateOnboardingStatusEnum)candidate.CandidateOnboardingStatusId).ToString();

            return model;
        }


        #region Skills
        public bool AddNewKeySkill(CandidateKeySkillModel model)
        {
            bool result = true;

            model.CreatedOnUtc = System.DateTime.UtcNow;
            model.UpdatedOnUtc = System.DateTime.UtcNow;

            CandidateKeySkill candidateKeySkill = model.ToEntity();
            _candidateKeySkillsService.InsertCandidateKeySkill(candidateKeySkill);

            // Update search keys
            this.UpdateCandidateSearchKeys(model.CandidateId);


            //activity log
            _activityLogService.InsertActivityLog("AddNewCandidateKeySkill", _localizationService.GetResource("ActivityLog.AddNewCandidateKeySkill"), candidateKeySkill.KeySkill);

            return result;
        }

        public bool SaveKeySkill(CandidateKeySkillModel model)
        {
            bool result = true;

            model.UpdatedOnUtc = System.DateTime.UtcNow;
            CandidateKeySkill candidateKeySkill = _candidateKeySkillsService.GetCandidateKeySkillById(model.Id);

            candidateKeySkill = model.ToEntity(candidateKeySkill);
            _candidateKeySkillsService.UpdateCandidateKeySkill(candidateKeySkill);

            // Update search keys
            this.UpdateCandidateSearchKeys(model.CandidateId);

            //activity log
            _activityLogService.InsertActivityLog("UpdateCandidateKeySkill", _localizationService.GetResource("ActivityLog.UpdateCandidateKeySkill"), candidateKeySkill.KeySkill);

            return result;
        }


        #endregion


        #region Mass Email

        public DataSourceResult GetCandidateModelListByProfiles(Wfm.Core.Domain.Accounts.Account account, CandidateProfile candidateProfile, PoolProfile poolProfile, PipelineProfile pipelineProfile, DataSourceRequest request)
        {
            var candidates = GetCandidateListByProfiles(account, candidateProfile, poolProfile, pipelineProfile)
                             .OrderByDescending(x => x.UpdatedOnUtc).PagedForCommand(request);
            return GetCandidateModelList(candidates);
        }


        public IQueryable<CandidateWithAddress> GetCandidateListByProfiles(Wfm.Core.Domain.Accounts.Account account, CandidateProfile candidateProfile, PoolProfile poolProfile, PipelineProfile pipelineProfile)
        {
            //var candidates = _candidateService.GetAllCandidatesAsQueryable(account);
            var candidates = _candidateService.GetAllCandidatesWithAddressAsQueryable(account, isSortingRequired: false);

            if (candidateProfile != null)
                candidates = _FilteringByCandidateProfile(candidates, candidateProfile);
            else if (poolProfile != null)
                candidates = _FilteringByPoolProfile(candidates, poolProfile);
            else if (pipelineProfile != null)
                candidates = _FilteringByPipelineProfile(candidates, pipelineProfile);

            return candidates;
        }


        private IQueryable<CandidateWithAddress> _FilteringByCandidateProfile(IQueryable<CandidateWithAddress> candidates, CandidateProfile candidateProfile)
        {
            if (candidateProfile.CityIds != null)
                candidates = candidates.Where(x => x.CityId != null && candidateProfile.CityIds.Contains(x.CityId.Value));

            if (!string.IsNullOrEmpty(candidateProfile.Intersection))
            {
                // var xsections = _xsectionService.GetAllIntersections().Where(x => candidateProfile.XsectionIds.Contains(x.Id)).Select(x => x.IntersectionName);
                candidates = candidates.Where(x => (x.MajorIntersection1 != null && x.MajorIntersection1.Contains(candidateProfile.Intersection)) ||
                                                   (x.MajorIntersection2 != null && x.MajorIntersection2.Contains(candidateProfile.Intersection)));
            }

            if (candidateProfile.XportationIds != null)
                candidates = candidates.Where(x => x.TransportationId != null && candidateProfile.XportationIds.Contains(x.TransportationId.Value));

            if (candidateProfile.ShiftIds != null && !candidateProfile.ShiftIds.Contains(_shiftService.GetShiftIdByName("Any")))
                candidates = candidates.Where(x => x.ShiftId != null && candidateProfile.ShiftIds.Contains(x.ShiftId.Value));

            if (candidateProfile.SkillIds != null)
            {
                var skills = _skillService.GetAllSkills().Where(x => candidateProfile.SkillIds.Contains(x.Id)).Select(x => x.SkillName);
                var candidateSkills = _candidateKeySkillsService.GetAllCandidateKeySkillsAsQueryable();
                candidates = (from c in candidates
                              join cs in candidateSkills on c.Id equals cs.CandidateId
                              where skills.Contains(cs.KeySkill)
                              select c).Distinct();
            }

            if (candidateProfile.MinEduLevel.HasValue)
            {
                // TODO
                //var eduLevel = ((EducationLevel)candidateProfile.MinEduLevel).ToString();
            }

            if (candidateProfile.GenderId.HasValue)
                candidates = candidates.Where(x => x.GenderId == candidateProfile.GenderId);

            if (candidateProfile.IsEmployee)
                candidates = candidates.Where(x => x.IsEmployee);
            else if (candidateProfile.Onboarded)
                candidates = candidates.Where(x => x.CandidateOnboardingStatusId == (int)CandidateOnboardingStatusEnum.Started);
            else if (candidateProfile.IsActive)
                candidates = candidates.Where(x => x.IsActive);

            candidates = _ExcludeByPlacementStatus(candidates, candidateProfile);

            return candidates;
        }


        private IQueryable<CandidateWithAddress> _ExcludeByPlacementStatus(IQueryable<CandidateWithAddress> candidates, CandidateProfile candidateProfile)
        {
            if (candidateProfile.ByPlacement && candidateProfile.StartDate != null && candidateProfile.EndDate != null)
            {
                var pipeline = _candidateJobOrderService.GetAllCandidateJobOrdersAsQueryable()
                               .Where(x => x.StartDate <= candidateProfile.EndDate && (!x.EndDate.HasValue || x.EndDate >= candidateProfile.StartDate))
                               .Where(x => x.CandidateJobOrderStatusId == (int)CandidateJobOrderStatusEnum.Placed);

                var pipelineIds = pipeline.Select(x => x.CandidateId);
                if (candidateProfile.IsPlaced)
                    candidates = candidates.Where(x => pipelineIds.Contains(x.Id));
                else
                    candidates = candidates.Where(x => !pipelineIds.Contains(x.Id));
            }

            return candidates;
        }


        private IQueryable<CandidateWithAddress> _FilteringByPoolProfile(IQueryable<CandidateWithAddress> candidates, PoolProfile poolProfile)
        {
            if (poolProfile.CompanyIds != null)
            {
                var pooled = _companyCandidateService.GetAllCompanyCandidatesAsQueryable(_workContext.CurrentAccount)
                             .Where(x => x.StartDate <= poolProfile.RefDate && (!x.EndDate.HasValue || x.EndDate >= poolProfile.RefDate))
                             .Where(x => poolProfile.CompanyIds.Contains(x.CompanyId));

                if (poolProfile.PositionIds != null)
                {
                    var positions = _positionService.GetAllPositions().Where(x => poolProfile.PositionIds.Contains(x.Id)).Select(x => x.Name);
                    pooled = pooled.Where(x => positions.Contains(x.Candidate.JobTitle));
                }

                var pooledIds = pooled.Select(x => x.CandidateId);
                candidates = candidates.Where(x => pooledIds.Contains(x.Id));
            }

            if (poolProfile.ShiftIds != null && !poolProfile.ShiftIds.Contains(_shiftService.GetShiftIdByName("Any")))
                candidates = candidates.Where(x => x.ShiftId != null && poolProfile.ShiftIds.Contains(x.ShiftId.Value));

            return candidates;
        }


        private IQueryable<CandidateWithAddress> _FilteringByPipelineProfile(IQueryable<CandidateWithAddress> candidates, PipelineProfile pipelineProfile)
        {
            if (pipelineProfile.JobOrderId > 0 && pipelineProfile.StartDate != null && pipelineProfile.EndDate != null)
            {
                var pipeline = _candidateJobOrderService.GetAllCandidateJobOrdersAsQueryable()
                               .Where(x => x.JobOrderId == pipelineProfile.JobOrderId)
                               .Where(x => x.StartDate <= pipelineProfile.EndDate && (!x.EndDate.HasValue || x.EndDate >= pipelineProfile.StartDate));

                if (pipelineProfile.StatusIds != null)
                    pipeline = pipeline.Where(x => pipelineProfile.StatusIds.Contains(x.CandidateJobOrderStatusId));

                var pipelineIds = pipeline.Select(x => x.CandidateId);
                candidates = candidates.Where(x => pipelineIds.Contains(x.Id));
            }

            return candidates;
        }


        public int SendMassMessageToSelectedCandidates(string ids, string message)
        {
            // WCF
            //string svcUser; string svcPassword;
            //ClientServiceReference.WfmServiceClient svcClient = WcfHelper.GetClientService(out svcUser, out svcPassword);
            //svcClient.SendMassMessage(svcUser, svcPassword, ids, message, null);
            //svcClient.Close();

            var IdList = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x));
            var numbers = _candidateService.GetAllCandidatesAsQueryable(_workContext.CurrentAccount)
                          .Where(x => IdList.Contains(x.Id)).Select(x => x.MobilePhone).ToList();

            return _textMessageSender.SendTextMessage(message, String.Join(",", numbers));
        }


        public void SendMassEmailToSelectedCandidates(Wfm.Core.Domain.Accounts.Account account, string ids, string subject, string message, int messageCategoryId, bool systemAccount = true)
        {
            var from = account.Email;
            var fromName = account.FullName;
            if (systemAccount)
            {
                var emailAccount = _emailAccountService.GetEmailAccountById(_candidateMassEmailSettings.FromEmailAccountId);
                if (emailAccount != null)
                {
                    from = emailAccount.Email;
                    fromName = emailAccount.FriendlyName;
                }
            }

            string svcUser; string svcPassword;
            ClientServiceReference.WfmServiceClient svcClient = WcfHelper.GetClientService(out svcUser, out svcPassword);
            svcClient.SendMassEmail(svcUser, svcPassword, account.AccountGuid.ToString(), account.FranchiseId, ids, "Candidate", from, fromName, subject, message, messageCategoryId);
            svcClient.Close();
        }

        #endregion


        #region Update Candidate

        public void UpdateCandidate(Wfm.Core.Domain.Candidates.Candidate candidate)
        {
            List<string> excludeList = new List<string>();
            excludeList.Add("Password"); excludeList.Add("PasswordFormatId"); excludeList.Add("PasswordSalt");
            excludeList.Add("LastIpAddress"); excludeList.Add("LastLoginDateUtc"); excludeList.Add("LastActivityDateUtc");
            excludeList.Add("CandidateGuid");
            if (candidate.SocialInsuranceNumber == null || candidate.SocialInsuranceNumber.Contains("*"))
                excludeList.Add("SocialInsuranceNumber");

            candidate.UpdatedOnUtc = DateTime.UtcNow;
            _candidateService.UpdateCandidate(candidate, excludeList.ToArray());

            //activity log
            _activityLogService.InsertActivityLog("UpdateCandidateProfile", _localizationService.GetResource("ActivityLog.UpdateCandidateProfile"), candidate.GetFullName());
        }

        #endregion


        #region Ban Candidate

        public string BanCandidate(Guid guid, DateTime startDate, int? clientId, string reason, string note, string clientName, int? jobOrderId)
        {
            StringBuilder result = new StringBuilder();

            var candidate = _candidateService.GetCandidateByGuid(guid);

            if (candidate == null)
                result.Append(_localizationService.GetResource("Admin.Candidate.Candidate.CandidateDoesntExist"));

            else
            {
                // remove from pipelines
                result.Append(_candidateJobOrderService.RemoveCandidateFromAllPipelines(candidate.Id, startDate, clientId));

                // remove from pools
                if (result.Length <= 0)
                    result.Append(_companyCandidateService.RemoveCandidateFromAllPools(candidate.Id, reason, startDate, clientId));

                // add to blakclist
                if (result.Length<=0)
                    result.Append(AddCandidateToBlacklist(candidate, startDate, clientId, reason, note, clientName, jobOrderId));
            }

            return result.ToString();
        }


        public string AddCandidateToBlacklist(Wfm.Core.Domain.Candidates.Candidate candidate, DateTime startDate, int? clientId, string reason,string note,string clientName, int? jobOrderId)
        {
            var result = String.Empty;

            var anyContainingRecord = _candidateBlacklistService.GetContainingCandidateBlacklist(candidate.Id, startDate, clientId);

            if (anyContainingRecord != null)
                result = "The candidate is already in the DNR list.";
            else
            {
                // removed contained records
                var allContained = _candidateBlacklistService.GetContainedCandidateBlacklists(candidate.Id, startDate, clientId);
                foreach (var c in allContained)
                    _candidateBlacklistService.DeleteCandidateBlacklist(c);

                // create new record
                var entity = new CandidateBlacklist();

                entity.CandidateId = candidate.Id;
                entity.ClientId = clientId;
                entity.EffectiveDate = startDate;
                entity.BannedReason = reason;
                entity.EnteredBy = _workContext.CurrentAccount.Id;
                entity.CreatedOnUtc = DateTime.UtcNow;
                entity.UpdatedOnUtc = entity.CreatedOnUtc;
                //entity.Candidate = candidate;
                entity.Note = String.IsNullOrWhiteSpace(note)?null:note;
                entity.ClientName = String.IsNullOrWhiteSpace(clientName) ? null : clientName;
                if (jobOrderId.HasValue && jobOrderId > 0)
                    entity.JobOrderId = jobOrderId;

                _candidateBlacklistService.InsertCandidateBlacklist(entity);

                // only set IsBanned, if banned for all clients since today
                if (!clientId.HasValue && startDate <= DateTime.Today)
                    _candidateService.SetCandidateToBannedStatus(candidate, reason);
            }

            return result;
        }


        public string RemoveCandidateFromBlacklist(int id)
        {
            var result = String.Empty;

            var blacklistRecord = _candidateBlacklistService.GetCandidateBlacklistById(id);

            if (blacklistRecord == null)
                result = "The record does not exist!";

            else
            {
                // reset status, if for all companies
                var candidate = _candidateService.GetCandidateById(blacklistRecord.CandidateId);
                if (candidate != null && !blacklistRecord.ClientId.HasValue)
                    _candidateService.ResetCandidateFromBannedStatus(candidate);

                // remove record
                _candidateBlacklistService.DeleteCandidateBlacklist(blacklistRecord);
            }

            return result;
        }


        public string BanSelectedCandidates(string selectedIds, DateTime startDate, int? clientId, string reason, string note, string clientName, int? jobOrderId)
        {
            var result = String.Empty;

            var ids = selectedIds.Split(',').Select(x => int.Parse(x));
            var guids = _candidateService.GetAllCandidatesAsQueryable(_workContext.CurrentAccount).Where(x => ids.Contains(x.Id)).Select(x => x.CandidateGuid).ToList();

            if (guids.Count == 0)
                result = _localizationService.GetResource("Admin.Candidate.Candidate.CandidateDoesntExist") + "\r\n";
            else
                foreach (var g in guids)
                    result += BanCandidate(g, startDate, clientId, reason, note, clientName, jobOrderId);

            return result;
        }

        #endregion


        #region Cancel Candidate Onboarding

        public string CancelCandidateOnboarding(CandidateOnboardingModel model, string reason)
        {
            var result = String.Empty;
            var candidate = _candidateService.GetCandidateById(model.Id);

            if (candidate == null)
                result = "Candidate does not exist!" + "\r\n";
            else
            {
                // remove from pipelines
                result = _candidateJobOrderService.RemoveCandidateFromAllPipelines(candidate.Id, DateTime.Today);

                // remove from pools
                result = _companyCandidateService.RemoveCandidateFromAllPools(candidate.Id, reason, DateTime.Today);

                // update status
                if (String.IsNullOrEmpty(result))
                {
                    candidate.CandidateOnboardingStatusId = (int)CandidateOnboardingStatusEnum.Canceled;
                    candidate.OwnerId = candidate.EnteredBy;
                    _candidateService.UpdateCandidate(candidate);

                    //log
                    _activityLogService.InsertActivityLog("CancelCandidateOnboarding", _localizationService.GetResource("ActivityLog.CancelCandidateOnboarding"), model.Id);
                }
            }

            return result;
        }

        #endregion
    }
}
