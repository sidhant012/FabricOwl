using FabricOwl.Rules;

namespace FabricOwlTests
{
    [TestClass]
    public class RelatedEventsConfigsTests
    {
        [TestMethod]
        public void Test_GetResourceStream_ThrowsArgumentException()
        {
            string resourcePath = string.Empty;

            Assert.ThrowsException<ArgumentException>(() => RelatedEventsConfigs.GetResourceStream(resourcePath));
        }

        [TestMethod]
        public void Test_GetResourceStream_ThrowsFileNotFoundException()
        {
            string invalidPath = "FabricOwl.Rules.DoesntExist.json";


            Assert.ThrowsException<FileNotFoundException>(() => RelatedEventsConfigs.GetResourceStream(invalidPath));
        }
    }
}
