using SHH.OPCProxy.Comm.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace SHH.OPCProxy.Comm.API
{
    /// <summary>
    /// 远程调用API基类
    /// </summary>
    public abstract class BaseAPI : MarshalByRefObject
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseAPI()
        { }

        /// <summary>
        /// 生存周期无限
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
