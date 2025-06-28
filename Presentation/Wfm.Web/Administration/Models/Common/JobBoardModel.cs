using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wfm.Admin.Validators.Common;

namespace Wfm.Admin.Models.Common
{
    [Validator(typeof(JobBoardValidator))]
    public class JobBoardModel
    {
        public int Id { get; set; }
        public string JobBoardName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string JobBoardUrl { get; set; }
        public int BoardId { get; set; }
        public string PublishUrl { get; set; }
    }
}