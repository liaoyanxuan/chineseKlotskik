
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using sw.util;
using System;
using System.IO;
namespace CommonLib.zlib
{
    public class ZlibUtil
    {
        /// <summary>
        /// 将字节数组进行压缩后返回压缩的字节数组
        /// </summary>
        /// <param name="data">需要压缩的数组</param>
        /// <returns>压缩后的数组</returns>
        public static byte[] Compress(byte[] data)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (DeflaterOutputStream outstream = new DeflaterOutputStream(stream))
                    {
                        outstream.Write(data, 0, data.Length);
                    }
                    return stream.ToArray();
                }
            }
         catch(Exception ex)
            {
                LoggerHelper.Error(ex.Message);
                return null;
            }



        }
        const int BUFF_SIZE = 100*1024;
        static byte[] decBuff;
        //static MemoryStream decStream;
        /// <summary>
        /// 解压字符数组
        /// </summary>
        /// <param name="data">压缩的数组</param>
        /// <returns>解压后的数组</returns>
        public static byte[] Decompress(byte[] data)
        {

            if (decBuff == null)
                decBuff = new byte[BUFF_SIZE];
            //if (decStream == null)
            //    decStream = new MemoryStream();
            try
            {
                using (MemoryStream stream = new MemoryStream(data))
                {
                
                    using (InflaterInputStream instream = new InflaterInputStream(stream, inf, infBuff))
                    {

                        using (MemoryStream resultstream = new MemoryStream())
                        {
                            while (true)
                            {
                                int len = instream.Read(decBuff, 0, BUFF_SIZE);
                                if (len <= 0)
                                    break;
                                resultstream.Write(decBuff, 0, len);
                            }
                            return resultstream.ToArray();
                        }
                    }

                }
            }
          catch(Exception ex)
            {
                LoggerHelper.Error("zlib uncompress error:" + ex);
                return null;
            }
            finally
            {

            }
           
        }
        static Inflater inf = new Inflater();
        static byte[] infBuff = new byte[4096];
        public static bool DecompressToStream(byte[] data,Stream dest,out int total)
        {

            if (decBuff == null)
                decBuff = new byte[BUFF_SIZE];
            //if (decStream == null)
            //    decStream = new MemoryStream();
            total = 0;
            try
            {
                using (MemoryStream stream = new MemoryStream(data))
                {
        
                    using (InflaterInputStream instream = new InflaterInputStream(stream, inf, infBuff))
                    {

                         
                        while (true)
                        {
                            int len = instream.Read(decBuff, 0, BUFF_SIZE);
                            if (len <= 0)
                                break;
                            dest.Write(decBuff, 0, len);
                            total += len;
                        }

                        return true;
                    }

                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("zlib uncompress error:" + ex);
                
            }
            finally
            {

            }
            return false;
        }
        static void decodeByte(byte[] data,int dataLen,int encOffset, string resEncryptKey)
        {
          
            int klen = resEncryptKey.Length;
            for (int i = 0; i < dataLen; i++)
            {
                int pos = (i + encOffset) % klen;
                data[i] = (byte)(data[i] ^ resEncryptKey[pos]);
            }
         
        }

     
        public static bool Decompress(string src,string dest,string encryptKey=null)
        {
            if (decBuff == null)
                decBuff = new byte[BUFF_SIZE];
            //if (decStream == null)
            //    decStream = new MemoryStream();
            try
            {
                using (FileStream stream = new FileStream(src,FileMode.Open,FileAccess.Read))
                {
                   
                    using (InflaterInputStream instream = new InflaterInputStream(stream, inf, infBuff))
                    {

                        using (FileStream resultstream = new FileStream(dest, FileMode.Create, FileAccess.Write))
                        {
                            int offset = 0;
                            while (true)
                            {
                                int len = instream.Read(decBuff, 0, BUFF_SIZE);
                                if (len <= 0)
                                    break;
                                if (!string.IsNullOrEmpty(encryptKey))
                                {
                                    decodeByte(decBuff,len, offset, encryptKey);
                                    offset += len;
                                }
                                resultstream.Write(decBuff, 0, len);
                            }
                            return true;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("zlib uncompress error:" + ex);
                return false;
            }

                return false;
        }
     
        public static bool DecompressThread(string src, string dest,byte[] decBuff)
        {
            
            //if (decStream == null)
            //    decStream = new MemoryStream();
            try
            {
            using (FileStream stream = new FileStream(src, FileMode.Open, FileAccess.Read))
            {
                using (InflaterInputStream instream = new InflaterInputStream(stream))
                {

                    using (FileStream resultstream = new FileStream(dest, FileMode.Create, FileAccess.Write))
                    {
                        while (true)
                        {
                            int len = instream.Read(decBuff, 0, decBuff.Length);
                            if (len <= 0)
                                break;
                            resultstream.Write(decBuff, 0, len);
                        }
                        return true;
                    }
                }

            }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("zlib uncompress error:" + ex);
                return false;
            }

            return false;
        }
    }
}
