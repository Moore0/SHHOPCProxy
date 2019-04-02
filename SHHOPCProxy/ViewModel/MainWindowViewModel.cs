﻿using SHH.OPCProxy.Comm.API;
using SHH.OPCProxy.Comm.Core;
using SHH.OPCProxy.Comm.DAL;
using SHH.OPCProxy.Comm.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SHH.OPCProxy.Client.ViewModel
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {

        public SHHOPCItemAPICollection SHHOPCItemAPICollection { set; get; } = new SHHOPCItemAPICollection();

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindowViewModel()
        {
            OnLoad();

        }


        public async void OnLoad()
        {

            await SHHOPCItemAPICollection.RegisterRemoteObject("127.0.0.1", "79", "tcp://127.0.0.1:79/SHHOPCItemAPI");



            //测试数据
            TestModels.Add(new SHHOPCItemAPIModel { IP = _IP, ServerName = _ServerName, Name = "Channel_10.Device_0.Tag_0" });
            TestModels.Add(new SHHOPCItemAPIModel { IP = _IP, ServerName = _ServerName, Name = "Channel_10.Device_0.Tag_1" });
            TestModels.Add(new SHHOPCItemAPIModel { IP = _IP, ServerName = _ServerName, Name = "Channel_10.Device_0.Tag_2" });
            TestModels.Add(new SHHOPCItemAPIModel { IP = _IP, ServerName = _ServerName, Name = "Channel_10.Device_0.Tag_3" });
            TestModels.Add(new SHHOPCItemAPIModel { IP = _IP, ServerName = _ServerName, Name = "Channel_10.Device_0.Tag_4" });

            await SHHOPCItemAPICollection.RegisterOPCItems(TestModels);
        }

        public ObservableCollection<SHHOPCItemAPIModel> TestModels { set; get; } = new ObservableCollection<SHHOPCItemAPIModel>();


        public string _Port = "79";
        public string _IP { set; get; } = "127.0.0.1";
        public string _ServerName { set; get; } = "KEPware.KEPServerEx.V4";
    }
}
