using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FabricOwl
{
    public class RCAItem
    {
        public string Kind { get; set; }

        public string Name { get; set; }

        public string EventInstanceId { get; set; }

        public DateTime? TimeStamp { get; set; }

        public string DataType { get; set; }
    }
}
