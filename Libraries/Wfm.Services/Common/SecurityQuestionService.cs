using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core.Domain.Common;
using Wfm.Core.Data;

namespace Wfm.Services.Common
{
    public partial class SecurityQuestionService : ISecurityQuestionService 
    {

        #region Fields

        private readonly IRepository<SecurityQuestion> _securityQuestionRepository;


        #endregion

        #region Ctors

        public SecurityQuestionService(
            IRepository<SecurityQuestion> securityQuestionRepository
            )
        {
            _securityQuestionRepository = securityQuestionRepository;
        }

        #endregion


        #region CRUD

        public void InsertSecurityQuestion(SecurityQuestion securityQuestion)
        {
            if (securityQuestion == null)
                throw new ArgumentNullException("securityQuestion");
            //insert
            _securityQuestionRepository.Insert(securityQuestion);

        }

        public void UpdateSecurityQuestion(SecurityQuestion securityQuestion)
        {
            if (securityQuestion == null)
                throw new ArgumentNullException("securityQuestion");

            _securityQuestionRepository.Update(securityQuestion);
        }

        public void DeleteSecurityQuestion(SecurityQuestion securityQuestion)
        {
            if (securityQuestion == null)
                throw new ArgumentNullException("securityQuestion");

            _securityQuestionRepository.Delete(securityQuestion);
        }

        #endregion

        #region SecurityQuestion

        public SecurityQuestion GetSecurityQuestionById(int id)
        {
            if (id == 0)
                return null;

            return _securityQuestionRepository.GetById(id);
        }

        #endregion

        #region LIST

        public IList<SecurityQuestion> GetAllSecurityQuestions()
        {
            var query = _securityQuestionRepository.Table;

            query = from dt in query
                    select dt;

            return query.ToList();
        }

        #endregion

    }
}
