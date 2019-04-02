using SHH.OPCProxy.Comm.API;
using SHH.OPCProxy.Comm.Core;
using SHH.OPCProxy.Comm.DAL;
using SHH.OPCProxy.Comm.Interface;
using SHH.OPCProxy.Comm.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SHH.OPCProxy.Pro
{
    /// <summary>
    /// OPC代理服务
    /// </summary>
    [Serializable]
    public partial class SHHOPCProxyPro : ServiceBase, IOPCProxyPro
    {
        /// <summary>
        /// SHHOPCItemAPI
        /// </summary>
        public static SHHOPCItemAPI SHHOPCItemAPI { set; get; }

        /// <summary>
        /// OPC连接池(添加服务之后不需要手动连接)
        /// </summary>
        public static SHHOPCServerPool OPCServerPool { set; get; } = new SHHOPCServerPool();

        /// <summary>
        /// OPC项集合
        /// </summary>
        public static ConcurrentDictionary<int, SHHOPCItem> SHHOPCItems { set; get; } = new ConcurrentDictionary<int, SHHOPCItem>();

        /// <summary>
        /// 检测服务状态的定时器
        /// </summary>
        public static Timer CheckServerStateTimer { set; get; }

        /// <summary>
        /// 通道名
        /// </summary>
        public static string ChannelName
        {
            set => ConfigHelper.WriteConfig(nameof(ChannelName), value);
            get => ConfigHelper.ReadConfig(nameof(ChannelName));
        }

        /// <summary>
        /// 端口号
        /// </summary>
        public static int Port
        {
            //这里不做验证
            set => ConfigHelper.WriteConfig(nameof(Port), value);
            get
            {
                if (int.TryParse(ConfigHelper.ReadConfig(nameof(Port)), out int result))
                    return result;
                return DefaultPort;
            }
        }

        /// <summary>
        /// 默认端口
        /// </summary>
        public const int DefaultPort = 79;

        /// <summary>
        /// 通道(Tcp)
        /// </summary>
        public TcpServerChannel Channel { set; get; }

        /// <summary>
        /// 输出日志事件(考虑额外封装一个消息对象)
        /// </summary>
        public static event Action<string> PrintMesssage;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SHHOPCProxyPro()
        {
            //初始化组件
            InitializeComponent();

            try
            {
                //如果通道不存在,注册通道
                if (ChannelServices.GetChannel(ChannelName) == null)
                {
                    BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider
                    {
                        TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full
                    };

                    Channel = new TcpServerChannel(ChannelName, Port, provider);
                    ChannelServices.RegisterChannel(Channel, false);
                    RemotingConfiguration.RegisterWellKnownServiceType(typeof(SHHOPCItemAPI), nameof(SHHOPCItemAPI), WellKnownObjectMode.Singleton);

                    OnPrintMessage(string.Format("注册通道成功,端口:{0}", Port));
                }
            }
            catch (Exception e)
            {
                SHHLog.WriteLog(e);
                OnPrintMessage(string.Format("注册通道失败,端口:{0}", Port));
            }


            //获取SHHOPCItemAPI
            SHHOPCItemAPI = (SHHOPCItemAPI)Activator.GetObject(typeof(SHHOPCItemAPI), string.Format(@"tcp://{0}:{1}/{2}", "127.0.0.1", Port, nameof(SHHOPCItemAPI)));
            //注入到远程对象
            SHHOPCItemAPI.OPCProxyPro = this;
            //启动监测服务状态的定时器
            CheckServerStateTimer = new Timer(new TimerCallback(CheckServerStateCallback), null, 0, Timeout.Infinite);
        }

        /// <summary>
        /// 监测OPC服务与OPCProxy之间的连接状态
        /// </summary>
        /// <param name="state"></param>
        private void CheckServerStateCallback(object state)
        {
            try
            {
                //遍历OPC服务池
                foreach (var server in OPCServerPool.Values)
                {
                    try
                    {
                        if (server == null)
                            continue;

                        //监测服务运行状态
                        if (server.CheckConnectState())
                        {
                            //什么也不做
                        }
                        else
                        {
                            //重新连接
                            server.ReConnect();
                        }
                    }
                    catch (Exception e)
                    {
                        SHHLog.WriteLog(e);
                    }
                }
            }
            catch (Exception e)
            {
                SHHLog.WriteLog(e);
            }

            //固定2秒
            CheckServerStateTimer.Change(2000, Timeout.Infinite);
        }

        /// <summary>
        /// 打印消息
        /// </summary>
        /// <param name="message"></param>
        public void OnPrintMessage(string message)
        {
            SHHLog.WriteLog(message);
            PrintMesssage?.Invoke(message);
        }

        /// <summary>
        /// 服务启动
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            SHHLog.WriteLog("服务启动");
        }

        /// <summary>
        /// 服务关闭
        /// </summary>
        protected override void OnStop()
        {
            SHHLog.WriteLog("服务关闭");
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="hashCode"></param>
        /// <returns></returns>
        public SHHOPCRealValue GetValue(int hashCode)
        {
            SHHOPCRealValue v = null;

            //如果不存在
            if (!SHHOPCItems.ContainsKey(hashCode))
                return null;

            //获取值
            v = SHHOPCItems[hashCode].GetValue();
            return v;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="hashCode"></param>
        /// <param name="value"></param>
        public bool SetValue(int hashCode, string value)
        {
            //如果不存在
            if (!SHHOPCItems.ContainsKey(hashCode))
                return false;

            //返回结果
            bool result = SHHOPCItems[hashCode].SetValue(value);

            return result;
        }

        /// <summary>
        /// 注册OPC项
        /// </summary>
        /// <param name="model"></param>
        public bool RegisterOPCItem(SHHOPCItemAPIModel model)
        {
            //如果存在
            if (SHHOPCItems.ContainsKey(model.GetOPCItemHashCode()))
                return false;


            bool result = false;


            try
            {
                SHHOPCItem item = new SHHOPCItem() { APIModel = model };
                if (SHHOPCItems.TryAdd(model.GetOPCItemHashCode(), item))
                {
                    OPCServerPool.AttachItem(item);
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif

                SHHLog.WriteLog(e);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 卸载OPC项
        /// </summary>
        /// <param name="serverID"></param>
        public void UnLoadOPCItem(int hashCode)
        {

        }

        /// <summary>
        /// 卸载所有项
        /// </summary>
        public void UnLoadAllOPCItems()
        {

        }
    }
}
