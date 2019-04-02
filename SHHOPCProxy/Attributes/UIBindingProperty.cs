using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHH.OPCProxy.Comm.Attributes
{
    /// <summary>
    /// 标识用于绑定UI界面的元素
    /// </summary>
    [Obsolete("弃用")]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class UIBindingPropertyAttribute : Attribute
    {

    }
}
