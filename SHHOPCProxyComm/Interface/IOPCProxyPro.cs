using SHH.OPCProxy.Comm.Core;
using SHH.OPCProxy.Comm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHH.OPCProxy.Comm.Interface
{
    /// <summary>
    /// 服务代理接口(远程对象不支持异步...)
    /// </summary>
    public interface IOPCProxyPro
    {
        /// <summary>
        /// 获取实时值
        /// </summary>
        /// <param name="hashCode"></param>
        /// <returns></returns>
        SHHOPCRealValue GetValue(int hashCode);

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="hashCode">项的hashCode</param>
        /// <param name="value">值</param>
        bool SetValue(int hashCode, string value);

        /// <summary>
        /// 注册OPC项
        /// </summary>
        /// <param name="model"></param>
        bool RegisterOPCItem(SHHOPCItemAPIModel model);

        /// <summary>
        /// 卸载OPC服务的所有项
        /// </summary>
        /// <param name="hashCode">项的hashCode</param>
        void UnLoadOPCItem(int hashCode);

        /// <summary>
        /// 卸载所有项
        /// </summary>
        void UnLoadAllOPCItems();
    }
}
