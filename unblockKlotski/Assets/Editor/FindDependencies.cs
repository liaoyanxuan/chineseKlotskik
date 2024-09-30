using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class FindDependencies : Editor
{
    [MenuItem("Assets/选中工具/z_资源依赖相关/查找资源被谁依赖了(目录或文件)", false, 20)]
    private static void FindAssetReferencing()
    {
        // 获取选中的对象
        Object selectedObject = Selection.activeObject;

        string selectedPath = null;
        if (selectedObject == null)
        {

            UnityEngine.Object[] selectedObjects = Selection.GetFiltered(typeof(DefaultAsset), SelectionMode.Assets);

            foreach (UnityEngine.Object obj in selectedObjects)
            {
                // 获取对象的路径
                selectedPath = AssetDatabase.GetAssetPath(obj);

                // 检查路径是否有效且是文件夹
                if (AssetDatabase.IsValidFolder(selectedPath))
                {
                    Debug.Log("选中的文件夹路径：" + selectedPath);
                    break;
                }
            }

            if(selectedPath == null)
            {
                Debug.LogWarning("不是文件也不是文件夹!");
                return;
            }
           
        }
        else
        {
            // 获取选中对象的路径
             selectedPath = AssetDatabase.GetAssetPath(selectedObject);
        }

        Debug.Log("选中的对象：" + selectedPath);

        if (Directory.Exists(selectedPath))
        {
            // 如果选中的是文件夹，遍历文件夹中的所有Asset
            FindReferencingInFolder(selectedPath);
        }
        else
        {
            // 如果选中的是单个Asset，查找该Asset的依赖项
            FindReferencingForAsset(selectedPath);
        }
    }

    private static void FindReferencingInFolder(string folderPath)
    {
        string[] assetPaths = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
        int totalReferencing = 0;
        foreach (string assetPath in assetPaths)
        {
            string assetPath2 = NormalizePathToMacOS(assetPath);
            if (assetPath2.EndsWith(".meta")) continue;
            totalReferencing= totalReferencing+FindReferencingForAsset(assetPath2);
        }

        Debug.Log($"文件夹 '{folderPath}' 总被引用数：{totalReferencing}:");

    }

    //统一成'/'貌似api才能正确找到依赖
    private static string NormalizePathToMacOS(string path)
    {
        return path.Replace('\\', '/');

    }

    private static int FindReferencingForAsset(string assetPath)
    {
       
        // 查找引用了这个资源的所有其他资源
        string[] allAssets = AssetDatabase.GetAllAssetPaths();
        List<string> referencingAssets = new List<string>();
        foreach (string asset in allAssets)
        {
            //asset依赖包含assetPath 且 不是同一个资源
            if (AssetDatabase.GetDependencies(asset).Contains(assetPath) && asset.Equals(assetPath)==false)
            {
                referencingAssets.Add(asset);
            }
        }

        if (referencingAssets.Count > 0)
        {
            Debug.Log($"Asset '{assetPath}' is referenced by the following assets:");
            foreach (string referencingAsset in referencingAssets)
            {
                Debug.Log($"  - {referencingAsset}");
            }
        }
        else
        {
            Debug.Log($"Asset '{assetPath}' is not referenced by any other assets.");
        }

        return referencingAssets.Count;
    }


    [MenuItem("Assets/选中工具/打印出选中的文件夹路径")]
    private static void GetSelectedFolderPath()
    {
        // 获取所有选中的对象
        Object[] selectedObjects = Selection.GetFiltered(typeof(DefaultAsset), SelectionMode.Assets);

        foreach (Object obj in selectedObjects)
        {
            // 获取对象的路径
            string path = AssetDatabase.GetAssetPath(obj);

            // 检查路径是否有效且是文件夹
            if (AssetDatabase.IsValidFolder(path))
            {
                Debug.Log("选中的文件夹路径：" + path);

            }
            else
            {
                Debug.LogError("选中的对象不是一个有效的文件夹！");
            }
        }
    }


    [MenuItem("Assets/选中工具/清除选中(Clear Selection)")]
    public static void ClearSelectionCacheRight()
    {
        Selection.objects = new UnityEngine.Object[0]; // 取消选中所有对象
    }

    [MenuItem("Tools/编辑器工具/清除选中(Clear Selection)")]
    public static void ClearSelectionCache()
    {
        Selection.objects = new UnityEngine.Object[0]; // 取消选中所有对象
    }
}
