using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SHH.OPCProxy.Comm.Ex
{
    /// <summary>
    /// IP操作扩展类
    /// </summary>
    public static class IPAddressEx
    {
        /// <summary>
        /// 判断IP是否能ping通
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIpActive(this string ip)
        {
            if(IPAddress.TryParse(ip,out IPAddress result))
                return result.IsIpActive();
            return false;
        }

        /// <summary>
        /// 判断IP是否能ping通
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIpActive(this IPAddress ip)
        {
            try
            {
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(ip);

                if (pingReply.Status == IPStatus.Success)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
