using PropertyChanged;
using SHH.OPCProxy.Comm.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SHH.OPCProxy.Comm.Model
{
    [ImplementPropertyChanged]
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性改变
        /// </summary>
        /// <param name="name"></param>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        /// <summary>
        /// 通知UI层发生数据改变
        /// </summary>
        [Obsolete("弃用")]
        public void InvokeUIPropertys()
        {
            PropertyInfo[] infos = GetType().GetProperties();

            for (int i = 0; i < infos.Count(); ++i)
            {
                if (infos[i].GetCustomAttribute(typeof(UIBindingPropertyAttribute)) == null)
                    continue;

                OnPropertyChanged(infos[i].Name);
            }
        }
    }
}
