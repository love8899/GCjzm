using System.Collections.Generic;
using Wfm.Core.Domain.Localization;

namespace Wfm.Services.Localization
{
    public partial interface ILanguageService
    {
        void Insert(Language language);

        void Update(Language language);

        void Delete(Language language);


        Language GetLanguageById(int languageId);

        int GetLanguageIdByName(string name);

        IList<Language> GetAllLanguages(bool showHidden = false);

        string ExportLanguageToXml(Language language);
    }
}
