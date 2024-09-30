using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AtlasImporter : AssetPostprocessor
{
    
    private static readonly HashSet<string> specificPaths = new HashSet<string>
    {
         "Assets/NavySoftUnblockWood/Images/UI.png",
         "Assets/NavySoftUnblockWood/Images/UI2.png",
        "Assets/NavySoftUnblockWood/Images/UI3.png",
        "Assets/NavySoftUnblockWood/Images/UI4.png",
        "Assets/NavySoftUnblockWood/Images/BackGround/map_playing.png",
        "Assets/Hyperbyte/Images/NewUIbg.png",
        "Assets/Hyperbyte/Images/rankbg.png",
       
        // 可以继续添加其他需要处理的路径
    };

    void OnPostprocessTexture(Texture2D texture)
    {
        // 获取当前纹理导入器
        TextureImporter textureImporter = (TextureImporter)assetImporter;

        // 检查 assetPath 是否在特定字典中
        if (specificPaths.Contains(assetPath))
        {
            // 如果在字典中，为Android平台设置为 ETC2_RGBA8Crunched 格式
            var androidSettings = textureImporter.GetPlatformTextureSettings("Android");
            androidSettings.format = TextureImporterFormat.ETC2_RGBA8Crunched;
            androidSettings.overridden = true;
            textureImporter.SetPlatformTextureSettings(androidSettings);

            Debug.Log($"Texture format set to ETC2_RGBA8Crunched for {assetPath}");
        }
        else
        {
            // 否则，为Android平台设置为 ETC2_RGBA8 格式
            var androidSettings = textureImporter.GetPlatformTextureSettings("Android");
           // androidSettings.format = TextureImporterFormat.ETC2_RGBA8;
            androidSettings.format = TextureImporterFormat.ETC2_RGBA8Crunched;
            androidSettings.overridden = true;
            textureImporter.SetPlatformTextureSettings(androidSettings);
        }

        // 为iOS平台设置ASTC格式，使用4x4块大小的最佳实践
        var iosSettings = textureImporter.GetPlatformTextureSettings("iPhone");
        iosSettings.format = TextureImporterFormat.ASTC_4x4;
        iosSettings.overridden = true;
        textureImporter.SetPlatformTextureSettings(iosSettings);

        // 强制重新导入纹理以应用新的设置
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

        Debug.Log($"Texture format set for Android and iOS for {assetPath}");
    }
}
