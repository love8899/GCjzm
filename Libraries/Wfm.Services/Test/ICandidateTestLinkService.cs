using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wfm.Core.Domain.Tests;

namespace Wfm.Services.Test
{
    public interface ICandidateTestLinkService
    {
        #region CRUD
        void Create(CandidateTestLink entity);
        CandidateTestLink Retrieve(int id);
        CandidateTestLink Retrieve(Guid? guid);
        void Update(CandidateTestLink entity);
        void Delete(CandidateTestLink entity);
        #endregion
    }
}
