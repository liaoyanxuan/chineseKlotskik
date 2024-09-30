using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using sw.util;
using Tools;

public class CreateAssetBundle : Editor
{
    //[MenuItem("Build/Update UI Asset")]
    public static void UpdateUIAsset()
    {
        string path = Application.dataPath + "/UI/UIPrefabs/builds/";
        string dirpre = Application.dataPath;
        int dirprelen = dirpre.Length - 6;
        dirpre = dirpre.Substring(0, dirprelen);
        sw.util.LoggerHelper.Debug("search path:" + dirpre + path);

        //UIPrefabs
        UpdateUIAsset_UIPrefabs();

        //Font
        UpdateUIAsset_Font();

        //appicon

        //fx
        UpdateUIAsset_fx();

        //sound
        UpdateUIAsset_sound();

        //Textures
        UpdateUIAsset_Textures();
        //UpdateUIAsset_EquipTextures();

        //Materials
        UpdateUIAsset_Materials();

        //Atlas
        UpdateUIAsset_Atlas();

        //asset
        UpdateUIAsset_asset();

        sw.util.LoggerHelper.Debug("Update UI Asset Success!");
    }
    private static void SetFolderAssetName(string folderName,string parentName)
    {
        string path = Application.dataPath +"/"+ folderName;
        List<string> subFolders = Tools.FileManager.GetSubFolders(path);
        for (int i = 0; i < subFolders.Count; i++)
        {
            SetOneFolderAssetName(subFolders[i],parentName);
        }
    }

    private static void SetOneFolderAssetName(string path,string parentName)
    {
        if (string.IsNullOrEmpty(path) == null)
        {
            return;
        }
        if (path[path.Length - 1] == '/')
        {
            path = path.Substring(0, path.Length - 1);
        }
        string folderName = Tools.FilePathHelper.GetFileName(path);
        List<string> subFiles = Tools.FileManager.GetAllFilesExcept(path, "meta");
        if (subFiles == null || subFiles.Count == 0)
        {
            return;
        }
        for (int i = 0; i < subFiles.Count; i++)
        {
            string subFileName = Tools.FilePathHelper.GetFileName(subFiles[i]);
            subFileName = Tools.FilePathHelper.RemoveExName(subFileName);
            string assetName = parentName+"/" + folderName + "/" + subFileName + ".unity3d";
            string subFilePath = "Assets" + subFiles[i].Replace(Application.dataPath, "");
            SetOneFileAssetName(subFilePath, assetName);

        }
    }

    private static void SetOneFileAssetName(string path, string assetName)
    {
        AssetImporter import = AssetImporter.GetAtPath(path);
        if (import == null)
        {
            return;
        }
        assetName = assetName.ToLower();
        if (import != null)
        {
            if (import is TextureImporter)
            {
                TextureImporter ti = (TextureImporter)import;
                if (ti.textureType != TextureImporterType.Sprite)
                {
                    ti.textureType = TextureImporterType.Sprite;
                    ti.SaveAndReimport();
                }
            }
        }
        assetName = assetName.ToLower();
        if (import.assetBundleName != assetName)
        {
            import.assetBundleName = assetName;
        }
    }
   // [MenuItem("Build/Update UI Asset")]
    public static void UpdateUGUIAsset()
    {
        string path = Application.dataPath + "/UIRes/UIPrefabs/builds/";
        string dirpre = Application.dataPath;
        int dirprelen = dirpre.Length - 6;
        dirpre = dirpre.Substring(0, dirprelen);
        Debug.Log("search path:" + dirpre + path);

        //texture
        SetFolderAssetName("UINew/texture","texture");

        //atlas
        path = "Assets/UINew/atlas/";
        string atlas_path = Application.dataPath + "/UINew/atlas/";
        List<string> AllAtlasPath = GetSubFolders(atlas_path);

        foreach (string filePath in AllAtlasPath)
        {
            string atlasName = filePath.Replace(atlas_path, "atlas/");
            string file_path = path + filePath.Replace(atlas_path, "");
            UpdateAssetBundleName(file_path, "*.jpg", atlasName + ".unity3d");
            UpdateAssetBundleName(file_path, "*.png", atlasName + ".unity3d");
        }

        //artFont
        path = "Assets/UINew/artFont/";
        string artFont_path = Application.dataPath + "/UINew/artFont/";
        List<string> AllArtFontPath = GetSubFolders(artFont_path);
        foreach (string filePath in AllArtFontPath)
        {
            string artFontName = filePath.Replace(artFont_path, "artFont/");
            string file_path = path + filePath.Replace(artFont_path, "");
            UpdateAssetBundleName(file_path, "", artFontName + ".unity3d",false,true,true);
        }

        //Font
        path = "Assets/UINew/font/";
        UpdateAssetBundleName(path, "*.ttf", "font/ttf/{0}.unity3d",false,true,true);
        UpdateAssetBundleName(path, "*.TTF", "font/ttf/{0}.unity3d",false,true,true);

        //-------------------------UINew----------------------------------- 
        // prefab
        path = "Assets/UINew/prefab/";
        string prefab_path = Application.dataPath + "/UINew/prefab/";
        List<string> AllPrefabPath = GetSubFolders(prefab_path);
        foreach (string filePath in AllPrefabPath)
        {
            string prefabName = filePath.Replace(prefab_path, "prefab/");
            string file_path = path + filePath.Replace(prefab_path, "");
            UpdateAssetBundleName(file_path, "*.prefab", prefabName + "/{0}.unity3d");
        }
        string rootPath = Application.dataPath.Replace("Assets", "");
        rootPath = rootPath +path;
        List<string> rootFolderPrefabs = Tools.FileManager.GetSubFiles(rootPath, "prefab");
        if(rootFolderPrefabs!=null&&rootFolderPrefabs.Count>0)
        {
            for(int i = 0;i<rootFolderPrefabs.Count;i++)
            {
                string prefabName = rootFolderPrefabs[i].Replace(prefab_path, "prefab/");
                prefabName = Tools.FilePathHelper.RemoveExName(prefabName);
                string file_path = path + rootFolderPrefabs[i].Replace(prefab_path, "");
                string abName = prefabName+".unity3d";
                abName = abName.ToLower();
                AssetImporter ai = AssetImporter.GetAtPath(file_path);
                if(ai!=null&&ai.assetBundleName!=abName)
                {
                    ai.assetBundleName = abName;
                }
            }
        }
        //path = "Assets/UINew/sound/";
        //prefab_path = Application.dataPath + "/UINew/sound/";
        //AllPrefabPath = GetSubFolders(prefab_path);
        foreach (string filePath in AllPrefabPath)
        {
            string prefabName = filePath.Replace(prefab_path, "sound/");
            string file_path = path + filePath.Replace(prefab_path, "");
            UpdateAssetBundleName(file_path, "*.mp3", prefabName + "/{0}.unity3d");
            UpdateAssetBundleName(file_path, "*.wav", prefabName + "/{0}.unity3d");
        }

        //mat
        path = "Assets/UINew/mat";
        UpdateAssetBundleName(path, "*.mat", "mat/{0}.unity3d");

        //shader
        path = "Assets/UINew/mat";
        UpdateAssetBundleName(path, "*.shader", "shader/shaders.unity3d");

        path = "Assets/UINew/config";
        UpdateAssetBundleName(path, "*.txt", "config/{0}.unity3d");

        Debug.Log("Update NewUI Asset Success!");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    //[MenuItem("Build/UpdateAtlasAb")]
    public static void UpdateAtlasAb()
    {
        string path = "Assets/atlas/";
        string atlas_path = Application.dataPath + "/atlas/";
        List<string> AllAtlasPath = GetSubFolders(atlas_path);

        foreach (string filePath in AllAtlasPath)
        {
            string atlasName = filePath.Replace(atlas_path, "atlas/");
            string file_path = path + filePath.Replace(atlas_path, "");
            UpdateAssetBundleName(file_path, "*.jpg", atlasName + ".unity3d");
            UpdateAssetBundleName(file_path, "*.png", atlasName + ".unity3d");
        }
    }

    const string kAssetBundlesOutputPath = "AssetBundles";
    [MenuItem("Build/Build atlas ab")]
    static public void UpdateAndBuildAtlasAssetBundle()
    {
        UpdateAtlasAb();
        BuildAssetBundles();
        string data_path = Application.dataPath;
        string data_path_parent = Application.dataPath.Replace("/Assets","");

        string targetflods = "Windows";
#if UNITY_IOS
		targetflods="iOS";
#elif UNITY_ANDROID
        targetflods = "Android";
#endif
        string srcPath = data_path_parent + "/AssetBundles/" + targetflods;
        string destPath = data_path + "/StreamingAssets/AssetBundles/" + targetflods;

        if (Directory.Exists(destPath))
        {
            Directory.Delete(destPath,true);
        }
        
        FileManager.CopyFolder(srcPath, destPath);
    }

    #region 生成ui文件过滤
    static public void CreateOrUpdateUIFilter()
    {
        string[] abs = AssetDatabase.GetAllAssetBundleNames();
        RefreshUIFilterDict();
        for (int i = 0; i < abs.Length; i++)
        {
            string key = "ui/" + abs[i];
            if (uiFilterDict.ContainsKey(key) == false)
            {
                uiFilterDict.Add(key, 3);
            }
        }
        SaveUIFilter();
    }

    static private Dictionary<string, int> uiFilterDict;

    static private void RefreshUIFilterDict()
    {
        uiFilterDict = new Dictionary<string, int>();
        string path = GetUIFilterPath();
        if (FileManager.IsFileExists(path) == false)
        {
            return;
        }
        string content = FileManager.ReadFileText(path);
        if (string.IsNullOrEmpty(content) == false)
        {
            uiFilterDict = ParserUIFilterDict(content);
        }

    }

    static private Dictionary<string, int> ParserUIFilterDict(string str)
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();
        string[] lines = str.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string[] content = lines[i].Trim().Split(',');
            if (content.Length >= 2)
            {
                string key = content[0].Trim();
                string valStr = content[1].Trim();
                int val = 3;
                try
                {
                    val = int.Parse(valStr);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }
                if (dict.ContainsKey(key) == true)
                {
                    dict[key] = val;
                }
                else
                {
                    dict.Add(key, val);
                }
            }
        }
        return dict;
    }

    static private void SaveUIFilter()
    {
        if (uiFilterDict == null || uiFilterDict.Count == 0)
        {
            return;
        }
        string path = GetUIFilterPath();
        string content = GetUIFilterStr();
        FileManager.SaveFile(path, content);
        string outputPath = Path.Combine(kAssetBundlesOutputPath, GetPlatformFolderForAssetBundles(EditorUserBuildSettings.activeBuildTarget));
        outputPath += "/uitype.txt";
        string txtContent = content.Replace(",", "\t");
        FileManager.SaveFile(outputPath,txtContent);
    }

    static private string GetUIFilterStr()
    {
        if (uiFilterDict == null || uiFilterDict.Count == 0)
        {
            return "";
        }
        string str = "";
        foreach (KeyValuePair<string, int> item in uiFilterDict)
        {
            str += (item.Key + "," + item.Value);
            str += "\n";
        }
        return str;
    }

    static private string GetUIFilterPath()
    {
        return Application.dataPath + "/UIFilter.csv";
    }
    #endregion

    //[MenuItem("Build/Update/Update UIPrefabs")]
    public static void UpdateUIAsset_UIPrefabs(bool bo = false)
    {
        string path = "";
        //UIPrefabs
        path = "Assets/UI/UIPrefabs/builds/";
        UpdateAssetBundleName(path, "*.prefab", "prefab/{0}.unity3d", bo);

        path = "Assets/UI/UIPrefabs/common/";
        UpdateAssetBundleName(path, "*.prefab", "prefab/{0}.unity3d", bo);

        path = "Assets/UI/UIPrefabs/effect/";
        UpdateAssetBundleName(path, "*.prefab", "prefab/{0}.unity3d", bo);

        sw.util.LoggerHelper.Debug("Update UIPrefabs Success!");
    }
    //[MenuItem("Build/Update/Update Font")]
    public static void UpdateUIAsset_Font(bool bo = false)
    {
        string path = "";
        //Font
        path = "Assets/NGUI/res/Font";
        UpdateAssetBundleName(path, "*.ttf", "font/ttf/{0}.unity3d", bo);
        path = "Assets/UI/Font";
        UpdateAssetBundleName(path, "*.mat", "font/mat/{0}.unity3d", bo);

        sw.util.LoggerHelper.Debug("Update Font Success!");
    }

    //[MenuItem("Build/Update/Update fx")]
    public static void UpdateUIAsset_fx(bool bo = false)
    {
        string path = "";
        //fx
        path = "Assets/UI/fx";
        UpdateAssetBundleName(path, "*.prefab", "fx/{0}.unity3d", bo);

        sw.util.LoggerHelper.Debug("Update fx Success!");
    }
    [MenuItem("Build/Update/Clear shader")]
    public static void UpdateUIAsset_shader()
    {
        string path = "";
        //fx
        path = "Assets/Resources/Shaders";
        UpdateAssetBundleName(path, "*.shader", "shader/{0}.shader", true);

        sw.util.LoggerHelper.Debug("Update fx Success!");
    }
    //[MenuItem("Build/Update/Update sound")]
    public static void UpdateUIAsset_sound(bool bo = false)
    {
        string path = "";
        //sound
        path = "Assets/UI/sound";
        UpdateAssetBundleName(path, "*.mp3", "sound/{0}.unity3d", bo);

        sw.util.LoggerHelper.Debug("Update sound Success!");
    }
    //[MenuItem("Build/Update/Update Texture")]
    public static void UpdateUIAsset_Textures(bool bo = false)
    {
        string path = "";
        //Texture
        path = "Assets/UI/Texture";
        UpdateAssetBundleName(path, "*.png", "texture/{0}.unity3d", bo);
        UpdateAssetBundleName(path, "*.jpg", "texture/{0}.unity3d", bo);

        sw.util.LoggerHelper.Debug("Update Textures Success!");
    }
    //[MenuItem("Build/Update/Update Equip Texture")]
    //public static void UpdateUIAsset_EquipTextures()
    //{
    //    string path = "";
    //    //Texture
    //    path = "Assets/UI/Texture/zhuangbei";
    //    UpdateAssetBundleName(path, "*.png", "texture/equip/{0}.unity3d");
    //    UpdateAssetBundleName(path, "*.jpg", "texture/equip/{0}.unity3d");

    //    sw.util.LoggerHelper.Debug("Update Textures Success!");
    //}
    //[MenuItem("Build/Update/Update Materials")]
    public static void UpdateUIAsset_Materials(bool bo = false)
    {
        string path = "";
        //Materials
        path = "Assets/UI/Materials";
        UpdateAssetBundleName(path, "*.mat", "material/{0}.unity3d", bo);

        sw.util.LoggerHelper.Debug("Update Materials Success!");
    }
    //[MenuItem("Build/Update/Update Atlas (UIAssets)")]
    public static void UpdateUIAsset_Atlas(bool bo = false)
    {
        string path = "";
        //UIAssets
        path = "Assets/UI/UIAssets";
        UpdateAssetBundleName(path, "*.mat", "texture/{0}.unity3d", bo);
        //UpdateAssetBundleName(path, "*.jpg", "texture/{0}.unity3d",bo);
        //UpdateAssetBundleName(path, "*.png", "texture/{0}.unity3d",bo);
        UpdateAssetBundleName(path, "*.prefab", "texture/{0}.unity3d", bo);

        sw.util.LoggerHelper.Debug("Update Atlas Success!");
    }
    //[MenuItem("Build/Update/Update asset")]
    public static void UpdateUIAsset_asset(bool bo = false)
    {
        string path = "";
        //asset
        path = "Assets/UI";
        UpdateAssetBundleName(path, "*.asset", "asset/{0}.unity3d", bo);

        sw.util.LoggerHelper.Debug("Update asset Success!");
    }

    //[MenuItem("Build/Clear UI Asset")]
    public static void ClearUIAsset()
    {
        //UIPrefabs
        UpdateUIAsset_UIPrefabs(true);

        //Font
        UpdateUIAsset_Font(true);

        //appicon

        //fx
        UpdateUIAsset_fx(true);

        //sound
        UpdateUIAsset_sound(true);

        //Textures
        UpdateUIAsset_Textures(true);
        //UpdateUIAsset_EquipTextures();

        //Materials
        UpdateUIAsset_Materials(true);

        //Atlas
        UpdateUIAsset_Atlas(true);

        //asset
        UpdateUIAsset_asset(true);
    }

    static public void ClearAllAssetBundleNameFun()
    {
        string[] allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        if (allAssetBundleNames == null || allAssetBundleNames.Length == 0)
        {
            Debug.Log("没有assetBundleName可以清除");
        }
        EditorUtility.ClearProgressBar();

        for (int i = 0; i < allAssetBundleNames.Length; i++)
        {
            EditorUtility.DisplayProgressBar("正在清除AssetBundleName:" + i + "/" + allAssetBundleNames.Length, allAssetBundleNames[i], (float)i / (float)allAssetBundleNames.Length);
            ClearOneAssetBundleName(allAssetBundleNames[i]);
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 清除一个路径的资源的assetBundleName
    /// </summary>
    /// <param name="assetBundleName"></param>
    static private void ClearOneAssetBundleName(string assetBundleName)
    {
        string[] paths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
        if (paths == null || paths.Length == 0)
        {
            return;
        }
        for (int i = 0; i < paths.Length; i++)
        {
            AssetImporter ai = AssetImporter.GetAtPath(paths[i]);
            ai.assetBundleName = "";
        }
    }


    [MenuItem("Build/设置AssetBundelName/Prefabs")]
    public static void SetAssetBundleName_Prefab()
    {
        Object selectObj = Selection.activeObject;
        if (selectObj == null)
        {
            return;
        }
        string path = AssetDatabase.GetAssetPath(selectObj);
       
        string directroyName = Path.GetDirectoryName(path);
        directroyName = directroyName.Replace("\\","/");

        string templeName = directroyName.Replace("Assets/", "");
        string templeAbName =templeName + "/{0}";


        int res = SetAssetBundleName(path, "Assets/prefabs/", templeAbName);
        if (res == 0)
        {
            sw.util.LoggerHelper.Debug("设置UI预设的AssetBundelName成功！ " + path);
        }
        else
        {
            sw.util.LoggerHelper.Debug(string.Format("设置UI预设的AssetBundelName失败，错误码[{0}]！ {1}", res, path));
        }
    }

    //[MenuItem("Assets/设置AssetBundelName/UIPrefabs")]
    public static void SetAssetBundleName_UIPrefab()
    {
        Object selectObj = Selection.activeObject;
        if (selectObj == null)
        {
            return;
        }
        string path = AssetDatabase.GetAssetPath(selectObj);
        int res = SetAssetBundleName(path, "Assets/UI/UIPrefabs/", "prefab/{0}.unity3d");
        if (res == 0)
        {
            sw.util.LoggerHelper.Debug("设置UI预设的AssetBundelName成功！ " + path);
        }
        else
        {
            sw.util.LoggerHelper.Debug(string.Format("设置UI预设的AssetBundelName失败，错误码[{0}]！ {1}", res, path));
        }
    }


    //[MenuItem("Assets/设置AssetBundelName/Texture")]
    public static void SetAssetBundleName_Texture()
    {
        Object selectObj = Selection.activeObject;
        if (selectObj == null)
        {
            return;
        }
        string path = AssetDatabase.GetAssetPath(selectObj);
        int res = SetAssetBundleName(path, "Assets/UI/Texture/", "texture/{0}.unity3d");
        if (res == 0)
        {
            sw.util.LoggerHelper.Debug("设置Texture预设的AssetBundelName成功！ " + path);
        }
        else
        {
            sw.util.LoggerHelper.Debug(string.Format("设置Texture预设的AssetBundelName失败，错误码[{0}]！ {1}", res, path));
        }
    }
    static int SetAssetBundleName(string path, string filePathLimit, string abFormat)
    {
        path = path.Replace('\\', '/');
        if (false == path.StartsWith(filePathLimit))
        {
            sw.util.LoggerHelper.Debug("这个文件不是UI预设文件！");
            return -1;
        }
        string aname = Path.GetFileNameWithoutExtension(path).ToLower();
        AssetImporter importer = AssetImporter.GetAtPath(path);
        if (importer == null)
        {
            return -2;
        }
        string assetbundleName = string.Format(abFormat, aname);
        if (assetbundleName != importer.assetBundleName)
        {
            importer.assetBundleName = assetbundleName;
        }

        return 0;
    }

    public static void UpdateAssetBundleName(string path, string exformat, string bundlenameFormat, bool clearABName = false, bool inonepath = true, bool clearPackingTag = false)
    {
        if (path.StartsWith("Assets/"))
        {
            path = path.Remove(0, "Assets".Length);
        }
        string dirpath = Application.dataPath + path;
        if (!string.IsNullOrEmpty(exformat))
        {
            string ex = exformat.Remove(0, 1);

            foreach (string fn in Directory.GetFiles(dirpath, exformat, SearchOption.AllDirectories))
            {
                string assetname = fn.Substring(dirpath.Length);
                string filename = assetname;
                if (inonepath)
                {
                    filename = Path.GetFileNameWithoutExtension(filename);
                }
                else
                {
                    filename = filename.Replace(ex, "");
                }
                string bundlename = string.Format(bundlenameFormat, filename);
                assetname = "Assets" + path + assetname;
                if (filename.Equals("DropIcon") && exformat.IndexOf("prefab") != -1)
                {
                    int indexSplit = bundlename.IndexOf("/");
                    bundlename = "atlas" + bundlename.Substring(indexSplit);
                }
                //LoggerHelper.Debug(bundlename);
                //s += "\n" + bundlename;
                //continue;
                //if (!clearABName&&assetname.IndexOf("_img") > 0)
                //    continue;
                SetAssetName(assetname, bundlename, clearABName,clearPackingTag);
            }
        }
        else
        {

            List<string> subObjPaths = Tools.FileManager.GetAllFilesExcept(dirpath, "meta");
            for (int i = 0; i < subObjPaths.Count; i++)
            {
                string fn = subObjPaths[i];
                string assetname = fn.Substring(dirpath.Length);
                string filename = assetname;
                filename = Path.GetFileNameWithoutExtension(filename);
                string bundlename = string.Format(bundlenameFormat, filename);
                assetname = "Assets" + path + assetname;
                SetAssetName(assetname, bundlename, clearABName, clearPackingTag);
            }
        }
        //LoggerHelper.Debug(s);
    }


    public static void SetAssetName(string assetname, string bundlename, bool clearABName,bool clearPackingTag = false)
    {
        AssetImporter ai = AssetImporter.GetAtPath(assetname);
        if (ai != null)
        {

            bool isDirty = false;
            if (clearABName)
            {
                ai.assetBundleName = "";
                ai.SaveAndReimport();
                return;
            }
            string packing_name = StringTools.RemoveExName(bundlename);
            if (clearPackingTag)
            {
                packing_name = "";
            }
            if (ai is TextureImporter)
            {
                TextureImporter ti = (TextureImporter)ai;
                if (ti.textureType != TextureImporterType.Sprite)
                {
                    ti.textureType = TextureImporterType.Sprite;
                    isDirty = true;
                }
                if (ti.spriteImportMode != SpriteImportMode.Single)
                {
                    isDirty = true;
                    ti.spriteImportMode = SpriteImportMode.Single;
                }
                if (ti.spritePackingTag != packing_name)
                {
                    ti.spritePackingTag = packing_name;
                    isDirty = true;
                }
                if (ti.mipmapEnabled == true)
                {
                    ti.mipmapEnabled = false;
                    isDirty = true;
                }
            }
            bundlename = bundlename.ToLower();
            if (ai.assetBundleName != bundlename)
            {
                ai.assetBundleName = bundlename;
                isDirty = true;
            }

            if (isDirty)
            {
                ai.SaveAndReimport();
            }
        }
    }

    //[MenuItem("Build/CheckAtlas")]
    //public static void CheckAtlas()
    //{
    //    if (Selection.activeGameObject == null)
    //        return;
    //    Dictionary<string, int> atlas = new Dictionary<string, int>();
    //    foreach (UISprite sprite in Selection.activeGameObject.GetComponentsInChildren<UISprite>())
    //    {
    //        atlas[sprite.atlas.name] = 1;
    //    }
    //    foreach (KeyValuePair<string, int> pair in atlas)
    //        sw.util.LoggerHelper.Debug("atlas:" + pair.Key);
    //    Dictionary<string, int> texs = new Dictionary<string, int>();
    //    foreach (UITexture tex in Selection.activeGameObject.GetComponentsInChildren<UITexture>())
    //    {
    //        texs[tex.mainTexture.name] = 1;
    //    }
    //    foreach (KeyValuePair<string, int> pair in texs)
    //        sw.util.LoggerHelper.Debug("tex:" + pair.Key);
    //}
    //[MenuItem("Build/SetLabelOrder")]
    //public static void SetLabelOrder()
    //{
    //    string path = "/UI/UIPrefabs";
    //    string dirpath = Application.dataPath + path;
    //    int prefixlen = Application.dataPath.Length - "Assets".Length;
    //    foreach (string fn in Directory.GetFiles(dirpath, "*.prefab", SearchOption.AllDirectories))
    //    {
    //        string assetpath = fn.Substring(prefixlen);
    //        GameObject prefab = AssetDatabase.LoadAssetAtPath(assetpath, typeof(GameObject)) as GameObject;
    //        if (prefab != null)
    //        {
    //            sw.util.LoggerHelper.Debug("prefab Name:" + prefab.name);
    //            GameObject go = GameObjectUtil.Instantiate(prefab);
    //            foreach (UILabel label in go.GetComponentsInChildren<UILabel>())
    //            {
    //                label.depth += 500;
    //            }
    //            assetpath = assetpath.Replace("\\", "/");
    //            GameObject updatePrefab = PrefabUtility.CreatePrefab(assetpath, go);
    //            //PrefabUtility.ReplacePrefab(go, updatePrefab, ReplacePrefabOptions.ConnectToPrefab);
    //            AssetDatabase.SaveAssets();
    //        }
    //    }
    //}
#if UNITY_EDITOR
    public static string GetPlatformFolderForAssetBundles(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "iOS";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";
            case BuildTarget.StandaloneOSX:
                return "OSX";
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformFolderForAssetBundles(RuntimePlatform) function.
            default:
                return null;
        }
    }
#endif
    
    //[MenuItem("Build/UIAssetBundles")]
    public static void BuildAssetBundles()
    {
        // Choose the output path according to the build target.
        string outputPath = Path.Combine(Path.Combine(kAssetBundlesOutputPath, GetPlatformFolderForAssetBundles(EditorUserBuildSettings.activeBuildTarget)), "ui");

        

        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        BuildPipeline.BuildAssetBundles(outputPath,BuildAssetBundleOptions.ChunkBasedCompression| BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.IgnoreTypeTreeChanges, EditorUserBuildSettings.activeBuildTarget);
    }
    //[MenuItem("Build/RebuildUIAssetBundles")]
    public static void ReBuildAssetBundles()
    {
        // Choose the output path according to the build target.
        string outputPath = Path.Combine(Path.Combine(kAssetBundlesOutputPath, GetPlatformFolderForAssetBundles(EditorUserBuildSettings.activeBuildTarget)), "ui");
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        BuildPipeline.BuildAssetBundles(outputPath,BuildAssetBundleOptions.ChunkBasedCompression| BuildAssetBundleOptions.ForceRebuildAssetBundle, EditorUserBuildSettings.activeBuildTarget);
    }

    //[MenuItem("Build/Print AssetBundle Output Path")]
    public static void PrintAssetBundleOutputPath()
    {
        sw.util.LoggerHelper.Debug(AssetBundleOutputPath());
    }

    static string AssetBundleOutputPath()
    {
        return Path.Combine(Path.Combine(kAssetBundlesOutputPath, GetPlatformFolderForAssetBundles(EditorUserBuildSettings.activeBuildTarget)), "ui");
    }


    static List<string> GetSubFolders(string path)
    {
        if (!IsDirectoryExists(path))
        {
            return null;
        }
        DirectoryInfo root = new DirectoryInfo(path);

        DirectoryInfo[] dirs = root.GetDirectories();
        List<string> folders = new List<string>();
        if (dirs.Length > 0)
        {
            for (int i = 0; i < dirs.Length; i++)
            {
                folders.Add(ChangePathFormat(dirs[i].FullName));
            }
        }

        return folders;

    }

    static bool IsDirectoryExists(string path)
    {
        if (Directory.Exists(path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    static string ChangePathFormat(string path)
    {
        string newPath = path.Replace('\\', '/');
        return newPath;
    }
    static public void UpdateAndBuildAssetBundleFromPython()
    {
        UpdateUGUIAsset();         
        BuildAssetBundles();
    }
    static public void UpdateAndBuildAssetBundle()
    {
        UpdateUGUIAsset();
        CreateOrUpdateUIFilter();
        BuildAssetBundles();
    }

    static public void UpdateUIFilter()
    {
        CreateOrUpdateUIFilter();
    }
}
