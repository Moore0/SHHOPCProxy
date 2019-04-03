using OPCAutomation;
using SHH.OPCProxy.Comm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHH.OPCProxy.Comm.Model
{
    /// <summary>
    /// OPC实时数据对象
    /// </summary>
    [Serializable]
    public class SHHOPCRealValue
    {
        /// <summary>
        /// 值
        /// </summary>
        public string Value { set; get; }
        /// <summary>
        /// 质量
        /// </summary>
        [Obsolete("弃用")]
        public SHHOPCQualityStatus Quality { set; get; } = SHHOPCQualityStatus.OPCQualityBad;
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time { set; get; }
    }
}
