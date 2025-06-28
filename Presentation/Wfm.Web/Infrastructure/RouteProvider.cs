using System.Web.Mvc;
using System.Web.Routing;
using Wfm.Web.Framework.Localization;
using Wfm.Web.Framework.Mvc.Routes;

namespace Wfm.Web.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //We reordered our routes so the most used ones are on top. It can improve performance.

            // Unified User log in
            #region Unified Account log in

            //login
            routes.MapRoute("AccountLogin",
                "login/",
                new { controller = "Account", action = "Login", id = UrlParameter.Optional },
                new[] { "Wfm.Web.Controllers" }).DataTokens.Add("Area", "");

            //password recovery
            routes.MapRoute("AccountPasswordRecovery",
                "user/passwordrecovery",
                new { controller = "Account", action = "PasswordRecovery" },
                new[] { "Wfm.Web.Controllers" }).DataTokens.Add("Area", "");

            //password recovery confirmation
            routes.MapRoute("AccountPasswordRecoveryConfirm",
                "user/passwordrecovery/confirm",
                new { controller = "Account", action = "PasswordRecoveryConfirm" },
                new[] { "Wfm.Web.Controllers" }).DataTokens.Add("Area", "");

            #endregion



            //Main
            #region Main

            //install
            routes.MapRoute("Installation",
                "install",
                new { controller = "Install", action = "Index" },
                new[] { "Wfm.Web.Controllers" });


            //home page
            routes.MapLocalizedRoute("HomePage",
                "",
                new { controller = "Home", action = "Index"},
                new[] { "Wfm.Web.Controllers" });

            //jobs
            routes.MapRoute("JobPost",
                "jobs/",
                new { controller = "JobPost", action = "Index"},
                new[] { "Wfm.Web.Controllers" });

            //blog
            routes.MapRoute("Blog",
                "blog/",
                new { controller = "Blog", action = "List" },
                new[] { "Wfm.Web.Controllers" });

            //contact us
            routes.MapRoute("Contact",
                 "contact-us/",
                 new { controller = "Home", action = "ContactUs", },
                 new[] { "Wfm.Web.Controllers" });

            // locations
            routes.MapRoute("Locations",
                 "locations/",
                 new { controller = "Home", action = "Locations", },
                 new[] { "Wfm.Web.Controllers" });

            // about us
            routes.MapRoute("About",
                 "about-us/",
                  new { controller = "Home", action = "AboutUs" },
                  new[] { "Wfm.Web.Controllers" });

            //franchise
            routes.MapRoute("FranchiseOpportunities",
                 "franchise-opportunities/",
                  new { controller = "Home", action = "FranchiseOpportunities" },
                  new[] { "Wfm.Web.Controllers" });
            
            #endregion

            //Blog
            #region Blog

            routes.MapLocalizedRoute(
                "BlogDetail",
                "blog/{blogPostId}",
                new { controller = "Blog", action = "BlogPostSeo" },
                new { blogPostId = @"^\d+$" },
                new[] { "Wfm.Web.Controllers" });

            routes.MapLocalizedRoute(
                "BlogPostSeo",
                "blog/{blogPostId}/{seoName}",
                new { controller = "Blog", action = "BlogPostDetails", seoName = UrlParameter.Optional },
                new { blogPostId = @"^\d+$" },
                new[] { "Wfm.Web.Controllers" });

            #endregion

            //Candidate
            #region Candidate
            routes.MapRoute("CandidateSignIn",
                "candidate/signin",
                new { controller = "Candidate", action = "SignIn" },
                new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("CandidateNewRegister",
                "job-seekers/register",
                new { controller = "Candidate", action = "NewRegister" },
                new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("RegistrationWizard",
                "candidate/registrationwizard",
                new { controller = "CandidateRegistration", action = "RegistrationWizard" },
                new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("CandidateForgetPassword",
                "job-seekers/retrieve-password",
                new { controller = "Candidate", action = "CandidateRecovery" },
                new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("CandidateMainBoard",
                "candidate/",
                new { controller = "Candidate", action = "Index" },
                new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("CandidateAnnoucements",
                "candidate/announcements",
                new { controller = "Candidate", action = "Announcements" },
                new[] { "Wfm.Web.Controllers" });

            //password recovery
            routes.MapRoute("CandidatePasswordRecovery",
                "passwordrecovery",
                new { controller = "Candidate", action = "PasswordRecovery" },
                new[] { "Wfm.Web.Controllers" });
            //password recovery confirmation
            routes.MapRoute("CandidatePasswordRecoveryConfirm",
                "passwordrecovery/confirm",
                new { controller = "Candidate", action = "PasswordRecoveryConfirm" },
                new[] { "Wfm.Web.Controllers" });
            //candidate activation
            routes.MapLocalizedRoute("CandidateActivation",
                "candidate/activation",
                new { controller = "Candidate", action = "CandidateActivation" },
                new[] { "Wfm.Web.Controllers" });
            //candidate confirm offer
            routes.MapLocalizedRoute("CandidateConfirm",
                "candidate/confirm",
                new { controller = "Candidate", action = "CandidateConfirm"},
                new[] { "Wfm.Web.Controllers" });
            //candidate begin test
            routes.MapLocalizedRoute("CandidateTest",
                "candidate/test",
                new { controller = "Candidate", action = "CandidateTest" },
                new[] { "Wfm.Web.Controllers" });

            #endregion


            //JobSeeker
            #region JobSeekers

            routes.MapRoute("JobSeekers",
                  "job-seekers/",
                  new { controller = "JobSeekers", action = "JobSeekers" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobCategory",
                  "job-seekers/job-category/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobCategory" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobCategory_ManagementAndExecutive",
                  "job-seekers/job-category/management-executive/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobCategory_ManagementAndExecutive" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobCategory_SalesAndMarketing",
                  "job-seekers/job-category/sales-marketing/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobCategory_SalesAndMarketing" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobCategory_FinancialAndAccounting",
                  "job-seekers/job-category/financial-accounting/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobCategory_FinancialAndAccounting" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobCategory_InformationTechnology",
                  "job-seekers/job-category/information-technology/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobCategory_InformationTechnology" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobCategory_LifeSciences",
                  "job-seekers/job-category/life-sciences/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobCategory_LifeSciences" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobCategory_Engineering",
                  "job-seekers/job-category/engineering/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobCategory_Engineering" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobCategory_IndustrialAndManufacturing",
                  "job-seekers/job-category/industrial-manufacturing/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobCategory_IndustrialAndManufacturing" },
                  new[] { "Wfm.Web.Controllers" });
            
            routes.MapRoute("JobSeekers_JobCategory_CallCentreAndCustomerService",
                  "job-seekers/job-category/cell-center-customer-service/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobCategory_CallCentreAndCustomerService" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobCategory_SupplyChainLogisticsAndWarehousing",
                  "job-seekers/job-category/supply-chain-logistcs-warehousing/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobCategory_SupplyChainLogisticsAndWarehousing" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobCategory_AdministrativeAndOffice",
                  "job-seekers/job-category/administrative-office/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobCategory_AdministrativeAndOffice" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobCategory_SkilledTrades",
                  "job-seekers/job-category/skilled-trades/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobCategory_SkilledTrades" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobCategory_Others",
                  "job-seekers/job-category/other/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobCategory_Others" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobSearchResource",
                  "job-seekers/job-search-resources/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobSearchResource" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobSearchResource_PreparingYourResume",
                  "job-seekers/job-search-resources/preparing-your-resume/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobSearchResource_PreparingYourResume" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobSearchResource_WritingEffectiveJobSearchLetters",
                  "job-seekers/job-search-resources/writing-effective-job-search-letters/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobSearchResource_WritingEffectiveJobSearchLetters" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobSearchResource_NetworkingEssentials",
                  "job-seekers/job-search-resources/networking-essentials/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobSearchResource_NetworkingEssentials" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_JobSearchResource_InterviewPreparation",
                  "job-seekers/job-search-resources/interview-preparation/",
                  new { controller = "JobSeekers", action = "JobSeekers_JobSearchResource_InterviewPreparation" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_TrainingAndDevelopment",
                  "job-seekers/training-development/",
                  new { controller = "JobSeekers", action = "JobSeekers_TrainingAndDevelopment" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("JobSeekers_Testimonials",
                  "job-seekers/testimonials/",
                  new { controller = "JobSeekers", action = "JobSeekers_Testimonials" },
                  new[] { "Wfm.Web.Controllers" });

            #endregion

            //employers
            #region Employers
            
            routes.MapRoute("Employers",
                  "employers/",
                   new { controller = "Employers", action = "Employers" },
                   new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("EmployersStaffingSolutions",
                  "employers/staffing-solutions/",
                  new { controller = "Employers", action = "Employers_StaffingSolutions" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_StaffingSolutions_PermanentStaffing",
                 "employers/staffing-solutions/permanent-staffing/",
                  new { controller = "Employers", action = "Employers_StaffingSolutions_PermanentStaffing" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_StaffingSolutions_TemporaryStaffing",
                 "employers/staffing-solutions/temporary-staffing/",
                  new { controller = "Employers", action = "Employers_StaffingSolutions_TemporaryStaffing" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_StaffingSolutions_MassRecruitmentServices",
                 "employers/staffing-solutions/mass-recruitment-services/",
                  new { controller = "Employers", action = "Employers_StaffingSolutions_MassRecruitmentServices" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_StaffingSolutions_TemporaryForeignWorkerProgram",
                 "employers/staffing-solutions/temporary-foregin-worker-program/",
                  new { controller = "Employers", action = "Employers_StaffingSolutions_TemporaryForeignWorkerProgram" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_SafetyAndPrevention",
                 "employers/safety-prevention/",
                  new { controller = "Employers", action = "Employers_SafetyAndPrevention" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_Training",
                 "employers/trainning/",
                  new { controller = "Employers", action = "Employers_Training" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("AreaSpecialties",
                 "employers/area-specialties/",
                  new { controller = "Employers", action = "Employers_AreaSpecialties" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_AreaSpecialties_ManagementAndExecutive",
                  "employers/area-specialties/management-executive/",
                  new { controller = "Employers", action = "Employers_AreaSpecialties_ManagementAndExecutive" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_AreaSpecialties_SalesAndMarketing",
                 "employers/area-specialties/sales-marketing/",
                  new { controller = "Employers", action = "Employers_AreaSpecialties_SalesAndMarketing" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_AreaSpecialties_FinancialAndAccounting",
                 "employers/area-specialties/financial-accounting/",
                  new { controller = "Employers", action = "Employers_AreaSpecialties_FinancialAndAccounting" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_AreaSpecialties_InformationTechnology",
                 "employers/area-specialties/information-technology/",
                  new { controller = "Employers", action = "Employers_AreaSpecialties_InformationTechnology" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_AreaSpecialties_LifeSciences",
                 "employers/area-specialties/life-sciences/",
                  new { controller = "Employers", action = "Employers_AreaSpecialties_LifeSciences" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_AreaSpecialties_Engineering",
                 "employers/area-specialties/engineering/",
                  new { controller = "Employers", action = "Employers_AreaSpecialties_Engineering" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_AreaSpecialties_IndustrialAndManufacturing",
                 "employers/area-specialties/industrial-manufacturing/",
                  new { controller = "Employers", action = "Employers_AreaSpecialties_IndustrialAndManufacturing" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_AreaSpecialties_CallCentreAndCustomerService",
                 "employers/area-specialties/cell-centre-customer-service/",
                  new { controller = "Employers", action = "Employers_AreaSpecialties_CallCentreAndCustomerService" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_AreaSpecialties_SupplyChainLogisticsAndWarehousing",
                 "employers/area-specialties/supply-chain-logistics-warehousing/",
                  new { controller = "Employers", action = "Employers_AreaSpecialties_SupplyChainLogisticsAndWarehousing" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_AreaSpecialties_AdministrativeAndOffice",
                 "employers/area-specialties/administrative-office/",
                  new { controller = "Employers", action = "Employers_AreaSpecialties_AdministrativeAndOffice" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_AreaSpecialties_SkilledTrades",
                 "employers/area-specialties/skilled-trades/",
                  new { controller = "Employers", action = "Employers_AreaSpecialties_SkilledTrades" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_AreaSpecialties_Others",
                 "employers/area-specialties/others/",
                  new { controller = "Employers", action = "Employers_AreaSpecialties_Others" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("Employers_Testimonials",
                 "employers/testimonials/",
                  new { controller = "Employers", action = "Employers_Testimonials" },
                  new[] { "Wfm.Web.Controllers" });
            #endregion

            //associate
            #region Associate
            
            routes.MapRoute("Associates",
                  "associates/",
                   new { controller = "Associate", action = "Associates" },
                   new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("EmploymentGuidelines",
                  "associate/employment-guideline/",
                   new { controller = "Associate", action = "EmploymentGuidelines" },
                   new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("SafetyInformation",
                  "associate/safey-information/",
                   new { controller = "Associate", action = "SafetyInformation" },
                   new[] { "Wfm.Web.Controllers" });

            #endregion

            //About us
            #region About Us

            routes.MapRoute("AboutUs",
                "about-us/",
                new { controller = "AboutUs", action = "Index" },
                new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("CorporateSocialResponsibility",
                "about-us/corporate-social-responsibility/",
                new { controller = "AboutUs", action = "CorporateSocialResponsibility" },
                new[] { "Wfm.Web.Controllers" });

            #endregion

            //JobPost SEO
            #region JobPost SEO

            routes.MapLocalizedRoute(
                "JobPosting",
                "jobs/{jobOrderId}",
                new { controller = "JobPost", action = "JobDetailsSeo", seoName = UrlParameter.Optional },
                new { jobOrderId = @"^\d+$" },
                new[] { "Wfm.Web.Controllers" });

            routes.MapLocalizedRoute(
                "JobPostingSeo",
                "jobs/{jobOrderId}/{seoName}",
                new { controller = "JobPost", action = "JobDetails", seoName = UrlParameter.Optional },
                new { jobOrderId = @"^\d+$" },
                new[] { "Wfm.Web.Controllers" });
            
            #endregion

            //Job Category
            #region Job Category

            //routes.MapLocalizedRoute("JobOrderCategory",
            //                 "jobpost/{jobordercategoryid}",
            //                 new { controller = "JobPost", action = "Index", seoName = UrlParameter.Optional },
            //                 new[] { "Wfm.Web.Controllers" });

            routes.MapLocalizedRoute("JobOrderCategory",
                "job-category/{jobordercategoryid}",
                new { controller = "JobPost", action = "JobCategoriesSeo", seoName = UrlParameter.Optional },
                new { jobordercategoryid = @"^\d+$" },
                new[] { "Wfm.Web.Controllers" });


            routes.MapLocalizedRoute("JobOrderCategorySEO",
                "job-category/{jobordercategoryid}/{seoName}",
                new { controller = "JobPost", action = "JobCategories", seoName = UrlParameter.Optional },
                new { jobordercategoryid = @"^\d+$" },
                new[] { "Wfm.Web.Controllers" });


            routes.MapLocalizedRoute("JobOrderCategoryPageSEO",
                "job-category/{jobordercategoryid}/{seoName}/{page}",
                new { controller = "JobPost", action = "JobCategories", seoName = UrlParameter.Optional },
                new { jobordercategoryid = @"^\d+$" },
                new[] { "Wfm.Web.Controllers" });

            #endregion

            //Others
            #region Others

            //robots.txt
            routes.MapRoute("robots.txt",
                "robots.txt",
                new { controller = "Common", action = "RobotsTextFile" },
                new[] { "Wfm.Web.Controllers" });

            // page not found
            routes.MapLocalizedRoute("PageNotFound",
                "page-not-found",
                new { controller = "Common", action = "PageNotFound" },
                new[] { "Wfm.Web.Controllers" });

            // access denied
            routes.MapLocalizedRoute("AccessDenied",
                "access-denied",
                new { controller = "Common", action = "AccessDenied" },
                new[] { "Wfm.Web.Controllers" });

            // unknown error
            routes.MapLocalizedRoute("UnknownError",
                "unknown-error",
                new { controller = "Common", action = "UnknownError" },
                new[] { "Wfm.Web.Controllers" });

            // yes or no
            routes.MapLocalizedRoute("YesOrNo",
                "yes-or-no",
                new { controller = "Common", action = "YesOrNo" },
                new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("PrivacyPolicy",
                "privacy-policy",
                new { controller = "Home", action = "PrivacyPolicy" },
                new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("TermsOfUse",
                  "terms-of-use",
                new { controller = "Home", action = "TermsOfUse" },
                new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("ApplicationAgreement",
                "application-agreement",
                new { controller = "Home", action = "ApplicationAgreement" },
                new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("ClientPrivacyPolicy", "PrivacyPolicy/",
                new { controller = "Home", action = "ClientPrivacyPolicy" },
                  new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("ClientApplicationAgreement",
                "ApplicationAgreement",
                new { controller = "Home", action = "ClientApplicationAgreement" },
                new[] { "Wfm.Web.Controllers" });

            routes.MapRoute("ClientTermsOfUse",
                "TermsOfUse",
                new { controller = "Home", action = "ClientTermsOfUse" },
                new[] { "Wfm.Web.Controllers" });

            //routes.MapLocalizedRoute("DeletePM",
            //                "deletepm/{privateMessageId}",
            //                new { controller = "PrivateMessages", action = "DeletePM" },
            //                new { privateMessageId = @"\d+" },
            //                new[] { "Wfm.Web.Controllers" });

            #endregion

            ////worktime
            //routes.MapRoute("xxx",
            //     "xxx/{Id}",
            //     new { controller = "City", action = "Edit", area = "" });

        }

        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
