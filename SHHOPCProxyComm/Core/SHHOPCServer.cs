using OPCAutomation;
using SHH.OPCProxy.Comm.API;
using SHH.OPCProxy.Comm.DAL;
using SHH.OPCProxy.Comm.Ex;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHH.OPCProxy.Comm.Core
{
    /// <summary>
    /// OPC服务
    /// </summary>
    public class SHHOPCServer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SHHOPCServer()
        { }

        /// <summary>
        /// 连接服务
        /// </summary>
        public void Connect()
        {
            try
            {
                if (IP.IsIpActive())
                {
                    //连接
                    Server.Connect(Name, IP);
                    //添加默认组
                    Server.OPCGroups.Add(GroupName);
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif

                SHHLog.WriteLog(e);
            }
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        public void ReConnect()
        {
            //重新实例化OPCServer对象
            Server = new OPCServerClass();
            Connect();
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            Server.OPCGroups.RemoveAll();
            Server.Disconnect();
        }

        /// <summary>
        /// 检测自身连接状态
        /// </summary>
        public bool CheckConnectState()
        {
            try
            {
                if ((OPCServerState)Server.ServerState == OPCServerState.OPCRunning || (OPCServerState)Server.ServerState == OPCServerState.OPCNoconfig)
                {
                    IsConn = true;
                }
                else
                {
                    IsConn = false;
                }
            }
            catch
            {
                IsConn = false;
            }
            return IsConn;
        }

        /// <summary>
        /// OPC服务
        /// </summary>
        public OPCServer Server { set; get; } = new OPCServerClass() { };

        /// <summary>
        /// OPC组(这里简化操作只用一个,不知道会不会影响效率...)
        /// </summary>
        public OPCGroup Group { get => Server.OPCGroups.GetOPCGroup(GroupName); }

        /// <summary>
        /// 组名
        /// </summary>
        public readonly string GroupName = "DefaultGroup";

        /// <summary>
        /// 连接状态
        /// </summary>
        public bool IsConn { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string IP { set; get; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return IP.GetHashCode() ^ Name.GetHashCode();
        }
    }
}
