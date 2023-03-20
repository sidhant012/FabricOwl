using FabricOwl;
using FabricOwl.IConfigs;

namespace FabricOwlTests
{
    [TestClass]
    public class BaseTests : Base
    {
        Usings test = new Usings();

        [TestMethod]
        public void Test_EventDoesNotExist()
        {
            string eventInstanceIds = "fc33417b-b1ca-429f-8d9f-01e9fc356d76, f01732cf-092e-4fcc-b174-a85b03345d30";
            eventInstanceIds = String.Concat(eventInstanceIds.Where(c => !Char.IsWhiteSpace(c)));
            string[] eventInstanceId = eventInstanceIds.Split(',');
            List<ICommonSFItems> inputEvents = test.getInputEvents();

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            List<ICommonSFItems> filteredInputEvents = getFilteredInputEvents(eventInstanceIds, eventInstanceId, inputEvents);
            string result = stringWriter.ToString();
            Assert.AreEqual(result.Trim(), "EventInstanceId fc33417b-b1ca-429f-8d9f-01e9fc356d76 does not exist");
        }

        [TestMethod]
        public void Test_LoadPlugins()
        {
            LoadPlugins();
            Assert.IsTrue(Plugins.Count != 0);
        }
    }
}
