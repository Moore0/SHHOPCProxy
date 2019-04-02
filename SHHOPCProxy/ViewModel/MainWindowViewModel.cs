using SHH.OPCProxy.Comm.API;
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
        /// <summary>
        /// SHHOPCItemAPI集合,以IP作为Key
        /// </summary>
        public ConcurrentDictionary<string, SHHOPCItemAPI> SHHOPCItemAPIs { set; get; } = new ConcurrentDictionary<string, SHHOPCItemAPI>();

        /// <summary>
        /// 读取值的定时器集合,以OPCServerHashCode为Key
        /// </summary>
        public ConcurrentDictionary<int, Timer> ReadValueTimers { set; get; } = new ConcurrentDictionary<int, Timer>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindowViewModel()
        {
            //测试API
            SHHOPCItemAPIs.TryAdd(_IP, (SHHOPCItemAPI)Activator.GetObject(typeof(SHHOPCItemAPI), string.Format(@"tcp://{0}:{1}/SHHOPCItemAPI", _IP, _Port)));

            //测试数据

            TestModels.Add(new SHHOPCItemAPIModel { IP = _IP, ServerName = _ServerName, Name = "Channel_10.Device_0.Tag_0" });
            TestModels.Add(new SHHOPCItemAPIModel { IP = _IP, ServerName = _ServerName, Name = "Channel_10.Device_0.Tag_1" });
            TestModels.Add(new SHHOPCItemAPIModel { IP = _IP, ServerName = _ServerName, Name = "Channel_10.Device_0.Tag_2" });
            TestModels.Add(new SHHOPCItemAPIModel { IP = _IP, ServerName = _ServerName, Name = "Channel_10.Device_0.Tag_3" });
            TestModels.Add(new SHHOPCItemAPIModel { IP = _IP, ServerName = _ServerName, Name = "Channel_10.Device_0.Tag_4" });


            //注册测试项
            for (int i = 0; i < TestModels.Count; ++i)
            {
                SHHOPCItemAPIs[_IP].RegisterOPCItem(TestModels[i]);
            }


            for (int i = 0; i < SHHOPCItemAPIs.Count; ++i)
            {
                foreach (var model in TestModels)
                {
                    if (!ReadValueTimers.ContainsKey(model.GetOPCServerHashCode()))
                    {
                        Timer timer = new Timer(new TimerCallback(TimerCallbackReadItemValues),
                            model.GetOPCServerHashCode(), Timeout.Infinite, Timeout.Infinite);
                        if (ReadValueTimers.TryAdd(model.GetOPCServerHashCode(), timer))
                            //启动
                            timer.Change(0, Timeout.Infinite);
                    }
                }
            }

            //添加自动监测掉线的机制




        }

        private void TimerCallbackReadItemValues(object state)
        {
            int serverHashCode = (int)state;

            for (int i = 0; i < TestModels.Count; ++i)
            {
                var model = TestModels[i];

                //不属于当前服务
                if (model.GetOPCServerHashCode() != serverHashCode)
                    return;


                //补充接口
                ////当连接池中存在该连接且连接状态为正常
                //if (SHHOPCItemAPI.OPCServerPool.ContainsKey(model.GetOPCServerHashCode())
                //    && !SHHOPCItemAPI.OPCServerPool[model.GetOPCServerHashCode()].IsConn)


                //获取实时值
                SHHOPCRealValue v = SHHOPCItemAPIs[model.IP].GetValue(model.GetOPCItemHashCode());

                //当值不为空
                if (v != null)
                {
                    //获取实时值
                    model.RealValue = v.Value;
                    model.Quality = v.Quality;
                    model.Time = v.Time;
                    //状态正常
                    model.State = SHHOPCServerState.Normal;
                }
                else
                {
                    //状态未知
                    model.State = SHHOPCServerState.UnKnown;
                    //重新附加到服务
                    //TryReAttach(model);
                }
            }


            ReadValueTimers[serverHashCode].Change(2000, Timeout.Infinite);
        }

        public ObservableCollection<SHHOPCItemAPIModel> TestModels { set; get; } = new ObservableCollection<SHHOPCItemAPIModel>();


        public string _Port = "79";
        public string _IP { set; get; } = "127.0.0.1";
        public string _ServerName { set; get; } = "KEPware.KEPServerEx.V4";
    }
}
