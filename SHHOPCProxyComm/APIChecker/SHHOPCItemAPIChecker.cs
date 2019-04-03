using SHH.OPCProxy.Comm.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SHH.OPCProxy.Comm.APIChecker
{
    /// <summary>
    /// API状态监测(可改为泛型)
    /// </summary>
    public class SHHOPCItemAPIChecker : BaseAPIChecker
    {
        /// <summary>
        /// 定时器
        /// </summary>
        public Timer Timer { set; get; }

        /// <summary>
        /// API集合
        /// </summary>
        private SHHOPCItemAPICollection Parent { set; get; }

        //private string IP { set; get; }

        /// <summary>
        /// 是否正常
        /// </summary>
        public bool IsNormal { set; get; } = true;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="api"></param>
        public SHHOPCItemAPIChecker(SHHOPCItemAPI api, string ip, SHHOPCItemAPICollection parent)
            : base(api)
        {
            //获取API集合
            Parent = parent;




            Timer = new Timer(_ =>
            {
                try
                {
                    //测试是否API正常
                    api.GetHashCode();
                    IsNormal = true;
                }
                catch
                {
                    IsNormal = false;

                    new Thread(async () =>
                    {
                        await parent.UnRegisterRemoteObject(ip);

                        await parent.RegisterRemoteObject("127.0.0.1", "79", "tcp://127.0.0.1:79/SHHOPCItemAPI");
                    }).Join();

                }

                //固定两秒
                Timer.Change(2000, Timeout.Infinite);
            }, null, 0, Timeout.Infinite);
        }
    }
}
