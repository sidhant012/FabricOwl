using FabricOwl.SFObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
