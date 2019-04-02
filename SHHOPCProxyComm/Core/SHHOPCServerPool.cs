using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SHH.OPCProxy.Comm.Core
{
    /// <summary>
    /// OPCServer连接池
    /// </summary>
    public class SHHOPCServerPool : ConcurrentDictionary<int, SHHOPCServer>
    {
        /// <summary>
        /// 断开所有服务
        /// </summary>
        public void DisconnectAll()
        {
            foreach (int i in Keys)
                Disconnect(i);
        }

        /// <summary>
        /// 断开指定服务
        /// </summary>
        public void Disconnect(int hashCode)
        {
            this[hashCode].Disconnect();
        }

        /// <summary>
        /// 添加Item
        /// </summary>
        /// <param name="sHHOPCItem"></param>
        public void AttachItem(SHHOPCItem item)
        {
            //如果不存在该服务
            if (!ContainsKey(item.ServerID))
            {
                //初始化服务
                SHHOPCServer server = new SHHOPCServer()
                {
                    IP = item.IP,
                    Name = item.ServerName
                };
                TryAdd(item.ServerID, server);
            }
            //设置OPCServer
            item.OPCServer = this[item.ServerID];
        }
    }
}
