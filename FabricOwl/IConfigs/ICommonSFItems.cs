using System;

namespace FabricOwl.IConfigs
{
    public interface ICommonSFItems
    {
        string Kind { get; set; }
        string EventInstanceId { get; set; }
        string DataType { get; set; }
        DateTime TimeStamp { get; set; }

    }
}
