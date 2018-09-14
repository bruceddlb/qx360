using QX360.Model.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Data.IServices.Report
{
    public interface IAuditReportService<P>
    {
        List<AuditCollectEntity> GetAuditCollectPageList(AuditCollectEntity para, ref P pagination);
        List<AuditCollectEntity> GetAuditCollectList(AuditCollectEntity para);
    }
}
