using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Data.IServices
{
    public interface ISeeCarOrderService<T, Q, P> : IDAL<T, Q, P>
    {
        string GetOrderNo();
    }
}
