using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Domain.Candidates;
using Wfm.Core.Domain.Tests;

namespace Wfm.Services.Test
{
    public partial interface ITestService
    {
        #region Common

        string UploadTestImage(int testQuestionId, byte[] imageBinary, string fileName);

        string GetTestImageUrl(string imageFileLocation, string franchiseLocation = null);

        #endregion

        #region TestCategory

        void InsertTestCategory(TestCategory testCategory);

        void UpdateTestCategory(TestCategory testCategory);

        void DeleteTestCategory(TestCategory testCategory);

        TestCategory GetTestCategoryById(int id);

        IList<TestCategory> GetAllCategories(bool showInactive = false, bool showHidden = false);

        IList<TestCategory> GetAllRequiredTestCategories(bool showInactive = false, bool showHidden = false);
        #endregion 

        #region TestQuestions

        void InsertTestQuestion(TestQuestion testQuestion);

        void UpdateTestQuestion(TestQuestion testQuestion);

        void DeleteTestQuestion(TestQuestion testQuestion);
        

        TestQuestion GetTestQuestionById(int id);

        List<TestQuestion> GetTestQuestionsByTestCategoryId(int testCategoryId);

        #endregion

        #region TestChoice

        void InsertTestChoice(TestChoice testChoice);

        void UpdateTestChoice(TestChoice testChoice);

        void DeleteTestChoice(TestChoice testChoice);

        TestChoice GetTestChoiceById(int id);

        IList<TestChoice> GetTestChoicesByTestQuestionId(int testQuestionId);

        #endregion

        #region TestMaterial
        #region CRUD
        void Create(TestMaterial entity);
        TestMaterial Retrieve(int id);
        TestMaterial RetrieveByGuid(Guid? guid);
        void Update(TestMaterial entity);
        void Delete(TestMaterial entity);
        #endregion

        #region Method
        IQueryable<TestMaterial> GetAllTestMaterials(bool? onlyVideo=null, bool showInactive = false);

        string UploadTestMaterial(byte[] attachment, string fileName, int attachmentTypeId, int? testCategoryId,string contentType);
        #endregion

        #endregion

    }
}
