namespace Wfm.Core.Domain.Accounts
{
    public static partial class SystemAccountAttributeNames
    {
        //Form fields
        public static string FirstName { get { return "FirstName"; } }
        public static string MiddleName { get { return "LastName"; } }
        public static string LastName { get { return "LastName"; } }
        public static string Gender { get { return "Gender"; } }
        public static string DateOfBirth { get { return "DateOfBirth"; } }
        public static string Company { get { return "Company"; } }
        public static string AddressLine1 { get { return "AddressLine1"; } }
        public static string AddressLine2 { get { return "AddressLine2"; } }
        public static string ZipPostalCode { get { return "ZipPostalCode"; } }
        public static string City { get { return "City"; } }
        public static string Country { get { return "CountryId"; } }
        public static string StateProvince { get { return "StateProvinceId"; } }
        public static string Phone { get { return "Phone"; } }
        public static string Fax { get { return "Fax"; } }
        public static string TimeZoneId { get { return "TimeZoneId"; } }

        //Other attributes
        public static string AvatarPictureId { get { return "AvatarPictureId"; } }
        public static string ForumPostCount { get { return "ForumPostCount"; } }
        public static string Signature { get { return "Signature"; } }
        public static string PasswordRecoveryToken { get { return "PasswordRecoveryToken"; } }
        public static string AccountActivationToken { get { return "AccountActivationToken"; } }
        public static string TokenExpiryDate { get { return "TokenExpiryDate"; } }

        public static string LastVisitedPage { get { return "LastVisitedPage"; } }
        public static string AdminAreaFranchiseScopeConfiguration { get { return "AdminAreaFranchiseScopeConfiguration"; } }
        public static string LanguageId { get { return "LanguageId"; } }
        public static string NotifiedAboutNewPrivateMessages { get { return "NotifiedAboutNewPrivateMessages"; } }
        public static string DontUseMobileVersion { get { return "DontUseMobileVersion"; } }

        public static string WorkingThemeName { get { return "WorkingThemeName"; } }
    }
}