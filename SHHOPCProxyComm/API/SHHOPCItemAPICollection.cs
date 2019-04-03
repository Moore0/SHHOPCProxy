using SHH.OPCProxy.Comm.APIChecker;
using SHH.OPCProxy.Comm.Core;
using SHH.OPCProxy.Comm.DAL;
using SHH.OPCProxy.Comm.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SHH.OPCProxy.Comm.API
{
    /// <summary>
    /// SHHOPCAPI集合(改成泛型???)(先这样吧)
    /// </summary>
    public class SHHOPCItemAPICollection : ConcurrentDictionary<string, SHHOPCItemAPI>
    {
        /// <summary>
        /// 掉线监测
        /// </summary>
        public ConcurrentDictionary<string, SHHOPCItemAPIChecker> SHHOPCItemAPICheckers { set; get; } = new ConcurrentDictionary<string, SHHOPCItemAPIChecker>();

        /// <summary>
        /// 读取值的定时器集合,以OPCServerHashCode为Key
        /// </summary>
        public ConcurrentDictionary<int, Timer> ReadValueTimers { set; get; } = new ConcurrentDictionary<int, Timer>();

        /// <summary>
        /// APIModels集合
        /// </summary>
        public ConcurrentDictionary<int, SHHOPCItemAPIModel> Models { set; get; } = new ConcurrentDictionary<int, SHHOPCItemAPIModel>();

        /// <summary>
        /// 注册远程对象
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<bool> RegisterRemoteObject(string ip, string port, string url)
        {
            //判断是否存在
            if (ContainsKey(ip))
                return false;

            bool result = false;

            await Task.Run(() =>
            {
                try
                {
                    //尝试添加API
                    if (TryAdd(ip, (SHHOPCItemAPI)Activator.GetObject(typeof(SHHOPCItemAPI), url)))
                    {
                        //掉线监测
                        SHHOPCItemAPIChecker checker = new SHHOPCItemAPIChecker(this[ip], ip, this);

                        //如果已经存在
                        if (SHHOPCItemAPICheckers.ContainsKey(ip))
                        {
                            //移除定时器
                            SHHOPCItemAPICheckers.TryRemove(ip, out var ck);
                            //释放定时器
                            ck?.Timer?.Dispose();
                        }

                        //添加到SHHOPCItemAPICheckers
                        SHHOPCItemAPICheckers.TryAdd(ip, checker);
                    }
                    else
                    {
                        result = false;
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    SHHLog.WriteLog(e);
                    result = false;
                }
            });

            return result;
        }



        /// <summary>
        /// 卸载远程对象
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public async Task UnRegisterRemoteObject(string ip)
        {
            await Task.Run(() =>
            {
                TryRemove(ip,out var _);
            });
        }


        /// <summary>
        /// 注册OPC项集合(无返回结果)
        /// </summary>
        public async Task RegisterOPCItems(IEnumerable<SHHOPCItemAPIModel> model)
        {
            for (int i = 0; i < model.Count(); ++i)
            {
                await RegisterOPCItem(model.ElementAt(i));
            }
        }

        /// <summary>
        /// 注册OPC项
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RegisterOPCItem(SHHOPCItemAPIModel model)
        {
            bool result = false;

            await Task.Run(() =>
            {
                try
                {
                    //注册OPC项
                    if (result = this[model.IP].RegisterOPCItem(model))
                    {
                        //添加到APIModels
                        Models.TryAdd(model.GetOPCItemHashCode(), model);

                        //添加读取值的定时器
                        if (!ReadValueTimers.ContainsKey(model.GetOPCServerHashCode()))
                        {
                            //实例化定时器
                            Timer timer = new Timer(new TimerCallback(ReadItemValuesCallBack), model.GetOPCServerHashCode(), Timeout.Infinite, Timeout.Infinite);
                            if (ReadValueTimers.TryAdd(model.GetOPCServerHashCode(), timer))
                                //启动
                                timer.Change(0, Timeout.Infinite);
                        }
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    SHHLog.WriteLog(e);

                    result = false;
                }
            });

            return result;
        }

        /// <summary>
        /// 获取值的定时器
        /// </summary>
        /// <param name="state"></param>
        private void ReadItemValuesCallBack(object state)
        {
#if DEBUG
            //计算处理时间
            DateTime time = DateTime.Now;
#endif
            //获取serverHashCode
            int serverHashCode = (int)state;

            Parallel.For(0, Models.Count, (i) =>
            {
                var model = Models.Values.ElementAt(i);

                //不属于当前服务
                if (model.GetOPCServerHashCode() != serverHashCode)
                    return;

                //更新时间
                model.Time = DateTime.Now;


                //如果远程对象意外断开,调用接口会抛出异常
                try
                {
                    //如果服务处于未激活状态
                    if (!this[model.IP].IsOPCServerAlive(serverHashCode))
                    {
                        //状态置为掉线
                        model.State = SHHOPCServerState.Offline;
                        model.RealValue = "-";

                        return;
                    }
                    else
                    {
                        //获取实时值
                        SHHOPCRealValue v = GetValue(model).Result;
                        //当值不为空
                        if (v != null)
                        {
                            //状态正常
                            model.State = SHHOPCServerState.Normal;
                            model.RealValue = v.Value;
                        }
                        else
                        {
                            //状态未知
                            model.State = SHHOPCServerState.UnKnown;
                            model.RealValue = "-";


                            //尝试重新附加
                            this[model.IP].UnRegisterOPCItem(model.GetHashCode());
                            this[model.IP].RegisterOPCItem(model);
                        }

                    }
                }
                catch
                {

                }


            });


#if DEBUG
            //打印处理时间
            Console.WriteLine(string.Format("OPC项处理时间: {0}", DateTime.Now - time));
#endif

            ReadValueTimers[serverHashCode].Change(0, Timeout.Infinite);
        }


        /// <summary>
        /// 获取OPC项实时值
        /// </summary>
        /// <returns></returns>
        public async Task<SHHOPCRealValue> GetValue(SHHOPCItemAPIModel model)
        {
            SHHOPCRealValue value = null;

            await Task.Run(() =>
            {
                value = this[model.IP].GetValue(model.GetOPCItemHashCode());
            });

            return value;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="model"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> SetValue(SHHOPCItemAPIModel model, string value)
        {
            bool result = false;

            await Task.Run(() =>
            {
                result = this[model.IP].SetValue(model.GetOPCItemHashCode(), value);
            });

            return result;
        }

        /// <summary>
        /// 判断API的状态是否正常
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async Task<bool> IsAPINormal(string ip)
        {
            //如果不存在
            if (!SHHOPCItemAPICheckers.ContainsKey(ip))
                return false;

            bool result = false;

            await Task.Run(() =>
            {

                result = SHHOPCItemAPICheckers[ip].IsNormal;
            });

            return result;
        }
    }
}
