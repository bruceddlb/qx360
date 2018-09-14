using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSDMS.Model
{
    public class Enums
    {
        /// <summary>
        /// 角色分类
        /// </summary>
        public enum RoleCategoryEnum : int
        {
            角色 = 1,
            岗位 = 2,
            职位 = 3,
            工作组 = 4
        }

        /// <summary>
        /// 用户分类
        /// </summary>
        public enum UserCategoryEnum : int
        {
            部门 = 1,
            角色 = 2,
            岗位 = 3,
            职位 = 4,
            工作组 = 5
        }
        /// <summary>
        /// 日志分类
        /// </summary>
        public enum LogCategoryEnum : int
        {
            登陆 = 1,
            访问 = 2,
            操作 = 3,
            异常 = 4
        }

        public enum FormElementTypeEnum : int
        {
            菜单 = 1,
            按钮 = 2,
            视图 = 3,
            表单 = 4
        }

        public enum GroupEnum
        {
            /// <summary>
            /// 系统风格
            /// </summary>
            [Description("systheme")]
            SysTheme
        }
    }
}
