//------------------------------------------------------------------------------
// Copyright (c) 2018-2019 Beijing Bytedance Technology Co., Ltd.
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//------------------------------------------------------------------------------

namespace ByteDance.Union
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ChillyRoom.UnityEditor.iOS.Xcode;

#if !UNITY_ANDROID
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEngine;

    /// <summary>
    /// The post processor for xcode.
    /// </summary>
    internal static class XCodePostProcess
    {
        private const string KEY_SK_ADNETWORK_ITEMS = "SKAdNetworkItems";
        private const string KEY_SK_ADNETWORK_ID = "SKAdNetworkIdentifier";

        [PostProcessBuild(700)]
        public static void OnPostProcessBuild(
            BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.iOS)
            {
                return;
            }

            string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject proj = new PBXProject();
            proj.ReadFromFile(projPath);


            #if (UNITY_2019_3_OR_NEWER)
            // 2019.3以上的新接口
            var targetGUID = proj.TargetGuidByName("UnityFramework");
            #else
            // 旧有接口
            var targetGUID = proj.TargetGuidByName("Unity-iPhone");
            #endif
            //*****************************************穿山甲 start*********
            proj.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ObjC");
            proj.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", " -l\"c++\"");
            proj.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", " -l\"c++abi\"");
            proj.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-l\"sqlite3\"");
            proj.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-l\"z\"");

            proj.SetBuildProperty(targetGUID,"Supported Platforms", "iOS");
            proj.SetBuildProperty(targetGUID, "ENABLE_BITCODE", "NO");
            proj.SetBuildProperty(targetGUID, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");


            // framework to project
            proj.AddFrameworkToProject(targetGUID, "CoreServices.framework", false);
            proj.AddFrameworkToProject(targetGUID, "libsqlite3.tbd", false);
            proj.AddFrameworkToProject(targetGUID, "WebKit.framework", false);
            proj.AddFrameworkToProject(targetGUID, "VideoToolbox.framework", false);
            proj.AddFrameworkToProject(targetGUID, "SystemConfiguration.framework", false);
            proj.AddFrameworkToProject(targetGUID, "StoreKit.framework", false);
            proj.AddFrameworkToProject(targetGUID, "Security.framework", false);
            proj.AddFrameworkToProject(targetGUID, "ReplayKit.framework", false);
            proj.AddFrameworkToProject(targetGUID, "Photos.framework", false);
            proj.AddFrameworkToProject(targetGUID, "MultipeerConnectivity.framework", false);
            proj.AddFrameworkToProject(targetGUID, "MobileCoreServices.framework", false);
            proj.AddFrameworkToProject(targetGUID, "MetalPerformanceShaders.framework", false);
            proj.AddFrameworkToProject(targetGUID, "MediaToolbox.framework", false);
            proj.AddFrameworkToProject(targetGUID, "MediaPlayer.framework", false);
            proj.AddFrameworkToProject(targetGUID, "libresolv.9.tbd", false);
            proj.AddFrameworkToProject(targetGUID, "libresolv.tbd", false);
            proj.AddFrameworkToProject(targetGUID, "libiconv.tbd", false);
            proj.AddFrameworkToProject(targetGUID, "libcompression.tbd", false);
            proj.AddFrameworkToProject(targetGUID, "libc++abi.tbd", false);
            proj.AddFrameworkToProject(targetGUID, "JavaScriptCore.framework", false);
            proj.AddFrameworkToProject(targetGUID, "GLKit.framework", false);
            proj.AddFrameworkToProject(targetGUID, "CoreTelephony.framework", false);
            proj.AddFrameworkToProject(targetGUID, "AVFoundation.framework", false);
            proj.AddFrameworkToProject(targetGUID, "AudioToolbox.framework", false);
            proj.AddFrameworkToProject(targetGUID, "AssetsLibrary.framework", false);
            proj.AddFrameworkToProject(targetGUID, "Accelerate.framework", false);
            proj.AddFrameworkToProject(targetGUID, "AuthenticationServices.framework", false);
            proj.AddFrameworkToProject(targetGUID, "libil2cpp.a", false);
            proj.AddFrameworkToProject(targetGUID, "libiPhone-lib.a", false);
            proj.AddFrameworkToProject(targetGUID, "AuthenticationServices.framework", false);
            proj.AddFrameworkToProject(targetGUID, "CoreText.framework", false);
            proj.AddFrameworkToProject(targetGUID, "AVKit.framework", false);
            proj.AddFrameworkToProject(targetGUID, "CFNetwork.framework", false);
            proj.AddFrameworkToProject(targetGUID, "CoreGraphics.framework", false);
            proj.AddFrameworkToProject(targetGUID, "CoreMedia.framework", false);
            proj.AddFrameworkToProject(targetGUID, "CoreMotion.framework", false);
            proj.AddFrameworkToProject(targetGUID, "CoreVideo.framework", false);
            proj.AddFrameworkToProject(targetGUID, "Foundation.framework", false);
            proj.AddFrameworkToProject(targetGUID, "OpenAL.framework", false);
            proj.AddFrameworkToProject(targetGUID, "OpenGLES.framework", false);
            proj.AddFrameworkToProject(targetGUID, "QuartzCore.framework", false);
            proj.AddFrameworkToProject(targetGUID, "UIKit.framework", false);
            proj.AddFrameworkToProject(targetGUID, "libiconv.2.dylib", false);
            proj.AddFrameworkToProject(targetGUID, "Metal.framework", false);
            proj.AddFrameworkToProject(targetGUID, "libc++.tbd", false);
            proj.AddFrameworkToProject(targetGUID, "libz.tbd", false);
            proj.AddFrameworkToProject(targetGUID, "libbz2.tbd", false);
            proj.AddFrameworkToProject(targetGUID, "libxml2.tbd", false);
            proj.AddFrameworkToProject(targetGUID, "CoreLocation.framework", false);
            proj.AddFrameworkToProject(targetGUID, "ImageIO.framework", false);
            proj.AddFrameworkToProject(targetGUID, "AdSupport.framework", false);
            proj.AddFrameworkToProject(targetGUID, "MapKit.framework", false);
            proj.AddFrameworkToProject(targetGUID, "CoreImage.framework", false);
            proj.AddFrameworkToProject(targetGUID, "AppTrackingTransparency.framework", true); //optional
        
            //*****************************************穿山甲 end*********

            //*****************************************Add Capability start*********
            proj.AddCapability(targetGUID, PBXCapabilityType.GameCenter);

            var project = new UnityEditor.iOS.Xcode.PBXProject();
            project.ReadFromString(System.IO.File.ReadAllText(projPath));
            var manager = new UnityEditor.iOS.Xcode.ProjectCapabilityManager(projPath, "Entitlements.entitlements", null, project.GetUnityMainTargetGuid());
            manager.AddGameCenter(); // AddFrameworkToProject() With StoreKit.framework
            manager.WriteToFile();

            //*****************************************Add Capability end*********

            //*****************************************localization start*****************************************//
            /// 生成InfoPlist本地化文件到xcode上，发布出去的包就会带上我们的本地化文件并且生效
            NativeLocale.AddLocalizedStringsIOS(proj, pathToBuiltProject, Path.Combine(Application.dataPath, "lyxEngine/Editor/NativeLocale/iOS"));
            //*****************************************localization end*****************************************//


            proj.WriteToFile(projPath);


            ProjectCapabilityManager pcm = new ProjectCapabilityManager(projPath, null, "Unity-iPhone");
            pcm.AddGameCenter();
           
            pcm.WriteToFile();

            //*****************************************plistinfo start*****************************************//
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            // Get root
            PlistElementDict rootDict = plist.root;
            // for share sdk 截屏
           // //rootDict.SetString("NSPhotoLibraryAddUsageDescription", "need your agreement to save picture");
           // //rootDict.SetString("NSPhotoLibraryUsageDescription", "need your agreement to save picture");
          rootDict.SetString("NSUserTrackingUsageDescription", "We use your data to deliver personalized ads to improve your experience. For example, we will suggest ads based on your app usage patterns.");
            rootDict.SetString("CFBundleDisplayName", "Klotski Unblock Puzzle");
            rootDict.SetBoolean("UIViewControllerBasedStatusBarAppearance", false);

             PlistElementArray CFBundleLocalizationsArr = rootDict.CreateArray("CFBundleLocalizations");
            CFBundleLocalizationsArr.AddString("en");
           //// CFBundleLocalizationsArr.AddString("zh-Hans");
            //// CFBundleLocalizationsArr.AddString("zh_CN");
            //// CFBundleLocalizationsArr.AddString("zh_TW");
            rootDict.SetString("GADApplicationIdentifier", "ca-app-pub-4488157848198084~4871653381");
            rootDict.SetString("NSLocationWhenInUseUsageDescription", "Get geolocation when using");  //使用时获取地理位置


            PlistElementDict transportSecurityDict = rootDict.CreateDict("NSAppTransportSecurity");
            transportSecurityDict.SetBoolean("NSAllowsArbitraryLoads", true);


            rootDict.values.Remove("UIApplicationExitsOnSuspend");

            //skNetworkIds
            List<string> skNetworkIds = new List<string> ();
            skNetworkIds.Add("238da6jt44.skadnetwork");
            skNetworkIds.Add("22mmun2rn5.skadnetwork");
            skNetworkIds.Add("f7s53z58qe.skadnetwork");
            if (skNetworkIds.Count > 0)
            {
                AddSKAdNetworkIdentifier(plist, skNetworkIds);
            }

            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
            //*****************************************plistinfo end*****************************************//

           // ProjectCapabilityManager pcm = ProjectCapabilityManager();
        }

        private static void AddSKAdNetworkIdentifier(PlistDocument document, List<string> skAdNetworkIds)
        {
            PlistElementArray array = GetSKAdNetworkItemsArray(document);
            if (array != null)
            {
                foreach (string id in skAdNetworkIds)
                {
                    if (!ContainsSKAdNetworkIdentifier(array, id))
                    {
                        PlistElementDict added = array.AddDict();
                        added.SetString(KEY_SK_ADNETWORK_ID, id);
                    }
                }
            }
            else
            {
                NotifyBuildFailure("SKAdNetworkItems element already exists in Info.plist, but is not an array.", false);
            }
        }

        private static PlistElementArray GetSKAdNetworkItemsArray(PlistDocument document)
        {
            PlistElementArray array;
            if (document.root.values.ContainsKey(KEY_SK_ADNETWORK_ITEMS))
            {
                try
                {
                    PlistElement element;
                    document.root.values.TryGetValue(KEY_SK_ADNETWORK_ITEMS, out element);
                    array = element.AsArray();
                }
#pragma warning disable 0168
                catch (Exception e)
#pragma warning restore 0168
                {
                    // The element is not an array type.
                    array = null;
                }
            }
            else
            {
                array = document.root.CreateArray(KEY_SK_ADNETWORK_ITEMS);
            }
            return array;
        }

        private static bool ContainsSKAdNetworkIdentifier(PlistElementArray skAdNetworkItemsArray, string id)
        {
            foreach (PlistElement elem in skAdNetworkItemsArray.values)
            {
                try
                {
                    PlistElementDict elemInDict = elem.AsDict();
                    PlistElement value;
                    bool identifierExists = elemInDict.values.TryGetValue(KEY_SK_ADNETWORK_ID, out value);

                    if (identifierExists && value.AsString().Equals(id))
                    {
                        return true;
                    }
                }
#pragma warning disable 0168
                catch (Exception e)
#pragma warning restore 0168
                {
                    // Do nothing
                }
            }

            return false;
        }

        private static void NotifyBuildFailure(string message, bool showOpenSettingsButton = true)
        {
            string dialogTitle = "Google Mobile Ads";
            string dialogMessage = "Error: " + message;

            if (showOpenSettingsButton)
            {
                bool openSettings = EditorUtility.DisplayDialog(
                    dialogTitle, dialogMessage, "Open Settings", "Close");
                if (openSettings)
                {
                   //// GoogleMobileAdsSettingsEditor.OpenInspector();
                }
            }
            else
            {
                EditorUtility.DisplayDialog(dialogTitle, dialogMessage, "Close");
            }

            ThrowBuildException("[GoogleMobileAds] " + message);
        }

        private static void ThrowBuildException(string message)
        {
#if UNITY_2017_1_OR_NEWER
            throw new BuildPlayerWindow.BuildMethodException(message);
#else
        throw new OperationCanceledException(message);
#endif
        }

        /// <summary>
        /// 生成InfoPlist本地化文件到xcode上，发布出去的包就会带上我们的本地化文件并且生效
        /// </summary>
        /// <param name="proj">Proj.</param>
        private static void generateProjFile(PBXProject proj)
        {
            string targetGuid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
            var codesign = Debug.isDebugBuild ? "iOS Developer" : "iOS Distribution";
            proj.SetBuildProperty(targetGuid, "CODE_SIGN_IDENTITY", codesign);//非发布版本为Develper，发布版为Distribution (代码标签)
            proj.SetBuildProperty(targetGuid, "PROVISIONING_PROFILE_SPECIFIER", "Automatic");//自动

            //localizable  自动添加Xcode语言文件 (这个目录下有.lproj ，每一个.lproj文件夹下都有 .strings本地化文件
            var infoDirs = Directory.GetDirectories(Application.dataPath + "/IOSFile/lang/infoplist/");
            for (var i = 0; i < infoDirs.Length; ++i)
            {
                var files = Directory.GetFiles(infoDirs[i], "*.strings");
                if (files != null & files.Length > 0)
                {
                    string filepath = files[0];
                    //个人理解:
                    //参数1:文件群组名(本地化文件群组名为"InfoPlist.strings"
                    //参数2:本地化文件名(保存于文件群组的文件名(Xcode上表现的名称?);
                    //参数3:本地化文件绝对路径（unity工程上的?)
                    //proj.AddLocalization("InfoPlist.strings", "InfoPlists.strings", filepath);
                }
            }
        }
    }

    #endif
}
