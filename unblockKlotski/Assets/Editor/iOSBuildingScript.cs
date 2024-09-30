using Unity.Android.Types;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor.Build.Content;
using UnityEditor.Build;
using UnityEditor.Callbacks;
using ByteDance.Union;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using System.Runtime.Remoting.Lifetime;
using UnityEditor.Build.Reporting;

public class iOSBuildingScript
{

    [MenuItem("发布设置/设置国内iOS发包参数")]
    public static void BuildAndridForChinaMarket()
    {

        // 确保在 Android 平台下执行
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
        {
            Debug.Log("请在iOS平台进行设置");
            return;
        }

        SetBuildSettingsForGlobalMarket();

        // 保存更改
        AssetDatabase.SaveAssets();

        // 显示提示
        Debug.Log("Player settings have been set successfully!.");
    }

    [MenuItem("发布设置/其他逐步设置/Set Android SDK Path")]
    public static void SetSDKPath()
    {
        // 清空 SDK Path 来取消勾选 Android SDK
        EditorPrefs.DeleteKey("AndroidSdkRoot");

        string sdkPath = "/Users/liaoyanxuan/Library/Android/SDK"; // 将此路径替换为你自己的 Android SDK 路径

        if (IsWindows())
        {
            Debug.Log("当前编辑器环境是 Windows");
            sdkPath = "C:\\Users\\PC\\AppData\\Local\\Android\\Sdk";
        }
        else if (IsMacOS())
        {
            Debug.Log("当前编辑器环境是 IsMacOS");
            sdkPath = "/Users/liaoyanxuan/Library/Android/SDK"; // 将此路径替换为你自己的 Android SDK 路径
        }


        // 设置 Android SDK 路径
        EditorPrefs.SetString("AndroidSdkRoot", sdkPath);

        // 如果你也想设置 NDK 和 JDK 的路径，可以使用以下代码：
        // EditorPrefs.SetString("AndroidNdkRoot", "C:/path/to/your/android/ndk");
        // EditorPrefs.SetString("JdkPath", "C:/path/to/your/jdk");

        Debug.Log("Android SDK path set to: " + sdkPath);
    }

    public static void SetSymbols(BuildTargetGroup targetGroup, string symbols)
    {
        // 获取当前的编译符号
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

        // 添加新的符号，如果已经存在则不会重复添加
        if (!currentSymbols.Contains(symbols))
        {
            currentSymbols += ";" + symbols;
        }

        // 设置更新后的符号
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, currentSymbols);
    }

    public static void ClearSymbols(BuildTargetGroup targetGroup)
    {
        // 清空所有符号
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Empty);
    }

    // [MenuItem("发布设置/设置国内安卓宏定义")]
    public static void SetDefineSymbols()
    {
        ClearSymbols(BuildTargetGroup.iOS);
       
       
       // SetSymbols(BuildTargetGroup.iOS, "CSJADJH");
        //SetSymbols(BuildTargetGroup.iOS, "TAP_TAP_ANTI");

    }



    public static int VersionNum = 41;
    public static void SetBuildSettingsForGlobalMarket()
    {
        VersionNum = int.Parse(PlayerSettings.iOS.buildNumber) + 1;
        // 设置包名
        PlayerSettings.applicationIdentifier = "com.liaoyanxuan.unblockmeklotski";

        // 设置应用版本号 (CFBundleShortVersionString)
        PlayerSettings.bundleVersion = $"1.0.{VersionNum}";

       

        // 设置构建号 (CFBundleVersion)
        PlayerSettings.iOS.buildNumber = VersionNum.ToString();

        PlayerSettings.iOS.allowHTTPDownload = true;

           // 设置脚本后端（如 IL2CPP 或 Mono）
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);

        // 设置 API 兼容性级别
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.iOS, ApiCompatibilityLevel.NET_Standard_2_0);

        // 设置 C++ 编译器配置
        PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.iOS, Il2CppCompilerConfiguration.Release);


        PlayerSettings.iOS.targetDevice = UnityEditor.iOSTargetDevice.iPhoneAndiPad;
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        PlayerSettings.stripEngineCode = true;

        Debug.Log("Build settings have been set successfully!");


        SetDefineSymbols();
        //SetKeystore();

    }



   

    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {

       
    }



    // 递归拷贝目录
    private static void CopyDirectory(string sourceDir, string destinationDir)
    {
        // 创建目标目录
        Directory.CreateDirectory(destinationDir);

        // 拷贝文件
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
            File.Copy(file, destFile);
        }

        // 递归拷贝子目录
        foreach (string subDir in Directory.GetDirectories(sourceDir))
        {
            string destSubDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
            CopyDirectory(subDir, destSubDir);
        }
    }

    public static bool IsWindows()
    {
        return Application.platform == RuntimePlatform.WindowsEditor;
    }

    public static bool IsMacOS()
    {
        return Application.platform == RuntimePlatform.OSXEditor;
    }

    public static void BuildWithReportToFile(BuildReport report)
    {
        // 定义报告文件路径
        string reportPath = Path.Combine(Application.dataPath, "../build/iOSBuildReport.txt");

        using (StreamWriter writer = new StreamWriter(reportPath))
        {
            writer.WriteLine("Build Report Summary:");
            writer.WriteLine($"Result: {report.summary.result}");
            writer.WriteLine($"Total Time: {report.summary.totalTime}");
            writer.WriteLine($"Total Size: {report.summary.totalSize} bytes");
            writer.WriteLine($"Output Path: {report.summary.outputPath}");

            writer.WriteLine("\nBuild Steps:");
            foreach (var step in report.steps)
            {
                writer.WriteLine($"Step: {step.name}, Duration: {step.duration}");
                foreach (var message in step.messages)
                {
                    writer.WriteLine($"  - {message.type}: {message.content}");
                }
            }
        }

        Debug.Log($"Build report saved to {reportPath}");

        // 处理构建结果
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded");
        }
        else if (report.summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
}
