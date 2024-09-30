using UnityEngine;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System;
using ZipMgr;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Checksums;
using sw.game.evt;
namespace ZipMgr
{
    public class ZipHelper
    {

        /// <summary>
        /// 所有文件缓存
        /// </summary>
        static List<string> files = new List<string>();

        /// <summary>
        /// 所有空目录缓存
        /// </summary>
        static List<string> paths = new List<string>();

        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="fileToZip">要压缩的文件</param>
        /// <param name="zipedFile">压缩后的文件全名</param>
        /// <param name="compressionLevel">压缩程度，范围0-9，数值越大，压缩程序越高</param>
        /// <param name="blockSize">分块大小</param>
        static public void ZipFile(string fileToZip, string zipedFile, int compressionLevel, int blockSize)
        {
            if (!System.IO.File.Exists(fileToZip))//如果文件没有找到，则报错
            {
                throw new FileNotFoundException("The specified file " + fileToZip + " could not be found. Zipping aborderd");
            }

            FileStream streamToZip = new FileStream(fileToZip, FileMode.Open, FileAccess.Read);
            FileStream zipFile = File.Create(zipedFile);
            ZipOutputStream zipStream = new ZipOutputStream(zipFile);
            ZipEntry zipEntry = new ZipEntry(fileToZip);
            zipStream.PutNextEntry(zipEntry);
            zipStream.SetLevel(compressionLevel);
            byte[] buffer = new byte[blockSize];
            int size = streamToZip.Read(buffer, 0, buffer.Length);
            zipStream.Write(buffer, 0, size);

            try
            {
                while (size < streamToZip.Length)
                {
                    int sizeRead = streamToZip.Read(buffer, 0, buffer.Length);
                    zipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }
            }
            catch (Exception ex)
            {
                GC.Collect();
                throw ex;
            }

            zipStream.Finish();
            zipStream.Close();
            streamToZip.Close();
            GC.Collect();
        }

        /// <summary>
        /// 压缩目录（包括子目录及所有文件）
        /// </summary>
        /// <param name="rootPath">要压缩的根目录</param>
        /// <param name="destinationPath">保存路径</param>
        /// <param name="compressLevel">压缩程度，范围0-9，数值越大，压缩程序越高</param>
        public void ZipFileFromDirectory(List<ZipData> datas, string destinationPath, int compressLevel)
        {

            // +"\\";//得到当前路径的位置，以备压缩时将所压缩内容转变成相对路径。
            Crc32 crc = new Crc32();
            ZipOutputStream outPutStream = new ZipOutputStream(File.Create(destinationPath));
            outPutStream.SetLevel(compressLevel); // 0 - store only to 9 - means best compression
            for (int i = 0; i < datas.Count; i++)
            {
                foreach (string file in datas[i].files)
                {
                    string exName = Tools.FilePathHelper.GetExName(file);
                    if (exName == "manifest")
                    {
                        continue;
                    }
                    FileStream fileStream = File.OpenRead(file);//打开压缩文件
                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    ZipEntry entry = new ZipEntry(file.Replace(datas[i].pathMark, string.Empty));
                    entry.DateTime = DateTime.Now;
                    // set Size and the crc, because the information
                    // about the size and crc should be stored in the header
                    // if it is not set it is automatically written in the footer.
                    // (in this case size == crc == -1 in the header)
                    // Some ZIP programs have problems with zip files that don't store
                    // the size and crc in the header.
                    entry.Size = fileStream.Length;
                    fileStream.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    outPutStream.PutNextEntry(entry);
                    outPutStream.Write(buffer, 0, buffer.Length);
                }

                ZipHelper.files.Clear();

                foreach (string emptyPath in ZipHelper.paths)
                {
                    ZipEntry entry = new ZipEntry(emptyPath.Replace(datas[i].pathMark, string.Empty) + "/");
                    outPutStream.PutNextEntry(entry);
                }
            }


            ZipHelper.paths.Clear();
            outPutStream.Finish();
            outPutStream.Close();
            GC.Collect();
        }


        /// <summary>
        /// 压缩目录（包括子目录及所有文件）
        /// </summary>
        /// <param name="rootPath">要压缩的根目录</param>
        /// <param name="destinationPath">保存路径</param>
        /// <param name="compressLevel">压缩程度，范围0-9，数值越大，压缩程序越高</param>
        static public void ZipFileFromList(List<string> files, string destinationPath, string pathMark, int compressLevel)
        {

            // +"\\";//得到当前路径的位置，以备压缩时将所压缩内容转变成相对路径。
            Crc32 crc = new Crc32();
            ZipOutputStream outPutStream = new ZipOutputStream(File.Create(destinationPath));
            outPutStream.SetLevel(compressLevel); // 0 - store only to 9 - means best compression

            foreach (string file in files)
            {
                FileStream fileStream = File.OpenRead(file);//打开压缩文件
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                ZipEntry entry = new ZipEntry(file.Replace(pathMark, string.Empty));
                entry.DateTime = DateTime.Now;
                // set Size and the crc, because the information
                // about the size and crc should be stored in the header
                // if it is not set it is automatically written in the footer.
                // (in this case size == crc == -1 in the header)
                // Some ZIP programs have problems with zip files that don't store
                // the size and crc in the header.
                entry.Size = fileStream.Length;
                fileStream.Close();
                crc.Reset();
                crc.Update(buffer);
                entry.Crc = crc.Value;
                outPutStream.PutNextEntry(entry);
                outPutStream.Write(buffer, 0, buffer.Length);
            }

            ZipHelper.files.Clear();




            ZipHelper.paths.Clear();
            outPutStream.Finish();
            outPutStream.Close();
            GC.Collect();
        }

        /// <summary>
        /// 取得目录下所有文件及文件夹，分别存入files及paths
        /// </summary>
        /// <param name="rootPath">根目录</param>
        static private void GetAllDirectories(string rootPath)
        {
            string[] subPaths = Directory.GetDirectories(rootPath);//得到所有子目录
            foreach (string path in subPaths)
            {
                GetAllDirectories(path);//对每一个字目录做与根目录相同的操作：即找到子目录并将当前目录的文件名存入List
            }
            string[] files = Directory.GetFiles(rootPath);
            foreach (string file in files)
            {
                ZipHelper.files.Add(file);//将当前目录中的所有文件全名存入文件List
            }
            if (subPaths.Length == files.Length && files.Length == 0)//如果是空目录
            {
                ZipHelper.paths.Add(rootPath);//记录空目录
            }
        }

        static public int GetUnzipCount(string zipFilePath)
        {
            if (zipFilePath == string.Empty)
            {
                throw new Exception("压缩文件不能为空！");
            }
            if (!File.Exists(zipFilePath))
            {
                throw new FileNotFoundException("压缩文件不存在！");
            }

            int count = 0;
            using (var s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    count++;
                    int size;
                    byte[] data = new byte[2048];
                    while (true)
                    {
                        size = s.Read(data, 0, data.Length);
                        if (size > 0)
                        {

                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return count;
        }

        static public void UnZipFile(string zipFilePath, string unZipDir)
        {

            if (zipFilePath == string.Empty)
            {
              
                return;
                //throw new Exception("压缩文件不能为空！");
            }
            if (!File.Exists(zipFilePath))
            {
               
                return;
                //throw new FileNotFoundException("压缩文件不存在！");
            }
            //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹  
            if (unZipDir == string.Empty)
                unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
            if (!unZipDir.EndsWith("/"))
                unZipDir += "/";
            if (!Directory.Exists(unZipDir))
                Directory.CreateDirectory(unZipDir);
            int ind = 0;
            using (var s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    ind++;
                    //EventDispatcher.DispatchEventMainThread(UIEventType.UPDATE_UNZIP_NUM, ind);
                    //string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);
                    if (string.IsNullOrEmpty(fileName))
                        continue;
                    //string realFileName = sw.util.CommonUtil.Encrypt(fileName);
                    string directoryName = fileName.Substring(0, 1).ToLower();
                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        Directory.CreateDirectory(unZipDir + directoryName);
                    }
                    //if (directoryName != null && !directoryName.EndsWith("/"))
                    //{
                    //}
                    //sw.util.LoggerHelper.Debug("unzip:" + fileName + "," + directoryName);
                    if (fileName != String.Empty)
                    {
                        string uFileName = unZipDir + directoryName + "/" + fileName;
                        using (FileStream streamWriter = File.Create(uFileName)) //File.Create(unZipDir + theEntry.Name))
                        {

                            int size;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        //if (realFileName != fileName)
                        //{
                        //    if (File.Exists(unZipDir + directoryName + "/" + realFileName))
                        //    {
                        //        File.Delete(unZipDir + directoryName + "/" + realFileName);
                        //    }

                        //    File.Move(uFileName, unZipDir + directoryName + "/" + realFileName);
                        //}
                        
                    }
                }
            }
         
        }

        /// <summary>
        /// 解压缩文件(压缩文件中含有子目录)
        /// </summary>
        /// <param name="zipfilepath">待解压缩的文件路径</param>
        /// <param name="unzippath">解压缩到指定目录</param>
        /// <returns>解压后的文件列表</returns>
        static public List<string> UnZip(string zipfilepath, string unzippath)
        {
            //解压出来的文件列表
            List<string> unzipFiles = new List<string>();

            //检查输出目录是否以“\\”结尾
            if (unzippath.EndsWith("/") == false || unzippath.EndsWith(":/") == false)
            {
                unzippath += "/";
            }

            ZipInputStream s = new ZipInputStream(File.OpenRead(zipfilepath));
            ZipEntry theEntry;
            int ind = 0;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                ind++;
                //EventDispatcher.DispatchEventMainThread(UIEventType.UPDATE_UNZIP_NUM, ind);
                string directoryName = Path.GetDirectoryName(unzippath);
                string fileName = Path.GetFileName(theEntry.Name);

                //生成解压目录【用户解压到硬盘根目录时，不需要创建】
                if (!string.IsNullOrEmpty(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                if (fileName != String.Empty)
                {
                    //如果文件的压缩后大小为0那么说明这个文件是空的,因此不需要进行读出写入
                    if (theEntry.CompressedSize == 0)
                        break;
                    //解压文件到指定的目录
                    directoryName = Path.GetDirectoryName(unzippath + theEntry.Name);
                    //建立下面的目录和子目录
                    Directory.CreateDirectory(directoryName);

                    //记录导出的文件
                    unzipFiles.Add(unzippath + theEntry.Name);

                    FileStream streamWriter = File.Create(unzippath + theEntry.Name);

                    int size = 2048;
                    byte[] data = new byte[2048];
                    while (true)
                    {
                        size = s.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            streamWriter.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }
                    streamWriter.Close();
                }
            }
            s.Close();
            GC.Collect();
      
            return unzipFiles;
        }
    }
}

