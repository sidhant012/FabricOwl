namespace FabricOwl.IConfigs
{
    public interface INodeItem : ICommonSFItems
    {
        public string NodeName { get; set; }
        public string Category { get; set; }
        public string HasCorrelatedEvents { get; set; }

    }
}
