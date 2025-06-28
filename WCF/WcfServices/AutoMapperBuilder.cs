using AutoMapper;
using Wfm.Core.Domain.Candidates;
using WcfServices.TimeSheets;


namespace WcfServices
{
    public static class AutoMapperBuilder 
    {
        public static void CreateMap()
        {
            Mapper.CreateMap<EmployeeTimeChartHistory, SimpleEmployeeTimeChartHistory>();
        }
    }
}