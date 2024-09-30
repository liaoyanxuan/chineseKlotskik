using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEditor;
using UnityEngine;

public class UIDependFindWin : EditorWindow {

    private static UIDependFindWin _instance;

    public static UIDependFindWin Instance
    {
        get { 
            if(_instance == null)
            {
                _instance = (UIDependFindWin)EditorWindow.GetWindow(typeof(UIDependFindWin));
                _instance.titleContent = new GUIContent("检查UI图片依赖");
                _instance.maxSize = new Vector2(680, 750);
                _instance.minSize = new Vector2(680, 750);
            }
            return UIDependFindWin._instance; 
        }
    }
    [MenuItem("Tools/UI图片依赖检查")]
    static void ShowWin()
    {
        UIDependFindWin.Instance.Show();
    }

    void OnGUI()
    {
        ShowSelect();
        ShowResult();
    }
    private string defalutPrefabPath = "Assets/";
    private string prefabPath = "Assets/";
    private string texPath = "";
    private Object prefabFolder;
    private Object texFolder;

    private List<string> prefabDepends;

    private List<string> texNoDependPaths;

    private List<Texture2D> texNoDependTexs;

    private Vector2 texPos = new Vector2();

    private int checkTexSum = 0;
    private void ShowSelect()
    {
        ShowLine("选择文件夹");
        GUILayout.Label("请把需要检查的预设文件夹拖到框内，默认是检查Assets/");
        GUILayout.BeginHorizontal();
        prefabFolder = EditorGUILayout.ObjectField(prefabFolder, typeof(Object), false, GUILayout.Width(200));
        GUILayout.EndHorizontal();
        GUILayout.Label("请把需要检查的图片文件夹拖到框内");
        GUILayout.BeginHorizontal();
        texFolder = EditorGUILayout.ObjectField(texFolder, typeof(Object), false, GUILayout.Width(200));
        GUILayout.EndHorizontal();
        if(GUILayout.Button("检查",GUILayout.Width(100)))
        {
            CheckFun();
            return;
        }
        
    }

    private void CheckFun()
    {
        if(texFolder == null)
        {
            ShowTips("请指定需要检查的图片文件夹");
            return;
        }
        checkTexSum = 0;
        CheckPrefabPath();
        CheckTexPath();
        GetPrefabDepends();
        CheckTexDepends();
    }

    private void CheckPrefabPath()
    {
        if(prefabFolder!=null)
        {
            prefabPath = AssetDatabase.GetAssetPath(prefabFolder);
        }
        else
        {
            prefabPath = defalutPrefabPath;
        }
    }

    private void CheckTexPath()
    {
        if(texFolder == null)
        {
            return;
        }
        texPath = AssetDatabase.GetAssetPath(texFolder);
    }

    private void GetPrefabDepends()
    {
        string projectPath = FilePathHelper.GetProjectPath();
        string realPath = projectPath + prefabPath;
        prefabDepends = new List<string>();
        List<string> subPrefabPaths = FileManager.GetAllFiles(realPath, "prefab");
        if(subPrefabPaths==null||subPrefabPaths.Count==0)
        {
            return;
        }
        for(int i = 0;i<subPrefabPaths.Count;i++)
        {
            EditorUtility.DisplayProgressBar("正在检查预设依赖", subPrefabPaths[i], (float)i / (float)subPrefabPaths.Count);
            CheckOnePrefabDepend(subPrefabPaths[i].Replace(projectPath,""));
            EditorUtility.ClearProgressBar();
        }
    }

    private void CheckOnePrefabDepend(string path)
    {
        string[] depends = AssetDatabase.GetDependencies(path, true);
        if(depends == null || depends.Length == 0)
        {
            return;
        }
        for(int i = 0;i<depends.Length;i++)
        {            
            if(prefabDepends.IndexOf(depends[i])<0)
            {
                prefabDepends.Add(depends[i]);
            }
        }
    }

    private void CheckTexDepends()
    {
        texNoDependPaths = new List<string>();
        texNoDependTexs = new List<Texture2D>();
        if(prefabDepends==null||prefabDepends.Count==0)
        {
            Debug.Log("没有预设的依赖信息");
            return;
        }

        string projectPath = FilePathHelper.GetProjectPath();
        string realPath = projectPath + texPath;
        List<string> subTexPaths = FileManager.GetAllFilesIncludeList(realPath, new List<string> { "png", "jpg", "tga" });
        if(subTexPaths == null||subTexPaths.Count==0)
        {
            return;
        }
        checkTexSum = subTexPaths.Count;
        for(int i = 0;i<subTexPaths.Count;i++)
        {
            string assetPath = subTexPaths[i].Replace(projectPath, "");
            EditorUtility.DisplayProgressBar("正在检查图片依赖", assetPath, (float)i / (float)subTexPaths.Count);
            if(prefabDepends.IndexOf(assetPath)<0)
            {
                if(texNoDependPaths.IndexOf(assetPath)<0)
                {
                    texNoDependPaths.Add(assetPath);
                }
            }
            EditorUtility.ClearProgressBar();

        }
        if(texNoDependPaths.Count>0)
        {
            for(int i = 0;i<texNoDependPaths.Count;i++)
            {
                Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(texNoDependPaths[i], typeof(Texture2D));
                if(tex!=null)
                {
                    texNoDependTexs.Add(tex);
                }
            }
        }
    }

    private void ShowResult()
    {
        ShowLine("检查结果，以下为没有引用的图片");
        if(texNoDependTexs==null)
        {
            GUILayout.Label("请先查找");
            return;
        }
        if(texNoDependTexs.Count==0)
        {
            GUILayout.Label("没有找到结果");
            return;
        }
        GUILayout.Label("文件夹内共有："+checkTexSum+"张图片，其中没有被预设用到的有："+texNoDependTexs.Count+"张");//"找到结果：" + texNoDependTexs.Count);
        texPos = GUILayout.BeginScrollView(texPos, GUILayout.Width(600), GUILayout.Height(550));
        for (int i = 0; i < texNoDependTexs.Count;i++ )
        {
            EditorGUILayout.ObjectField(texNoDependTexs[i], typeof(Texture2D), false, GUILayout.Width(200));
        }

        GUILayout.EndScrollView();
    }

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
