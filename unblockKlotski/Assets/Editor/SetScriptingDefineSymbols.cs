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

public class SetScriptingDefineSymbols
{

    [MenuItem("发布设置/设置国内安卓发包参数")]
    public static void BuildAndridForChinaMarket()
    {

        // 确保在 Android 平台下执行
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            Debug.Log("请在Android平台进行设置");
            return;
        }

        SetBuildSettingsForChinaMarket();

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
        ClearSymbols(BuildTargetGroup.Android);
        // 设置符号，目标平台是 Windows Standalone
        SetScriptingDefineSymbols.SetSymbols(BuildTargetGroup.Android, "TAP_TAP_ANTI");
        SetScriptingDefineSymbols.SetSymbols(BuildTargetGroup.Android, "CSJADJH");

        // 如果需要清除符号
        // SetScriptingDefineSymbols.ClearSymbols(BuildTargetGroup.Standalone.ToString());
    }





    public static void SetKeystore()
    {

        // 获取 Assets 文件夹的路径
        string assetsPath = Application.dataPath;

        // 获取父的父级目录的路径
        string projectParentDirectory = Directory.GetParent(Directory.GetParent(assetsPath).FullName).FullName;

        // 拼接目标文件路径
        string keystoreFilePath = Path.Combine(projectParentDirectory, "工程设置相关", "upload-keystore.jks");

        // 设置 Keystore 文件路径
        string keystorePath = keystoreFilePath;
        string keystorePassword = "289100LIao";
        string keyAlias = "upload";
        string keyAliasPassword = "289100LIao";

        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystoreName = keystorePath;
        PlayerSettings.Android.keystorePass = keystorePassword;
        PlayerSettings.Android.keyaliasName = keyAlias;
        PlayerSettings.Android.keyaliasPass = keyAliasPassword;

        // 提示用户已经完成设置
        Debug.Log("Keystore settings have been applied.");
    }

    public static int VersionNum = 72;
    public static void SetBuildSettingsForChinaMarket()
    {
        VersionNum = PlayerSettings.Android.bundleVersionCode + 1;
        // 设置包名
        PlayerSettings.applicationIdentifier = "com.liaoyanxuan.unblockMeKlotski";

        // 设置版本号
        PlayerSettings.bundleVersion = $"1.0.{VersionNum}";

        // 设置 Bundle Version Code (Android)
        PlayerSettings.Android.bundleVersionCode = VersionNum;

        // 设置最小 API 级别
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22; // API Level 21 (Android 5.0)

        // 设置目标 API 级别
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)31;//AndroidSdkVersions.AndroidApiLevel30; // API Level 30 (Android 11)

        // 设置脚本后端（如 IL2CPP 或 Mono）
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

        // 设置 API 兼容性级别
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_Standard_2_0);

        // 设置 C++ 编译器配置
        PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Android, Il2CppCompilerConfiguration.Release);


        // 设置 ARMv7 和 ARM64 目标架构
        PlayerSettings.Android.targetArchitectures =
            UnityEditor.AndroidArchitecture.ARMv7 | UnityEditor.AndroidArchitecture.ARM64;

        PlayerSettings.Android.buildApkPerCpuArchitecture = false;

        PlayerSettings.Android.androidTargetDevices = UnityEditor.AndroidTargetDevices.AllDevices;

        PlayerSettings.Android.preferredInstallLocation = UnityEditor.AndroidPreferredInstallLocation.PreferExternal;

        Debug.Log("Build settings have been set successfully!");

        // 设置 Internet Access
        /*
         * <uses-permission android:name="android.permission.INTERNET" />
         */
        PlayerSettings.Android.forceInternetPermission = true; // 可以选择 Auto, Require, None

        // 设置 Write Permission
        /*
         * *
         * 当 forceSDCardPermission 设置为 true 时，Unity 会确保在生成的 Android 应用的 AndroidManifest.xml 文件中包含对外部存储的写权限声明。
         * 这意味着即使应用程序没有明确需要写入外部存储的权限，Unity 也会强制为应用程序添加此权限
         * 设置 forceSDCardPermission 为 true 时，应用程序在安装时会请求用户同意授予对外部存储设备的写入权限。
         * 这在某些情况下是必要的，例如当应用程序需要将数据写入外部存储（如 SD 卡）时。
            如果你不需要强制要求此权限，可以将 forceSDCardPermission 设置为 false，让权限请求根据实际需要来决定。

            用途场景
            True: 适用于需要访问 SD 卡的应用，如媒体播放器或文件管理器。
            False: 适用于不需要访问外部存储的应用，以减少权限请求，提升用户的信任度。
        <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
        <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />

         * 
         */
        PlayerSettings.Android.forceSDCardPermission = false; // 必须为, Internal，否则隐私政策处理会很麻烦

        PlayerSettings.stripEngineCode = true;

        SetDefineSymbols();
        SetKeystore();

    }


    [MenuItem("发布设置/导出国内安卓工程")]
    public static void BuildAndroidProject()
    {
        //设置PlayerSetting参数
        BuildAndridForChinaMarket();

        /////*******************导出工程-start********************/////
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;

        // 获取 PlayerSettings 中的包名
        string packageName = PlayerSettings.applicationIdentifier;

        string[] packageNameParts = packageName.Split('.');
        string packageNameLastPart = packageNameParts.Length > 0 ? packageNameParts[packageNameParts.Length - 1] : "Unknown";

        // 获取 PlayerSettings 中的版本号
        string version = PlayerSettings.Android.bundleVersionCode.ToString();

        // 获取 Assets 文件夹的路径
        string assetsPath = Application.dataPath;

        // 获取父的父级目录的路径
        string gitProjectDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(assetsPath).FullName).FullName).FullName;

        // 指定基础导出目录
        string baseExportPath = Path.Combine(gitProjectDirectory, "AndroidExport");
        // 拼接完整的导出路径
        string exportPath = Path.Combine(baseExportPath, packageNameLastPart);

        exportPath = $"{exportPath}V{version}";

        // 检查目录是否存在，不存在则创建
        if (!Directory.Exists(exportPath))
        {
            Directory.CreateDirectory(exportPath);
        }

        // 设置构建目标
        BuildTarget buildTarget = BuildTarget.Android;

        // 设置构建选项
        // BuildOptions buildOptions = BuildOptions.AcceptExternalModificationsToPlayer | BuildOptions.DetailedBuildReport;
        //BuildOptions.DetailedBuildReport; 在MacOS会闪退
        BuildOptions buildOptions = BuildOptions.AcceptExternalModificationsToPlayer;
        // 构建 Android 工程
        BuildReport buildReport = BuildPipeline.BuildPlayer(GetBuildScenes(), exportPath, buildTarget, buildOptions);

        BuildWithReportToFile(buildReport);
        /////*******************导出工程-end********************/////



        Debug.Log("Android Project exported to: " + exportPath);
    }

    private static string[] GetBuildScenes()
    {
        // 获取所有启用的场景
        return EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
    }

    // 修改 gradle.properties 文件
    private static void ModifyGradleProperties(string pathToBuiltProject)
    {
        // gradle.properties 文件路径
        string gradlePropertiesPath = Path.Combine(pathToBuiltProject, "gradle.properties");

        if (File.Exists(gradlePropertiesPath))
        {
            string[] lines = File.ReadAllLines(gradlePropertiesPath);
            bool useAndroidXExists = lines.Any(line => line.Contains("android.useAndroidX=true"));
            bool enableJetifierExists = lines.Any(line => line.Contains("android.enableJetifier=true"));


            using (StreamWriter writer = new StreamWriter(gradlePropertiesPath))
            {
                foreach (string line in lines)
                {
                    // 去掉 android.enableR8 这一行
                    if (!line.Contains("android.enableR8"))
                    {
                        writer.WriteLine(line);
                    }
                }

                // 如果 android.useAndroidX=true 和 android.enableJetifier=true 不存在，则添加它们
                if (!useAndroidXExists)
                {
                    writer.WriteLine("android.useAndroidX=true");
                }

                if (!enableJetifierExists)
                {
                    writer.WriteLine("android.enableJetifier=true");
                }
            }

            Debug.Log("Modified gradle.properties successfully.");
        }
        else
        {
            Debug.LogError("gradle.properties file not found at: " + gradlePropertiesPath);
        }
    }
    //AGP设置为7.4.2
    private static void ModifyBuildGradle(string pathToBuiltProject)
    {
        // build.gradle 文件路径
        string gradleFilePath = Path.Combine(pathToBuiltProject, "build.gradle");

        if (File.Exists(gradleFilePath))
        {
            string[] lines = File.ReadAllLines(gradleFilePath);
            bool agpVersionUpdated = false;

            using (StreamWriter writer = new StreamWriter(gradleFilePath))
            {
                foreach (string line in lines)
                {
                    // 如果找到 AGP 的 classpath 行，则进行替换
                    if (line.Contains("com.android.tools.build:gradle"))
                    {
                        // 替换为 AGP 7.4.2
                        writer.WriteLine("    classpath 'com.android.tools.build:gradle:7.4.2'");
                        agpVersionUpdated = true;
                    }
                    else
                    {
                        writer.WriteLine(line);
                    }
                }

                // 如果没有找到 AGP 行，可以选择添加它（不推荐，因为它应该已经存在）
                if (!agpVersionUpdated)
                {
                    Debug.LogWarning("AGP classpath line not found in build.gradle. Please check the file format.");
                }
            }

            Debug.Log("Modified build.gradle successfully.");
        }
        else
        {
            Debug.LogError("build.gradle file not found at: " + gradleFilePath);
        }
    }

    private static void CopyGradleWrapper(string pathToBuiltProject)
    {
        // 获取 Assets 文件夹的路径
        string assetsPath = Application.dataPath;

        // 获取父的父级目录的路径
        string projectParentDirectory = Directory.GetParent(Directory.GetParent(assetsPath).FullName).FullName;

        // 设置源 gradle 文件夹路径
        string sourceGradleFolder = Path.Combine(projectParentDirectory, "工程设置相关", "国内安卓发布相关", "gradle");
        //设置源 gradlew 脚本 文件夹路径
        string sourceGradlewFolder = Path.Combine(projectParentDirectory, "工程设置相关", "国内安卓发布相关");

        // 设置目标 gradle 文件夹路径
        string targetGradleFolder = Path.Combine(pathToBuiltProject, "gradle");

        // 如果目标目录中已存在 gradle 文件夹，则先删除它
        if (Directory.Exists(targetGradleFolder))
        {
            Directory.Delete(targetGradleFolder, true);
        }

        // 拷贝源 gradle 文件夹到目标目录
        CopyDirectory(sourceGradleFolder, targetGradleFolder);


        // 构建完整的源文件路径
        string gradlewFilePath = Path.Combine(sourceGradlewFolder, "gradlew");
        string gradlewBatFilePath = Path.Combine(sourceGradlewFolder, "gradlew.bat");

        // 构建完整的目标文件路径
        string destinationGradlewFilePath = Path.Combine(pathToBuiltProject, "gradlew");
        string destinationGradlewBatFilePath = Path.Combine(pathToBuiltProject, "gradlew.bat");

        // 拷贝 gradlew
        if (File.Exists(gradlewFilePath))
        {
            File.Copy(gradlewFilePath, destinationGradlewFilePath, true);
            Debug.Log("gradlew file copied successfully.");
        }
        else
        {
            Debug.LogError("gradlew file not found in source folder.");
        }

        // 拷贝 gradlew.bat
        if (File.Exists(gradlewBatFilePath))
        {
            File.Copy(gradlewBatFilePath, destinationGradlewBatFilePath, true);
            Debug.Log("gradlew.bat file copied successfully.");
        }
        else
        {
            Debug.LogError("gradlew.bat file not found in source folder.");
        }

        Debug.Log("Copy GradleWrapper successfully.");
    }

    private static void renameUnityPlayerActivity(string pathToBuiltProject)
    {
        // 定义文件路径
        string javaFilePath = Path.Combine(pathToBuiltProject, "unityLibrary/src/main/java/com/unity3d/player");

        // 检查路径是否存在
        if (Directory.Exists(javaFilePath))
        {
            string oldFileName = "UnityPlayerActivityNew.java";
            string newFileName = "UnityPlayerActivity.java";

            string oldFilePath = Path.Combine(javaFilePath, oldFileName);
            string newFilePath = Path.Combine(javaFilePath, newFileName);


            // 先删除原来的UnityPlayerActivity.java
            if (File.Exists(newFilePath))
            {
                // 如果已有 UnityPlayerActivity.java，先删除
                File.Delete(newFilePath);
                Debug.Log("Deleted existing UnityPlayerActivity.java");
            }

            // 如果旧文件存在，则重命名
            if (File.Exists(oldFilePath))
            {
                File.Move(oldFilePath, newFilePath);
                Debug.Log("File renamed from " + oldFileName + " to " + newFileName);
            }
            else
            {
                Debug.LogWarning("File " + oldFileName + " not found!");
            }
        }
        else
        {
            Debug.LogWarning("Directory " + javaFilePath + " not found!");
        }
    }


    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {

        if (target == BuildTarget.Android)
        {
#if CSJADJH
            // 修改 gradle.properties 文件
            ModifyGradleProperties(pathToBuiltProject);

            //拷贝gradle wrapper
            CopyGradleWrapper(pathToBuiltProject);

            //AGP设置为7.4.2
            ModifyBuildGradle(pathToBuiltProject);

            ///// addKeyStroteToGradleBuild(pathToBuiltProject);
            //重命名UnityPlayerActivity
            renameUnityPlayerActivity(pathToBuiltProject);
#endif
        }
    }


    private static void addKeyStroteToGradleBuild(string pathToBuiltProject)
    {
        // 获取 Assets 文件夹的路径
        string assetsPath = Application.dataPath;

        // 获取父的父级目录的路径
        string projectParentDirectory = Directory.GetParent(Directory.GetParent(assetsPath).FullName).FullName;

        // 拼接目标文件路径
        string keystoreFilePath = Path.Combine(projectParentDirectory, "工程设置相关", "upload-keystore.jks");

        // 设置 Keystore 文件路径
        string keystorePath = keystoreFilePath;
        string keystorePassword = "289100LIao";
        string keyAlias = "upload";
        string keyAliasPassword = "289100LIao";

        string gradleFile = Path.Combine(pathToBuiltProject, "build.gradle");

        if (File.Exists(gradleFile))
        {
            string gradleContent = File.ReadAllText(gradleFile);

            string signingConfig = $@"
                                    signingConfigs {{
                                        release {{
                                            storeFile file(""{keystorePath.Replace("\\", "/")}"")
                                            storePassword ""{keystorePassword}""
                                            keyAlias ""{keyAlias}""
                                            keyPassword ""{keyAliasPassword}""
                                        }}
                                    }}

                                    buildTypes {{
                                        release {{
                                            signingConfig signingConfigs.release
                                        }}
                                    }}
                                    ";

            // 插入签名配置到 gradle 文件
            if (!gradleContent.Contains("signingConfigs"))
            {
                int androidBlockIndex = gradleContent.IndexOf("android {");
                gradleContent = gradleContent.Insert(androidBlockIndex + 8, signingConfig);
                File.WriteAllText(gradleFile, gradleContent);
            }
        }
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
        string reportPath = Path.Combine(Application.dataPath, "../build/BuildReport.txt");

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
