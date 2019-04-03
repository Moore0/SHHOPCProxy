using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHH.OPCProxy.Comm.Model
{
    /// <summary>
    /// OPC服务状态
    /// </summary>
    public enum SHHOPCServerState
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 断线
        /// </summary>
        Offline = 1,
        /// <summary>
        /// 报警
        /// </summary>
        Alarm = 2,
        /// <summary>
        /// 未知
        /// </summary>
        UnKnown = 3,
        /// <summary>
        /// 通讯异常
        /// </summary>
        CommFail = 4,
    }
}
