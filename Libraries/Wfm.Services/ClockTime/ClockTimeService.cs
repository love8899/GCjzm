using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.ClockTime;
using Wfm.Core.Domain.Companies;
using Wfm.Core.Domain.Candidates;
using Wfm.Services.Logging;
using Wfm.Services.Helpers;
using Wfm.Services.Candidates;
using Wfm.Services.Companies;
using Wfm.Services.Messages;

namespace Wfm.Services.ClockTime
{
    public partial class ClockTimeService : IClockTimeService
    {
        #region Fields

        private readonly IRepository<CandidateClockTime> _clockTimeRepository;
        private readonly IRepository<CompanyLocation> _companyLocationRepository;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        private readonly IGenericHelper _genericHelper;
        private readonly TimeClockSettings _timeClockSettings;
        private readonly ISmartCardService _smartCardService;
        private readonly IClockDeviceService _clockDeviceService;
        private readonly ICandidateService _candidateService;
        private readonly IRecruiterCompanyService _recruiterCompanyService;
        private readonly IWorkflowMessageService _workflowMessageService;

        #endregion

        #region Ctor

        public ClockTimeService(IRepository<CandidateClockTime> clockTimeRepository,
            IRepository<CompanyLocation> companyLocationRepository,
            IWorkContext workContext,
            IGenericHelper genericHelper,
            ILogger logger,
            TimeClockSettings timeClockSettings,
            ISmartCardService smartCardService,
            IClockDeviceService clockDeviceService,
            ICandidateService candidateService,
            IRecruiterCompanyService recruiterCompanyService,
            IWorkflowMessageService workflowMessageService)
        {
            _clockTimeRepository = clockTimeRepository;
            _companyLocationRepository = companyLocationRepository;
            _workContext = workContext;
            _genericHelper = genericHelper;
            _logger = logger;
            _timeClockSettings = timeClockSettings;
            _smartCardService = smartCardService;
            _clockDeviceService = clockDeviceService;
            _candidateService = candidateService;
            _recruiterCompanyService = recruiterCompanyService;
            _workflowMessageService = workflowMessageService;
        }

        #endregion


        #region CRUD

        public void Insert(CandidateClockTime candidateClockTime)
        {
            if (candidateClockTime == null)
                throw new ArgumentNullException("candidateClockTime");
            candidateClockTime.CreatedOnUtc = candidateClockTime.UpdatedOnUtc = DateTime.UtcNow;
            //insert
            _clockTimeRepository.Insert(candidateClockTime);
        }

        public void Update(CandidateClockTime candidateClockTime)
        {
            if (candidateClockTime == null)
                throw new ArgumentNullException("candidateClockTime");
             candidateClockTime.UpdatedOnUtc = DateTime.UtcNow;
            _clockTimeRepository.Update(candidateClockTime);
        }

        public void Delete(CandidateClockTime candidateClockTime)
        {
            if (candidateClockTime == null)
                throw new ArgumentException("candidateClockTime");

            _clockTimeRepository.Delete(candidateClockTime);
        }


        public void SoftDelete(CandidateClockTime candidateClockTime)
        {
            if (candidateClockTime == null)
                return;

            candidateClockTime.IsDeleted = true;
            Update(candidateClockTime);
        }


        public string AdvancedInsert(CandidateClockTime candidateClockTime)
        {
            if (candidateClockTime == null)
                throw new ArgumentNullException("candidateClockTime");

            var result = _ValidateAndRefreshClockTime(candidateClockTime);

            if (String.IsNullOrWhiteSpace(result))
            {
                candidateClockTime.UpdatedBy = _workContext.CurrentAccount == null ? 0 : _workContext.CurrentAccount.Id;
                candidateClockTime.UpdatedOnUtc = DateTime.UtcNow;
                candidateClockTime.CreatedOnUtc = DateTime.UtcNow;

                _clockTimeRepository.Insert(candidateClockTime);
            }

            return result;
        }


        public string AdvancedUpdate(CandidateClockTime candidateClockTime)
        {
            if (candidateClockTime == null)
                throw new ArgumentNullException("candidateClockTime");

            var result = _ValidateAndRefreshClockTime(candidateClockTime);

            if (String.IsNullOrWhiteSpace(result))
            {
                candidateClockTime.UpdatedBy = _workContext.CurrentAccount == null ? 0 : _workContext.CurrentAccount.Id;
                candidateClockTime.UpdatedOnUtc = DateTime.UtcNow;

                _clockTimeRepository.Update(candidateClockTime);
            }

            return result;
        }


        private string _ValidateAndRefreshClockTime(CandidateClockTime candidateClockTime)
        {
            var result = String.Empty;

            Candidate candidate = null;
            if (candidateClockTime.SmartCardUid.StartsWith("ID"))
                candidate = _candidateService.GetCandidateById(Convert.ToInt32(candidateClockTime.SmartCardUid.Substring(2)));
            else
                candidate = _smartCardService.GetCandidateBySmartCardUid(candidateClockTime.SmartCardUid, activeOnly: true, refDate: candidateClockTime.ClockInOut.Date);
            if (candidate == null || !candidate.IsActive || candidate.IsDeleted)
            {
                result = String.Format("The smart card [{0}] is invalid or inactive.", candidateClockTime.SmartCardUid);
                return result;
            }
            
            else
            {
                candidateClockTime.CandidateId = candidate.Id;
                candidateClockTime.CandidateLastName = candidate.LastName;
                candidateClockTime.CandidateFirstName = candidate.FirstName;

                if (_candidateService.IsCanidateOnboarded(candidate) && candidateClockTime.CandidateClockTimeStatusId == (int)CandidateClockTimeStatus.NotOnboarded)
                    candidateClockTime.CandidateClockTimeStatusId = (int)CandidateClockTimeStatus.NoStatus;

                var clockDevice = _clockDeviceService.GetClockDeviceByClockDeviceUid(candidateClockTime.ClockDeviceUid);
                if (clockDevice == null || !clockDevice.IsActive || clockDevice.IsDeleted)
                {
                    result = String.Format("The clock device [{0}] is invalid or inactive.", candidateClockTime.ClockDeviceUid);
                    return result;
                }

                else
                {
                    candidateClockTime.CompanyLocationId = clockDevice.CompanyLocationId;
                    var company = _clockDeviceService.GetCompanyByClockDeviceUid(candidateClockTime.ClockDeviceUid);
                    if (company == null || !company.IsActive || company.IsDeleted)
                    {
                        result = String.Format("The company with clock device [{0}], is invalid or inactive.", candidateClockTime.ClockDeviceUid);
                        return result;
                    }
                    
                    else
                    {
                        candidateClockTime.CompanyId = company.Id;
                        candidateClockTime.CompanyName = company.CompanyName;
                    }
                }
            }

            return result;
        }


        public void UpdateClockTimeStatus(IList<CandidateClockTime> candidateClockTimes, int statusId = (int)CandidateClockTimeStatus.Processed, int updatedBy = 0,bool isForRescheduling=false)
        {
            if (candidateClockTimes != null)
            {
                foreach (var canClockTime in candidateClockTimes)
                {
                    canClockTime.CandidateClockTimeStatusId = statusId;
                    canClockTime.UpdatedBy = updatedBy;
                    canClockTime.UpdatedOnUtc = DateTime.UtcNow;
                    canClockTime.IsRescheduleClockTime = isForRescheduling;
                    Update(canClockTime);
                }
            }
        }

        #endregion


        #region ClockTime

        public CandidateClockTime GetClockTimeById(int id)
        {
            if (id == 0)
                return null;

            var query = _clockTimeRepository.Table;

            query = from c in query
                    where c.Id == id
                    select c;

            return query.FirstOrDefault();
        }


        public CandidateClockTime GetClockTimeByDeviceUidAndSmartCardUidAndClockTime(string clockDeviceUid, string smartCardUid, DateTime clockInOut)
        {
            if (string.IsNullOrWhiteSpace(clockDeviceUid) || string.IsNullOrWhiteSpace(smartCardUid))
                return null;

            var query = _clockTimeRepository.Table.Where(x => !x.IsDeleted);

            query = from c in query
                    where c.ClockDeviceUid == clockDeviceUid && c.SmartCardUid == smartCardUid && c.ClockInOut == clockInOut
                    select c;

            return query.FirstOrDefault();
        }

        #endregion


        #region LIST

        public IQueryable<CandidateClockTime> GetAllCandidateClockTimesAsQueryable(bool includeDeleted = false)
        {
            var query = _clockTimeRepository.Table.Where(x => includeDeleted || !x.IsDeleted);
            Account account = _workContext.CurrentAccount;
            if (account != null)
            {
                if (!account.IsClientAccount)
                {
                    if (account.IsVendor())
                        query = query.Where(x => x.CandidateId != null && x.Candidate.FranchiseId == account.FranchiseId);
                    if (account.IsRecruiterOrRecruiterSupervisor())
                    {
                        List<int> ids = _recruiterCompanyService.GetCompanyIdsByRecruiterId(account.Id);
                        query = query.Where(x => x.CompanyId != null && ids.Contains(x.CompanyId.Value));
                    }
                }
                else
                {
                    query = query.Where(x => x.CompanyId == account.CompanyId);
                }
                
            }

            query = from c in query
                    orderby c.ClockInOut descending
                    select c;

            return query.AsQueryable();
        }


        /// <summary>
        /// Gets all clock times by smart card uid and clock device uid and date time range.
        /// </summary>
        /// <param name="smartCardUid">The smart card uid.</param>
        /// <param name="clockDeviceUid">The clock device uid.</param>
        /// <param name="startDateTime">The start date time.</param>
        /// <param name="endDateTime">The end date time.</param>
        /// <returns></returns>
        public List<CandidateClockTime> GetAllClockTimesBySmartCardUidAndClockDeviceUidAndDateTimeRange(string smartCardUid, string clockDeviceUid, DateTime startDateTime, DateTime endDateTime)
        {
            var query = _clockTimeRepository.Table.Where(x => !x.IsDeleted);

            query = query.Where(c => c.SmartCardUid == smartCardUid);
            query = query.Where(c => c.ClockDeviceUid == clockDeviceUid);
            query = query.Where(c => c.ClockInOut >= startDateTime && c.ClockInOut <= endDateTime);

            //exclude clock time that is used for rescheduling
            query = query.Where(c => !c.IsRescheduleClockTime);
            // exclude candidates not onbaorded yet
            query = query.Where(c => c.CandidateClockTimeStatusId != (int)CandidateClockTimeStatus.NotOnboarded);

            query = from c in query
                    orderby c.ClockInOut ascending
                    select c;

            return query.ToList();
        }


        public List<CandidateClockTime> GetAllClockTimesByCandidateIdAndLocationIdAndDateTimeRange(int candidateId, int locationId, DateTime? startDateTime, DateTime? endDateTime)
        {
            if (!startDateTime.HasValue && !endDateTime.HasValue)
                return new List<CandidateClockTime>();
            
            // missing punch, valid if within 3 minutes
            else if (!startDateTime.HasValue)
                startDateTime = endDateTime.Value.AddMinutes(-3);
            else if (!endDateTime.HasValue)
                endDateTime = startDateTime.Value.AddMinutes(3);
            
            var query = _clockTimeRepository.Table.Where(x => !x.IsDeleted);

            query = query.Where(c => c.CandidateId == candidateId &&
                                     c.CompanyLocationId == locationId &&
                                     c.ClockInOut >= startDateTime && 
                                     c.ClockInOut <= endDateTime);

            return query.ToList();
        }


        public List<CandidateClockTime> GetAllClockTimesByCandidateIdAndCompanyIdAndDateTimeRange(int candidateId, int companyId, DateTime? startDateTime, DateTime? endDateTime)
        {
            if (!startDateTime.HasValue && !endDateTime.HasValue)
                return new List<CandidateClockTime>();

            // missing punch, valid if within 3 minutes
            else if (!startDateTime.HasValue)
                startDateTime = endDateTime.Value.AddMinutes(-3);
            else if (!endDateTime.HasValue)
                endDateTime = startDateTime.Value.AddMinutes(3);

            var query = _clockTimeRepository.Table.Where(x => !x.IsDeleted)
                .Where(c => c.CandidateId == candidateId &&
                            c.CompanyId == companyId &&
                            c.ClockInOut >= startDateTime &&
                            c.ClockInOut <= endDateTime);

            return query.ToList();
        }

        #endregion


        #region Load Punch Clock Time

        /// <summary>
        /// Loads the punch clock files.
        /// </summary>
        /// <returns></returns>
        public IList<string> LoadPunchClockTime(Account account = null)
        {
            IList<string> errors = new List<string>(); // to store error messages

            // Get punch clock file location
            string punchClockFilePath = _timeClockSettings.PunchClockFilesLocation;

            // Verify if the path exists
            if (!System.IO.Directory.Exists(punchClockFilePath))
            {
                errors.Add(string.Format("The time clock file path [{0}] does not exist, verify the file directory and setting of 'PunchClockFilesLocation' in configuration.", punchClockFilePath));
                return errors;
            }

            // Get all files
            var clockFiles = System.IO.Directory.EnumerateFiles(punchClockFilePath, "*.txt", System.IO.SearchOption.TopDirectoryOnly);

            // Check if there is any file
            if (clockFiles.Count() == 0)
            {
                errors.Add("There is no outstanding time clock file, try again later.");
                return errors;
            }

            foreach (var clockFile in clockFiles)
            {
                // Back up the file first

                #region Back up file

                string backupFileName = System.IO.Path.GetFileName(clockFile);
                try
                {
                    // Construct backup folder - \Archives\Year\Month
                    string file = System.IO.Path.GetFullPath(clockFile);
                    System.IO.FileInfo fi = new System.IO.FileInfo(file);
                    string fileCreatedYear = fi.CreationTime.ToString("yyyy");
                    string fileCreatedMonth = fi.CreationTime.ToString("MM");
                    //string fileCreatedDay = fi.CreationTime.ToString("dd");
                    string archivePath = string.Format("{0}\\{1}\\{2}\\{3}", fi.DirectoryName, "Archives", fileCreatedYear, fileCreatedMonth);


                    // Create backup directory
                    if (!System.IO.Directory.Exists(archivePath)) System.IO.Directory.CreateDirectory(archivePath);

                    string backupFile = System.IO.Path.Combine(archivePath, backupFileName);
                    // Check if same file already exists
                    if (System.IO.File.Exists(backupFile))
                    {
                        // *** Check if they are exactly same ***
                        // There is a bug with punch clock, it could send same file name with different content
                        // Compare the content to check if they are same
                        if (_genericHelper.FileCompare(clockFile, backupFile))
                        {
                            // Same content, do nothing
                        }
                        else
                        {
                            // Same file name, different content, try to back up using diffent file name: _001, _002, _003 ...
                            int n = 0;
                            do
                            {
                                n++;
                                backupFileName = string.Format("{0}_{1}{2}", System.IO.Path.GetFileNameWithoutExtension(clockFile), n.ToString().PadLeft(3, '0'), System.IO.Path.GetExtension(clockFile));
                                backupFile = System.IO.Path.Combine(archivePath, backupFileName);
                                if (System.IO.File.Exists(backupFile))
                                {
                                    if (_genericHelper.FileCompare(clockFile, backupFile))
                                    {
                                        // Same content, do nothing
                                        break;
                                    }
                                }
                                else
                                {
                                    fi.CopyTo(backupFile);
                                    break;
                                }

                            } while (1 == 1);
                        }
                    }
                    else
                    {
                        // Copy the file
                        fi.CopyTo(backupFile);
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(string.Format("Error occured while backing up time clock file : {0}, {1}", System.IO.Path.GetFileName(clockFile), ex.Message));
                    return errors;
                }

                #endregion

                //// Check if file was already loaded, clean up for reload
                //IList<CandidateClockTime> clockTimeList = this.GetClockTimeByPunchClockFile(backupFileName);
                //if (clockTimeList != null)
                //    foreach(var clockTime in clockTimeList)
                //        Delete(clockTime);

                char[] delimiter = { ',' }; // delimter set to ","

                using (System.IO.StreamReader sr = new System.IO.StreamReader(clockFile))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        // *******************************************************
                        // Punch clock file record layout:
                        // 1.ClockDeviceUid, 2.RecordNumber, 3.SmartCardUid, 4.ClockInOut.
                        // *******************************************************
                        string[] tokens = line.Split(delimiter);

                        if (tokens.Count() >= 4)
                        {
                            try
                            {
                                // get data field
                                string clockDeviceUid = tokens[0];
                                int recordNumber = Int32.Parse(tokens[1]);
                                string smartCardUid = tokens[2];
                                DateTime clockInOut = Convert.ToDateTime(tokens[3]);

                                var error = this.AddClockTime(clockDeviceUid, recordNumber, smartCardUid, clockInOut, backupFileName, null, account);
                                if (!String.IsNullOrWhiteSpace(error))
                                    errors.Add(error);
                            }
                            catch (Exception ex)
                            {
                                errors.Add(string.Format("Error occured while converting clock time record : {0} / {1} / Error Message - {2} / Details: {3}", 
                                    backupFileName, line, ex.Message, ex.ToString()));
                            }
                        }
                        else
                        {
                            if(line.Trim().Length > 0)
                                errors.Add(string.Format("Incompleted clock time record detected : {0} / {1} ", backupFileName, line));
                        }

                    } //end while

                    sr.Close();
                }

                try
                {
                    // Delete the completed file
                    System.IO.File.Delete(clockFile);
                }
                catch(Exception ex)
                {
                    errors.Add("Error occured while deleting file " + clockFile + " : " + ex.Message);
                }

            } //foreach file

            // Log the error if any
            if (errors.Count > 0)
            {
                _logger.Error("Error occurred while loading punch clock files into database.", new Exception(String.Join(" | ", errors)));
            }

            return errors;
        }


        public string AddClockTime(string clockDeviceUid, int recordNumber, string smartCardUid, DateTime clockInOut,
            string backupFileName, int? candidateId, Account account = null)
        {
            var error = string.Empty;

            var canClockTime = this.GetClockTimeByDeviceUidAndSmartCardUidAndClockTime(clockDeviceUid, smartCardUid, clockInOut);

            bool newClockTime = false;
            if (canClockTime == null)
            {
                canClockTime = new CandidateClockTime();
                canClockTime.CreatedOnUtc = System.DateTime.UtcNow;
                newClockTime = true;
            }

            #region Key data

            canClockTime.ClockDeviceUid = clockDeviceUid;
            canClockTime.RecordNumber = recordNumber;
            canClockTime.SmartCardUid = smartCardUid;
            canClockTime.ClockInOut = clockInOut;
            canClockTime.PunchClockFileName = backupFileName;
            canClockTime.EnteredBy = account == null ? 0 : account.Id;
            canClockTime.UpdatedOnUtc = System.DateTime.UtcNow;

            #endregion

            #region additional data

            if (!candidateId.HasValue && smartCardUid.StartsWith("ID"))     // from barcode scanners
            {
                if (int.TryParse(smartCardUid.Substring(2), out int id))
                    candidateId = id;
            }

            Candidate candidate = null;
            if (candidateId.HasValue && candidateId.Value > 0)
                candidate = _candidateService.GetCandidateById(candidateId.Value);
            else
                candidate = _smartCardService.GetCandidateBySmartCardUid(smartCardUid);

            if (candidate != null && candidate.IsActive)
            {
                // incomplete code ???
                if (smartCardUid.StartsWith("ID") 
                    && !candidate.IsEmployee
                    && candidate.CandidateOnboardingStatusId != (int)CandidateOnboardingStatusEnum.Started)
                {
                    candidate = null;
                }
                else
                {
                    canClockTime.CandidateId = candidate.Id;
                    canClockTime.CandidateLastName = candidate.LastName;
                    canClockTime.CandidateFirstName = candidate.FirstName;
                }
            }

            var clockDevice = _clockDeviceService.GetClockDeviceByClockDeviceUid(clockDeviceUid);
            if (clockDevice != null)
            {
                canClockTime.CompanyLocationId = clockDevice.CompanyLocationId;

                var companyLocation = clockDevice.CompanyLocation;
                if (companyLocation != null)
                {
                    companyLocation.LastPunchClockFileUploadDateTimeUtc = DateTime.UtcNow;
                    _companyLocationRepository.Update(companyLocation);

                    var company = companyLocation.Company;
                    if (company == null) company = _clockDeviceService.GetCompanyByClockDeviceUid(clockDeviceUid);
                    if (company != null)
                    {
                        canClockTime.CompanyId = company.Id;
                        canClockTime.CompanyName = company.CompanyName;
                    }
                }
            }

            #endregion

            // add or update
            if (newClockTime)
            {
                if (candidate != null)
                {
                    var prevAllowedTime = canClockTime.ClockInOut.AddMinutes(-3);
                    var prevClocktime = this.GetAllCandidateClockTimesAsQueryable()
                        .FirstOrDefault(c => c.CandidateId == canClockTime.CandidateId
                                             && c.ClockInOut <= canClockTime.ClockInOut
                                             && prevAllowedTime <= c.ClockInOut);

                    if (prevClocktime == null)  // Only send alert if punch is not repeated.
                    {
                        if (canClockTime.CandidateClockTimeStatusId == (int)CandidateClockTimeStatus.NotOnboarded)
                        {
                            _workflowMessageService.SendNotOnBoardedPunchRecruiterNotification(canClockTime, candidate, 1);
                        }
                        _workflowMessageService.SendUnexpectedPunchNotification(canClockTime, candidate, 1);
                    }
                }

                this.Insert(canClockTime);
            }
            else
                this.Update(canClockTime);

            return error;
        }

        #endregion

    }
}
