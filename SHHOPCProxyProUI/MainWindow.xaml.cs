using SHH.OPCProxy.Comm.API;
using SHH.OPCProxy.Pro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SHHOPCProxyProUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //SHHOPCItemAPI.SHHOPCItemAPILogPrint += SHHOPCItemAPI_SHHOPCItemAPILogPrint;
        }

        private async void SHHOPCItemAPI_SHHOPCItemAPILogPrint(string obj)
        {
            await Dispatcher.InvokeAsync(()=> {
                tbx.Text += obj + "\n";
            }, DispatcherPriority.SystemIdle);
        }




        /// <summary>
        /// 关闭时调用
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            //关闭所有API
            //SHHOPCProxyPro.CloseALLAPIs();
        }

        /// <summary>
        /// 代理服务对象
        /// </summary>
        public SHHOPCProxyPro Proxy { set; get; } = new SHHOPCProxyPro();
    }
}
