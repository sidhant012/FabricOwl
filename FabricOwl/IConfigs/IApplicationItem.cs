using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FabricOwl.IConfigs
{
    public interface IApplicationItem : ICommonSFItems
    {
        public string ApplicationId { get; set; }
        public string Category { get; set; }
        public string HasCorrelatedEvents { get; set; }
    }
}
