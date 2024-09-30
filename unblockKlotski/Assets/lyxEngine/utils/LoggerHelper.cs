namespace sw.util
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using UnityEngine;

    public class LoggerHelper
    {
        public static sw.util.LogLevel CurrentLogLevels = (sw.util.LogLevel.CRITICAL | sw.util.LogLevel.EXCEPT | sw.util.LogLevel.ERROR | sw.util.LogLevel.WARNING | sw.util.LogLevel.INFO | sw.util.LogLevel.DEBUG);
        //public static sw.util.LogLevel CurrentLogLevels = (sw.util.LogLevel.CRITICAL | sw.util.LogLevel.EXCEPT | sw.util.LogLevel.ERROR | sw.util.LogLevel.WARNING | sw.util.LogLevel.INFO);
        public static string DebugFilterStr = string.Empty;
        private static ulong index = 0L;
        private static sw.util.LogWriter m_logWriter = new sw.util.LogWriter();
        private const bool SHOW_STACK = true;
        static Queue<string> postErrors;

        public static List<string> localShowLog = new List<string>();
        public static List<string> localShowLog_normal = new List<string>();
        public static List<string> localShowLog_error = new List<string>();
        private static Action< string,object, string> sdkReporter = ReportError;
        public static bool ShowLogLocal;//显示在本地ui界面上

        //public static int mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        static Thread mainThread;
        public static void Start()
        {
            mainThread = Thread.CurrentThread;
        }
        public static void Dispose()
        {
            if (postErrThread != null)
                postErrThread.Abort();
            postErrThread = null;
        }
        static Thread postErrThread;
        static LoggerHelper()
        {
#if UNITY_5
            Application.logMessageReceived += LoggerHelper.ProcessExceptionReport;
#else
          //  Application.RegisterLogCallback (LoggerHelper.ProcessExceptionReport);
#endif
            
            //var v = ThreadLogger.Instance;


        }
        public static string ContextInfo = "";
        public static string userid, ptid;
        public static string appver="1.0.1";
        static string deviceModel, operatingSystem, graphicsDeviceName;
        static int systemMemorySize;
        static private string tempMsg = "";

        public static bool IsMainThread
        {
            get { return System.Threading.Thread.CurrentThread.Equals(mainThread); }
        }
        static void ReportError(string title,object message, string stackMsg)
        {
             

        }
        public static void StartPostError()
        {
            deviceModel = SystemInfo.deviceModel;
            operatingSystem = SystemInfo.operatingSystem;
            graphicsDeviceName = SystemInfo.graphicsDeviceName;
            systemMemorySize = SystemInfo.systemMemorySize;
            postErrors = new Queue<string>();
            postErrThread = new Thread(new ThreadStart(PostError));
            postErrThread.Start();
        }
        public static string PostWebRequest(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.Timeout = 600000;
                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                //Log("post error log error:"+ex.Message, sw.util.LogLevel.CRITICAL, true);
            }
            return ret;
        }
        public static string reportUrl = "http://192.168.1.11:88/int_error_inside.php";
        static void PostError()
        {

            while (postErrors != null)
            {
                tempMsg = null;
                lock (postErrors)
                {
                    if (postErrors.Count > 0)
                    {
                        tempMsg = postErrors.Dequeue();
                    }
                }


                if (tempMsg == null)
                    Thread.Sleep(50);
                else
                    PostWebRequest(reportUrl, new StringBuilder().Append("ptid=").Append(ptid).Append("&msg=").Append(WWW.EscapeURL(ContextInfo + "\n")).Append(WWW.EscapeURL(tempMsg)).Append("&tp=0&appVer=").Append(appver).
                        Append("&device=").Append(deviceModel)
                        .Append(";o:").Append(operatingSystem)
                        .Append(";g:").Append(graphicsDeviceName)
                        .Append(";m:").Append(systemMemorySize).Append("&userid=").Append(userid).ToString(), Encoding.UTF8);

            }

        }



        /// <summary>
        /// 删除日志，安卓需要清除广告残留的cache文件
        /// </summary>
        public static void DeleteLogs()
        {
            DeleteOTLog("/log/");
            DeleteOTLog("/runlog/");

            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                DeleteCache();
                DeleteDownload();
            }
        }
            /// <summary>
            /// 删除过期的日志文件
            /// </summary>
        private static void DeleteOTLog(string logfile)
        {
            try
            {
                string m_logPath = FileUtilNew.GetPersisitPath() + logfile;
                DateTime dtNow = DateTime.Now;
                int saveDay = 1;   //2天前的日志删除
                //遍历文件夹
                DirectoryInfo theFolder = new DirectoryInfo(m_logPath);
                FileInfo[] fileInfo = theFolder.GetFiles("*.txt", SearchOption.AllDirectories);
                if (fileInfo != null)
                {
                    foreach (FileInfo tmpfi in fileInfo) //遍历文件
                    {
                        TimeSpan ts = dtNow.Subtract(tmpfi.LastWriteTime);
                        if (ts.TotalDays >= saveDay)//超过了保存时间，删除文件
                        {
                            tmpfi.Delete();
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private static void DeleteCache()
        {

            try
            {
                string m_logPath = FileUtilNew.GetPersisitPath() + "/../cache/";

                if (Directory.Exists(m_logPath))
                {
                    DateTime dtNow = DateTime.Now;
                    int saveDay = 1;   //1天前的cache
                                       //遍历文件夹
                    DirectoryInfo theFolder = new DirectoryInfo(m_logPath);
                    FileInfo[] fileInfo = theFolder.GetFiles("*", SearchOption.AllDirectories);


                    if (fileInfo != null)
                    {
                        foreach (FileInfo tmpfi in fileInfo) //遍历文件
                        {
                            TimeSpan ts = dtNow.Subtract(tmpfi.LastWriteTime);
                            if (ts.TotalDays >= saveDay)//超过了保存时间，删除文件
                            {
                                UnityEngine.Debug.Log("DeleteCache:" + tmpfi.FullName + " TotalDays:" + ts.TotalDays);
                                tmpfi.Delete();
                            }
                            
                        }
 
                    }
                }
               
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }


        private static void DeleteDownload()
        {

            try
            {
                string m_logPath = FileUtilNew.GetPersisitPath() + "/Download/";

                if (Directory.Exists(m_logPath))
                {
                    DateTime dtNow = DateTime.Now;
                    double saveDay = 0.5;   //1天前的cache
                                       //遍历文件夹
                    DirectoryInfo theFolder = new DirectoryInfo(m_logPath);
                    FileInfo[] fileInfo = theFolder.GetFiles("*", SearchOption.AllDirectories);


                    if (fileInfo != null)
                    {
                        foreach (FileInfo tmpfi in fileInfo) //遍历文件
                        {
                            TimeSpan ts = dtNow.Subtract(tmpfi.LastWriteTime);
                            if (ts.TotalDays >= saveDay)//超过了保存时间，删除文件
                            {
                                UnityEngine.Debug.Log("DeleteDownload:" + tmpfi.FullName + " TotalDays:" + ts.TotalDays);
                                tmpfi.Delete();
                            }

                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }


        public static void Critical(object message, bool isShowStack = false)
        {
            if (sw.util.LogLevel.CRITICAL == (CurrentLogLevels & sw.util.LogLevel.CRITICAL))
            {
                //string msg = string.Concat(new object[] { " [CRITICAL]: ", message, '\n', isShowStack ? GetStacksInfo() : "" });
                tempMsg = StringTools.AppendString(" [CRITICAL]: ", message, '\n', isShowStack ? GetStacksInfo() : "");
                Log(tempMsg, sw.util.LogLevel.CRITICAL, true);
                if (postErrors != null)
                {
                    lock (postErrors)
                    {
                        postErrors.Enqueue(tempMsg);
                    }
                }
            }
        }

        public static void Critical2(object message, bool isShowStack = false)
        {
            if (sw.util.LogLevel.CRITICAL == (CurrentLogLevels & sw.util.LogLevel.CRITICAL))
            {
                //string msg = string.Concat(new object[] { " [CRITICAL]: ", message, '\n', isShowStack ? GetStacksInfo() : "" });
                tempMsg = StringTools.AppendString(" [CRITICAL]: ", message, '\n', isShowStack ? GetStacksInfo() : "");
                Log(tempMsg, sw.util.LogLevel.DEBUG, true);
            }
        }

        public static void Debug(object message, bool isShowStack = false, int user = 0)
        {
            //if(IsMainThread==false)
            //{
            //    ThreadLogger.Debug(message.ToString());
            //    return;
            //}
            if (!(DebugFilterStr != "") && (sw.util.LogLevel.DEBUG == (CurrentLogLevels & sw.util.LogLevel.DEBUG)))
            {
                object[] objArray = new object[5];
                objArray[0] = " [DEBUG]: ";
                objArray[1] = isShowStack ? GetStackInfo() : "";
                objArray[2] = message;
                objArray[3] = " Index = ";
                index += (ulong)1L;
                objArray[4] = index;
                //Log(string.Concat(objArray), sw.util.LogLevel.DEBUG, true);
                Log(StringTools.AppendString(objArray[0],objArray[1],objArray[2],objArray[3],objArray[4]), sw.util.LogLevel.DEBUG, true);
            }
        }

        public static void DebugStackFrame(object message, int stackframeIdx)
        {

            if (!(DebugFilterStr != "") && (sw.util.LogLevel.DEBUG == (CurrentLogLevels & sw.util.LogLevel.DEBUG)))
            {

                Log(" [DEBUG]: " + GetStackInfo(stackframeIdx) + " " + message, sw.util.LogLevel.DEBUG, true);

            }
        }

        public static void Debug(string filter, object message, bool isShowStack = true)
        {
            //if (IsMainThread == false)
            //{
            //    ThreadLogger.Debug(StringTools.AppendString(filter,":",message.ToString()));
            //    return;
            //}
            if ((!(DebugFilterStr != "") || !(DebugFilterStr != filter)) && (sw.util.LogLevel.DEBUG == (CurrentLogLevels & sw.util.LogLevel.DEBUG)))
            {

                //Log(" [DEBUG]: " + (isShowStack ? GetStackInfo() : "") + message, sw.util.LogLevel.DEBUG, true);
                Log(StringTools.AppendString(" [DEBUG]: " , (isShowStack ? GetStackInfo() : "") , message), sw.util.LogLevel.DEBUG, true);

            }
        }

        public static void Error(object message, bool isShowStack = true)
        {
            //if (IsMainThread == false)
            //{
            //    ThreadLogger.Error(message.ToString());
            //    return;
            //}

            if (sw.util.LogLevel.ERROR == (CurrentLogLevels & sw.util.LogLevel.ERROR))
            {
                //string msg = string.Concat(new object[] { " [ERROR]: ", message, '\n', isShowStack ? GetStacksInfo() : "" });
                string stackMsg =  isShowStack ? GetStacksInfo() : "";
                tempMsg = StringTools.AppendString(" [ERROR]: ", message, '\n', stackMsg);
                Log(tempMsg, sw.util.LogLevel.ERROR, true);

        #if UNITY_ANDROID
                //BuglyAgent.ReportException("error", message.ToString(), stackMsg);
        #endif

#if UNITY_ANDROID && !UNITY_EDITOR


                string msg = message.ToString();
                int p = msg.IndexOf("\n");
                if(p>0)
                {
                    stackMsg  = msg.Substring(p+1)+"\n"+stackMsg;
                    msg = msg.Substring(0,p);
                }
                ReportError("[ERROR]", msg, stackMsg);
                //sdkReporter.BeginInvoke("[ERROR]",message,stackMsg,null,null);
#endif
                if (postErrors != null)
                {
                    lock (postErrors)
                    {
                        postErrors.Enqueue(tempMsg);
                    }
                }
            }

        }

        public static void Except(Exception ex, object message)
        {
            if (sw.util.LogLevel.EXCEPT == (CurrentLogLevels & sw.util.LogLevel.EXCEPT))
            {
                Exception innerException = ex;
                while (innerException.InnerException != null)
                {
                    innerException = innerException.InnerException;
                }
                //Log(" [EXCEPT]: " + ((message == null) ? "" : (message + "\n")) + ex.Message + innerException.StackTrace, sw.util.LogLevel.CRITICAL, true);
                string msg = StringTools.AppendString( ((message == null) ? "" : (message + "\n")) , ex.Message  );
                Log(StringTools.AppendString(" [EXCEPT]: ",msg) , sw.util.LogLevel.CRITICAL, true);
#if UNITY_ANDROID &&!UNITY_EDITOR
                ReportError("[EXCEPT]",msg,innerException.StackTrace);
                //sdkReporter.BeginInvoke("[EXCEPT]",msg,innerException.StackTrace,null,null);
#endif
            }
        }

        private static string GetStackInfo()
        {

            StackTrace trace = new StackTrace(true);
            if (trace.GetFrame(2) == null)
            {
                //UnityEngine.
                return string.Empty;
            }

            StackFrame stackFrame = trace.GetFrame(2);
            MethodBase method = stackFrame.GetMethod();
            String filename = stackFrame.GetFileName();

            return string.Format("{0}.{1}(): ", method.DeclaringType.ToString(), method.Name);
            //  return string.Format("{0}: ", stackFrame.ToString());
        }

        private static string GetStackInfo(int frameidx)
        {

            StackTrace trace = new StackTrace(true);
            if (trace.GetFrame(frameidx) == null)
            {
                //UnityEngine.
                return string.Empty;
            }

            StackFrame stackFrame = trace.GetFrame(frameidx);
            MethodBase method = stackFrame.GetMethod();
            String filename = stackFrame.GetFileName();
            return string.Format("{0}.{1}(): ", filename, method.Name);
        }

        private static string GetStacksInfo()
        {
            StringBuilder builder = new StringBuilder();
            StackFrame[] frames = new StackTrace(true).GetFrames();
            for (int i = 2; i < frames.Length; i++)
            {
                builder.AppendLine(frames[i].ToString());
            }
            return builder.ToString();
        }

        public static void Info(object message, bool isShowStack = true)
        {
            if (sw.util.LogLevel.INFO == (CurrentLogLevels & sw.util.LogLevel.INFO))
            {
                //Log(" [INFO]: " + (isShowStack ? GetStackInfo() : "") + message, sw.util.LogLevel.INFO, true);
                Log(StringTools.AppendString(" [INFO]: " , (isShowStack ? GetStackInfo() : "") , message), sw.util.LogLevel.INFO, true);
            }
        }
        public static Action<string, LogLevel> OnLog;
        public static Action<string, LogLevel> testDebugLog;

        private static void Log(string message, sw.util.LogLevel level, bool writeEditorLog = true)
        {
            if (m_logWriter == null)
                return;
            tempMsg = StringTools.AppendString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"), message);
            m_logWriter.WriteLog(tempMsg, level, writeEditorLog);
            if (OnLog != null)
                OnLog(tempMsg, level);
            if (testDebugLog != null)//测试脚本记录错误日志
                testDebugLog(tempMsg, level);
            if (ShowLogLocal)
            {
                switch (level)
                {
                    case LogLevel.ERROR:
                        tempMsg = StringTools.AppendString("[ff0000]", tempMsg, "[-]");
                        localShowLog.Add(tempMsg);
                        localShowLog_error.Add(tempMsg);
                        break;
                    case LogLevel.WARNING:
                        //sb.Append("[ffff00]").Append(msg).Append("[-]");
                        //localShowLog.Add(sb.ToString());
                        break;
                    default:
                        localShowLog.Add(tempMsg);
                        localShowLog_normal.Add(tempMsg);
                        break;
                }

                if (localShowLog.Count > 2000)
                    localShowLog.RemoveAt(0);
            }

            //UnityEngine.
            //Messenger.Broadcast<string>(CommonEvent.SHOW_STATUS, message);

        }
        public static void log1(string msg)
        {
            return;
            tempMsg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff") + msg;
            m_logWriter.WriteLog1(tempMsg);
        }
        public static string GetInfo()
        {
            return m_logWriter.LogPath + "," + m_logWriter.error;
        }
        private static void ProcessExceptionReport(string message, string stackTrace, LogType type)
        {
            sw.util.LogLevel dEBUG = sw.util.LogLevel.DEBUG;
            switch (type)
            {
                case LogType.Error:
                    dEBUG = sw.util.LogLevel.ERROR;
                    break;

                case LogType.Assert:
                    dEBUG = sw.util.LogLevel.DEBUG;
                    break;

                case LogType.Warning:
                    dEBUG = sw.util.LogLevel.WARNING;
                    break;

                case LogType.Log:
                    dEBUG = sw.util.LogLevel.DEBUG;
                    return;
                    break;

                case LogType.Exception:
                    dEBUG = sw.util.LogLevel.EXCEPT;
                    break;
            }
            if (dEBUG == (CurrentLogLevels & dEBUG))
            {
                //string msg = string.Concat(new object[] { " [SYS_", dEBUG, "]: ", message, '\n', stackTrace });
                tempMsg = StringTools.AppendString(new object[] { " [SYS_", dEBUG, "]: ", message, '\n', stackTrace });
                Log(tempMsg, dEBUG, false);
                if (postErrors != null && dEBUG == LogLevel.ERROR || dEBUG == LogLevel.EXCEPT)
                {

                    lock (postErrors)
                    {
                        postErrors.Enqueue(tempMsg);
                    }

                }
            }
        }

        public static void Release()
        {
            m_logWriter.Release();
            if(postReportDataThread!=null)
            {
                postReportDataThread.Abort();
            }
        }

        public static void UploadLogFile()
        {
            m_logWriter.UploadTodayLog();
        }

        public static void Warning(object message, bool isShowStack = true)
        {
            //if (IsMainThread == false)
            //{
            //    ThreadLogger.Warn(message.ToString());
            //    return;
            //}
            if (sw.util.LogLevel.WARNING == (CurrentLogLevels & sw.util.LogLevel.WARNING))
            {
                //Log(" [WARNING]: " + (isShowStack ? GetStackInfo() : "") + message, sw.util.LogLevel.WARNING, true);
                Log(StringTools.AppendString(" [WARNING]: " , (isShowStack ? GetStackInfo() : "") , message), sw.util.LogLevel.WARNING, true);
            }
        }

        public static sw.util.LogWriter LogWriter
        {
            get
            {
                return m_logWriter;
            }
        }
        static Queue<string> postReportData;
        static Thread postReportDataThread;
        // public static string reportDataUrl = "http://182.254.229.168/syinterface/report3.php";
        public static void StartPostReportData()
        {
            postReportData = new Queue<string>();
            postReportDataThread = new Thread(new ThreadStart(PostReportData));
            postReportDataThread.Start();
        }
        static void PostReportData()
        {
            while (postReportData != null)
            {
                tempMsg = null;
                lock (postReportData)
                {
                    if (postReportData.Count > 0)
                    {
                        tempMsg = postReportData.Dequeue();
                    }
                }
                if (tempMsg == null)
                {
                    Thread.Sleep(50);
                }
                else
                {
                    //PostWebRequest(GameConfig.srvListUrl + "/report3.php", tempMsg, Encoding.UTF8);
                }
            }
        }
        public static void reportData(string message)
        {
            if (postReportData != null)
            {
                lock (postReportData)
                {
                    postReportData.Enqueue(message.ToString());
                    //LoggerHelper.Debug(StringTools.AppendString("reportData" , message));
                }
            }
        }
    }
}

