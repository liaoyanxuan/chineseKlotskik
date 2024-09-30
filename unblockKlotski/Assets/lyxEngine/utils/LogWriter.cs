namespace sw.util
{
    using System;
    using System.IO;
    using System.Threading;
    using UnityEngine;

    public class LogWriter
    {
        private FileStream m_fs;
        private static readonly object m_locker = new object();
        private string m_logFileName = "log_{0}.txt";
        private string m_runlogFileName = "runlog_{0}.txt";

        private string m_logFilePath;
        private string m_logPath = (FileUtilNew.GetPersisitPath() + "/log/");
        private Action<string, sw.util.LogLevel, bool> m_logWriter;
        private StreamWriter m_sw;
        public string error;

        private Action<string, sw.util.LogLevel, bool> m_logWriter1;
        private StreamWriter m_sw1;
        private FileStream m_fs1;

        public LogWriter()
        {
           
          
            #if !NEED_LOG
                        return;
            #endif

            m_logPath = FileUtilNew.GetPersisitPath() + "/log/";
            this.m_logFilePath = this.m_logPath + string.Format(this.m_logFileName, DateTime.Today.ToString("yyyyMMdd"));
            try
            {
                if (!Directory.Exists(this.m_logPath))
                {

                    Directory.CreateDirectory(this.m_logPath);

                }
                this.m_logWriter = new Action<string, sw.util.LogLevel, bool>(this.Write);
                this.m_fs = new FileStream(this.m_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                this.m_sw = new StreamWriter(this.m_fs);
                WriteLog("start.....", LogLevel.DEBUG, false);
                error = "success";
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message + ",data dir:" + FileUtilNew.GetPersisitPath() + ",log path:" + m_logPath);
                error = exception.Message;
            }

            m_logPath = FileUtilNew.GetPersisitPath() + "/runlog/";
            this.m_logFilePath = this.m_logPath + string.Format(this.m_runlogFileName, DateTime.Today.ToString("yyyyMMdd"));
            try
            {
                if (!Directory.Exists(this.m_logPath))
                {

                    Directory.CreateDirectory(this.m_logPath);

                }
                this.m_fs1 = new FileStream(this.m_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                this.m_sw1 = new StreamWriter(this.m_fs1);
                error = "success";
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message + ",data dir:" + FileUtilNew.GetPersisitPath() + ",log path:" + m_logPath);
                error = exception.Message;
            }
        }

        public LogWriter(string directoryname, string filename, FileMode mode = FileMode.Append)
        {
            m_logPath = FileUtilNew.GetLocalPath() + "/" + directoryname + "/";//Application.persistentDataPath + "/" + directoryname + "/";
            m_logFileName = filename; ;
            if (!Directory.Exists(this.m_logPath))
            {
                Directory.CreateDirectory(this.m_logPath);
            }
            this.m_logFilePath = this.m_logPath + m_logFileName;
            try
            {
                this.m_logWriter = new Action<string, sw.util.LogLevel, bool>(this.Write);
                this.m_fs = new FileStream(this.m_logFilePath, mode, FileAccess.Write, FileShare.ReadWrite);
                this.m_sw = new StreamWriter(this.m_fs);
                WriteLog("start.....", LogLevel.DEBUG, false);
                error = "success";
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message);
                error = exception.Message;
                LoggerHelper.ContextInfo = error;
            }
        }

        public void Release()
        {
            lock (m_locker)
            {
                if (this.m_logWriter!=null)
                {
                    this.m_logWriter = null;
                }
                if (this.m_sw != null)
                {
                    this.m_sw.Close();
                    this.m_sw.Dispose();
                    this.m_sw = null;
                }
                if (this.m_fs != null)
                {
                    this.m_fs.Close();
                    this.m_fs.Dispose();
                    this.m_fs = null;
                }
                if (this.m_sw1 != null)
                {
                    this.m_sw1.Close();
                    this.m_sw1.Dispose();
                    this.m_sw1 = null;
                }
                if (this.m_fs1 != null)
                {
                    this.m_fs1.Close();
                    this.m_fs1.Dispose();
                    this.m_fs1 = null;
                }
            }
        }

        public void UploadTodayLog()
        {
        }

        private void Write(string msg, sw.util.LogLevel level, bool writeEditorLog)
        {
            object obj2;
            Monitor.Enter(obj2 = m_locker);
            try
            {
                if (writeEditorLog)
                {
                    sw.util.LogLevel level2 = level;
                    //if (level2 <= sw.util.LogLevel.ERROR)
                    {
                        switch (level2)
                        {
                            case sw.util.LogLevel.DEBUG:
                            case sw.util.LogLevel.INFO:
                                Debug.Log(msg);
                                break;

                            case (sw.util.LogLevel.INFO | sw.util.LogLevel.DEBUG):
                                break;

                            case sw.util.LogLevel.WARNING:
                                Debug.LogWarning(msg);
                                break;

                            case sw.util.LogLevel.ERROR:
                            case sw.util.LogLevel.EXCEPT:
                            case sw.util.LogLevel.CRITICAL:
                                Debug.LogError(msg);
                                break;
                        }

                    }
                }

                if (this.m_sw != null)
                {
                    this.m_sw.WriteLine(msg);
                    this.m_sw.Flush();
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message + "  " + msg + " " + level.ToString() + "    " + writeEditorLog);
            }
            finally
            {
                Monitor.Exit(obj2);
            }
        }

        public void WriteLog(string msg, sw.util.LogLevel level, bool writeEditorLog)
        {
            if(this.m_logWriter == null)
            {
                return;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                this.m_logWriter(msg, level, writeEditorLog);
            else
                this.m_logWriter.BeginInvoke(msg, level, writeEditorLog, null, null);
        }

        public void WriteLog1(string msg)
        {
            if (this.m_sw1 != null)
            {
                this.m_sw1.WriteLine(msg);
                this.m_sw1.Flush();
            }
        }


        public string LogPath
        {
            get
            {
                return this.m_logPath;
            }
        }
    }
}

