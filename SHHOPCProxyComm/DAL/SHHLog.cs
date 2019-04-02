using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SHH.OPCProxy.Comm.DAL
{
    /// <summary>
    /// 错误日志类
    /// </summary>
    public class SHHLog
    {
        private static readonly object flag = new object();

        /// <summary>
        /// 日志路径
        /// </summary>
        public static string Log { get => "C://SHHOPCProxyCommLog"; }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="e"></param>
        public static void WriteLog(Exception e)
        {
            try
            {
                if (GetDirectoryLength(Log) < 1024 * 1024 * 1024)
                {
                    lock (flag)
                    {
                        //判断路径是否存在
                        if (!Directory.Exists(Log))
                        {
                            Directory.CreateDirectory(Log);
                        }

                        using (StreamWriter sw = new StreamWriter(string.Format("{0}\\{1}.txt", Log, DateTime.Now.ToString("yyyy-MM-dd")), true))
                        {
                            sw.WriteLine("{0} {1} {2} {3} {4}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), e.GetType(), e.Message, e.Source, e.StackTrace);
                            sw.Flush();
                        }
                    }
                }
                else
                {
                    DeleteFolder(Log);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 自定义日志
        /// </summary>
        /// <param name="s"></param>
        public static void WriteLog(string s)
        {
            try
            {
                if (GetDirectoryLength(Log) < 1024 * 1024 * 1024)
                {
                    lock (flag)
                    {
                        //判断路径是否存在
                        if (!Directory.Exists(Log))
                        {
                            Directory.CreateDirectory(Log);
                        }

                        using (StreamWriter sw = new StreamWriter(string.Format("{0}\\{1}.txt", Log, DateTime.Now.ToString("yyyy-MM-dd")), true))
                        {
                            sw.WriteLine("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), s);
                            sw.Flush();
                        }
                    }
                }
                else
                {
                    DeleteFolder(Log);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 获取日志大小
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static long GetDirectoryLength(string dirPath)
        {
            //判断给定的路径是否存在,如果不存在则退出
            if (!Directory.Exists(dirPath))
                return 0;
            long len = 0;
            //定义一个DirectoryInfo对象
            DirectoryInfo di = new DirectoryInfo(dirPath);
            //通过GetFiles方法,获取di目录中的所有文件的大小
            foreach (FileInfo fi in di.GetFiles())
            {
                len += fi.Length;
            }
            //获取di中所有的文件夹,并存到一个新的对象数组中,以进行递归
            DirectoryInfo[] dis = di.GetDirectories();
            if (dis.Length > 0)
            {
                for (int i = 0; i < dis.Length; i++)
                {
                    len += GetDirectoryLength(dis[i].FullName);
                }
            }
            return len;
        }

        /// <summary>
        /// 递归删除文件夹
        /// </summary>
        /// <param name="dir"></param>
        private static void DeleteFolder(string dir)
        {
            lock (flag)
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                    {
                        try
                        {
                            FileInfo fi = new FileInfo(d);
                            if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                                fi.Attributes = FileAttributes.Normal;
                            File.Delete(d);//直接删除其中的文件 
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        try
                        {
                            DirectoryInfo d1 = new DirectoryInfo(d);
                            if (d1.GetFiles().Length != 0)
                            {
                                DeleteFolder(d1.FullName);////递归删除子文件夹
                            }
                            Directory.Delete(d);
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }


    }
}
