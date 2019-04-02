using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;
using System.Collections.Concurrent;
using SHH.OPCProxy.Comm.Core;
using SHH.OPCProxy.Comm.Model;
using SHH.OPCProxy.Comm.DAL;
using SHH.OPCProxy.Comm.Interface;

namespace SHH.OPCProxy.Comm.API
{
    /// <summary>
    /// OPCItemAPI(远程对象)
    /// </summary>
    public class SHHOPCItemAPI : BaseAPI, IOPCProxyPro
    {
        /// <summary>
        /// 服务端引用
        /// </summary>
        public IOPCProxyPro OPCProxyPro { set; get; }

        /// <summary>
        /// 是否关闭
        /// </summary>
        public override bool IsClosed { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        private SHHOPCItemAPI()
        { }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        public SHHOPCRealValue GetValue(int hashCode)
        {
            return OPCProxyPro.GetValue(hashCode);
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="hashCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValue(int hashCode, string value)
        {
            return OPCProxyPro.SetValue(hashCode, value);
        }

        /// <summary>
        /// 注册项
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool RegisterOPCItem(SHHOPCItemAPIModel model)
        {
            return OPCProxyPro.RegisterOPCItem(model);
        }

        /// <summary>
        /// 卸载OPC项
        /// </summary>
        /// <param name="hashCode"></param>
        public void UnLoadOPCItem(int hashCode)
        {

        }
        /// <summary>
        /// 卸载所有OPC项
        /// </summary>
        public void UnLoadAllOPCItems()
        {

        }
    }
}
