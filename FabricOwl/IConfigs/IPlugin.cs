using System.Collections.Generic;
using System.Threading.Tasks;

namespace FabricOwl.IConfigs
{
    public interface IPlugin
    {
        Task<List<ICommonSFItems>> ReturnEvents(string startTimeUTC, string endTimeUTC);
    }
}
