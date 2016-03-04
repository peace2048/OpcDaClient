using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpcDaClient.DeviceXPlorer;
using FluentAssertions;
using Moq;
using OpcDaClient;
using OpcDaClient.Rcw;

namespace OpcDaClientTests
{
    /// <summary>
    /// Summary description for DxpUnitTest
    /// </summary>
    [TestClass]
    public class DxpUnitTest
    {
        public DxpUnitTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void WordDevice()
        {
            var w = MelDevice.D.Address(0).ToItemInt16();
            w.Result.Value.Should().BeNull();
            w.Value.Should().Be(0);

            w = MelDevice.D.Address(0).ToItemInt16(1);
            w.Result.Value.As<Int16>().Should().Be(1);
            w.Value.Should().Be(1);

            var wa = MelDevice.D.Address(0).ToItemInt16Array(2);
            wa.Result.Value.Should().BeNull();
            wa.Value.Should().BeNull();

            wa = MelDevice.D.Address(0).ToItemInt16Array(new short[] { 1, 2, 3 });
            wa.Result.Value.As<Int16[]>().Should().BeEquivalentTo(new Int16[] { 1, 2, 3 });
            wa.Value.Should().BeEquivalentTo(new Int16[] { 1, 2, 3 });

            var ws = MelDevice.D.Address(0).ToItemString(1);
            ws.Result.Value.Should().BeNull();
            ws.Value.Should().BeNull();

            ws = MelDevice.D.Address(0).ToItemString(1, "A");
            ws.Result.Value.As<short[]>().Should().BeEquivalentTo(new short[] { 0x41 });
            ws.Value.Should().Be("A");

            ws.Value = "12345";
            ws.Result.Value.As<short[]>().Should().BeEquivalentTo(new short[] { 0x3231 });
            ws.Value.Should().Be("12");
        }
    }
}
