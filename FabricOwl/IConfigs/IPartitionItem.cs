using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FabricOwl.IConfigs
{
    public interface IPartitionItem : ICommonSFItems
    {
        public string PartitionId { get; set; }
        public string Category { get; set; }
        public bool HasCorrelatedEvents { get; set; }

    }
}
