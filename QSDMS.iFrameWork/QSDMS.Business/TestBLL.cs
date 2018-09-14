using iFramework.Business;
using QSDMS.Data.IService;
using QSDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSDMS.Business
{
    public class TestBLL : BaseBLL<ITestService>, ITestService
    {
        /// <summary>
        /// 访问实例
        /// </summary>
        private static TestBLL m_Instance = new TestBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TestBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        private TestBLL() { }

        public List<TestEntity> Query()
        {
            return InstanceDAL.Query();
        }

        public int Insert()
        {
            return InstanceDAL.Insert();
        }
        public int Update()
        {
            return InstanceDAL.Update();
        }
        public void Delete()
        {
            InstanceDAL.Detele();
        }


        public void Detele()
        {
            throw new NotImplementedException();
        }
    }
}
