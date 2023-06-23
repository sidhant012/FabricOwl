namespace FabricOwl.IConfigs
{
    public interface IClusterItem : ICommonSFItems
    {
        public string Category { get; set; }
        public string HasCorrelatedEvents { get; set; }
    }
}
