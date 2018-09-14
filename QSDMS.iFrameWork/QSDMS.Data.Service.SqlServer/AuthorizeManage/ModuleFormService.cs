
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.Extension;
using QSDMS.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace QSDMS.Data.Service.SqlServer
{
    /// <summary>   
    /// 描 述：系统表单
    /// </summary>
    public class ModuleFormService : IModuleFormService
    {

        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            throw new NotImplementedException();
        }

        public ModuleFormEntity GetEntity(string keyValue)
        {
            throw new NotImplementedException();
        }

        public ModuleFormEntity GetEntityByModuleId(string moduleId)
        {
            throw new NotImplementedException();
        }

        public bool IsExistModuleId(string keyValue, string moduleId)
        {
            throw new NotImplementedException();
        }

        public int SaveEntity(string keyValue, ModuleFormEntity entity)
        {
            throw new NotImplementedException();
        }

        public int VirtualDelete(string keyValue)
        {
            throw new NotImplementedException();
        }
    }
}
