using LaborAndSocialSecurity.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaborAndSocialSecurity.Uploaders
{
    public class Dispatcher
    {
        public class ApiInvokeCommand
        {
            private HjApi api;
            private object input;
            public JObject Output { get; private set; }

            public ApiInvokeCommand(HjApi api, object input)
            {
                this.api = api;
                this.input = input;
            }

            public void Execute()
            {
                this.Output = HjApiCaller.CallOpenApi(this.api, this.input);
            }
        }

        #region 字段

        /// <summary>
        /// 命令队列。
        /// </summary>
        private Queue<Tuple<ApiInvokeCommand, AutoResetEvent>> mQueues = new Queue<Tuple<ApiInvokeCommand, AutoResetEvent>>();

        private readonly Timer timer;

        #endregion

        #region 属性

        public static Dispatcher Instance
        {
            get;
            set;
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 单例模式。
        /// </summary>
        static Dispatcher()
        {
            Instance = new Dispatcher();
        }

        private Dispatcher()
        {
            timer = new Timer(TimerCallback, null, 1000, 1200);
        }

        #endregion

        #region 成员方法

        public AutoResetEvent EnqueueCommand(ApiInvokeCommand command)
        {
            lock (this)
            {
                AutoResetEvent autoEvent = new AutoResetEvent(false);
                this.mQueues.Enqueue(new Tuple<ApiInvokeCommand, AutoResetEvent>(command, autoEvent));
                return autoEvent;
            }
        }

        private static readonly object obj = new object();

        private void TimerCallback(object state)
        {
            lock (obj)
            {
                if (mQueues.Count > 0)
                {
                    var item = mQueues.Dequeue();
                    item.Item1.Execute();
                    LogUtils4Debug.Logger.Debug($"命令队列弹出命令项并执行...当前数量: { mQueues.Count }");
                    item.Item2.Set();
                }
            }
        }

        #endregion
    }
}
