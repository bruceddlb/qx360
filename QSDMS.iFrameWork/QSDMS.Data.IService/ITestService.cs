using QSDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSDMS.Data.IService
{
    public interface ITestService
    {
        List<TestEntity> Query();

        int Insert();
        int Update();
        void Detele();
    }

}
