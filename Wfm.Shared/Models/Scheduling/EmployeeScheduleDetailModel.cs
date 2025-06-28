using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Scheduling;
using Wfm.Services.Scheduling;

namespace Wfm.Shared.Models.Scheduling
{
    public class EmployeeScheduleDetailModel : IEmployeeScheduleDetailModel
    {
        public int CompanyJobRoleId
        {
            get; set;
        }

        public long EndTimeTicks
        {
            get; set;
        }

        public string EntryTitle
        {
            get; set;
        }

        public IEnumerable<IScheduleDetailErrorModel> ErrorWarnings
        {
            get; set;
        }

        public DateTime ScheduleDate
        {
            get; set;
        }

        public long StartTimeTicks
        {
            get; set;
        }
    }

    public class ScheduleDetailErrorModel : IScheduleDetailErrorModel
    {
        public string ErrorMessage
        {
            get; set;
        }
        public ScheduleWarningScope Scope
        {
            get; set;
        }
        public ScheduleWarningLevel LevelOfError
        {
            get; set;
        }
        public int? EmployeeId
        {
            get; set;
        }
        public override bool Equals(object obj)
        {
            var _obj = obj as ScheduleDetailErrorModel;
            if (_obj != null)
            {
                return _obj.LevelOfError == this.LevelOfError && string.Equals(_obj.ErrorMessage, this.ErrorMessage);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ErrorMessage.GetHashCode() + LevelOfError.GetHashCode();
        }
    }
}
