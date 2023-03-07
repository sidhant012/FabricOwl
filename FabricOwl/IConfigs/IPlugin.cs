using System.Collections.Generic;

namespace FabricOwl.IConfigs
{
    public interface IPlugin
    {
        List<ICommonSFItems> ReturnEvents(List<ICommonSFItems> inputEvents, string startTimeUTC, string endTimeUTC);

    }
}
