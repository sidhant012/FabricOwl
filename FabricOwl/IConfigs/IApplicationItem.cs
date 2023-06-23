namespace FabricOwl.IConfigs
{
    public interface IApplicationItem : ICommonSFItems
    {
        public string ApplicationId { get; set; }
        public string Category { get; set; }
        public string HasCorrelatedEvents { get; set; }
    }
}
