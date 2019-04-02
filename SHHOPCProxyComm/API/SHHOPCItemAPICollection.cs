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
                        //关联掉线监测
                        SHHOPCItemAPIChecker checker = new SHHOPCItemAPIChecker(this[ip], this);

                        //如果已经存在
                        if (SHHOPCItemAPICheckers.ContainsKey(ip))
                        {
                            //移除定时器
                            SHHOPCItemAPICheckers.TryRemove(ip, out var a);
                            //释放定时器
                            a?.Timer?.Dispose();
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
            //if (ContainsKey(model.IP))
            //    return false;

            bool result = false;

            await Task.Run(() =>
            {
                try
                {
                    this[model.IP].RegisterOPCItem(model);
                    result = true;
                    Models.TryAdd(model.GetOPCItemHashCode(), model);


                    //添加读取值的定时器
                    if (!ReadValueTimers.ContainsKey(model.GetOPCServerHashCode()))
                    {
                        Timer timer = new Timer(new TimerCallback(ReadItemValuesCallBack),
                            model.GetOPCServerHashCode(), Timeout.Infinite, Timeout.Infinite);
                        if (ReadValueTimers.TryAdd(model.GetOPCServerHashCode(), timer))
                            //启动
                            timer.Change(0, Timeout.Infinite);
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

        private void ReadItemValuesCallBack(object state)
        {
            int serverHashCode = (int)state;



            for (int i = 0; i < Models.Count; ++i)
            {
                var model = Models.Values.ElementAt(i);

                //不属于当前服务
                if (model.GetOPCServerHashCode() != serverHashCode)
                    return;


                //补充接口
                ////当连接池中存在该连接且连接状态为正常
                //if (SHHOPCItemAPI.OPCServerPool.ContainsKey(model.GetOPCServerHashCode())
                //    && !SHHOPCItemAPI.OPCServerPool[model.GetOPCServerHashCode()].IsConn)


                //获取实时值
                SHHOPCRealValue v = GetValue(model).Result;

                //当值不为空
                if (v != null)
                {
                    //获取实时值
                    model.RealValue = v.Value;
                    model.Quality = v.Quality;
                    model.Time = v.Time;
                    //状态正常
                    model.State = SHHOPCServerState.Normal;
                }
                else
                {
                    //状态未知
                    model.State = SHHOPCServerState.UnKnown;
                    //重新附加到服务
                    //TryReAttach(model);
                }
            }


            ReadValueTimers[serverHashCode].Change(2000, Timeout.Infinite);
        }


        /// <summary>
        /// 获取OPC项实时值
        /// </summary>
        /// <returns></returns>
        public async Task<SHHOPCRealValue> GetValue(SHHOPCItemAPIModel model)
        {
            //if (ContainsKey(model.IP))
            //    return null;

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
            //if (ContainsKey(model.IP))
            //    return false;

            bool result = false; ;

            await Task.Run(() =>
            {
                result = this[model.IP].SetValue(model.GetOPCItemHashCode(), value);
            });

            return result;
        }
    }
}
