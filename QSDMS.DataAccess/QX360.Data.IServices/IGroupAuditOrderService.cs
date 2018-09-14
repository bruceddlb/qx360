using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Data.IServices
{
    public interface IGroupAuditOrderService<T, Q, P> : IDAL<T, Q, P>
    {
        string GetOrderNo();
    }
}
