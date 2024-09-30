using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZipMgr
{
    public class ZipData
    {
        public string path;
        public string pathMark;
        public List<string> files;
        public List<string> paths;

        public ZipData(string p)
        {
            InitData();
            path = p;
            GetPathMark();
            GetAllDirectories(p);
        }

        private void InitData()
        {
            files = new List<string>();
            paths = new List<string>();
        }

        private void GetPathMark()
        {
            if(path[path.Length-1]=='/')
            {
                pathMark = path.Substring(0, path.Length - 1);
            }
            else
            {
                pathMark = path;
            }
            int ind = pathMark.LastIndexOf("/");
            if(ind>=0)
            {
                pathMark = pathMark.Substring(0, ind);
            }
        }
        private void GetAllDirectories(string rootPath)
        {
            if(Tools.FileManager.IsDirectoryExists(rootPath)==false)
            {
                //UnityEngine.Debug.Log("打包路径不存在：" + rootPath);
                return;
            }
            string[] subPaths = Directory.GetDirectories(rootPath);//得到所有子目录
            foreach (string path in subPaths)
            {
                GetAllDirectories(path);//对每一个字目录做与根目录相同的操作：即找到子目录并将当前目录的文件名存入List
            }
            string[] files = Directory.GetFiles(rootPath);
            foreach (string file in files)
            {
                string exName = Tools.FilePathHelper.GetExName(file);
                if (exName == "manifest")
                {
                    continue;
                }
                this.files.Add(file);//将当前目录中的所有文件全名存入文件List
            }
            if (subPaths.Length == files.Length && files.Length == 0)//如果是空目录
            {
                this.paths.Add(rootPath);//记录空目录
            }
        }
    }
}
