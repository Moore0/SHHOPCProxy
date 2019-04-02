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
    /// API状态监测
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

        /// <summary>
        /// 是否正常
        /// </summary>
        public bool IsNormal { set; get; } = true;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="api"></param>
        public SHHOPCItemAPIChecker(BaseAPI api, SHHOPCItemAPICollection parent) 
            : base(api)
        {
            //获取API集合
            Parent = parent;


            //Timer = new Timer(_ =>
            //{
            //    try
            //    {
            //        //测试是否API正常
            //        api.GetHashCode();
            //        IsNormal = true;
            //    }
            //    catch
            //    {
            //        IsNormal = false;
            //    }

            //    //固定两秒
            //    Timer.Change(2000, Timeout.Infinite);
            //}, null, 0, Timeout.Infinite);
        }
    }
}
