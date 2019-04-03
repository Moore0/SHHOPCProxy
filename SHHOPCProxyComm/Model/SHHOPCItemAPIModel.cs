using OPCAutomation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHH.OPCProxy.Comm.Model
{
    /// <summary>
    /// 传输数据用的Model
    /// </summary>
    [Serializable]
    public class SHHOPCItemAPIModel : INotifyPropertyChanged
    {
        private string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
            get => name;
        }

        private DateTime time;
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime Time
        {
            set
            {
                time = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Time)));
            }
            get => time;
        }


        private string realValue;
        /// <summary>
        /// 值
        /// </summary>
        public string RealValue
        {
            set
            {
                realValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RealValue)));
            }
            get => realValue;
        }

        private string ip;
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP
        {
            set
            {
                ip = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IP)));
            }
            get => ip;
        }

        private string serverName;
        /// <summary>
        /// 服务名
        /// </summary>
        public string ServerName
        {
            set
            {
                serverName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ServerName)));
            }
            get => serverName;
        }


        private SHHOPCServerState state = SHHOPCServerState.Offline;
        /// <summary>
        /// 状态
        /// </summary>
        public SHHOPCServerState State
        {
            set
            {
                state = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
            }
            get => state;
        }


        private string note;
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            set
            {
                note = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Note)));
            }
            get => note;
        }


        [field: NonSerialized()]
        private object parent;
        /// <summary>
        /// 父对象引用
        /// </summary>
        public object Parent
        {
            set
            {
                parent = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Parent)));
            }
            get => parent;
        }


        private object tag;
        /// <summary>
        /// 附加内容
        /// </summary>
        public object Tag
        {
            set
            {
                tag = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tag)));
            }
            get => tag;
        }


        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns></returns>
        public int GetOPCItemHashCode()
        {
            return GetOPCServerHashCode() ^ Name.GetHashCode();
        }

        /// <summary>
        /// 获取其附属OPCServer的HashCode
        /// </summary>
        /// <returns></returns>
        public int GetOPCServerHashCode()
        {
            return IP.GetHashCode() ^ ServerName.GetHashCode();
        }


        public override int GetHashCode()
        {
            return GetOPCItemHashCode();
        }

        [field: NonSerialized()]
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
