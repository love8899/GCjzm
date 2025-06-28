using System;
using System.Collections.Generic;
using RecogSys.RdrAccess;


namespace Wfm.Core.Domain.ClockTime
{
    public class HandReader : IDisposable
    {
        // constants to deal with int return of BOOL methods
        private const int TRUE = 1;
        private const int FALSE = 0;

        private CRsiHandReader _reader;
        private CRsiNetwork _net;
        private CRsiComWinsock _com;

        public HandReader(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new ArgumentNullException("ipAddress");

            _SetupReader(ipAddress);
        }


        private bool _SetupReader(string ipAddress = null)
        {
            if (_SetupCom(ipAddress))
            {
                //_reader = new CRsiHandReader(_com, 0);
                _net = new CRsiNetwork(_com);
                _reader = new CRsiHandReader(_net, 0);
            }

            return _reader != null;
        }


        private bool _SetupCom(string ipAddress = null)
        {
            var result = false;

            string host = null;
            ushort port = 0;
            if (!String.IsNullOrWhiteSpace(ipAddress))
            {
                var address = ipAddress.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (address.Length == 2 && !String.IsNullOrWhiteSpace(address[1]))
                {
                    host = address[0];
                    ushort.TryParse(address[1], out port);
                }
            }
            else if (_com != null)
            {
                host = _com.GetHost();
                port = _com.GetPort();
                this.Dispose();     // ensures nothing is hanging so we can reconnect
            }

            if (!string.IsNullOrWhiteSpace(host))
            {
                _com = new CRsiComWinsock();
                _com.SetHost(host);
                if (port > 0)
                    _com.SetPortA(port);
                result = true;
            }

            return result;
        }


        public bool TryConnect()
        {
            var result = FALSE;

            var connected = 0;
            var connectAttempts = 0;
            while (connected < 2 && connectAttempts < 4)   // the 2nd connected is the REAL target, for port forwarding
            {
                connectAttempts += 1;
                result = _com.Connect();
                if (result != FALSE)
                {
                    connected += 1;
                    if (connected < 2)
                    {
                        _com.Disconnect();
                        _SetupReader();     // re-setup reader for reconnection
                    }
                }
                else
                {
                    _com.Disconnect();
                    if (_com.ResetSocket() == FALSE)
                        _SetupReader();     // re-setup reader for reconnection
                }
            }

            if (connected > 0)
            {
                _reader.CmdGetStatus();     // The first command to a reader should always be CmdGetStatus
                result = _com.IsConnected();
            }

            return result != FALSE;
        }


        public bool IsConnected()
        {
            return _com != null && _reader.IsConnected() != FALSE;
        }


        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            _reader.SetName(name);
        }


        public string GetName()
        {
            return _reader.GetName();
        }


        public DateTime? GetTime()
        {
            var clockTime = new RSI_TIME_DATE();
            if (_reader.CmdGetTime(clockTime) != FALSE)
                return CommonHelper.ToDateTime(clockTime);
            else
                return null;
        }


        public bool PutTime(DateTime dateTime)
        {
            return _reader.CmdPutTime(CommonHelper.ToRsiTimeDate(dateTime)) != FALSE;
        }


        public bool SyncClockTime(int? minDrift, int? maxDrift)
        {
            var result = false;

            var clockTime = this.GetTime();
            if (clockTime.HasValue)
            {
                var timeNow = DateTime.Now;
                var drift = Math.Abs((clockTime.Value - timeNow).TotalSeconds);
                if (drift > (minDrift ?? 5) && (!maxDrift.HasValue || drift < maxDrift))
                    result = this.PutTime(timeNow);
            }

            return result;
        }


        public RSI_SETUP_DATA GetSetup()
        {
            var setupData = new RSI_SETUP_DATA();
            if (_reader.CmdGetSetup(setupData) != FALSE)
                return setupData;
            else
                return null;
        }


        public bool PutSetup(RSI_SETUP_DATA setupData)
        {
            return _reader.CmdPutSetup(setupData) != FALSE;
        }


        public bool SetDayLightSavingTimes(int? refYear)
        {
            var result = false;

            var setupData = this.GetSetup();
            if (setupData != null)
            {
                var year = refYear ?? DateTime.Today.Year;
                var dstStart = CommonHelper.GetDesiredWeekDay(year, 3, 2, DayOfWeek.Sunday).AddHours(2);    // 2am, 2nd Sunday of March
                var dstEnd = CommonHelper.GetDesiredWeekDay(year, 11, 1, DayOfWeek.Sunday).AddHours(2);     // 2am, 1st Sunday of November

                setupData.pDlsOn.year = setupData.pDlsOff.year = (byte) (year - 2000);  // year===0 in setup data returned!!!
                var clkDstStart = CommonHelper.ToDateTime(setupData.pDlsOn);
                var clkDstEnd = CommonHelper.ToDateTime(setupData.pDlsOff);

                if (!clkDstStart.HasValue || !clkDstEnd.HasValue || clkDstStart != dstStart || clkDstEnd != dstEnd)
                {
                    setupData.pDlsOn = CommonHelper.ToRsiTimeDate(dstStart);
                    setupData.pDlsOff = CommonHelper.ToRsiTimeDate(dstEnd);
                    result = this.PutSetup(setupData);
                }
            }

            return result;
        }


        public RSI_READER_INFO GetInfo(bool fromReader = false)
        {
            if (fromReader)
                return _GetInfoFromReader();

            return _reader.GetInfo();
        }


        private RSI_READER_INFO _GetInfoFromReader()
        {
            var rdrInfo = new RSI_READER_INFO();
            _reader.CmdGetReaderInfo(rdrInfo);
            _reader.SetInfo(rdrInfo);

            return rdrInfo;
        }


        public bool Beep(int len, int num)
        {
            return _reader.CmdBeep((ushort)len, (ushort)num) != FALSE;
        }


        public bool DisplayMessage(string msg)
        {
            msg = msg ?? string.Empty;

            var displayMsg = new RSI_DISPLAY_MESSAGE();
            displayMsg.pMsg.Set(msg);
            return _reader.CmdPutDisplayMessage(displayMsg) != FALSE;
        }


        public bool ClearUserDatabase(RSI_STATUS rsp)
        {
            return _reader.CmdClearUserDatabase(rsp) != FALSE;
        }


        public bool GetUser(RSI_USER_RECORD pUser)
        {
            return _reader.CmdGetUser(pUser.pID, pUser) != FALSE;
        }


        public bool PutUserRecord(RSI_USER_RECORD pUser, RSI_STATUS rsp)
        {
            return _reader.CmdPutUserRecord(pUser, rsp) != FALSE;
        }


        public bool RemoveUser(string idStr, RSI_STATUS rsp)
        {
            return _reader.CmdRemoveUser(idStr, rsp) != FALSE;
        }


        public IList<CRsiDataBank> GetAllUserData()
        {
            var banks = new List<CRsiDataBank>();

            var rdrInfo = new RSI_READER_INFO();
            int banksToGet;

            var retrievedBank = new CRsiDataBank();
            var backupMessage = new RSI_DISPLAY_MESSAGE();

            var rtn = _reader.CmdGetReaderInfo(rdrInfo);
            if (rtn != FALSE && rdrInfo.usersEnrolled > 0)
            {
                _reader.CmdEnterIdleMode();     // idle mode to prevent change

                backupMessage.userSpecific = 0;
                backupMessage.pMsg.Set("Please Wait--   Backing Up");   // spaces to force new line
                _reader.CmdPutDisplayMessage(backupMessage);

                if (rdrInfo.model == RSI_MODEL.RSI_MODEL_HP4K)
                    banksToGet = (int)(Math.Ceiling(rdrInfo.usersEnrolled / 53.0));
                else
                    banksToGet = (int)(Math.Ceiling(rdrInfo.usersEnrolled / 256.0));

                for (ushort counter = 0; counter < banksToGet; counter++)
                {
                    rtn = _reader.CmdGetDataBank(counter, retrievedBank);
                    if (rtn != FALSE && retrievedBank != null)
                        banks.Add(retrievedBank);
                }

                backupMessage.pMsg.Set("-    READY     -");
                rtn = _reader.CmdPutDisplayMessage(backupMessage);

                rtn = _reader.CmdExitIdleMode();
            }

            return banks;
        }


        public bool PutUserData(IList<CRsiDataBank> banks)
        {
            var rtn = _reader.CmdEnterIdleMode();
            if (rtn != FALSE && banks.Count > 0)
            {
                var restoreMessage = new RSI_DISPLAY_MESSAGE();
                restoreMessage.userSpecific = 0;
                restoreMessage.pMsg.Set("Please Wait--   Restoring Users");
                rtn = _reader.CmdPutDisplayMessage(restoreMessage);

                for (ushort counter = 0; counter < banks.Count; counter++)
                {
                    var storedBank = banks[counter];
                    rtn = _reader.CmdPutDataBank(counter, storedBank);
                }

                restoreMessage.pMsg.Set("-    READY     -");
                rtn = _reader.CmdPutDisplayMessage(restoreMessage);

                rtn = _reader.CmdExitIdleMode();
            }

            return rtn != FALSE;
        }


        // for non web apps or testing
        public string EnrollUser(RSI_PROMPT aPrompt, RSI_STATUS rsp, RSI_USER_RECORD userRecord, bool addUser = true)
        {
            var error = string.Empty;

            if (_reader.CmdGetStatus() == TRUE)
            {
                var _rdrStatus = new RSI_STATUS();
                var rtn = _reader.CmdEnrollUser(aPrompt, _rdrStatus);
                _reader.CmdBeep(2, 3);
                //var busy = _rdrStatus.cmd_bsy != 0;
                //var failed = _rdrStatus.failed_cmd != 0;
                var busy = true;
                var failed = false;

                while (rtn == 1 && busy && !failed)
                {
                    // if it is responding but busy, check status every second until it is no longer busy or the cmd fails
                    System.Threading.Thread.Sleep(1000);
                    rtn = _reader.CmdGetStatus(_rdrStatus);
                    if (rtn == 1)
                    {
                        busy = (_rdrStatus.cmd_bsy != 0);
                        failed = (_rdrStatus.failed_cmd != 0);
                    }

                    if (!busy && !failed)
                    {
                        rtn = GetLastEnrolledUser(userRecord, addUser) ? 1 : 0;
                        if (rtn != FALSE)
                            break;
                    }
                }

                if (rtn == 0 || failed)
                    error = "Enrollment of user " + userRecord.pID.GetID() + " failed.";
            }
            else
                error = "Enrollment failed as the hand reader is not ready";

            return error;
        }


        public bool StartEnrolling(RSI_PROMPT aPrompt, RSI_STATUS rsp)
        {
            return _reader.CmdEnrollUser(aPrompt, rsp) != FALSE;
        }


        public bool GetLastEnrolledUser(RSI_USER_RECORD userRecord, bool addUser = true)
        {
            var result = false;

            var lastTemplate = new RSI_LAST_TEMPLATE();
            if (GetLastTemplate(lastTemplate))
            {
                InitUserRecord(userRecord);
                userRecord.pTemplateVector = lastTemplate.pTemplateVector;

                if (addUser)    // add user to clock
                {
                    var rsp = new RSI_STATUS();
                    result = PutUserRecord(userRecord, rsp);
                }
                else
                    result = true;
            }

            return result;
        }


        public void InitUserRecord(RSI_USER_RECORD userRecord,
            RSI_AUTHORITY_LEVEL? authorityLevel = null, int? rejectThreshold = null, int? timeZone = null)
        {
            userRecord.authorityLevel = authorityLevel ?? RSI_AUTHORITY_LEVEL.RSI_AUTHORITY_NONE;
            if (rejectThreshold.HasValue)
                userRecord.rejectThreshold = (ushort)rejectThreshold.Value;
            if (timeZone.HasValue)
                userRecord.timeZone = (byte)(timeZone.Value);
        }


        public bool GetLastTemplate(RSI_LAST_TEMPLATE lastTemplate)
        {
            return _reader.CmdGetTemplate(lastTemplate) != FALSE;
        }


        public bool DisplaySelectedAccessMessage(RSI_MESSAGE_CODE code, int time)
        {
            return _reader.CmdDisplaySelectedAccessMessage(code, Convert.ToByte(time)) != FALSE;
        }


        public IList<RSI_DATALOG> GetDataLog(out string error)
        {
            error = string.Empty;
            var log = new List<RSI_DATALOG>();

            int rtn = 1;
            bool hasLog = false;

            var rdrStatus = new RSI_STATUS();
            rtn = _reader.CmdGetStatus(rdrStatus);
            hasLog = rdrStatus.dlog_rdy > 0;
            while (rtn != FALSE && hasLog)
            {
                var dataLog = new RSI_DATALOG();
                rtn = _reader.CmdGetDatalog(dataLog);
                if (rtn == FALSE)
                {
                    int attempts = 0;
                    while (attempts < 3 && rtn == FALSE)    // retry 3 times
                    {
                        rtn = _reader.CmdGetPreviousDatalog(dataLog);
                        attempts++;
                    }
                }

                if (rtn != FALSE && dataLog != null)
                    log.Add(dataLog);

                rtn = _reader.CmdGetStatus(rdrStatus);
                hasLog = rdrStatus.dlog_rdy > 0;
            }

            if (rtn == FALSE)
                error = "Error while retrieving data logs from the reader";

            return log;
        }


        public void Dispose()
        {
            if (_net != null)
            {
                _net.SetCom(null);
                _net.Dispose();
                _net = null;
            }

            if (_com != null)
            {
                _com.Dispose();
                _com = null;
            }
        }
    }

}
