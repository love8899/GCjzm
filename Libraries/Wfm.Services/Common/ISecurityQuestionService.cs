using System.Collections.Generic;
using Wfm.Core.Domain.Common;

namespace Wfm.Services.Common
{
    public partial interface ISecurityQuestionService
    {
        #region CRUD

        void InsertSecurityQuestion(SecurityQuestion securityQuestion);

        void UpdateSecurityQuestion(SecurityQuestion securityQuestion);

        void DeleteSecurityQuestion(SecurityQuestion securityQuestion);

        #endregion

        #region SecurityQuestion

        SecurityQuestion GetSecurityQuestionById(int id);



        #endregion

        #region LIST

        IList<SecurityQuestion> GetAllSecurityQuestions();

        #endregion

    }
}
