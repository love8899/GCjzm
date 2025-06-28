using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Payroll;

namespace Wfm.Services.Payroll
{
    public interface IPayGroupService
    {
        bool DeletePayGroupById(int payGroupId, out string errorMessage);
        IList<PayGroup> GetAllPayGroupsByFranchiseId(int franchiseId);
        IQueryable<PayGroup> GetAllPayGroups();
        List<PayGroup> GetAllPayGroups(int year);
        PayGroup GetPayGroupById(int? id);
        int GetPayGroupIdByName(string name);
        void InsertPayGroup(PayGroup PayGroup);
        void UpdatePayGroup(PayGroup PayGroup, string[] excludeList);
    }
}
