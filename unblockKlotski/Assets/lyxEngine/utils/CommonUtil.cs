using sw.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.IO;
using UnityEngine.Profiling;
using System.Security.Cryptography;


namespace sw.util
{
    public class CommonUtil
    {
        static public void SetActiveSelf(GameObject go, bool state)
        {
            if (state != go.activeSelf)
                go.SetActive(state);
        }
        public static void CopyArray<T>(List<T> lst, T[] arr)
        {
            foreach (T v in arr)
                lst.Add(v);
        }
        public static void CopyArray<T>(List<T> lst, List<T> arr)
        {
            foreach (T v in arr)
                lst.Add(v);
        }
        public static bool Verify(bool boolCondition, string errorString)
        {
            if (!boolCondition)
            {
                LoggerHelper.Error(errorString);
            }
            return boolCondition;
        }

        public static int countArray(ArrayList arr)
        {
            int count = 0;

            foreach (GameObject o in arr)
                count++;
            return count;
        }
        //类似 AS3 obejct["属性ID"]获取属性的方法
        public static object getObjectValue(string id, object obj)
        {
            object value = null;
            Type type = obj.GetType();//获取类型
            try
            {
                FieldInfo field = type.GetField(id);

                if (field != null)
                {
                    value = field.GetValue(obj);
                }
                else
                {
                    PropertyInfo prop = type.GetProperty(id);
                    if (prop != null)
                        value = prop.GetValue(obj, null);
                }
            }
            catch
            {
                value = null;
            }
            return value;
        }
        //类似 AS3 obejct["属性ID"]设置属性的方法
        public static void setObjectValue(string id, object obj, object data)
        {
            Type type = obj.GetType();//获取类型
            FieldInfo f = type.GetField(id);
            Type type1 = f.FieldType;
            f.SetValue(obj, Convert.ChangeType(data, type1));//给对应属性赋值
        }


        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        public static Color HexToColor(string hex)
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }

        static char[] md5Str = new char[] { '0','1','2','3','4','5','6','7','8','9','a','b','c','d','e','f' };
        static StringBuilder md5Sb;
        static System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        static char[] md5StrBuff = new char[32];
        public static string Md5Hex2Str(byte[] data)
        {
            
            //else
            //    md5Sb.Length = 0;
            for (int i = 0; i < 16; i++)
            {
                md5StrBuff[i * 2] = md5Str[data[i] / 16];
                md5StrBuff[i * 2 + 1] = md5Str[data[i] % 16];
                 

                //hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }
            //md5StrBuff[32] = '\0';
            //string str=  hashString.PadLeft(32, '0');
            return new string(md5StrBuff);
        }
        public static string Md5Sum(string strToEncrypt)
        {
          
            byte[] bytes =System.Text.Encoding.UTF8.GetBytes(strToEncrypt);

            // encrypt bytes
            
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
             
            md5Sb = new StringBuilder(33);
            //else
            //    md5Sb.Length = 0;
            for (int i = 0; i < hashBytes.Length; i++)
            {
                md5Sb.Append(md5Str[hashBytes[i] / 16]);
                md5Sb.Append(md5Str[hashBytes[i] % 16]);

                //hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            //string str=  hashString.PadLeft(32, '0');
            return md5Sb.ToString();
        }
        //攻击类型根据职业转换
        public static bool properlyToOccupation(int occ, int properly)
        {
            if (properly == 15 || properly == 16)
            {
                if (occ == 1 || occ == 3 && properly == 15)
                    return true;
                else if (occ == 2 || occ == 4 && properly == 15)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }
        public static string Md5Sum(byte[] data)
        {


            // encrypt bytes
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(data);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }
        public static string Md5File(string fn)
        {

            using (FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.Read, 8192))
            {
                System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hashBytes = md5.ComputeHash(fs);
                string hashString = "";

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
                }

                return hashString.PadLeft(32, '0');
            }

        }

        //public static bool CheckFunctionOpen(string key, int mainLevel)
        //{
        //    bool b = true;
        //    if (ConfigAsset2.Instance.GetById<FunctionOpenConfig>(key) != null && mainLevel >= ConfigAsset2.Instance.GetById<FunctionOpenConfig>(key).openNeedLevel)
        //    {
        //        b = true;
        //    }
        //    else
        //    {
        //        b = false;
        //    }
        //    return b;
        //}
        public static int getIdByOccupy(String ids, int occupy)
        {

            string[] ary = ids.Split(new string[] { "," }, StringSplitOptions.None);
            if (ary.Length == 4)
            {
                return int.Parse(ary[occupy - 1]);
            }
            else
            {
                return int.Parse(ids);
            }
        }
        public static int GetObjSize(UnityEngine.Object obj)
        {
#if ENABLE_PROFILER
            if (!Profiler.enabled) Profiler.enabled = true;
            return Profiler.GetRuntimeMemorySize(obj);
#else
        return -1;
#endif
        }

        public static void ShowMemInfo(string prefix)
        {
          

        }




        static string encryptKey = "";    //定义密钥 
        static DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();
        static byte[] key = Encoding.UTF8.GetBytes(encryptKey); //定义字节数组，用来存储密钥   
        static byte[] vk = new byte[8];

        /// <summary>
        /// 只需要调用一次就行
        /// </summary>
        /// <param name="k"></param>
        public static void SetKey(string k)
        {
            encryptKey = k;
            if(string.IsNullOrEmpty(encryptKey)==false)
            {
                if (encryptKey.Length < 8)
                {
                    int offset = 8 - encryptKey.Length;
                    for (int i = 0; i < offset; i++)
                    {
                        encryptKey += " ";
                    }
                }
                if (encryptKey.Length > 8)
                {
                    encryptKey = encryptKey.Substring(0, 8);
                }
            }
            
            key = Encoding.UTF8.GetBytes(encryptKey); //定义字节数组，用来存储密钥
        }

        /// <summary> /// 加密字符串   
        /// </summary>  
        /// <param name="str">要加密的字符串</param>  
        /// <returns>加密后的字符串</returns>  
        public static string Encrypt(string str)
        {
            return str;
            //if(string.IsNullOrEmpty(encryptKey)==true)
            //{
            //    return str;
            //}
            ////return StringTools.AppendString(str, encryptKey);
            //byte[] data = Encoding.UTF8.GetBytes(str);//定义字节数组，用来存储要加密的字符串  

            //MemoryStream MStream = new MemoryStream(); //实例化内存流对象      

            ////使用内存流实例化加密流对象   
            //CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(key, vk), CryptoStreamMode.Write);

            //CStream.Write(data, 0, data.Length);  //向加密流中写入数据      

            //CStream.FlushFinalBlock();              //释放加密流 

            //byte[] streamArr = MStream.ToArray();

            //CStream.Close();
            //MStream.Close();

            //CStream.Dispose();
            //MStream.Dispose();

            //string resultStr = Convert.ToBase64String(streamArr);//返回加密后的字符串  
            //resultStr = resultStr.Replace("/", "_");
            //return resultStr;
        }

        /// <summary>  
        /// 解密字符串   
        /// </summary>  
        /// <param name="str">要解密的字符串</param>  
        /// <returns>解密后的字符串</returns>  
        public static string Decrypt(string str)
        {
            if(string.IsNullOrEmpty(encryptKey)==true)
            {
                return str;
            }
            str = str.Replace("_", "/");
            byte[] data = Convert.FromBase64String(str);//定义字节数组，用来存储要解密的字符串  

            MemoryStream MStream = new MemoryStream(); //实例化内存流对象      

            //使用内存流实例化解密流对象       
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(key, vk), CryptoStreamMode.Write);

            CStream.Write(data, 0, data.Length);      //向解密流中写入数据     

            CStream.FlushFinalBlock();               //释放解密流     

            byte[] streamArr = MStream.ToArray();

            MStream.Close();
            CStream.Close();

            MStream.Dispose();
            CStream.Dispose();




            return Encoding.UTF8.GetString(streamArr);       //返回解密后的字符串  
        }



        public static string GetLocalEncryptPath(string str)
        {
            string encryptStr = str;//CommonUtil.Encrypt(str);
            encryptStr = StringTools.AppendString(str.Substring(0, 1) , "/" , encryptStr);
            return encryptStr;
        }

        private static Dictionary<string, string> localEncryptDict;
        static private object lockObj = new object();


        private static void Add2LocalEncryptPath(string key,string val)
        {
            if (localEncryptDict == null)
            {
                localEncryptDict = new Dictionary<string, string>();
            }
            if(localEncryptDict.ContainsKey(key)==true)
            {
                localEncryptDict[key] = val;
            }
            else
            {
                localEncryptDict.Add(key, val);
            }
        }

        public static void ClearData()
        {
            localEncryptDict = null;
        }

    }
}
