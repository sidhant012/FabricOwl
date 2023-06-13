using FabricOwl;
using FabricOwl.Rules;

namespace FabricOwlTests
{
    [TestClass]
    public class TransformationTests
    {
        [TestMethod]
        public void Test_trimFront()
        {
            string batchId = "RM/SFRP-b0340003-e2f4-4d05-a2c8-4aa9f05c0caa-UD-4";
            string delimiter = "/";
            string result = Transformations.TrimFront(batchId, delimiter);
            Assert.AreEqual(result, "SFRP-b0340003-e2f4-4d05-a2c8-4aa9f05c0caa-UD-4");
        }

        [TestMethod]
        public void Test_trimBack()
        {
            string message = "Aborting since deactivation failed. Deactivating becasue Fabric Node is closing. For information about common termination errors, please visit https://aka.ms/service-fabric-termination-errors";
            string delimiter = "For information";
            string result = Transformations.TrimBack(message, delimiter);
            Assert.AreEqual(result, "Aborting since deactivation failed. Deactivating becasue Fabric Node is closing. ");
        }

        [TestMethod]
        public void Test_trimWhiteSpace() 
        {
            string message = "      Aborting since deactivation failed. Deactivating becasue Fabric Node is closing. ";
            string result = Transformations.TrimWhiteSpace(message);
            Assert.AreEqual(result, "Aborting since deactivation failed. Deactivating becasue Fabric Node is closing.");
        }

        [TestMethod]
        public void Test_prefix()
        {
            string message = "System_Node_1";
            string prefix = "New Primary Node Name is ";
            string result = Transformations.Prefix(message, prefix);
            Assert.AreEqual(result, "New Primary Node Name is System_Node_1");
        }

        [TestMethod]
        public void Test_getTransformations()
        {
            Transform[] source = new Transform[]
            {
                new Transform{Type="trimFront", Value=":"},
                new Transform{Type="trimFront", Value=":"},
                new Transform{Type="trimBack", Value="("},
                new Transform{Type="trimWhiteSpace"},
                new Transform{Type="prefix", Value="The result is: "}
            };

            string message = "Some seed nodes are unhealthy. Loss of a majority of seed nodes can cause cluster failure. \n1 out of 5 seed nodes are Down. Down seed nodes: \nNodeName(NodeId): System_2(b064614bb4e47c90927d0bcc3e00098b) Node Down At: 2022-09-20 21:26:05.056\nFor more information to fix this, see: https://aka.ms/sfhealth";
            string result = Transformations.GetTransformations(source, message);
            Assert.AreEqual(result, "The result is: System_2");
        }
    }
}