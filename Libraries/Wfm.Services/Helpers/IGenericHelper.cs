
namespace Wfm.Services.Helpers
{
    /// <summary>
    /// Represents a Generic Helper
    /// </summary>
    public partial interface IGenericHelper
    {
        bool IsSearchableDigits(string str);
        string ToSearchableString(string str);

        bool FileCompare(string file1, string file2);

    }
}