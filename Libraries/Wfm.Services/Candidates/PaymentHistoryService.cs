using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Wfm.Core.Data;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Payroll;
using Wfm.Data;
using Wfm.Services.Logging;
using Wfm.Services.Payroll;


namespace Wfm.Services.Candidates
{
    public partial class PaymentHistoryService : IPaymentHistoryService
    {
        #region Fields

        private readonly IPayGroupService _payGroupService;
        private readonly IRepository<Payroll_Calendar> _payrollCalendarRepository;
        private readonly IRepository<Payroll_Batch> _payroll_Batch;
        private readonly IRepository<Candidate_Payment_History> _candidatePaymentHistoryRepository;
        private readonly IRepository<Candidate_Payment_History_Detail> _candidatePaymentHistoryDetailRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IDbContext _dbContext;
        private readonly ILogger _logger;
        #endregion

        #region Ctors

        public PaymentHistoryService(
            IPayGroupService payGroupService,
            IRepository<Payroll_Calendar> payrollCalendarRepository,
            IRepository<Payroll_Batch> payroll_Batch,
            IRepository<Candidate_Payment_History> candidatePaymentHistoryRepository,
            IRepository<Candidate_Payment_History_Detail> candidatePaymentHistoryDetailRepository,
            IRepository<Company> companyRepository,
            IDbContext dbContext,
            ILogger logger
            )
        {
            _payGroupService = payGroupService;
            _payrollCalendarRepository = payrollCalendarRepository;
            _payroll_Batch = payroll_Batch;
            _candidatePaymentHistoryRepository = candidatePaymentHistoryRepository;
            _candidatePaymentHistoryDetailRepository = candidatePaymentHistoryDetailRepository;
            _companyRepository = companyRepository;
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion


        #region Payment History

        public IQueryable<PaymentHistory> GetPaymentHistoryByCandidatId(int candidateId, int franchiseId, bool excludeReversed = true)
        {
            IQueryable<PaymentHistory> result = new List<PaymentHistory>().AsQueryable();

            if (candidateId == 0) return result;

            var payGroups = _payGroupService.GetAllPayGroups();
            var payCalendar = _payrollCalendarRepository.TableNoTracking;
            var batches = _payroll_Batch.TableNoTracking;
            var histories = _candidatePaymentHistoryRepository.TableNoTracking.Where(x => x.CandidateId == candidateId && (!excludeReversed || x.CheckStatusId != 6));
            var details = _candidatePaymentHistoryDetailRepository.TableNoTracking;
            var companies=_companyRepository.TableNoTracking;

            result= from h in histories  
                        from b in batches where h.PayrollBatchId==b.Id
                        from c in payCalendar where b.Payroll_CalendarId == c.Id 
                        from pg in payGroups where pg.Id == c.PayGroupId
                        from dp in details where dp.Payment_HistoryId==h.Id && dp.Payroll_Item.Payroll_Item_SubType.Code == "GROSS_PAY" && dp.Payroll_Item.FranchiseId==franchiseId
                        from dd in details where dd.Payment_HistoryId==h.Id && dd.Payroll_Item.Payroll_Item_SubType.Code == "TOTAL_DED" && dd.Payroll_Item.FranchiseId==franchiseId
                        from dn in details where dn.Payment_HistoryId==h.Id && dn.Payroll_Item.Payroll_Item_SubType.Code == "NET_PAY" && dn.Payroll_Item.FranchiseId==franchiseId
                        from comp in companies where comp.Id==h.CompanyId
                        select new PaymentHistory
                        {
                            CandidatePaymentHistoryGuid = h.CandidatePaymentHistoryGuid,
                            PaymentHistory_Id = h.Id,
                            CandidateId = candidateId,
                            Year = h.Year,
                            PayrollBatch_Id = h.PayrollBatchId.Value,
                            Company = comp.CompanyName,
                            PayGroup = pg.Name,
                            PayPeriodStart = c.PayPeriodStartDate,
                            PayPeriodEnd = c.PayPeriodEndDate,
                            PaymentDate = h.Payment_Date.Value,
                            ChequeNumber = h.Cheque_Number,
                            DirectDepositNumber = h.Direct_Deposit_Number,
                            ProvinceCode = h.ProvinceCode,
                            GrossPay = dp.Amount.Value,
                            TotalDeductions = dd.Amount.Value,
                            NetPay = dn.Amount.Value,
                            IsEmailed = h.IsEmailed ?? false,
                            IsPrinted = h.IsPrinted ?? false,
                            Note = h.Note
                        };          

            return result;
        }


        public IQueryable<PaymentHistory> GetPaymentHistoryByCandidatIdAndDate(int candidateId, int franchiseId, DateTime refDate)
        {
            return GetPaymentHistoryByCandidatId(candidateId, franchiseId).Where(x => x.PayPeriodEnd >= refDate);
        }


        public IQueryable<PaymentHistory> GetPaymentHistoryByCandidatIdAndYear(int candidateId, int franchiseId, int year)
        {
            return GetPaymentHistoryByCandidatId(candidateId, franchiseId, excludeReversed: false).Where(x => x.Year == year.ToString());
        }


        public IEnumerable<PaymentDetail> GetPaymentDetails(int historyId)
        {
            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("parentID", historyId);
            string query = @"SELECT  detail.Id, detail.Payroll_ItemId, subtype.Code as SubTypeCode, item.Code,
                                    (CAST(itemType.Sort_Order as varchar(4)) + '-'+itemType.Description) as GroupName, 
                                    Case when JobOrder_Id > 0 then null else item.Code end as DisplayCode, 
		                            Case when JobOrder_Id > 0 then null else item.Description end as Description, 
                                    Case when subtype.Code in ('HOURLY', 'REG_HOURS', 'OT_HOURS') then Unit else null end as Unit, 
	                                Case when subtype.Code in ('HOURLY', 'REG_HOURS', 'OT_HOURS') then detail.Rate else null end as Rate, 
	                                Case when subtype.Code in ('HOURLY', 'REG_HOURS', 'OT_HOURS') then YTD_Unit else null end as YTD_Unit, 
	                                Amount, YTD_Amount,
                                    Case when JobOrder_Id = 0 then null else JobOrder_Id end as JobOrder_Id,
		                            detail.ItemDate, WSIB_Code, WSIB_Rate
                            FROM [Candidate_Payment_History_Detail] detail
                                Inner Join Payroll_Item item on detail.Payroll_ItemId = item.ID
	                            Inner join Payroll_Item_Type itemType on item.TypeID = itemType.Id
                                Inner Join Payroll_Item_SubType subtype on item.SubTypeId = subtype.Id
                            Where detail.Payment_HistoryId = @parentID
                            Order by Sort_Order, Code, JobOrder_Id
                            ";
            var result = _dbContext.SqlQuery<PaymentDetail>(query, paras).ToList();
            return result;
        }


        public void UdpatePaymentHistory(int id, bool? isEmailed = null, bool? isPrinted = null, string note = null)
        {
            var history = _candidatePaymentHistoryRepository.GetById(id);
            if (history != null && (isEmailed.HasValue || isPrinted.HasValue || note != null))
            {
                if (isEmailed.HasValue) history.IsEmailed = isEmailed;
                if (isPrinted.HasValue) history.IsPrinted = isPrinted;
                if (note != null) history.Note = note;

                _candidatePaymentHistoryRepository.Update(history);
            }
        }


        public byte[] GetPaystub(int historyId)
        {
            var history = _candidatePaymentHistoryRepository.GetById(historyId);

            return history != null ? history.Paystub : null;
        }


        public byte[] GetPaystub(Guid historyGuid)
        {
            var history = _candidatePaymentHistoryRepository.Table.FirstOrDefault(x => x.CandidatePaymentHistoryGuid == historyGuid);

            return history != null ? history.Paystub : null;
        }

        #endregion

        #region Payment History with pay stubs
        public IEnumerable<PaymentHistoryWithPayStub> GetAllPaymentHistoryWithPayStubByCandidateId(int candidateId)
        {
            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("candidateId", candidateId);
            string query = @"
                                select cph.CandidatePaymentHistoryGuid,cph.CandidateId,
                                        cph.Payment_Date as PaymentDate,pc.PayPeriodStartDate,pc.PayPeriodEndDate
                                from Candidate_Payment_History cph
                                inner join Payroll_Batch pb on pb.Id = cph.PayrollBatchId
                                inner join Payroll_Calendar pc on pc.Id = pb.Payroll_CalendarId
                                where cph.CandidateId = @candidateId";
            var result = _dbContext.SqlQuery<PaymentHistoryWithPayStub>(query, paras);
            return result;
        }

        public PaymentHistoryWithPayStub GetPayStubByPaymentGuid(Guid? guid,int candidateId)
        {
            if (!guid.HasValue || guid == Guid.Empty)
                return null;
            SqlParameter[] paras = new SqlParameter[2];
            paras[0] = new SqlParameter("guid", guid.Value);
            paras[1] = new SqlParameter("candidateId", candidateId);
            string query = @"
                                select cph.CandidatePaymentHistoryGuid,cph.CandidateId,
                                        cph.Payment_Date as PaymentDate,cph.Paystub,pc.PayPeriodStartDate,pc.PayPeriodEndDate
                                from Candidate_Payment_History cph
                                inner join Payroll_Batch pb on pb.Id = cph.PayrollBatchId
                                inner join Payroll_Calendar pc on pc.Id = pb.Payroll_CalendarId
                                where cph.CandidatePaymentHistoryGuid = @guid and cph.CandidateId = @candidateId";
            var result = _dbContext.SqlQuery<PaymentHistoryWithPayStub>(query, paras).FirstOrDefault();
            return result;
        }


        public bool PayStub_Password(int CandidateId, out string password)
        {
            password = "";
            string query = @"Select  datepart(MM, BirthDate) as [Month], datepart(DD, BirthDate) as [Day], Right(SocialInsuranceNumber, 3) as [Last_3_SIN], PayStubPassword 
                             From Candidate 
                               left outer join EmployeePayrollSetting eps on Candidate.Id = eps.EmployeeId 
                             Where Candidate.Id = @CandidateId ";

            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("CandidateId", CandidateId);

            PaystubPassword paystubPassword = _dbContext.SqlQuery<PaystubPassword>(query, parameters).FirstOrDefault();
            //string[] result = new string[3];

            if (paystubPassword!=null)
            {
                if (paystubPassword.PayStubPassword == null)
                {
                    if (!String.IsNullOrWhiteSpace(paystubPassword.Last_3_SIN) && !String.IsNullOrWhiteSpace(paystubPassword.Month.ToString()) && !String.IsNullOrWhiteSpace(paystubPassword.Day.ToString()))
                    {
                        string MM = string.Empty;
                        string DD = string.Empty;
                        string SSS = paystubPassword.Last_3_SIN;
                        if (paystubPassword.Month.Value < 10)
                            MM = "0" + paystubPassword.Month.ToString();
                        else
                            MM = paystubPassword.Month.ToString();
                        if (paystubPassword.Day.Value < 10)
                            DD = "0" + paystubPassword.Day;
                        else
                            DD = paystubPassword.Day.ToString();

                        password = MM + DD + SSS;
                        return true;
                    }
                    else
                    {
                        _logger.Error(String.Format("PayStub_Password_and_Email():Employee's SIN or Date of birth is invalid! EmployeeId:{0}", CandidateId));
                        return false;
                    }
                }
                else
                {
                    password = paystubPassword.PayStubPassword;
                    return true;
                }
            }
            else
            {
                _logger.Error(String.Format("PayStub_Password_and_Email():This employee {0} does not exist!", CandidateId));
                return false;
            }
        }


        public string SecurePayStubPDFFile(int candidateId, byte[] bytes)
        {
            string password = string.Empty;
            try
            {
                if (PayStub_Password(candidateId, out password))
                {
                    StringBuilder fileName = new StringBuilder();
                    fileName.Append(Path.GetTempPath());
                    fileName.Append(DateTime.Now.ToString("MMddyyyyhhmmss_"));
                    fileName.Append(candidateId);
                    fileName.Append(".pdf");

                    using (FileStream fs = new FileStream(fileName.ToString(), FileMode.OpenOrCreate))
                    {
                        iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(bytes);
                        iTextSharp.text.pdf.PdfEncryptor.Encrypt(reader, fs, true, password, null, iTextSharp.text.pdf.PdfWriter.ALLOW_SCREENREADERS | iTextSharp.text.pdf.PdfWriter.ALLOW_PRINTING);
                    }
                    return fileName.ToString();
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _logger.Error("SecurePayStubPDFFile(): ", ex);
                return null;
            }
        }
        #endregion

        #region Payroll Batch
        public IQueryable<Payroll_Batch> GetPayrollBatchByPayCalendarId(int payCalendarId)
        {
            var result = _payroll_Batch.Table.Where(x => x.Payroll_CalendarId == payCalendarId);
            return result;
        }
        #endregion


        #region Employee Seniority

        public IEnumerable<EmployeeSeniority> GetEmployeeSeniorityReport(DateTime refDate, int scope, decimal threshold)
        {
            var query = String.Format(@"select distinct c.Id as CandidateId, c.FirstName, c.LastName, 
	                                        pay.FirstHireDate, pay.LastHireDate , pay.TerminationDate,
	                                        (select top 1 ROE.CreateTimestamp from ROE where ROE.CandidateId = c.Id order by ROE.CreateTimestamp desc) as ROE_Date
                                        from Candidate_Payment_History cph
                                        inner join Candidate c on cph.CandidateId = c.Id and c.IsActive = 1 
                                        inner join EmployeePayrollSetting pay on c.Id = pay.EmployeeId and pay.FirstHireDate <= DATEADD(month, {0}, GetDate())
                                        where cph.Payment_Date >= dateadd(day, {1}, getdate())
                                        Order by C.Id", -12 * threshold, -scope);
            
            return _dbContext.SqlQuery<EmployeeSeniority>(query);
        }


        public IEnumerable<EmployeeSeniority> GetEmployeeSeniorityReport(string dateField, DateTime fromDate, DateTime toDate, bool exlcudePlaced, DateTime placedFrom, DateTime placedTo, string companyIds = null)
        {
            var dayWorkedRefField = string.Empty;
            var hireDateString = string.Empty;
            switch (dateField)
            {
                case "FirstHireDate":
                    dayWorkedRefField = "pay.FirstHireDate";
                    hireDateString = @"and pay.FirstHireDate >= @fromDate and pay.FirstHireDate < @toDate ";
                    break;
                case "LastHireDate":
                    dayWorkedRefField = "pay.LastHireDate";
                    hireDateString = @"and pay.LastHireDate >= @fromDate and  pay.LastHireDate < @toDate ";
                    break;
                case "LastWorkingDay":
                    dayWorkedRefField = "@fromDate";
                    break;
                default:
                    return Enumerable.Empty<EmployeeSeniority>();
            }

            var query = new StringBuilder();
            query.Append(String.Format(@"
                ; with LatestPlacement as (
                    Select cjo.CandidateId, cjo.StartDate, cjo.EndDate
                    from CandidateJobOrder as cjo
                    join JobOrder as jo on jo.Id = cjo.JobOrderId
                    where cjo.CandidateJobOrderStatusId = 12 and (@companyIds is null or jo.CompanyId in (select Item from dbo.SplitString(@companyIds, ',')))
                    and cjo.StartDate <= @placedDateTo and (cjo.EndDate is null or cjo.EndDate >= @placedDateFrom)
                )
                , FirstPlacement as (
                    Select CandidateId, (case when Min(StartDate) > @placedDateFrom then Min(StartDate) else @placedDateFrom end) as StartDate
                    from LatestPlacement group by CandidateId
                )
                , LastPlacement as (
                    Select CandidateId, (case when Max(isnull(EndDate, '9999-12-31')) < @placedDateTo then Max(isnull(EndDate, '9999-12-31')) else @placedDateTo end) as EndDate
                    from LatestPlacement group by CandidateId
                )
                , AllWorkDays as (
	                select CandidateId, CompanyId, JobStartDateTime from CandidateWorkTime 
	                where (@companyIds is null or CompanyId in (select Item from dbo.SplitString(@companyIds, ',')))
		                and JobStartDateTime>= @fromDate and JobStartDateTime <=  @toDate
                )
                , WorkDays as (
	                select CandidateId, CompanyId, JobStartDateTime
	                from AllWorkDays as d
	                inner join EmployeePayrollSetting as pay on pay.EmployeeId = d.CandidateId
		                {0}
	                where d.JobStartDatetime >= {1}
                )

                select distinct c.Id as CandidateId, c.CandidateGuid, c.FirstName, c.LastName, 
                                c.HomePhone, c.MobilePhone, c.Email, 
				                _lastAddress.AddressLine1, 
				                _lastAddress.AddressLine2, 
				                City.CityName as City, 
				                StateProvince.Abbreviation as Province,
				                Country.ThreeLetterIsoCode as Country, 
				                _lastAddress.PostalCode, 
                                pay.FirstHireDate, pay.LastHireDate , 
                                (select top 1 Min(JobStartDateTime) from WorkDays) as FirstDayWorked, 
				                _lastWorkDay.JobStartDateTime as LastDayWorked, _lastWorkDay.CompanyId as LastClientWorked,
                                pay.TerminationDate, 
				                _lastROE.CreateTimestamp as ROE_Date, _lastROE.Reason as ROE_Reason,
				                _lastDNR.CreatedOnUtc as DNR_Date, _lastDNR.BannedReason as DNR_Reason,
				                fp.StartDate as FirstPlacement, lp.EndDate as LastPlacement
                from Candidate c  
                inner join WorkDays as d on d.CandidateId = c.Id
                inner join EmployeePayrollSetting pay on pay.EmployeeId = d.CandidateId

                outer apply (select top 1 * from WorkDays where WorkDays.CandidateId = c.Id order by WorkDays.JobStartDateTime desc) as _lastWorkDay 

                outer apply (select top 1 * from ROE where ROE.CandidateId = c.Id order by ROE.CreateTimestamp desc) _lastROE 
                outer apply (select top 1 * from CandidateBlacklist as DNR where DNR.CandidateId = c.Id order by DNR.CreatedOnUtc desc) _lastDNR 

                outer apply (select top 1 * from CandidateAddress as ca where ca.CandidateId = c.Id and ca.IsActive = 1 order by ca.CreatedOnUtc desc) _lastAddress
                left join City on City.Id = _lastAddress.CityId
                left join StateProvince on StateProvince.Id = _lastAddress.StateProvinceId
                left join Country on Country.Id = _lastAddress.CountryId

                left outer join FirstPlacement fp on c.Id = fp.CandidateId
                left outer join LastPlacement lp on c.Id = lp.CandidateId

                where c.IsActive = 1 and (@excludePalcedEmployees = 0 or C.Id not in (select distinct CandidateId from LatestPlacement))
                Order by c.Id
            ", hireDateString, dayWorkedRefField));

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("fromDate", fromDate) { SqlDbType = System.Data.SqlDbType.DateTime });
            parameters.Add(new SqlParameter("toDate", toDate) { SqlDbType = System.Data.SqlDbType.DateTime });
            parameters.Add(new SqlParameter("excludePalcedEmployees", exlcudePlaced) { SqlDbType = System.Data.SqlDbType.Bit });
            parameters.Add(new SqlParameter("placedDateFrom", placedFrom) { SqlDbType = System.Data.SqlDbType.DateTime });
            parameters.Add(new SqlParameter("placedDateTo", placedTo) { SqlDbType = System.Data.SqlDbType.DateTime });
            parameters.Add(new SqlParameter("companyIds", companyIds) { SqlDbType = System.Data.SqlDbType.VarChar });

            return _dbContext.SqlQuery<EmployeeSeniority>(query.ToString(), parameters.ToArray());
        }

        #endregion
    }
}
