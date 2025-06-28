namespace Wfm.Core.Domain.Candidates
{
    public enum CandidateOnboardingStatusEnum
    {
        NoStatus = 1,
        Started = 2,
        Canceled = 3,
        Finished = 4
    }

    public enum CandidateDocumentTypeEnum
    {
        LETTEROFHIRE = 1,
        VOIDCHECK = 2 ,
        PHOTOID = 3,
        RESUME = 4,
        OTHER = 5,
        CERTIFICATES = 6,
        CLIENTDOCUMENTS = 7,
        FORKLIFTLICENSE = 8,
        SIN = 9
    }
}
