using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Wfm.Data;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.TimeSheet;


namespace WcfServices.BL.MobilePunch
{
    public class MobilePunchBL
    {
        public void ProcessMobilePunch(WcfSession session, MobilePunchEntry mobilePunch, BasicResult result)
        {
            // The candidate MUST be placed to a job order for the client; otherwise we can't create the valid punch entry for him/her
            Common _common = new Common();
            var workTimeSettings = _common.LoadSettings<CandidateWorkTimeSettings>().GetAwaiter().GetResult();

            const string query =
            @"select Account.CompanyId, cmp.CompanyName, jo.CompanyLocationId, csc.SmartCardUid, 
                   csc.CandidateId, c.LastName as CandidateLastName, c.FirstName as CandidateFirstName, ccd.ClockDeviceUid
            from CandidateJobOrder cjo
              inner join CandidateJobOrderStatus cjos on cjo.CandidateJobOrderStatusId = cjos.Id and cjos.StatusName = 'Placed' 
                                                     and cjo.StartDate <= Convert(date, @punch) and (cjo.EndDate is Null OR cjo.EndDate >= Convert(date, @punch) ) 
              inner join JobOrder jo on cjo.JobOrderId = jo.id and jo.IsDeleted = 0 
			                        and (  -- apply the time check too (from settings)
                                         convert(time, DateAdd(MINUTE, @earlierStartTime, jo.StartTime) ) <= Convert(time, @punch)
                                         or
									     convert(time, DateAdd(MINUTE, @laterEndTime, jo.EndTime) ) >= Convert(time, @punch)
                                        )
              inner join Account on jo.CompanyId = account.CompanyId
              inner join Candidate c on cjo.CandidateId = c.Id and c.FranchiseId = jo.FranchiseId
              inner join CandidateSmartCard csc on c.Id = csc.CandidateId and csc.IsActive = 1 and csc.IsDeleted = 0
              inner join Company cmp on account.CompanyId = cmp.Id and cmp.IsActive = 1 
              left outer join CompanyClockDevice ccd on jo.CompanyLocationId = ccd.CompanyLocationId and ccd.IsActive = 1 and ccd.IsDeleted = 0
            Where c.CandidateGuid = @candidateGuid and c.Id = @candidateId
              and account.Id = @accountId";

            List<CandidateClockTime> clockTimeEntries = new List<CandidateClockTime>();

            SqlDatabase database = new SqlDatabase(Common.GetConnectionString());
            using (DbCommand command = database.GetSqlStringCommand(query))
            {
                database.AddInParameter(command, "accountId", DbType.Int32, session.AccountId);
                database.AddInParameter(command, "candidateGuid", DbType.String, mobilePunch.CandidateGuid.ToString());
                database.AddInParameter(command, "candidateId", DbType.String, mobilePunch.CandidateId);
                database.AddInParameter(command, "punch", DbType.DateTime, mobilePunch.Punch);
                database.AddInParameter(command, "earlierStartTime", DbType.Int32, -workTimeSettings.MatchBeforeStartTimeInMinutes);
                database.AddInParameter(command, "laterEndTime", DbType.Int32, workTimeSettings.MatchAfterEndTimeInMinutes);

                using (var objReader = database.ExecuteReader(command))
                {
                    clockTimeEntries = objReader.DataReaderToObjectList<CandidateClockTime>();
                }
            }

            if (clockTimeEntries.Count == 0)
            {
                // Can we find the candidate?
                using (DbCommand command = database.GetSqlStringCommand(
                                @"select top 1 csc.SmartCardUid, csc.CandidateId, c.LastName as CandidateLastName, c.FirstName as CandidateFirstName
                                from  Candidate c 
                                    left join CandidateSmartCard csc on c.Id = csc.CandidateId and csc.IsActive = 1 and csc.IsDeleted = 0
                                Where c.CandidateGuid = @candidateGuid and c.Id = @candidateId"))
                {
                    database.AddInParameter(command, "candidateGuid", DbType.String, mobilePunch.CandidateGuid.ToString());
                    database.AddInParameter(command, "candidateId", DbType.String, mobilePunch.CandidateId);

                    using (var objReader = database.ExecuteReader(command))
                    {
                        clockTimeEntries = objReader.DataReaderToObjectList<CandidateClockTime>();
                    }
                }

                if (clockTimeEntries.Count == 0)
                {
                    result.Message = "Employee record not found!";
                    clockTimeEntries.Add(new CandidateClockTime());
                }
                else
                {
                    result.Message = "Employee is not placed in the job order!";
                    // candidate record is found, now try to match the job order
                    using (DbCommand command = database.GetSqlStringCommand(
                              @"select distinct cmp.CompanyName, jo.CompanyLocationId, ccd.ClockDeviceUid
                                from  Account
                                    inner join Company cmp on account.CompanyId = cmp.Id and cmp.IsActive = 1 
	                                left  join JobOrder jo on cmp.Id = jo.CompanyId and jo.IsDeleted = 0 and jo.StartDate <= Convert(date, @punch) and (jo.EndDate is Null or jo.EndDate >= Convert(date, @punch))
	                                left  join CompanyClockDevice ccd on jo.CompanyLocationId = ccd.CompanyLocationId and ccd.IsActive = 1 and ccd.IsDeleted = 0
	                                left  join CandidateJobOrder cjo on jo.Id = cjo.JobOrderId 
                                                                   and cjo.StartDate <= Convert(date, @punch) and (cjo.EndDate is Null OR cjo.EndDate >= Convert(date, @punch) ) 
                                    inner join CandidateJobOrderStatus cjos on cjo.CandidateJobOrderStatusId = cjos.Id and cjos.StatusName = 'Placed' 

                                Where cjo.CandidateId = @candidateId  and  account.Id = @accountId"))
                    {
                        database.AddInParameter(command, "accountId", DbType.Int32, session.AccountId);
                        database.AddInParameter(command, "candidateId", DbType.String, mobilePunch.CandidateId);
                        database.AddInParameter(command, "punch", DbType.DateTime, mobilePunch.Punch);

                        using (var objReader = database.ExecuteReader(command))
                        {
                            if (objReader.Read())
                            {
                                if (objReader["CompanyLocationId"] != DBNull.Value) clockTimeEntries[0].CompanyLocationId = Convert.ToInt32(objReader["CompanyLocationId"]);
                                if (objReader["ClockDeviceUid"] != DBNull.Value) clockTimeEntries[0].ClockDeviceUid = Convert.ToString(objReader["ClockDeviceUid"]);
                                clockTimeEntries[0].CompanyName = Convert.ToString(objReader["CompanyName"]);
                            }
                        }
                    }
                }

                clockTimeEntries[0].ClockInOut = mobilePunch.Punch;
                clockTimeEntries[0].CompanyId = session.UserAccount.CompanyId;

                if (String.IsNullOrWhiteSpace(clockTimeEntries[0].ClockDeviceUid))
                {
                    if (session.UserAccount.CompanyLocationId > 0)
                    {
                        using (DbCommand command = database.GetSqlStringCommand(
                              @"select top 1 ClockDeviceUid from CompanyClockDevice where CompanyLocationId = @CompanyLocationId and IsActive = 1 and IsDeleted = 0"))
                        {
                            database.AddInParameter(command, "CompanyLocationId", DbType.Int32, session.UserAccount.CompanyLocationId);

                            using (var objReader = database.ExecuteReader(command))
                            {
                                if (objReader.Read())
                                {
                                    if (objReader["ClockDeviceUid"] != DBNull.Value) clockTimeEntries[0].ClockDeviceUid = Convert.ToString(objReader["ClockDeviceUid"]);
                                }
                            }
                        }
                    }
                }

                this.InsertCandidateClockTime(clockTimeEntries[0]);
            }
            else if (clockTimeEntries.Count == 1)
            {
                result.Success = true;
                result.Message = String.Concat(clockTimeEntries[0].CandidateFirstName, ", ", clockTimeEntries[0].CandidateLastName);

                clockTimeEntries[0].ClockInOut = mobilePunch.Punch;
                this.InsertCandidateClockTime(clockTimeEntries[0]);
            }
            else
            {
                result.Message = "Punch is not acceptable!";
                WcfLogger.LogError("Invalid mobile punch entry.",
                   String.Concat("WCF - Incoming QR punch entry cannot be saved: CandidateId=", mobilePunch.CandidateId, 
                                 " UserId=", session.AccountId, Environment.StackTrace),
                   session.UserAccount.Id);
            }

        }


        private void InsertCandidateClockTime(CandidateClockTime entry)
        {
            const string query =
            @"INSERT INTO CandidateClockTime (ClockDeviceUid, CompanyId, CompanyName, CompanyLocationId, RecordNumber, SmartCardUid, CandidateId, CandidateLastName,
                                              CandidateFirstName, ClockInOut, Source, EnteredBy, UpdatedBy, CreatedOnUtc, CandidateClockTimeStatusId, Note)
              VALUES
               (@ClockDeviceUid,
                @CompanyId, 
                @CompanyName, 
                @CompanyLocationId, 
                0, --RecordNumber
                @SmartCardUid,
                @CandidateId,
                @CandidateLastName, 
                @CandidateFirstName, 
                @ClockInOut, 
                Null, -- Source
                0, --EnteredBy
                0, --UpdatedBy
                GetUTCDate(), --CreatedOnUtc
                0, --CandidateClockTimeStatusId
                'Mobile Punch'
               )";

            List<CandidateClockTime> clockTimeEntries = new List<CandidateClockTime>();

            SqlDatabase database = new SqlDatabase(Common.GetConnectionString());
            using (DbCommand command = database.GetSqlStringCommand(query))
            {
                database.AddInParameter(command, "ClockDeviceUid", DbType.String, entry.ClockDeviceUid);
                database.AddInParameter(command, "CompanyId", DbType.Int32, entry.CompanyId);
                database.AddInParameter(command, "CompanyName", DbType.String, entry.CompanyName);
                database.AddInParameter(command, "CompanyLocationId", DbType.Int32, entry.CompanyLocationId);

                database.AddInParameter(command, "SmartCardUid", DbType.String, entry.SmartCardUid);
                database.AddInParameter(command, "CandidateId", DbType.Int32, entry.CandidateId);
                database.AddInParameter(command, "CandidateLastName", DbType.String, entry.CandidateLastName);
                database.AddInParameter(command, "CandidateFirstName", DbType.String, entry.CandidateFirstName);
                database.AddInParameter(command, "ClockInOut", DbType.DateTime, entry.ClockInOut);

                int affectedRows = database.ExecuteNonQuery(command);
            }
        }
    }
}