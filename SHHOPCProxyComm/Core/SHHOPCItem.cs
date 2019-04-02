using OPCAutomation;
using SHH.OPCProxy.Comm.DAL;
using SHH.OPCProxy.Comm.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SHH.OPCProxy.Comm.Core
{
    /// <summary>
    /// OPC项(为了简化操作,这里依赖方向为服务->项)
    /// </summary>
    public class SHHOPCItem
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SHHOPCItem()
        { }

        /// <summary>
        /// APIModel
        /// </summary>
        public SHHOPCItemAPIModel APIModel { set; get; } = new SHHOPCItemAPIModel();


        private OPCItem opcItem = null;
        /// <summary>
        /// OPC项
        /// </summary>
        public OPCItem OPCItem
        {
            get
            {
                if (opcItem == null)
                    opcItem = OPCServer.Group.OPCItems.AddItem(Name, GetHashCode());
                return opcItem;
            }
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <returns></returns>
        public bool SetValue(string s)
        {
            try
            {
                //尝试写入
                OPCItem.Write(s);
                return true;
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif
                SHHLog.WriteLog(e);
            }

            //写入失败
            return false;
        }

        /// <summary>
        /// 获取实时值
        /// </summary>
        /// <returns></returns>
        public SHHOPCRealValue GetValue()
        {
            SHHOPCRealValue v = null;

            try
            {
                if (OPCItem == null)
                    return null;

                OPCItem.Read(1, out object value, out object quality, out object timeStamp);

                //够造SHHOPCRealValue,写的有点蠢
                v = new SHHOPCRealValue()
                {
                    Value = value?.ToString(),
                    Quality = (SHHOPCQualityStatus)Enum.Parse(typeof(SHHOPCQualityStatus), quality?.ToString()),
                    Time = Convert.ToDateTime(timeStamp.ToString()).ToLocalTime()
                };

            }
            catch (Exception e)
            {
#if DEBUG
                //Debugger.Break();
#endif

                SHHLog.WriteLog(e);
            }
            return v;
        }



        /// <summary>
        /// OPC服务
        /// </summary>
        public SHHOPCServer OPCServer { set; get; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get => APIModel.Name;
            set => APIModel.Name = value;
        }

        /// <summary>
        /// IP
        /// </summary>
        public string IP
        {
            get => APIModel.IP;
            set => APIModel.IP = value;
        }

        /// <summary>
        /// 服务名
        /// </summary>
        public string ServerName
        {
            get => APIModel.ServerName;
            set => APIModel.ServerName = value;
        }

        /// <summary>
        /// 获取Server的HashCode
        /// </summary>
        public int ServerID
        {
            get => IP.GetHashCode() ^ ServerName.GetHashCode();
        }

        /// <summary>
        /// HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ServerID ^ Name.GetHashCode();
        }
    }
}
