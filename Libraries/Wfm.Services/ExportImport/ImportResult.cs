using System.Collections.Generic;
using System.IO;


namespace Wfm.Services.ExportImport
{
    public class ImportResult
    {
        public int Attempted { get; set; }
        public int Imported { get; set; }
        public int NotImported { get; set; }
        public IList<string> ErrorMsg { get; set; }
    }

}
