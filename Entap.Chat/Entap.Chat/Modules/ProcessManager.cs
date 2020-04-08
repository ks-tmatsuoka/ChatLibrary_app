using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Entap.Chat.Modules
{
    /// <summary>
    /// プロセスの多重実行を制御するため実行状態を管理する
    /// </summary>
    public class ProcessManager
    {
        public static ProcessManager Current => instance;
        private static readonly ProcessManager instance = new ProcessManager();


        /// <summary>
        /// プロセスの実行状態
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 実行中プロセス名
        /// </summary>
        public string RunningProcessName { get; private set; }

        /// <summary>
        /// プロセスを実行する
        /// </summary>
        /// <param name="processName">プロセス名</param>
        /// <param name="action">同期処理</param>
        public void Invoke(string processName, Action action)
        {
            bool started = false;
            try
            {
                started = OnStart(processName);
                if (started)
                    action();
            }
            finally
            {
                if (started)
                    OnComplete();
            }
        }

        /// <summary>
        /// プロセスを実行する
        /// </summary>
        /// <param name="processName">プロセス名</param>
        /// <param name="funcTask">非同期処理</param>
        public async void Invoke(string processName, Func<Task> funcTask)
        {
            bool started = false;
            try
            {
                started = OnStart(processName);
                if (started)
                    await funcTask().ConfigureAwait(false);
            }
            finally
            {
                if (started)
                    OnComplete();
            }
        }

        /// <summary>
        /// プロセス実行時処理
        /// </summary>
        /// <param name="processName">プロセス名</param>
        bool OnStart(string processName)
        {
            if (IsRunning)
            {
                Debug.WriteLine($"[Warning]Process is Running : {RunningProcessName}");
                return false;
            }

            IsRunning = true;
            RunningProcessName = processName;
            Debug.WriteLine($"[Trace]Start Process : {RunningProcessName}");
            return true;
        }

        /// <summary>
        /// プロセス完了時処理
        /// </summary>
        void OnComplete()
        {
            Debug.WriteLine($"[Trace]Process Completed : {RunningProcessName}");
            IsRunning = false;
            RunningProcessName = null;
        }
    }
}