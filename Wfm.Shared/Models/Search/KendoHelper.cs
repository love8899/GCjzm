using Kendo.Mvc;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using Wfm.Core.Infrastructure;
using Wfm.Services.Companies;


namespace Wfm.Shared.Models.Search
{
    public partial class KendoHelper
    {
        public static void GetGeneralFilters([DataSourceRequest] DataSourceRequest request, object model, 
            string fromName = "From", string toName = "To", 
            IList<string> skip = null, IDictionary<string, string> mapping = null, IList<string> nameCols = null)
        {
            // reset filters, or add/update filters???
            request.Filters = new List<IFilterDescriptor>();

            foreach (var p in model.GetType().GetProperties())
            {
                var name = p.Name.TrimStart("sf_".ToCharArray());
                if (skip != null && skip.Contains(name))
                    continue;

                if (mapping != null && mapping.ContainsKey(name))
                    name = mapping[name];

                var value = p.GetValue(model, null);
                if (value != null)
                {
                    if (value.GetType() == typeof(int))
                    {
                        if ((int)value > 0)
                        {
                            if (nameCols != null && nameCols.Contains(name))    // gird columns using NAME i.s.o. ID
                            {
                                var orgNameSvc = EngineContext.Current.Resolve<IOrgNameService>();
                                var orgName = orgNameSvc.GetOrgNameById(name, (int)value, out string org);
                                if (!String.IsNullOrWhiteSpace(org) && !String.IsNullOrWhiteSpace(orgName))
                                    request.Filters.Add(new FilterDescriptor(org, FilterOperator.IsEqualTo, orgName));
                            }
                            else    // normal case
                                request.Filters.Add(new FilterDescriptor(name, FilterOperator.IsEqualTo, value));
                        }
                    }

                    else if (value.GetType() == typeof(bool) && (bool)value)   // only apply for true
                    {
                        request.Filters.Add(new FilterDescriptor(name, FilterOperator.IsEqualTo, value));
                    }

                    else if (value.GetType() == typeof(string))
                    {
                        if (!String.IsNullOrWhiteSpace((string)value))
                            request.Filters.Add(new FilterDescriptor(name, FilterOperator.Contains, value));
                    }

                    else if (value.GetType() == typeof(DateTime))
                    {
                        if (!(name == fromName || name == toName))      //  other than date range which filtered already
                            request.Filters.Add(new FilterDescriptor(name, FilterOperator.IsEqualTo, value));
                    }
                }
            }
        }


        // for SearchJobOrderModel using Start/End as date range of job order
        public static void CustomizeJobOrderBasedFilters([DataSourceRequest] DataSourceRequest request, object model,
            IList<string> skip = null, IDictionary<string, string> mapping = null, IList<string> nameCols = null)
        {
            GetGeneralFilters(request, model, "Start", "End", skip, mapping, nameCols);
        }


        // for SearchPlacementModel and derived, using From/To as date range, default way
        public static void CustomizePlacementBasedFilters([DataSourceRequest] DataSourceRequest request, object model,
            IList<string> skip = null, IDictionary<string, string> mapping = null, IList<string> nameCols = null)
        {
            if (skip == null)
                skip = new List<string>();
            skip.Add("Start"); skip.Add("End");     // from SearchJobOrderModel, not relevant any more

            GetGeneralFilters(request, model, skip: skip, mapping: mapping, nameCols: nameCols);
        }
    }
}
