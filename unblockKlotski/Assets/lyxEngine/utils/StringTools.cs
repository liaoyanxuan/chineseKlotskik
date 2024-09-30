using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class StringTools
    {
        static private StringTools _instance;

        public static StringTools Instance
        {
            get { 
                if(_instance==null)
                {
                    _instance = new StringTools();
                }
                return _instance;
            }
        }
        /// <summary>
        /// 获取第一个匹配
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regexStr"></param>
        /// <returns></returns>
        static public string GetFirstMatch(string str,string regexStr)
        {
            if(string.IsNullOrEmpty(str)||string.IsNullOrEmpty(regexStr))
            {
                return null;
            }
            Match m = Regex.Match(str, regexStr);
            if(!string.IsNullOrEmpty(m.ToString()))
            {
                return m.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取第一个匹配，返回match
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regexStr"></param>
        /// <returns></returns>
        static public Match GetFirstMatchStruct(string str, string regexStr)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(regexStr))
            {
                return null;
            }
            Match m = Regex.Match(str, regexStr);
            if (!string.IsNullOrEmpty(m.ToString()))
            {
                return m;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 获取所有匹配,返回string[]
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regexStr"></param>
        /// <returns></returns>
        static public string[] GetAllMatchs(string str,string regexStr)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(regexStr))
            {
                return null;
            }
            MatchCollection mc = Regex.Matches(str, regexStr);
            if(mc.Count==0)
            {
                return null;
            }
            string[] matchs = new string[mc.Count];
            for(int i = 0;i<mc.Count;i++)
            {
                matchs[i] = mc[i].Value.ToString();
            }
            return matchs;
        }

        /// <summary>
        /// 获取所有匹配，返回match
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regexStr"></param>
        /// <returns></returns>
        static public Match[] GetAllMatchsStruct(string str, string regexStr)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(regexStr))
            {
                return null;
            }
            MatchCollection mc = Regex.Matches(str, regexStr);
            if (mc.Count == 0)
            {
                return null;
            }
            Match[] matchs = new Match[mc.Count];
            for (int i = 0; i < mc.Count; i++)
            {
                matchs[i] = mc[i];
            }
            return matchs;
        }

        /// <summary>
        /// 获取所有匹配,返回List<string>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regexStr"></param>
        /// <returns></returns>
        static public List<string> GetAllMatchs2(string str, string regexStr)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(regexStr))
            {
                return null;
            }
            MatchCollection mc = Regex.Matches(str, regexStr);
            if (mc.Count == 0)
            {
                return null;
            }
            List<string> matchs = new List<string>();
            for (int i = 0; i < mc.Count; i++)
            {
                matchs.Add(mc[i].Value.ToString());
            }
            return matchs;
        }

        /// <summary>
        /// 格式化字符串，替代{数字}内容
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static public string Format(string str, params object[] args)
        {
            string newStr = str;
            string rexStr = @"\{[0-9]+\}";
            MatchCollection mc = Regex.Matches(str, rexStr);
            if (mc.Count > 0)
            {
                for (int i = 0; i < mc.Count; i++)
                {
                    string subStr = "{" + i + "}";
                    if (i < args.Length)
                    {
                        Type t = args[i].GetType();
                        newStr = newStr.Replace(subStr, args[i].ToString());
                    }
                    else
                    {
                        newStr = newStr.Replace(subStr,"");
                    }

                }


            }
            return newStr;
        }

       
        /// <summary>
        /// 切割字符串，返回string[]
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitStr"></param>
        /// <returns></returns>
        static public string[] Split(string str,string splitStr)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(splitStr))
            {
                return null;
            }
            string[] newStrs = Regex.Split(str, splitStr);
            return newStrs;
        }
        /// <summary>
        /// 切割字符串，返回List<string>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitStr"></param>
        /// <returns></returns>
        static public List<string> Split2(string str, string splitStr)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(splitStr))
            {
                return null;
            }
            string[] newStrs = Regex.Split(str, splitStr);
            List<string> strs = new List<string>();

            for(int i = 0;i<newStrs.Length;i++)
            {
                strs.Add(newStrs[i]);
            }
            return strs;
        }

        static public string[] SplitStr(string str,char splitStr)
        {
            if (string.IsNullOrEmpty(str) || splitStr == null)
            {
                return null;
            }
            return str.Split(splitStr);
        }


        /// <summary>
        /// 去掉换行符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public string RemoveReturn(string str)
        {
            return StringTools.Replace(str, "\r\n|\r|\n", "");
        }

        /// <summary>
        /// 用正则表达式替换字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        static public string Replace(string str,string str1,string str2)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(str1)||string.IsNullOrEmpty(str2))
            {
                return null;
            }
            Regex rgx = new Regex(str1);
            string result = rgx.Replace(str, str2);
            return result;
        }



        /// <summary>
        /// 检查版本是否需要更新
        /// </summary>
        /// <param name="wwwVer"></param>
        /// <param name="localVer"></param>
        /// <returns></returns>
        static public bool isNeedUpdata(string wwwVer, string localVer)
        {
            if (string.IsNullOrEmpty(localVer))
            {
                return true;
            }
            string[] wwwVers = wwwVer.Split('.');
            string[] localVers = localVer.Split('.');
            int len = wwwVers.Length > localVers.Length ? wwwVers.Length : localVers.Length;
            bool needUpdata = false;
            int wwwValue;
            int localValue;
            for (int i = 0; i < len; i++)
            {
                wwwValue = GetValue(wwwVers, i);
                localValue = GetValue(localVers, i);
                if (wwwValue > localValue)
                {
                    needUpdata = true;
                    break;
                }
                if (localValue > wwwValue)
                {
                    break;
                }
            }
            return needUpdata;
        }

        static private int GetValue(string[] strs, int ind)
        {
            if (strs.Length > ind)
            {
                int val = System.Int32.Parse(strs[ind]);
                return val;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获得扩展名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public string GetExName(string str)
        {
            string regexStr = @"(?<=\.)[^\.]+$";
            string strs = GetFirstMatch(str,regexStr);
            return strs;
        }

        /// <summary>
        /// 移除扩展名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public string RemoveExName(string str)
        {
            string regexStr = @".+(?=\.)";
            string strs = GetFirstMatch(str, regexStr);
            if(string.IsNullOrEmpty(strs))
            {
                strs = str;
            }
            return strs;
        }

        /// <summary>
        /// 从字符串获取unicode码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public int String2Unicode(string str)
        {
            System.Text.UnicodeEncoding unicodeEncodeing = new System.Text.UnicodeEncoding();
            byte[] bs = unicodeEncodeing.GetBytes(str);
            int ts = 0;
            for (int i = 0; i < bs.Length; i++)
            {
                int sub = bs[i];
                int addSub = (int)Math.Pow(256, i) * sub;
                ts += addSub;
            }
            return ts;
        }

        /// <summary>
        /// 把字符串拆分再分别获取unicode码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public int[] String2Unicodes(string str)
        {
            int[] res = new int[str.Length];
            for(int i = 0;i<str.Length;i++)
            {
                string subStr = str.Substring(i, 1);
                res[i] = String2Unicode(subStr);
            }
            return res;
        }

        /// <summary>
        /// 获取是否为纯数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public bool IsNumber(string expression)
        {
            string getIsNumber = @"^\-?[0-9]+(\.[0-9]*)?$";
            MatchCollection mc = Regex.Matches(expression, getIsNumber);
            if (mc.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static public string ChangePathFormat(string path)
        {
            string newPath = path.Replace('\\', '/');
            return newPath;
        }
        //private static string appendTempStr = "";
        static public string AppendString(params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0;i<args.Length;i++)
            {
                sb.Append(args[i].ToString());
            }
            //appendTempStr = sb.ToString();
            return sb.ToString();
        }

        public static string UrlEncode(string str)
        {
            return Uri.EscapeUriString(str);
            //StringBuilder sb = new StringBuilder();
            //byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            //for (int i = 0; i < byStr.Length; i++)
            //{
            //    sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            //}

            //return (sb.ToString());
        }

        public static bool CompareVer(string ver1,string ver2)
        {
            string[] verStr1 = ver1.Split('.');
            string[] verStr2 = ver2.Split('.');
            int vn1 = 0;
            int vn2 = 0;
            for(int i = 0;i<verStr1.Length;i++)
            {
                if(i>=verStr2.Length)
                {
                    return true;
                }
                vn1 = int.Parse(verStr1[i]);
                vn2 = int.Parse(verStr2[i]);
                if(vn1>vn2)
                {
                    return true;
                }
                if(vn1<vn2)
                {
                    return false;
                }
            }
            return false;
        }
        static string encryptKey = "fsjy";    //定义密钥  
        #region 加密字符串
        /// <summary> /// 加密字符串   
        /// </summary>  
        /// <param name="str">要加密的字符串</param>  
        /// <returns>加密后的字符串</returns>  
        static public string Encrypt(string str)
        {
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象   

            byte[] key = Encoding.Unicode.GetBytes(encryptKey); //定义字节数组，用来存储密钥    

            byte[] data = Encoding.Unicode.GetBytes(str);//定义字节数组，用来存储要加密的字符串  

            MemoryStream MStream = new MemoryStream(); //实例化内存流对象      

            //使用内存流实例化加密流对象   
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);

            CStream.Write(data, 0, data.Length);  //向加密流中写入数据      

            CStream.FlushFinalBlock();              //释放加密流      

            return Convert.ToBase64String(MStream.ToArray());//返回加密后的字符串  
        }
        #endregion

        #region 解密字符串
        /// <summary>  
        /// 解密字符串   
        /// </summary>  
        /// <param name="str">要解密的字符串</param>  
        /// <returns>解密后的字符串</returns>  
        static public string Decrypt(string str)
        {
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象    

            byte[] key = Encoding.Unicode.GetBytes(encryptKey); //定义字节数组，用来存储密钥    

            byte[] data = Convert.FromBase64String(str);//定义字节数组，用来存储要解密的字符串  

            MemoryStream MStream = new MemoryStream(); //实例化内存流对象      

            //使用内存流实例化解密流对象       
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);

            CStream.Write(data, 0, data.Length);      //向解密流中写入数据     

            CStream.FlushFinalBlock();               //释放解密流      

            return Encoding.Unicode.GetString(MStream.ToArray());       //返回解密后的字符串  
        }
    #endregion


    public static string trimInvisiblByte(string str)
    {
        ASCIIEncoding ascii = new ASCIIEncoding();

        byte[] bytestr = ascii.GetBytes(str);

        
            int beginIndex = 0;
            int LengthMax = bytestr.Length;
            int newLength = bytestr.Length;

            if (bytestr[0] < 48 || bytestr[0] > 57)
            {
                beginIndex = 1;
                newLength = newLength - 1;
            }

            if (bytestr[LengthMax - 1] < 48 || bytestr[LengthMax - 1] > 57)
            {

                newLength = newLength - 1;
                LengthMax = LengthMax - 1;
            }


            if (newLength > 0)
            {
                List<byte> newbytestr = new List<byte>();

                for (int i = beginIndex; i < LengthMax; i++)
                {
                    newbytestr.Add(bytestr[i]);
                }

                return ascii.GetString(newbytestr.ToArray<byte>());
            }
            else
            {
                return "";
            }

        
    }

    public static bool IsNumeric(string str)
    {

        ASCIIEncoding ascii = new ASCIIEncoding();
        
        byte[] bytestr = ascii.GetBytes(str);
       
      
        foreach (byte c in bytestr)
        {
            if (c < 48 || c > 57)
            {
                return false;
            }
        }
        return true;
    }

    public static bool IsNewLineN(string str)
    {
        System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
        byte[] bytestr = ascii.GetBytes(str);
        foreach (byte c in bytestr)
        {
            if (c == 10)
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsNewLineR(string str)
    {
        System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
        byte[] bytestr = ascii.GetBytes(str);
        foreach (byte c in bytestr)
        {
            if (c == 13)
            {
                return true;
            }
        }
        return false;
    }
}

