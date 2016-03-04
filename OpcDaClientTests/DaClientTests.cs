using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Moq;
using OpcDaClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpcDaClient.Rcw;
using OpcDaClient.DeviceXPlorer;

namespace OpcDaClient.Tests
{
    [TestClass()]
    public class DaClientTests
    {
        [TestMethod()]
        public void ReadTest()
        {
            var factoryMock = new Mock<IServerFactory>();
            factoryMock.Setup(_ => _.CreateFromProgId("aaa")).Returns(() =>
            {
                var serverMock = new Mock<IOpcServer>();
                serverMock.Setup(_ => _.AddGroup("default", true, 1000, 0, 0, 0)).Returns(() =>
                {
                    var groupMock = new Mock<IOpcGroup>();
                    groupMock.Setup(_ => _.AddItems(It.IsAny<OpcItemDefine[]>())).Returns(() => new[] {
                        new OpcItemResult { ServerHandle = 1 }
                    });
                    groupMock.Setup(_ => _.Read(OpcDataSource.Device, new[] { 1 })).Returns(() => new[] {
                        new OpcItemState { DataValue = (short)1 }
                    });
                    return groupMock.Object;
                });
                return serverMock.Object;
            });


            var w = MelDevice.D.Address(0).ToItemInt16();
            var wa = new DxpItemInt16[] {w};

            using (var client = new DaClient(factoryMock.Object))
            {
                client.Connect("aaa");

                w.Value.Should().Be(0);
                client.Read(wa);
                w.Value.Should().Be(1);
            }
        }
    }
}