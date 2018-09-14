using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSDMS.Data.Service.MySqlServer
{
    public class TestService : ITestService
    {
        public List<TestEntity> Query()
        {
            var list = t_test.Query("");
            List<TestEntity> aa = EntityConvertTools.CopyToList<t_test, TestEntity>(list.ToList());           
            return aa;
        }

        public int Insert()
        {
            t_test test = new t_test();
            test.Name = "aaaa";
            test.Remark = "备注";
            int count = QSDMS_MySQLDB.GetInstance().Execute("insert into t_test(name,remark) values(@0,@1)","aaaaa","备注111");
            return count;
        }

        public int Update()
        {
            var test = t_test.SingleOrDefault("where Name=@0", "aaaa");
            test.Remark = "备注-修改";
            return test.Update();
        }

        public void Detele()
        {
            t_test.Delete("where name=@0", "aaaa");
        }
    }
}
