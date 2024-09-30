

using System;
using System.IO;
using UnityEngine;
namespace sw.util
{
    public interface IFileUtilNew_PlatformData
    {
        string InternalGetPersistentDataPath();
    }
    public static class FileUtilNew
    {
        static string _persistData = null;
        static IFileUtilNew_PlatformData _ptData;
        public static void ResetPlatformData(IFileUtilNew_PlatformData data)
        {
            _ptData = data;
        }
        static string _streamingPath = null;
        public static string GetStreamingPath()
        {
            if (_streamingPath == null)
                _streamingPath = Application.streamingAssetsPath;
            return _streamingPath;
        }
        static string _appPath = null;
        public static string GetAppDownloadPath()
        {
            if(_appPath == null)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                _appPath = AndroidUtil.getPackageName();
                if(string.IsNullOrEmpty(_appPath))
                    _appPath = string.Empty;
                else
                    _appPath = "/data/data/"+_appPath+"/files";

#else
                _appPath = GetLocalPath();
#endif
            }
            return _appPath;
        }
        public static string GetPersisitPath()
        {
            if (_persistData == null)
            {
                ////if (string.IsNullOrEmpty(Application.persistentDataPath))
                //if (string.IsNullOrEmpty(FileUtilNew.GetLocalPath()))
                //{
                //    if (_ptData != null)
                //        _persistData = _ptData.InternalGetPersistentDataPath();
                //}
                //else
                    _persistData = FileUtilNew.GetLocalPath();
                    //_persistData = Application.persistentDataPath;
            }
            return _persistData;
        }
        private static string InternalGetPersistentDataPath()
        {
            string path = "";
            try
            {
                IntPtr obj_context = AndroidJNI.FindClass("android.os.Environment");
                IntPtr method_getExternalStorageDirectory = AndroidJNIHelper.GetMethodID(obj_context, "getExternalStorageDirectory", "()Ljava/io/File;", true);
                IntPtr file = AndroidJNI.CallStaticObjectMethod(obj_context, method_getExternalStorageDirectory, new jvalue[0]);
                IntPtr obj_file = AndroidJNI.FindClass("java/io/File");
                IntPtr method_getAbsolutePath = AndroidJNIHelper.GetMethodID(obj_file, "getAbsolutePath", "()Ljava/lang/String;");

                path = AndroidJNI.CallStringMethod(file, method_getAbsolutePath, new jvalue[0]);

                if (path != null)
                {
                    path += "/Android/data/" + GetPackageName() + "/files";
                }
                else
                {
                    path = "";
                }
            }
            catch (Exception e)
            {
                //
            }
            //
            return path;
        }

        private static string GetPackageName()
        {
            string packageName = "";
            try
            {
                using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        IntPtr obj_context = AndroidJNI.FindClass("android/content/ContextWrapper");
                        IntPtr method_getPackageName = AndroidJNIHelper.GetMethodID(obj_context, "getPackageName", "()Ljava/lang/String;");
                        packageName = AndroidJNI.CallStringMethod(obj_Activity.GetRawObject(), method_getPackageName, new jvalue[0]);
                    }
                }
            }
            catch (Exception e)
            {
                //
            }
            return packageName;
        }
        public static string GetFilePath(string fn)
        {
            //string pathFn =Path.Combine( Path.Combine(Application.persistentDataPath ,"res"), fn);
            string pathFn = fn;
            //sw.util.LoggerHelper.Debug("local path:" + pathFn);
            if (File.Exists(pathFn))
            {
                //if (Application.platform == RuntimePlatform.WindowsEditor)
                //    return pathFn;
                if (pathFn.StartsWith("/"))
                    return "file://" + pathFn;
                return "file:///" + pathFn;
            }
            else
            {
                pathFn = Application.streamingAssetsPath + fn;

                if (pathFn.IndexOf("://") >= 0)
                    return pathFn;

                if (pathFn.StartsWith("/"))
                    return "file://" + pathFn;
                return "file:///" + pathFn;
            }
        }
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[0x8000];
            while (true)
            {
                int count = input.Read(buffer, 0, buffer.Length);
                if (count <= 0)
                {
                    return;
                }
                output.Write(buffer, 0, count);
            }
        }
        public static string AdjustWWWUrl(string url)
        {
            if (url.Contains("://"))
                return url;
            if (url.StartsWith("/"))
                return "file://" + url;
            else
                return "file:///" + url;
        }
        private static string localPath = "";
        public static string GetLocalPath()
        {
            if(string.IsNullOrEmpty(localPath)==false)
            {
                return localPath;
            }
            else
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    localPath = Application.persistentDataPath;
                    if(string.IsNullOrEmpty(localPath))
                        localPath = InternalGetPersistentDataPath();
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    localPath = Application.persistentDataPath;
                }
                else
                {
                    localPath = Application.dataPath;
                    int index = localPath.LastIndexOf("/");
                    if (index != -1)
                    {
                        localPath = localPath.Substring(0, index + 1);
                    }
                    localPath = localPath + "LocalFile";
                }
                return localPath;
            }
        }
        static private string editorProjectPath = "";
        public static string GetEditorProjectPath()
        {
            if(string.IsNullOrEmpty(editorProjectPath)==false)
            {
                return editorProjectPath;
            }
            editorProjectPath = Application.dataPath;
            int lastIndex = editorProjectPath.LastIndexOf("Assets");
            if(lastIndex>=0)
            {
                editorProjectPath = editorProjectPath.Substring(0, lastIndex);
            }
            return editorProjectPath;
        }
    }
}
