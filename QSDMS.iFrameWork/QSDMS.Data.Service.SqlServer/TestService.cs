using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSDMS.Data.Service.SqlServer
{
    public class TestService : ITestService
    {
        public List<TestEntity> Query()
        {
            var list = T_Test.Query("");
            List<TestEntity> aa = EntityConvertTools.CopyToList<T_Test, TestEntity>(list.ToList());           
            return aa;
        }

        public int Insert()
        {
            T_Test test = new T_Test();
            test.Name = "aaaa";
            test.Remark = "备注";
            int count = QSDMS_SQLDB.GetInstance().Execute("insert into t_test(name,remark) values(@0,@1)","aaaa","备注111");
            return count;
        }

        public int Update()
        {
            var test = T_Test.SingleOrDefault("where Name=@0", "aaaa");
            test.Remark = "备注-修改";
            return test.Update();
        }

        public void Detele()
        {
            T_Test.Delete("where name=@0", "aaaa");
        }
    }
}
