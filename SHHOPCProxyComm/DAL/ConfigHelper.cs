using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;

namespace SHH.OPCProxy.Comm.DAL
{
    /// <summary>
    /// 主程序配置文件管理类
    /// </summary>
    public class ConfigHelper
    {
        //读写锁
        private static readonly ReaderWriterLock readerWriterLock = new ReaderWriterLock();

        //超时时间
        private static int TIMEOUT { set; get; } = 300;

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ReadConfig(string key)
        {
            readerWriterLock.AcquireReaderLock(TIMEOUT);
            string result = "";
            try
            {
                //刷新配置节
                RefreshSection();
                result = ConfigurationManager.AppSettings[key];
            }
            catch (Exception e)
            {
                //不加这个会导致栈溢出
                if (key != "Log")
                    SHHLog.WriteLog(e);
            }
            finally
            {
                readerWriterLock.ReleaseReaderLock();
            }
            return result;
        }


        /// <summary>
        /// 删除配置项
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveConfig(string key)
        {
            ConfigurationManager.AppSettings.Remove(key);
        }

        /// <summary>
        /// 获取默认字符串
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static string ReadConnectionString(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

        /// <summary>
        /// 获取所有的键
        /// </summary>
        public static string[] AllKeys
        {
            get => ConfigurationManager.AppSettings.AllKeys;
        }
            
        /// <summary>
        /// 刷新配置节
        /// </summary>
        public static void RefreshSection()
        {
            //刷新配置节
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void WriteConfig(string key, object value)
        {
            readerWriterLock.AcquireWriterLock(TIMEOUT);
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (!config.AppSettings.Settings.AllKeys.Contains(key))
                {
                    config.AppSettings.Settings.Add(key, value.ToString());
                }
                else
                {
                    config.AppSettings.Settings[key].Value = value.ToString();
                }
                config.Save();
            }
            catch (Exception e)
            {
                SHHLog.WriteLog(e);
            }
            finally
            {
                readerWriterLock.ReleaseWriterLock();
            }
        }
    }
}
