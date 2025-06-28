using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wfm.Admin.Models.Payroll
{
    public class TaxFormBoxModel
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string Form { get; set; }
        public string Box { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}