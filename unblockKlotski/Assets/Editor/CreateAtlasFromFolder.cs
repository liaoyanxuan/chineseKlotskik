using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;
using System.IO;
using System.Collections.Generic;
using System;
using ChillyRoom.UnityEditor.iOS.Xcode.PBX;
using System.Text.RegularExpressions;
using System.Linq;


public class CreateAtlasFromFolder : MonoBehaviour
{
    //[MenuItem("Assets/创建文件夹图集")]   //unity图集，会把图集与散图都打进包里，项目中要通过spriteAtals引用到图片，才不会让散图和图集都打到包里
    private static void CreateAtlas()
    {
        // 获取选中的文件夹路径
        // 获取所有选中的对象
        UnityEngine.Object[] selectedObjects = Selection.GetFiltered(typeof(DefaultAsset), SelectionMode.Assets);

        string folderPath=string.Empty;
        foreach (UnityEngine.Object obj in selectedObjects)
        {
            // 获取对象的路径
            folderPath = AssetDatabase.GetAssetPath(obj);

            // 检查路径是否有效且是文件夹
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.Log("选中的文件夹路径：" + folderPath);

            }
            else
            {
                Debug.LogError("选中的对象不是一个有效的文件夹！");
            }
        }

        // 获取父目录
        string parentDirectory = Path.GetDirectoryName(folderPath);

        // 图集文件路径
        string atlasPath = $"{parentDirectory}/{Path.GetFileName(folderPath)}.spriteatlas";
        SpriteAtlas atlas;

        // 检查是否存在同名图集
        if (File.Exists(atlasPath))
        {
            atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
        }
        else
        {
            // 如果不存在则创建一个新的图集
            atlas = new SpriteAtlas();

            // 设置图集参数
            SpriteAtlasPackingSettings packingSettings = new SpriteAtlasPackingSettings
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                padding = 4
            };
            atlas.SetPackingSettings(packingSettings);

            SpriteAtlasTextureSettings textureSettings = new SpriteAtlasTextureSettings
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true
            };
            atlas.SetTextureSettings(textureSettings);

            TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings
            {
                name = "Android",
                overridden = true,
                maxTextureSize = 2048,
                format = TextureImporterFormat.ETC2_RGBA8,
                compressionQuality = 50,
                crunchedCompression = true
            };
            atlas.SetPlatformSettings(platformSettings);

            // 保存图集到指定路径
            AssetDatabase.CreateAsset(atlas, atlasPath);
        }

        // 获取文件夹中的所有Sprite
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });
        foreach (string guid in guids)
        {
            string spritePath = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            atlas.Add(new[] { sprite });
        }

        // 保存和刷新资产数据库
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("图集创建/更新完成：" + atlasPath);
    }


    [MenuItem("图集与压缩/列出项目中所有的图集")]
    private static void ListAllSpriteAtlases()
    {
        // 获取所有SpriteAtlas资产路径
        string[] atlasGuids = AssetDatabase.FindAssets("t:SpriteAtlas");

        List<string> atlasPaths = new List<string>();

        foreach (string guid in atlasGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            atlasPaths.Add(path);
        }

        // 打印所有SpriteAtlas的路径
        Debug.Log("Found " + atlasPaths.Count + " Sprite Atlases:");
        foreach (string path in atlasPaths)
        {
            Debug.Log("Sprite Atlas Path: " + path);
        }
    }

    [MenuItem("图集与压缩/列出项目中所有的TPSheet")]
    public static void FindAllTPSheetFiles()
    {
        // 获取项目中的所有文件路径
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();

        // 遍历所有路径，找到以 .tpsheet 结尾的文件
        foreach (string path in allAssetPaths)
        {
            if (path.EndsWith(".tpsheet"))
            {
                Debug.Log("Found TPSheet file: " + path);
            }
        }

        Debug.Log("Search completed.");
    }

  
    static void ReplaceInMetaFilesExcute(Dictionary<string, string> replacements)
    {
        // 指定Unity项目的根目录
        string projectPath = Application.dataPath;

        ReplaceInMetaFiles(projectPath, replacements);
    }

    static void ReplaceInMetaFiles(string directoryPath, Dictionary<string, string> replacements)
    {
        // 获取所有的文件
        string[] allFiles = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);

        // 过滤出场景、材质、预设和网格文件
        var filteredFiles = allFiles.Where(file =>
            file.EndsWith(".unity", StringComparison.OrdinalIgnoreCase) ||  // 场景文件
            file.EndsWith(".mat", StringComparison.OrdinalIgnoreCase) ||    // 材质文件
            file.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase) || // 预设文件
            file.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase) ||    // 网格文件（假设是FBX格式）
            file.EndsWith(".obj", StringComparison.OrdinalIgnoreCase)  ||     // 网格文件（OBJ格式）
            file.EndsWith(".mesh", StringComparison.OrdinalIgnoreCase) ||    // 网格文件（mesh格式）
            file.EndsWith(".anim", StringComparison.OrdinalIgnoreCase) ||   // 动画文件（anim）
            file.EndsWith(".controller", StringComparison.OrdinalIgnoreCase) ||  // 动画控制文件（controller）
            file.EndsWith(".asset", StringComparison.OrdinalIgnoreCase)
        ).ToArray();

        foreach (string file in filteredFiles)
        {
            string fileContent = File.ReadAllText(file);
            bool replaced = false;

            // 检查并替换每个替换对
            foreach (var replacement in replacements)
            {
                if (fileContent.Contains(replacement.Key))
                {
                    fileContent = fileContent.Replace(replacement.Key, replacement.Value);
                    replaced = true;
                }
            }

            // 如果进行了替换，则写回文件
            if (replaced)
            {
                File.WriteAllText(file, fileContent);
                Debug.Log($"Replaced in file: {file}");
            }
        }

        Debug.Log("Replacement completed.");
    }

   
    [MenuItem("Assets/选中工具/图集相关/分析 TPSheet png的Meta 文件且进行替换和9宫处理", false, 1000)]
    private static void AnalyzeSelectedTPSheetMetaFileAndExcuteAll()
    {
        AnalyzeSelectedTPSheetMetaFileAndExcute();
        AnalyzeSelectedTPSheetAtlasBorder();
    }

    /*
     * 
     * 替换TP图集的步骤
     * [MenuItem("Assets/选中工具/图集相关/分析 TPSheet png的Meta 文件且进行替换", false, 1000)]
     * 0.散图只使用一级目录，避免修改图片原始命名！！
     * 1.散图放到TexturePacker软件中，输出tpsheet文件（TxturePacker插件会通过tpsheet文件生成untiy的Sprite Mode是multiple模式图集png，meta里有所有散图信息）
     * 2.分析 TPSheet png的Meta 文件且进行替换
         * 2-1.分析 TPSheet png的Meta,会取出散图，形成 图片名字与meta中的唯一引用字符串 {"tab_active_border", "{fileID: -726829255, guid: 719d1a8f2e3fd405abe83b543ee65cb2, type: 3}"} 
         * 2-2.分析 同名目录下的散图，将形成 图片名字与现存 meta中的唯一引用字符串      {"tab_active_border", "{fileID: 21300000, guid: c03b79152f59d0949af2cdc5b79f94e3, type: 3}"}  
         * 2-3.合并 两个Dictionary,用散图唯一引用字符串为key（有2个或多个，可以形成多条，key不一样value一样）, 图集唯一字符串为value，形成新的Dic
         * 2-4.全局搜索替换，遍历字典，将key都替换成value （需要unity开启forceText模式）
     * 3.替换完后，检查原散图文件夹的被依赖情况，如果被依赖数为0，则表示已经全部替换完
     * 4.处理图集9宫数据，把散图9宫格应用到图集9宫
     * 5.检查，提交资源，项目中完成图集替换散图流程
     * 6.将散图移出项目外
     * =================================================================================================
     * 
     * 替换完毕，进行被引用验证，查看散图是否已无全局引用 
     * [MenuItem("Assets/选中工具/z_资源依赖相关/查找资源被谁依赖了(目录或文件)", false, 20)]
     * 
     * ***/
    private static Dictionary<string, string> TPSheetNameToGuidDic = new Dictionary<string, string>();
    [MenuItem("Assets/选中工具/图集相关/分析 TPSheet png的Meta 文件且进行替换", false, 1000)]
    private static void AnalyzeSelectedTPSheetMetaFileAndExcute()
    {
        TPSheetNameToGuidDic.Clear();
        // 获取当前选中的资产路径
        string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        // 检查是否选中了 选中tpsheet图集png 文件
        if (selectedPath.EndsWith(".png"))
        {
            string metaFilePath = selectedPath + ".meta";
            if (File.Exists(metaFilePath))
            {
                AnalyzeMetaFile(metaFilePath);
            }
            else
            {
                Debug.LogError("Meta file not found for the selected TPSheet.");
            }


            // 获取不带扩展名的文件名
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(selectedPath);

            // 获取文件的目录路径
            string directoryPath = Path.GetDirectoryName(selectedPath);

            // 组合成同名文件夹路径
            string folderPath = Path.Combine(directoryPath, fileNameWithoutExtension);

            extractPNGGuidsFromPath(folderPath);

            updatedTPSheetNameToGuidDic();
        }
        else
        {
            Debug.LogError("Please select a .tpsheet file.");
        }
    }


    [MenuItem("Assets/选中工具/图集相关/分析 TPSheet png的Meta 文件(只分析不替换)", false, 1000)]
    private static void AnalyzeSelectedTPSheetMetaFile()
    {
        TPSheetNameToGuidDic.Clear();
        // 获取当前选中的资产路径
        string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        // 检查是否选中了 选中tpsheet图集png 文件
        if (selectedPath.EndsWith(".png"))
        {
            string metaFilePath = selectedPath + ".meta";
            if (File.Exists(metaFilePath))
            {
                AnalyzeMetaFile(metaFilePath);
            }
            else
            {
                Debug.LogError("Meta file not found for the selected TPSheet.");
            }


            // 获取不带扩展名的文件名
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(selectedPath);

            // 获取文件的目录路径
            string directoryPath = Path.GetDirectoryName(selectedPath);

            // 组合成同名文件夹路径
            string folderPath = Path.Combine(directoryPath, fileNameWithoutExtension);

            extractPNGGuidsFromPath(folderPath);

           
        }
        else
        {
            Debug.LogError("Please select a .tpsheet file.");
        }
    }

    private static void updatedTPSheetNameToGuidDic() 
    {

        // 创建一个新的字典来存储替换后的键值对
        Dictionary<string, string> updatedTPSheetNameToGuidDic = new Dictionary<string, string>();

        foreach (var entry in TPSheetNameToGuidDic)
        {
            string key = entry.Key;
            string value = entry.Value;

            // 如果在PngNameToGuidDic中找到了相同的key
            if (PngNameToGuidDic1111.TryGetValue(key, out string newKey))
            {
                // 使用PngNameToGuidDic中的value作为新的key
                updatedTPSheetNameToGuidDic[newKey] = value;
                updatedTPSheetNameToGuidDic[PngNameToGuidDic2222[key]] = value;
            }
            else
            {
                // 如果没有找到相应的key，就保留原有的key值
                //updatedTPSheetNameToGuidDic[key] = value;
                DebugEx.Log("图集中有对应文件夹中不存在的图片，请检查是否已经删除：", key);
            }

        }

        foreach (var entry in updatedTPSheetNameToGuidDic)
        {
            string key = entry.Key;
            string value = entry.Value;

            // 在这里对 key 和 value 执行你需要的操作
            Debug.Log($"Key: {key}, Value: {value}");
        }

        ReplaceInMetaFilesExcute(updatedTPSheetNameToGuidDic);
    }



    private static void AnalyzeMetaFile(string metaFilePath)
    {
        Debug.Log($"AnalyzeMetaFile:{metaFilePath}");
        string[] lines = File.ReadAllLines(metaFilePath);
        string currentName = string.Empty;
        string internalID = string.Empty;
        string pngGuid = string.Empty;

        foreach (string line in lines)
        {
            if (pngGuid.Equals(string.Empty))
            {
                if (line.Trim().StartsWith("guid:"))
                {
                    pngGuid = line.Trim().Substring("guid:".Length).Trim();
                }
            }
           

            if (line.Trim().StartsWith("name:"))
            {
                currentName = line.Trim().Substring("name:".Length).Trim();
                // 去除引号
                currentName = currentName.Trim('"');
                currentName = Regex.Unescape(currentName);
            }
            else if (line.Trim().StartsWith("internalID:"))
            {
                internalID = line.Trim().Substring("internalID:".Length).Trim();

                if (currentName.Equals(string.Empty) == false)
                {

                    // 输出 name 和 spriteID
                    Debug.Log($"{{\"{currentName}\", \"{{fileID: {internalID}, guid: {pngGuid}, type: 3}}\"}} ");

                    TPSheetNameToGuidDic.Add(currentName, $"{{fileID: {internalID}, guid: {pngGuid}, type: 3}}");
                }

                //匹配完清空
                currentName = string.Empty;
                internalID = string.Empty;
                
            }
        }

        Debug.Log($"{metaFilePath} Analysis completed.");
    }

    //同名文件夹下guid分析
    private static void extractPNGGuidsFromPath(string path)
    {
        PngNameToGuidDic1111.Clear();
        PngNameToGuidDic2222.Clear();

        if (Directory.Exists(path))
        {
            ProcessDirectory(path);
        }
        else if (Path.GetExtension(path).ToLower() == ".png")
        {
            ProcessFile(path);
        }
        
    }


    private static Dictionary<string, string> PngNameToGuidDic1111 =new Dictionary<string, string>();
    private static Dictionary<string, string> PngNameToGuidDic2222 = new Dictionary<string, string>();


    [MenuItem("Assets/选中工具/图集相关/分析文件夹下所有 PNG GUIDs")]
    public static void ExtractPNGGuids()
    {
        PngNameToGuidDic1111.Clear();
        PngNameToGuidDic2222.Clear();

        // 获取选中的资源路径
        var selectedPaths = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Assets);

        foreach (var obj in selectedPaths)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            if (Directory.Exists(path))
            {
                ProcessDirectory(path);
            }
            else if (Path.GetExtension(path).ToLower() == ".png")
            {
                ProcessFile(path);
            }
        }
    }

    private static void ProcessDirectory(string directoryPath)
    {
        // 遍历当前文件夹中的所有文件
        string[] files = Directory.GetFiles(directoryPath, "*.png", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            ProcessFile(file);
        }
    }
    /**
     * 
     * 以下是 fileID 的一些常见值及其含义：
     *
     *   2800000: 以Texture原始图形式引用
     *   21300000: 以Sprite形式引用，只有Sprite才能使用图集
     *  
     * */
    private static void ProcessFile(string filePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
        string guid = AssetDatabase.AssetPathToGUID(assetPath);

         //不是21300000
        Debug.Log($"{{\"{fileName}\", \"{{fileID: 21300000, guid: {guid}, type: 3}}\"}} ");
     

        PngNameToGuidDic1111.Add(fileName, $"{{fileID: 21300000, guid: {guid}, type: 3}}");
        PngNameToGuidDic2222.Add(fileName, $"{{fileID: 21300000, guid: {guid},\n        type: 3}}");
    }




    private static Dictionary<string, string> PngNameToBorderDic = new Dictionary<string, string>();

    [MenuItem("Assets/选中工具/图集相关/分析 TPSheet png的Meta 的9宫数据(替换)", false, 1000)]
    private static void AnalyzeSelectedTPSheetAtlasBorder()
    {
        PngNameToBorderDic.Clear();
        // 获取当前选中的资产路径
        string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        // 检查是否选中了 选中tpsheet图集png 文件
        if (selectedPath.EndsWith(".png"))
        {
            string metaFilePath = selectedPath + ".meta";
            if (File.Exists(metaFilePath))
            {
                AnalyzeAtlasBorder(metaFilePath);
            }
            else
            {
                Debug.LogError("Meta file not found for the selected TPSheet.");
            }


            // 获取不带扩展名的文件名
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(selectedPath);

            // 获取文件的目录路径
            string directoryPath = Path.GetDirectoryName(selectedPath);

            // 组合成同名文件夹路径
            string folderPath = Path.Combine(directoryPath, fileNameWithoutExtension);

            extractSpriteBorderFromPath(folderPath);

            ProcessMetaFileAndChangBorder(metaFilePath, PngNameToBorderDic);
        }
        else
        {
            Debug.LogError("Please select a .tpsheet file.");
        }
    }
    //处理border,九宫拉伸
    private static void AnalyzeAtlasBorder(string metaFilePath)
    {
        Debug.Log($"AnalyzeMetaFile:{metaFilePath}");
        string[] lines = File.ReadAllLines(metaFilePath);
        string currentName = string.Empty;
        string borderID = string.Empty;
        string pngGuid = string.Empty;

        foreach (string line in lines)
        {
            if (pngGuid.Equals(string.Empty))
            {
                if (line.Trim().StartsWith("guid:"))
                {
                    pngGuid = line.Trim().Substring("guid:".Length).Trim();
                }
            }


            if (line.Trim().StartsWith("name:"))
            {
                currentName = line.Trim().Substring("name:".Length).Trim();
                // 去除引号
                currentName = currentName.Trim('"');
                currentName = Regex.Unescape(currentName);
            }
            else if (line.Trim().StartsWith("border:"))
            {
                borderID = line.Trim().Substring("border:".Length).Trim();

                if (currentName.Equals(string.Empty) == false)
                {

                    // 输出 name 和 spriteID
                    Debug.Log($"name:{currentName},border:{borderID}");



                }

                //匹配完清空
                currentName = string.Empty;
                borderID = string.Empty;

            }
        }

        Debug.Log($"{metaFilePath} Analysis completed.");
    }

    //同名文件夹下guid分析
    private static void extractSpriteBorderFromPath(string path)
    {
       

        if (Directory.Exists(path))
        {
            ProcessDirectoryBorder(path);
        }
        else if (Path.GetExtension(path).ToLower() == ".png")
        {
            ProcessFileBorder(path);
        }

    }

    private static void ProcessDirectoryBorder(string directoryPath)
    {
        // 遍历当前文件夹中的所有文件
        string[] files = Directory.GetFiles(directoryPath, "*.png", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            ProcessFileBorder(file);
        }
    }
    /**
     * 
     * 以下是 fileID 的一些常见值及其含义：
     *
     *   2800000: 以Texture原始图形式引用
     *   21300000: 以Sprite形式引用，只有Sprite才能使用图集
     *  
     * */
    private static void ProcessFileBorder(string pngFilePath)
    {
        string metaFilePath = pngFilePath + ".meta";

        if (!File.Exists(metaFilePath))
        {
            Debug.LogError("Meta file not found: " + metaFilePath);
            return;
        }


        string fileName = Path.GetFileNameWithoutExtension(pngFilePath);

        string[] metaFileLines = File.ReadAllLines(metaFilePath);

        for (int i = 0; i < metaFileLines.Length; i++)
        {
            if (metaFileLines[i].Contains("spriteBorder:"))
            {
                // Extract the value after spriteBorder:
                string spriteBorderValue = metaFileLines[i].Replace("spriteBorder:", "").Trim();
                Debug.Log($"name:{fileName},border:{spriteBorderValue}");
                PngNameToBorderDic[fileName]= spriteBorderValue;
                return ;
            }
        }

        Debug.LogWarning("spriteBorder not found in meta file.");

    }

    public static void ProcessMetaFileAndChangBorder(string metaFilePath, Dictionary<string, string> nameToBorderDic)
    {
      

        if (!File.Exists(metaFilePath))
        {
            Debug.LogError("Meta file not found: " + metaFilePath);
            return;
        }

        string[] metaFileLines = File.ReadAllLines(metaFilePath);
        bool isMultiple = false;
        bool hasChanges = false;

        for (int i = 0; i < metaFileLines.Length; i++)
        {
            // Check if the sprite mode is multiple
            if (metaFileLines[i].Contains("spriteMode:") && metaFileLines[i].Contains("2"))
            {
                isMultiple = true;
            }

            if (isMultiple)
            {
                // Look for sprite names and borders
                if (metaFileLines[i].Contains("name:"))
                {
                    string spriteName = metaFileLines[i].Replace("name:", "").Trim();

                    if (nameToBorderDic.ContainsKey(spriteName))
                    {
                        // Look for the border property
                        for (int j = i; j < metaFileLines.Length; j++)
                        {
                            if (metaFileLines[j].Contains("border:"))
                            {
                                string newBorderValue = nameToBorderDic[spriteName];
                                metaFileLines[j] = "      border: " + newBorderValue; // Modify the border line
                                hasChanges = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        if (hasChanges)
        {
            /*
             * 总结
                \n (LF): Unix/Linux, modern macOS.
                \r\n (CRLF): Windows.
                \r (CR): Classic macOS (pre-OS X).
                了解这些换行符格式对于跨平台开发或处理来自不同操作系统的文本文件特别重要。
             */
            // Write lines with macOS line endings (\n)
            string macOSLineEnding = "\n";
            // 使用 string.Join 将行组合在一起，并在末尾加上一个 "\n"
            string content = string.Join(macOSLineEnding, metaFileLines) + "\n";

            File.WriteAllText(metaFilePath, content);
            Debug.Log("Meta file updated: " + metaFilePath);
        }
        else
        {
            Debug.LogWarning("No changes made to the meta file: " + metaFilePath);
        }
    }


}
