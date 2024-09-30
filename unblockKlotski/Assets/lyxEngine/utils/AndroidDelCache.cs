using System;
using System.Collections;
using System.Collections.Generic;
using sw.util;
using System.IO;
using UnityEngine;

public class AndroidDelCache
{
    /// <summary>
    /// 删除日志，安卓需要清除广告残留的cache文件
    /// </summary>
    public static void DeleteLogs()
    {
       // DeleteOTLog("/log/");
       // DeleteOTLog("/runlog/");

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
            Debug.LogError(ex.Message);
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
            Debug.LogError(ex.Message);
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
            Debug.LogError(ex.Message);
        }
    }
}
