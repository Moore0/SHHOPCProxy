using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SHH.OPCProxy.Comm.API;
using SHH.OPCProxy.Comm.Model;

namespace SHH.OPCProxy.Test.SHHOPCProxyComm
{
    [TestClass]
    public class SHHOPCItemAPICollection_Test
    {
        [TestMethod]
        public async void TestMethod1()
        {
            //var collection = new SHHOPCItemAPICollection();

            //IList<SHHOPCItemAPIModel> models = new List<SHHOPCItemAPIModel>();


            //for (int i = 0; i < 1000; ++i)
            //{
            //    models.Add(new SHHOPCItemAPIModel { IP = _IP, Port = _Port, ServerName = _ServerName, Name = "Channel_0.Device_0.Tag_" + i });
            //}

            //await collection.RegisterOPCItem(models);


        }


        public string _Port = "79";
        public string _IP { set; get; } = "127.0.0.1";
        public string _ServerName { set; get; } = "KEPware.KEPServerEx.V4";
    }
}
