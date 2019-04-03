using PropertyChanged;
using SHH.OPCProxy.Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SHH.OPCProxy.Client.Window
{
    /// <summary>
    /// 页面基类
    /// </summary>
    /// <typeparam name="T">ViewModel</typeparam>
    [ImplementPropertyChanged]
    public class WindowBase<T> : System.Windows.Window where T : ViewModelBase, new()
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public T ViewModel { set; get; } = new T();

        /// <summary>
        /// 构造函数
        /// </summary>
        public WindowBase()
        {
            //设置DataContext
            DataContext = ViewModel;
        }
    }
}
