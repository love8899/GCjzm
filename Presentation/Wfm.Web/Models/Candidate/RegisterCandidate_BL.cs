using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Common;
using Wfm.Services.Candidates;
using Wfm.Services.Common;
using Wfm.Services.Localization;
using Wfm.Services.Logging;
using Wfm.Services.Media;
using Wfm.Services.Messages;
using Wfm.Web.Extensions;
using Wfm.Web.Framework;
using System.IO;
using Wfm.Services.Franchises;

namespace Wfm.Web.Models.Candidate
{
    public class RegisterCandidate_BL
    {
        #region Fields

        private readonly ICandidateService _candidateService;
        private readonly ICandidateAddressService _candidateAddressService;       
        private readonly IActivityLogService _activityLogService;
        private readonly ICandidateKeySkillService _candidateKeySkillsService;      
        private readonly IAttachmentService _attachmentService;
        private readonly IAttachmentTypeService _attachmentTypeService; 
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkflowMessageService _workflowMessageService;     
        private readonly CandidateSettings _candidateSettings;       
        private readonly ILogger _logger;
        private readonly IDocumentTypeService _documentTypeService; 
        private readonly Wfm.Services.Accounts.IAccountPasswordPolicyService _accountPasswordPolicyService;
        private readonly ICandidatePictureService _candidatePictureService;
        private readonly IFranchiseService _franchiseService;
        
        #endregion

         #region Ctor

        public RegisterCandidate_BL(
            ICandidateService candidatesService,
            ICandidateAddressService candidateAddressService,         
            IActivityLogService activityLogService,
            ICandidateKeySkillService candidateKeySkillsService,
            IAttachmentService attachmentService,
            IAttachmentTypeService attachmentTypeService,
            ILocalizationService localizationService,
            IGenericAttributeService genericAttributeService,
            IWorkflowMessageService workflowMessageService,
            CandidateSettings candidateSettings,
            ILogger logger,
            IDocumentTypeService documentTypeService,
            Wfm.Services.Accounts.IAccountPasswordPolicyService accountPasswordPolicyService,
           ICandidatePictureService candidatePictureService,
            IFranchiseService franchiseService
            )
        {
            _candidateService = candidatesService;
            _candidateAddressService = candidateAddressService;          
            _activityLogService = activityLogService;
            _candidateKeySkillsService = candidateKeySkillsService;           
            _attachmentService = attachmentService;
            _attachmentTypeService = attachmentTypeService;
            _localizationService = localizationService;
            _genericAttributeService = genericAttributeService;
            _workflowMessageService = workflowMessageService;           
            _candidateSettings = candidateSettings;           
            _logger = logger;
            _documentTypeService = documentTypeService;         
            _accountPasswordPolicyService = accountPasswordPolicyService;         
            _candidatePictureService = candidatePictureService;
            _franchiseService = franchiseService;
        } 

        public bool RegisterCandidate(CreateEditCandidateModel model, HttpPostedFileBase attachments, out int CandidateId, out string message)
        {

            Wfm.Core.Domain.Candidates.Candidate candidate = model.CandidateModel.ToEntity();
               
                if (String.IsNullOrWhiteSpace(candidate.SocialInsuranceNumber) || !candidate.SocialInsuranceNumber.StartsWith("9"))
                    candidate.SINExpiryDate = null;

                string dup = string.Empty;
                // Ensure the candidate is unique
                // -----------------------------------------------------
                if (_candidateService.IsDuplicate(candidate, out dup))
                {
                    message=_localizationService.GetResource("Web.Candidate.Candidate.Added.Fail.CandidateExists");
                    CandidateId = 0;
                    return false;
                }


                // step 2- Prepare new Candidate
                // -----------------------------------------------------
                var franchise = _franchiseService.GetPublicFranchise();
                if (franchise == null)
                {
                    message = "Cannot find any franchise!";
                    CandidateId = 0;
                    return false;
                }
                candidate.OwnerId = 1;
                candidate.FranchiseId = franchise.Id;
                candidate.EnteredBy = 1;

                // Determine gender
                if (model.CandidateModel.SalutationId == (int)SalutationEnum.NotSpecified) // SalutationId - 1, Not specified
                    candidate.GenderId = (int)GenderEnum.NotSpecified;
                else if (model.CandidateModel.SalutationId == (int)SalutationEnum.Mr) // SalutationId - 3, Mr.
                    candidate.GenderId = (int)GenderEnum.Male;
                else
                    candidate.GenderId = (int)GenderEnum.Female;


                // step 3 - Add new candidate
                string candidateRegisterResult = _candidateService.RegisterCandidate(candidate, false, true); //, false);

                if (!String.IsNullOrWhiteSpace(candidateRegisterResult))
                {
                    message = candidateRegisterResult;
                    CandidateId = 0;
                    return false;
                }


                //notifications
                //if (_candidateSettings.NotifyNewCustomerRegistration)
                //_workflowMessageService.SendCandidateRegisteredNotificationMessage(candidate, 1); // tempararily disabled

                //email validation message
                var emailValidationToken = "TwoGuid".ToToken(2);
                _genericAttributeService.SaveAttribute(candidate, SystemCandidateAttributeNames.CandidateActivationToken, emailValidationToken);
                if (!String.IsNullOrWhiteSpace(candidate.Email))
                {
                    message = "We've sent a confirmation to the provided email, open it up to activate your account.";
                    _workflowMessageService.SendCandidateEmailValidationMessage(candidate, 1);
                }
                else
                    message = "You don't have any email to be used to activate your account. Please contact our recruiters to manually activate it!";

                //activity log
                _activityLogService.InsertActivityLog("Candidate.RegisterNew", _localizationService.GetResource("ActivityLog.Candidate.RegisterNew"), candidate, candidate.Username);

                //Add address
                var candidateHomeAddress = new CandidateAddress
                {
                    CandidateId = candidate.Id,
                    AddressTypeId = (int)AddressTypeEnum.Residential,
                    UnitNumber = model.UnitNumber,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    CityId = model.CityId,
                    StateProvinceId = model.StateProvinceId,
                    CountryId = model.CountryId,
                    PostalCode = model.PostalCode,
                    IsActive = true
                };

                _candidateAddressService.InsertCandidateAddress(candidateHomeAddress);

                //Add skills
                var skill1 = new CandidateKeySkill()
                {
                    CandidateId = candidate.Id,
                    KeySkill = model.KeySkill1.SanitizeContent(20),
                    YearsOfExperience = model.YearsOfExperience1,
                    LastUsedDate = model.LastUsedDate1
                };

                _candidateKeySkillsService.InsertCandidateKeySkill(skill1);

                // add attachment/resume
                if (attachments != null)
                {
                    int documentTypeId = _documentTypeService.GetDocumentTypeByCode("RESUME").Id;
                    HttpPostedFile httpPostedFile = HttpContext.Current.Request.Files[0];
                    if (httpPostedFile != null)
                    {
                        var fileName = Path.GetFileName(httpPostedFile.FileName);
                        var contentType = httpPostedFile.ContentType;
                        using (Stream stream = httpPostedFile.InputStream)
                        {
                            var fileBinary = new byte[stream.Length];
                            stream.Read(fileBinary, 0, fileBinary.Length);
                            // upload attachment
                            _attachmentService.UploadCandidateAttachment(candidate.Id, fileBinary, fileName, contentType, documentTypeId);
                        }
                    }
                }

                //Update search keys
                Wfm.Core.Domain.Candidates.Candidate candidateUpdateSearchKeys = _candidateService.GetCandidateById(candidate.Id);
                IList<CandidateKeySkill> candidateKeySkills = _candidateKeySkillsService.GetCandidateKeySkillsByCandidateId(candidate.Id);
                CandidateAddress candidateAddress = _candidateAddressService.GetAllCandidateAddressesByCandidateId(candidate.Id).FirstOrDefault();
                candidateUpdateSearchKeys.SearchKeys = GetCandidateSearchKeyWords(candidateKeySkills, candidateAddress);

                _candidateService.UpdateCandidate(candidateUpdateSearchKeys);
            
                CandidateId = candidate.Id;
                return true; 
        }
        #endregion
        #region Utilities
        private string GetCandidateSearchKeyWords(IList<CandidateKeySkill> skills, CandidateAddress address)
        {
            IList<String> keyWords = new List<String>();
            foreach (var item in skills)
            {
                keyWords.Add(item.KeySkill);
            }

            return string.Join(", ", keyWords);
        }

        #endregion
    }
}