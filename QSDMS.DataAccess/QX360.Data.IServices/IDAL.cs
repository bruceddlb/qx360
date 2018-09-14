using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Data.IServices
{

    /// <summary>
    /// 数据操作层接口
    /// </summary>
    /// <typeparam name="T">数据对象</typeparam>
    /// <typeparam name="Q">查询对象</typeparam>
    /// <typeparam name="P">分页对象</typeparam>
    public interface IDAL<T, Q, P>
    {
        /// <summary>
        /// 查询记录数
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        int QueryCount(Q para);

        /// <summary>
        /// 查询数据对象 带分页
        /// </summary>
        /// <param name="para"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        List<T> GetPageList(Q para, ref P pagination);

        List<T> GetList(Q para);
        /// <summary>
        /// 获取当前对象
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        T GetEntity(string keyValue);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        bool Add(T entity);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        bool Update(T entity);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        bool Delete(string keyValue);      
       
    }
}
