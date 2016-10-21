using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace YiYuan.Extensions
{
    public sealed class LogFactory
    {
        private static readonly string ModuleName;

        private readonly string _logPath;

        private static readonly bool IsDebugLog;

        private static readonly bool IsInfoLog;

        static LogFactory()
        {
            LogFactory.ModuleName = typeof(LogFactory).Module.Name;
            LogFactory.IsDebugLog = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDebugLog"]);
            LogFactory.IsInfoLog = Convert.ToBoolean(ConfigurationManager.AppSettings["IsInfoLog"]);
        }

        public LogFactory(string logPath = "")
        {
            this._logPath = logPath;
            string path = AppDomain.CurrentDomain.BaseDirectory + "App_Log\\{0}".Formats(new object[]
            {
                logPath
            });
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void Error(string strLog, bool isHour = false)
        {
            LogFactory.Write("{0}\\Error".Formats(new object[]
            {
                this._logPath
            }), strLog, isHour);
        }

        public void Error(string strLog)
        {
            LogFactory.Write("{0}\\Error".Formats(new object[]
            {
                this._logPath
            }), strLog, false);
        }

        public void Error(string dir, string strLog, bool isHour = false)
        {
            LogFactory.Write("{0}\\{1}\\Error".Formats(new object[]
            {
                this._logPath,
                dir
            }), strLog, isHour);
        }

        public void Debug(string strLog, bool isHour = false)
        {
            if (LogFactory.IsDebugLog)
            {
                LogFactory.Write("{0}\\Debug".Formats(new object[]
                {
                    this._logPath
                }), strLog, isHour);
            }
        }

        public void Debug(string dir, string strLog, bool isHour = false)
        {
            if (LogFactory.IsDebugLog)
            {
                LogFactory.Write("{0}\\{1}\\Debug".Formats(new object[]
                {
                    this._logPath,
                    dir
                }), strLog, isHour);
            }
        }

        public void Info(string strLog, bool isHour = false)
        {
            if (LogFactory.IsInfoLog)
            {
                LogFactory.Write("{0}\\Info".Formats(new object[]
                {
                    this._logPath
                }), strLog, isHour);
            }
        }

        public void TicketInfo(string dir, string strLog, bool isHour = false)
        {
            if (LogFactory.IsInfoLog)
            {
                LogFactory.Write("{0}\\{1}\\TicketInfo".Formats(new object[]
                {
                    this._logPath,
                    dir
                }), strLog, isHour);
            }
        }

        public void TicketError(string dir, string strLog, bool isHour = false)
        {
            if (LogFactory.IsInfoLog)
            {
                LogFactory.Write("{0}\\{1}\\TicketError".Formats(new object[]
                {
                    this._logPath,
                    dir
                }), strLog, isHour);
            }
        }

        public void Info(string dir, string strLog, bool isHour = false)
        {
            if (LogFactory.IsInfoLog)
            {
                LogFactory.Write("{0}\\{1}\\Info".Formats(new object[]
                {
                    this._logPath,
                    dir
                }), strLog, isHour);
            }
        }

        public void Warn(string strLog, bool isHour = false)
        {
            LogFactory.Write("{0}\\Warn".Formats(new object[]
            {
                this._logPath
            }), strLog, isHour);
        }

        public void Warn(string dir, string strLog, bool isHour = false)
        {
            LogFactory.Write("{0}\\{1}\\Warn".Formats(new object[]
            {
                this._logPath,
                dir
            }), strLog, isHour);
        }

        public static void Write(string logType, string strLog, bool isHour = false)
        {
            string text = AppDomain.CurrentDomain.BaseDirectory + "\\App_Log\\" + logType + "\\";
            string text2 = "";
            if (isHour)
            {
                text2 = text + DateTime.Now.ToString("yyyy-MM-dd-HH") + ".log";
            }
            else
            {
                text2 = text + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            }
            Mutex mutex = null;
            try
            {
                mutex = new Mutex(false, ExtendHelper.MD5(text2));
                mutex.WaitOne();
            }
            catch
            {
            }
            StringBuilder stringBuilder = new StringBuilder(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss | "));
            try
            {
                StackTrace stackTrace = new StackTrace();
                List<StackFrame> list = stackTrace.GetFrames().Reverse<StackFrame>().ToList<StackFrame>();
                list = list.Skip(list.FindLastIndex((StackFrame a) => a.GetMethod().ReflectedType.FullName.StartsWith("System.")) + 1).ToList<StackFrame>();
                foreach (StackFrame current in list)
                {
                    MethodBase method = current.GetMethod();
                    if (!method.ReflectedType.FullName.StartsWith("Utility.Log"))
                    {
                        stringBuilder.Append(method.ReflectedType.FullName).Append(".").Append(method.Name).Append(" -> ");
                    }
                }
            }
            catch
            {
            }
            StreamWriter streamWriter = null;
            try
            {
                if (strLog != null)
                {
                    stringBuilder.Append(strLog);
                }
                if (!Directory.Exists(text))
                {
                    Directory.CreateDirectory(text);
                }
                streamWriter = new StreamWriter(new FileStream(text2, FileMode.Append, FileAccess.Write), Encoding.UTF8);
                streamWriter.WriteLine(stringBuilder.ToString());
            }
            catch
            {
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                }
            }
            if (mutex != null)
            {
                try
                {
                    mutex.ReleaseMutex();
                    mutex.Close();
                }
                catch
                {
                }
            }
        }
    }
}
