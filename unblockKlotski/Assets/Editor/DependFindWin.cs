using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using Tools;
namespace DependFind
{
    public class DependFindWin : EditorWindow
    {
        static private DependFindWin _instance;

        public static DependFindWin Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (DependFindWin)EditorWindow.GetWindow(typeof(DependFindWin));
                    _instance.titleContent = new GUIContent("检查资源依赖");
                    _instance.maxSize = new Vector2(500, 650);
                    _instance.minSize = new Vector2(500, 650);
                }
                return DependFindWin._instance;
            }
        }

        private int titleSelectIndex = 0;
        private string[] titleStrs = new string[] { "资源依赖","查找资源","资源使用情况","查找大图"};

        private int typeIndex = 0;
        private string[] typeStrs = new string[] { "查找该资源有哪些依赖", "查找该资源被谁依赖了" };

        private int checkDependDeep = 0;
        private string[] dependDeepStrs = new string[] { "检查完整依赖", "只检查第一层依赖" };

        private Object selectObj;

        private Object checkFolder;

        private List<Object> dependObjs;

        private Vector2 resultPos = new Vector2();

        private List<string> assetPaths;

        private string guidStr = "";
        private Object findObj;

        [MenuItem("Tools/其他/资源依赖查找")]
        static void ShowWin()
        {
            DependFindWin.Instance.Show();
        }

        void OnGUI()
        {
            ShowSelectTitle();
            if(titleSelectIndex == 0)
            {
                ShowMain();
            }
            else if(titleSelectIndex == 1)
            {
                ShowFindRes();
            }
            else if(titleSelectIndex == 2)
            {
                ShowResUsedChcek();
            }
            else if(titleSelectIndex==3)
            {
                ShowFindLargePic();
            }
        }

        private void ShowSelectTitle()
        {
            titleSelectIndex = GUILayout.SelectionGrid(titleSelectIndex, titleStrs, 4, GUILayout.Width(450),GUILayout.Height(30));
            ShowLine();
        }

        private void ShowMain()
        {
            ShowCtrl();
            ShowResult();
        }

        private void ShowCtrl()
        {
            ShowLine("操作");
            GUILayout.Label("操作类型：");
            typeIndex = GUILayout.SelectionGrid(typeIndex, typeStrs, 2, GUILayout.Width(300),GUILayout.Height(30));
            if(typeIndex == 0)
            {
                checkDependDeep = GUILayout.SelectionGrid(checkDependDeep, dependDeepStrs,2, GUILayout.Width(250),GUILayout.Height(30));
            }
            
            //if (GUILayout.Button("刷新项目文件", GUILayout.Width(120), GUILayout.Height(40)))
            //{
            //    assetPaths = null;
            //}

            if (typeIndex == 1)
            {
                GUILayout.Label("请把需要查找的文件夹拖到下面的框中");
                GUILayout.Label("如果没有指定文件夹就是搜整个项目，比较慢");
                checkFolder = EditorGUILayout.ObjectField(checkFolder, typeof(Object), GUILayout.Width(200));
            }
            GUILayout.Label("请把需要查找的资源拖到下面的框中");
            selectObj = EditorGUILayout.ObjectField(selectObj, typeof(Object), GUILayout.Width(200));
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("检查", GUILayout.Width(120), GUILayout.Height(50)))
            {
                Check();
            }
            GUILayout.EndHorizontal();
        }

        private void ShowResult()
        {
            ShowLine("依赖结果");
            if(dependObjs == null ||dependObjs.Count == 0)
            {
                return;
            }
            resultPos = GUILayout.BeginScrollView(resultPos, GUILayout.Width(450), GUILayout.Height(350));
            for(int i = 0;i<dependObjs.Count;i++)
            {
                dependObjs[i] = EditorGUILayout.ObjectField(dependObjs[i], typeof(Object), GUILayout.Width(300));
            }
            GUILayout.EndScrollView();
            
        }

        private void ShowFindRes()
        {
            ShowLine("查找资源");
            GUILayout.BeginHorizontal();
            GUILayout.Label("请输入GUID：",GUILayout.Width(60));
            guidStr = EditorGUILayout.TextField(guidStr, GUILayout.Width(250));

            if (GUILayout.Button("查找", GUILayout.Width(80)))
            {
                FindRes();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("资源：", GUILayout.Width(60));
            findObj = EditorGUILayout.ObjectField(findObj,typeof(Object),GUILayout.Width(150));
            GUILayout.EndHorizontal();
        }


        private Object checkUsedFolder;
        private Object checkUsedOrgFolder;
        private List<Object> noUsedObjs;
        private Vector2 checkUsedPos = new Vector2();
        private void ShowResUsedChcek()
        {
            ShowLine("批量检查资源是否被使用");
            GUILayout.Label("请将查找范围拖到下框，如果不拖就是在整个项目内检查");
            checkUsedOrgFolder = EditorGUILayout.ObjectField(checkUsedOrgFolder, typeof(Object), GUILayout.Width(150));
            GUILayout.Label("请将需要检查的文件夹拖到下框");
            checkUsedFolder = EditorGUILayout.ObjectField(checkUsedFolder, typeof(Object), GUILayout.Width(150));
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("检查",GUILayout.Width(100),GUILayout.Height(50)))
            {
                CheckResUsedFun();
            }
            if (GUILayout.Button("加速检查", GUILayout.Width(100), GUILayout.Height(50))) {
                CheckResUsedFun2();
            }
            GUILayout.EndHorizontal();
            ShowResUsedResult();


            
        }

        private void ShowResUsedResult()
        {
            ShowLine("结果");
            int count = noUsedObjs != null ? noUsedObjs.Count : 0;
            if (count > 0) {
                GUILayout.Label("以下为没有在检查范围内被使用的资源，共：" + count + "个");
            } else {
                GUILayout.Label("以下为没有在检查范围内被使用的资源");
            }
            checkUsedPos = GUILayout.BeginScrollView(checkUsedPos, GUILayout.Width(450), GUILayout.Height(350));
            if(noUsedObjs!=null&&noUsedObjs.Count>0)
            {
                for(int i = 0;i<noUsedObjs.Count;i++)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(noUsedObjs[i], typeof(Object), GUILayout.Width(200));
                    if (GUILayout.Button("删除", GUILayout.Width(70), GUILayout.Height(20))) {
                        string path = AssetDatabase.GetAssetPath(noUsedObjs[i]);
                        AssetDatabase.MoveAssetToTrash(path);
                        noUsedObjs.RemoveAt(i);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();

        }

        private Object findLargePicObj;
        private float largeWidth = 500;
        private float largeHeight = 500;
        private List<Object> largePicList;
        private Vector2 largePicPos = new Vector2();

        private void ShowFindLargePic()
        {
            ShowLine("查找大尺寸图片");
            GUILayout.Label("请把需要查找的文件夹拖到下面");
            findLargePicObj = EditorGUILayout.ObjectField(findLargePicObj, typeof(Object), GUILayout.Width(150));
            GUILayout.Label("设置最小的宽高");
            GUILayout.BeginHorizontal();
            GUILayout.Label("宽：",GUILayout.Width(20));
            largeWidth = EditorGUILayout.FloatField(largeWidth, GUILayout.Width(50));

            GUILayout.Label("X",GUILayout.Width(20));

            GUILayout.Label("高：", GUILayout.Width(20));
            largeHeight = EditorGUILayout.FloatField(largeHeight, GUILayout.Width(50));

            GUILayout.EndHorizontal();

            if(GUILayout.Button("检查",GUILayout.Width(100),GUILayout.Height(50)))
            {
                CheckLargePicFun();
            }

            ShowFindLargePicResult();
        }

        private void ShowFindLargePicResult()
        {
            ShowLine("查找结果");
            if(largePicList!=null&&largePicList.Count>0)
            {
                largePicPos = GUILayout.BeginScrollView(largePicPos, GUILayout.Width(450), GUILayout.Height(350));
                for (int i = 0; i < largePicList.Count;i++ )
                {
                    EditorGUILayout.ObjectField(largePicList[i], typeof(Object),GUILayout.Width(150));
                }


                    GUILayout.EndScrollView();
            }
        }

        #region 实现

        private void Check()
        {
            if(selectObj == null)
            {
                ShowTips("请先选择需要检查的资源，把资源拖到框中！");
                return;
            }
            double t = sw.util.TimeHelper.GetSystemTime();
            dependObjs = new List<Object>();
            if(typeIndex == 0)
            {
                CheckDepend();
            }
            else
            {
                CheckBeDepended();
            }
            EditorUtility.ClearProgressBar();
            double uset = sw.util.TimeHelper.GetSystemTime() - t;
            Debug.Log("-----检查资源依赖，用时：" + (uset / 1000).ToString("0.0") + "秒");
            ShowTips("检查完成");
        }

        /// <summary>
        /// 检查依赖
        /// </summary>
        private void CheckDepend()
        {
            if (selectObj == null)
            {
                ShowTips("请先选择需要检查的资源，把资源拖到框中！");
                return;
            }
            string[] paths = null;
            bool isCheckAll = true;
            if(checkDependDeep != 0)
            {
                isCheckAll = false;
            }
            string objPath = AssetDatabase.GetAssetPath(selectObj);
            if(string.IsNullOrEmpty(objPath))
            {
                return;
            }
            paths = AssetDatabase.GetDependencies(objPath, isCheckAll);
            if(paths!=null && paths.Length>0)
            {
                for(int i = 0;i<paths.Length;i++)
                {
                    Object obj = AssetDatabase.LoadAssetAtPath(paths[i],typeof(Object));
                    if(obj!=selectObj)
                    {
                        dependObjs.Add(obj);
                    }
                    
                }
            }

        }


        /// <summary>
        /// 检查被依赖
        /// </summary>
        private void CheckBeDepended()
        {
            if (selectObj == null)
            {
                ShowTips("请先选择需要检查的资源，把资源拖到框中！");
                return;
            }
            //string selectPath = AssetDatabase.GetAssetPath(selectObj);
            //if(string.IsNullOrEmpty(selectPath))
            //{
            //    return;
            //}

            Object[] objs = GetProjectUsedObjs();
            float count = (float)objs.Length;
            for(int i = 0;i<objs.Length;i++)
            {
                EditorUtility.DisplayProgressBar("正在收集依赖信息", i + "/" + count, (float)i / count);
                if(objs[i]!=selectObj)
                {
                    bool isBeDepend = CheckIsDepend(objs[i], selectObj);
                    if(isBeDepend==true)
                    {
                        dependObjs.Add(objs[i]);
                    }
                }
            }

            //string guid = AssetDatabase.AssetPathToGUID(selectPath);
            //string rootPath = Application.dataPath;
            //if (assetPaths == null)
            //{
            //    List<string> expectList = new List<string> { "meta", "txt", "jpg", "png", "map3d", "asset", "exr" };

            //    assetPaths = FileManager.Instance.GetAllFilesExceptList(rootPath, expectList);
            //}

            //if (assetPaths == null || assetPaths.Count == 0)
            //{
            //    return;
            //}

            //float count = (float)assetPaths.Count;
            //for (int i = 0; i < assetPaths.Count; i++)
            //{
            //    EditorUtility.DisplayProgressBar("正在收集依赖信息", i + "/" + count, (float)i / count);
            //    string subAssetPath = assetPaths[i].Replace(rootPath, "");
            //    subAssetPath = "Assets" + subAssetPath;
            //    if (subAssetPath == selectPath)
            //    {
            //        continue;
            //    }
            //    Object obj = AssetDatabase.LoadAssetAtPath(subAssetPath, typeof(Object));
            //    bool isBeDepend = CheckIsDepend(obj, selectObj);
            //    if (isBeDepend)
            //    {
            //        //Object obj = AssetDatabase.LoadAssetAtPath(subAssetPath, typeof(Object));
            //        if (obj != null)
            //        {
            //            dependObjs.Add(obj);
            //        }

            //    }
            //}
            
        }

        private Object[] GetProjectUsedObjs()
        {
            string path = Application.dataPath + "/";// "/Resources/";
            if(checkFolder!=null)
            {
                string folderPath = AssetDatabase.GetAssetPath(checkFolder);
                int lastIndex = path.LastIndexOf("Assets");
                if(lastIndex>=0)
                {
                    path = path.Substring(0, lastIndex);
                }
                path = path + folderPath;                
            }
            Debug.Log("检查的文件夹:" + path);
            List<string> dontNeedFindDepend = new List<string> { "asset", "meta", "txt", "png", "jpg", "tga", "psd", "dds", "fbx", "mp3", "wav", "wma", "m4a", "cs" };
            List<string> finalList = new List<string>();

            List<string> subPaths = FileManager.GetAllFilesExceptList(path, dontNeedFindDepend);
            if (subPaths == null || subPaths.Count == 0)
            {
                return null;
            }


            for (int i = 0; i < subPaths.Count; i++)
            {
                EditorUtility.DisplayProgressBar("正在收集项目依赖", i + "/" + subPaths.Count, (float)i / subPaths.Count);
                string tempStr = subPaths[i].Replace(Application.dataPath, "");
                tempStr = "Assets" + tempStr;
                if (finalList.IndexOf(tempStr) < 0)
                {
                    finalList.Add(tempStr);
                }
                string[] subDepends = AssetDatabase.GetDependencies(tempStr);
                if (subDepends != null && subDepends.Length > 0)
                {
                    for (int j = 0; j < subDepends.Length; j++)
                    {
                        if (finalList.IndexOf(subDepends[j]) < 0)
                        {
                            finalList.Add(subDepends[j]);
                        }
                    }
                }
            }

            if (finalList.Count == 0)
            {
                return null;
            }
            List<Object> objList = new List<Object>();
            for (int i = 0; i < finalList.Count; i++)
            {
                EditorUtility.DisplayProgressBar("正在收集检查物体", i + "/" + finalList.Count, (float)i / finalList.Count);
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(finalList[i]);
                if (obj != null && objList.IndexOf(obj) < 0)
                {
                    objList.Add(obj);
                }
            }
            if (objList.Count == 0)
            {
                return null;
            }
            Object[] objs = new Object[objList.Count];
            for (int i = 0; i < objList.Count; i++)
            {
                objs[i] = objList[i];
            }
            EditorUtility.ClearProgressBar();
            return objs;
        }

        private bool CheckIsDepend(string path,string guid)
        {
            string[] paths = AssetDatabase.GetDependencies(path,true);
            if(paths==null||paths.Length==0)
            {
                return false;
            }
            for(int i = 0;i<paths.Length;i++)
            {
                string subGuid = AssetDatabase.AssetPathToGUID(paths[i]);
                if(subGuid == guid)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckIsDepend(Object obj1, Object obj2)
        {
            Object[] objs = EditorUtility.CollectDependencies(new Object[]{obj1});
            if (objs == null || objs.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < objs.Length; i++)
            {
                if(objs[i]==obj2)
                {
                    return true;
                }
            }
            return false;
        }

        private void FindRes()
        {
            if(string.IsNullOrEmpty(guidStr))
            {
                ShowTips("请输入guid");
                return;
            }
            string path = AssetDatabase.GUIDToAssetPath(guidStr);
            if(string.IsNullOrEmpty(path))
            {
                ShowTips("找不到对应的资源");
                return;
            }
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            if(obj == null)
            {
                ShowTips("找到对应的路径，查找资源出错："+path);
                return;
            }
            findObj = obj;
            ShowTips("找到资源：" + path);
        }

        private void CheckResUsedFun()
        {
            noUsedObjs = new List<Object>();
            if(checkUsedFolder==null)
            {
                ShowTips("请先选择需要检查的文件夹");
                return;
            }
            string checkUsedPath = GetFileRealPath(checkUsedFolder);
            if(string.IsNullOrEmpty(checkUsedPath))
            {
                ShowTips("需要检查的文件夹不存在，请重新选择");
                return;
            }
            List<string> checkFiles = FileManager.GetAllFilesExcept(checkUsedPath,"meta");
            if(checkFiles==null||checkFiles.Count == 0)
            {
                ShowTips("需要检查的文件夹里面没有需要检查的文件，请重新选择");
                return;
            }
            string origPath = GetCheckResOrigPath();
            List<string> origFilePaths = FileManager.GetAllFilesExcept(origPath, "meta");
            if(origFilePaths == null||origFilePaths.Count==0)
            {
                ShowTips("检查范围的文件夹内没有可以检查的文件，请重新选择");
                return;
            }
            List<string> dependFiles = new List<string>();
            for(int i = 0;i<origFilePaths.Count;i++)
            {
                if(checkFiles.IndexOf(origFilePaths[i])>=0)
                {
                    continue;
                }
                string realOrigPath = GetAssetPathByRealPath(origFilePaths[i]);
                string[] dPaths = AssetDatabase.GetDependencies(realOrigPath, true);
                if(dPaths.Length>0)
                {
                    for(int j = 0;j<dPaths.Length;j++)
                    {
                        if(dependFiles.IndexOf(dPaths[j])<0)
                        {
                            dependFiles.Add(dPaths[j]);
                        }
                    }
                }
            }
            List<string> preCheckPaths = new List<string>();
            List<string> noUsedPaths = new List<string>();

            for(int i = 0;i<checkFiles.Count;i++)
            {
                preCheckPaths.Add(GetAssetPathByRealPath(checkFiles[i]));
            }

            for (int i = 0; i < preCheckPaths.Count; i++)
            {
                if (dependFiles.IndexOf(preCheckPaths[i]) < 0)
                {
                    noUsedPaths.Add(preCheckPaths[i]);
                }
            }
            if(noUsedPaths.Count>0)
            {
                for(int i = 0;i<noUsedPaths.Count;i++)
                {
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(noUsedPaths[i]);
                    noUsedObjs.Add(obj);
                }
            }
        }

        private void CheckResUsedFun2() {
            noUsedObjs = new List<Object>();
            if (checkUsedFolder == null) {
                ShowTips("请先选择需要检查的文件夹");
                return;
            }
            if (checkUsedOrgFolder == null) {
                ShowTips("请先选择需要检查的范围文件夹");
                return;
            }
            string checkUsedPath = GetFileRealPath(checkUsedFolder);
            if (string.IsNullOrEmpty(checkUsedPath)) {
                ShowTips("需要检查的文件夹不存在，请重新选择");
                return;
            }
            EditorUtility.DisplayProgressBar("检查图片文件中：", "请稍后", 0.5f);
            List<string> checkFiles = FileManager.GetAllFilesExcept(checkUsedPath, "meta");
            if (checkFiles == null || checkFiles.Count == 0) {
                ShowTips("需要检查的文件夹里面没有需要检查的文件，请重新选择");
                return;
            }
            EditorUtility.DisplayProgressBar("检查范围文件中：", "请稍后", 1.0f);
            string origPath = GetCheckResOrigPath();
            List<string> origFilePaths = FileManager.GetAllFiles(origPath, "prefab");
            if (origFilePaths == null || origFilePaths.Count == 0) {
                ShowTips("检查范围的文件夹内没有可以检查的文件，请重新选择");
                return;
            }
            Dictionary<string, string> dependFilesDic = new Dictionary<string, string>();
            for (int i = 0; i < origFilePaths.Count; i++) {
                EditorUtility.DisplayProgressBar("正在检查依赖：", i + "/" + origFilePaths.Count, (float)i / origFilePaths.Count);
                string realOrigPath = GetAssetPathByRealPath(origFilePaths[i]);
                string[] dPaths = AssetDatabase.GetDependencies(realOrigPath, true);
                for (int j = 0; j < dPaths.Length; j++) {
                    string temp = null;
                    if (dependFilesDic.TryGetValue(dPaths[j], out temp) == false) {
                        dependFilesDic.Add(dPaths[j], dPaths[j]);
                    }
                }
            }
            EditorUtility.DisplayProgressBar("输出结果中：", "请稍后", 1.0f);
            for (int i = 0; i < checkFiles.Count; i++) {
                string path = GetAssetPathByRealPath(checkFiles[i]);
                string temp = null;
                if (dependFilesDic.TryGetValue(path, out temp) == false) {
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                    noUsedObjs.Add(obj);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        private string GetCheckResOrigPath()
        {
            if(checkUsedOrgFolder!=null)
            {
                string path = GetFileRealPath(checkUsedOrgFolder);
                return path;
            }
            else
            {
                return Application.dataPath;
            }
        }

        private string GetFileRealPath(Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            path = GetRealPath(path);
            return path;
        }

        private string GetAssetPathByRealPath(string path)
        {
            path = path.Replace(Application.dataPath, "");
            path = "Assets" + path;
            return path;
        }

        private string GetRealPath(string path)
        {
            string appPath = Application.dataPath;
            int lastIndex = appPath.LastIndexOf("/");
            if (lastIndex >= 0)
            {
                appPath = appPath.Substring(0, lastIndex);
            }
            path = appPath+"/" + path;
            return path;
        }

        private void CheckLargePicFun()
        {
            largePicList = new List<Object>();
            if(findLargePicObj == null)
            {
                ShowTips("请先指定文件夹");
                return;
            }
            string path = GetFileRealPath(findLargePicObj);
            if(string.IsNullOrEmpty(path))
            {
                ShowTips("选择的文件夹无效，请重新选择");
                return;
            }

            List<string> subPath = FileManager.GetAllFilesExcept(path, "meta");
            if(subPath==null||subPath.Count == 0)
            {
                ShowTips("选择的文件夹内没有可以检查的文件");
                return;
            }

            for(int i = 0;i<subPath.Count;i++)
            {
                EditorUtility.DisplayProgressBar("正在检查图片", i + "/" + subPath.Count, (float)i / subPath.Count);
                string assetPath = GetAssetPathByRealPath(subPath[i]);
                Texture2D obj = (Texture2D)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D));
                if(obj == null)
                {
                    continue;
                }
                if(obj.width>=largeWidth||obj.height>=largeHeight)
                {
                    largePicList.Add(obj);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        #endregion

        #region 其他显示

        /// <summary>
        /// 显示提示
        /// </summary>
        /// <param name="content"></param>
        private void ShowTips(string content)
        {
            EditorUtility.DisplayDialog("提示", content, "确定");
        }

        /// <summary>
        /// 显示横条分割
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        private void ShowLine(string t = "", float w = -1, float h = -1)
        {
            string content = "";
            float ww;
            float hh;
            if (!string.IsNullOrEmpty(t))
            {
                content = t;
            }
            if (string.IsNullOrEmpty(content))
            {
                if (w < 0)
                {
                    ww = this.maxSize.x;
                }
                else
                {
                    ww = w;
                }

                if (h < 0)
                {
                    hh = 5;
                }
                else
                {
                    hh = h;
                }
            }
            else
            {
                if (w < 0)
                {
                    ww = this.maxSize.x;
                }
                else
                {
                    ww = w;
                }

                if (h < 0)
                {
                    hh = 20;
                }
                else
                {
                    hh = h;
                }
            }
            GUILayout.Box(content, GUILayout.Width(ww), GUILayout.Height(hh));
        }

        #endregion

    }
}

