using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Wfm.Core;
using NUnit.Framework;

namespace Wfm.Data.Tests
{
    [TestFixture]
    public abstract class PersistenceTest
    {
        protected WfmObjectContext context;

        [SetUp]
        public void SetUp()
        {
            //TODO fix compilation warning (below)
            #pragma warning disable 0618
            Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
            context = new WfmObjectContext(GetTestDbName());
            context.Database.Delete();
            context.Database.Create();
        }

        protected string GetTestDbName()
        {
            string testDbName = "Data Source=" + (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)) + @"\\Nop.Data.Tests.Db.sdf;Persist Security Info=False";
            //string testDbName = @"Data source= localhost\SQLEXPRESS;  Initial Catalog=Workforce3; UID=sa; PWD=123456; MultipleActiveResultSets=True; Connection Timeout=200";
            return testDbName;
        }        
        
        /// <summary>
        /// Persistance test helper
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="disposeContext">A value indicating whether to dispose context</param>
        protected T SaveAndLoadEntity<T>(T entity, bool disposeContext = true) where T : BaseEntity
        {

            context.Set<T>().Add(entity);
            context.SaveChanges();

            object id = entity.Id;

            if (disposeContext)
            {
                context.Dispose();
                context = new WfmObjectContext(GetTestDbName());
            }

            var fromDb = context.Set<T>().Find(id);
            return fromDb;
        }

        protected T GetEntity<T>(int Id) where T : BaseEntity
        {
            var fromDb = context.Set<T>().Find(Id);
            return fromDb;
        }
    }
}
