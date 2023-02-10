using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FabricOwl.IConfigs
{
    public interface IPlugin
    {
        string GetApplicationsEventList(string startTimeUTC, string endTimeUTC);
        string GetNodesEventList(string startTimeUTC, string endTimeUTC);
        string GetClusterEventList(string startTimeUTC, string endTimeUTC);
        string GetRepairTasksEventList();
    }
}
