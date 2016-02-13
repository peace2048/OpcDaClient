using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Moq;
using OpcDaClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Tests
{
    [TestClass()]
    public class DaClientTests
    {
        [TestMethod()]
        public void ConnectTest()
        {
            var groupMock = new Mock<RcwWrapper.IOpcGroup>();
            groupMock.Setup(_ => _.AddItems(It.IsAny<RcwWrapper.OpcItemDefine[]>())).Callback<RcwWrapper.OpcItemDefine[]>(_ => new PrivateObject(_[0]).SetProperty(nameof(RcwWrapper.OpcItemDefine.ServerHandle), 1));
            groupMock.Setup(_ => _.Read(new[] { 1 })).Returns(new[] { new DaValue { Value = 0 } });
            groupMock.Setup(_ => _.Dispose());

            var serverMock = new Mock<RcwWrapper.IOpcServer>();
            serverMock.Setup(_ => _.AddGroup("default", true, 1000, 0, 0)).Returns(groupMock.Object);
            serverMock.Setup(_ => _.Dispose());

            var factoryMock = new Mock<IServerFactory>();
            factoryMock.Setup(factory => factory.CreateFromProgId("aaa")).Returns(serverMock.Object);

            //var target = new DaClient(factoryMock.Object);
            using (var target = new DaClient(factoryMock.Object))
            {
                target.Connect("aaa");
                var item = new DaItem { Node = new DaNode { ItemId = "AAA" } };
                target.Read(new[] { item });
                item.RawValue.Should().Be(0);
            }
            groupMock.VerifyAll();
        }
    }
}