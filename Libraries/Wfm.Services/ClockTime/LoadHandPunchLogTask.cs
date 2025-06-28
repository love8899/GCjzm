using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RecogSys.RdrAccess;
using Wfm.Core;
using Wfm.Services.Logging;
using Wfm.Services.Tasks;
using Wfm.Core.Domain.ClockTime;


namespace Wfm.Services.ClockTime
{
    public partial class LoadHandPunchLogTask : IScheduledTask
    {
        #region Fields

        private readonly IClockDeviceService _clockDeviceService;
        private readonly IClockCandidateService _clockCandidateService;
        private readonly IClockTimeService _clockTimeService;
        private readonly ISmartCardService _smartCardService;
        private readonly IHandTemplateService _handTemplateService;
        private readonly ILogger _logger;

        #endregion


        #region Ctor

        public LoadHandPunchLogTask(
            IClockDeviceService clockDeviceService,
            IClockCandidateService clockCandidateService,
            IClockTimeService clockTimeService,
            ISmartCardService smartCardService,
            IHandTemplateService handTemplateService,
            ILogger logger)
        {
            this._clockDeviceService = clockDeviceService;
            this._clockCandidateService = clockCandidateService;
            this._clockTimeService = clockTimeService;
            this._smartCardService = smartCardService;
            this._handTemplateService = handTemplateService;
            this._logger = logger;
        }

        #endregion


        public virtual void Execute()
        {
            try
            {
                var clocks = _clockDeviceService.GetAllClockDevicesWithIPAddress(excludeEnrolment: false).ToList();     // include enrolment clock
                foreach (var clock in clocks)
                {
                    var logs = new List<RSI_DATALOG>();

                    using (var hr = new HandReader(clock.IPAddress))
                    {
                        var error = string.Empty;
                        if (hr != null && hr.TryConnect())
                            logs = hr.GetDataLog(out error).ToList();
                        else
                            error = String.Format("The clock {0} is not reachable.", clock.ClockDeviceUid);

                        if (!String.IsNullOrWhiteSpace(error))
                            _logger.Error(String.Concat("Error occurred while loading hand punch log into database: ", error));
                    }

                    if (logs.Any() && clock.AddOnEnroll)    // exclude enrolment only clocks
                    {
                        var errors = _ProcessHandPunchLog(clock, logs);
                        if (errors.Any())
                            _logger.Error(String.Join("; ", errors));
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.Error(string.Format("Error occurred while loading hand punch log into database. Error message : {0}", exc.Message), exc);
            }
        }


        public async Task ExecuteAsync()
        {
            await Task.Run(() => this.Execute());
        }


        private List<string> _ProcessHandPunchLog(CompanyClockDevice clock, IList<RSI_DATALOG> logs)
        {
            var errors = new List<string>();

            foreach (var log in logs)
            {
                switch (log.format)
                {
                    case RSI_DATALOG_FORMAT.RSI_DLF_IDENTITY_VERIFIED:
                        _LoadHandPunchClockTime(clock, log, errors);
                        break;

                    case RSI_DATALOG_FORMAT.RSI_DLF_USER_ENROLLED:
                        _LoadHandTemplates(clock, log, errors);
                        break;
                }
            }

            return errors;
        }


        private void _LoadHandPunchClockTime(CompanyClockDevice clock, RSI_DATALOG log, List<string> errors)
        {
            int? candidateId = null;
            var smartCardUid = string.Empty;

            var idStr = log.pOperand.GetID();
            var clockInOut = CommonHelper.ToDateTime(log.pTimestamp);

            if (clock.ManualID)
            {
                candidateId = Convert.ToInt32(idStr);
                smartCardUid = string.Format("H{0}", candidateId.ToString().PadLeft(9, '0'));
            }
            else
            {
                var candidate = _smartCardService.GetCandidateByIdString(clock, idStr, out smartCardUid, refDate: clockInOut.Value.Date);
                if (candidate != null)
                    candidateId = candidate.Id;
            }

            idStr = clock.IDLength.HasValue && clock.IDLength != 10 ? idStr.Substring(10 - clock.IDLength.Value) : idStr;   // remove leading zeros for non 10-digits
            var error = _clockTimeService.AddClockTime(clock.ClockDeviceUid, 0, smartCardUid, clockInOut.Value, idStr, candidateId);
            if (!String.IsNullOrWhiteSpace(error))
                errors.Add(error);
        }


        private void _LoadHandTemplates(CompanyClockDevice clock, RSI_DATALOG log, List<string> errors)
        {
            var result = false;

            var idStr = log.pOperand.GetID();
            var userRecord = new RsiUserRecord();
            userRecord.pID.SetID(idStr);
            using (var hr = new HandReader(clock.IPAddress))
            {
                if (hr != null && hr.TryConnect())
                    result = hr.GetUser(userRecord);
            }

            if (result)
            {
                var candidateIdStr = idStr;
                HandTemplate template = null;
                if (clock.ManualID)
                    template = userRecord.ToHandTemplate();
                else
                {
                    var candidate = _smartCardService.GetCandidateByIdString(clock, idStr, out string smartCardUid, refDate: DateTime.Today);
                    if (candidate != null)
                    {
                        candidateIdStr = candidate.Id.ToString();
                        template = userRecord.ToHandTemplate(candidateIdStr);
                    }
                    else
                        errors.Add(string.Format("Error occurred while geting hand template of the new enrolled employee ({0}): not identified.", idStr));
                }

                if (template != null)
                {
                    template.EnteredBy = Convert.ToInt32(log.pOperator.GetID());
                    _handTemplateService.InsertOrUpdate(template);

                    _clockCandidateService.InsertOrUpdate(clock.Id, Convert.ToInt32(candidateIdStr));
                }
            }
            else
                errors.Add(string.Format("Error occurred while geting hand template of the new enrolled employee ({0}) from clock {1}.", idStr, clock.ClockDeviceUid));
        }

    }
}
