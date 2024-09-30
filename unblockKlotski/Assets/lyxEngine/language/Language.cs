using sw.util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Language
{
    public const string EMPTY = "empty";

    public const string gameCenterOpenTips = "gameCenterOpenTips";
    public static string isloadingAdvertisement = "isloadingAdvertisement";
    public static string advertisementLoadFail = "advertisementLoadFail";
    public static string advertisementOpenFail = "advertisementOpenFail";
    public static string shareToSocailText = "shareToSocailText";


    public const string SimpleChinese = "SimpleChinese";
    
    public const string English = "English";    
    //选择自已需要的本地语言    
    public string language = English;
    private Dictionary<string, string> languageDic = new Dictionary<string, string>();    
    //单例模式    
    private static Language _instance;    
    public static Language getInstance()    
    {    
           
            if (_instance == null)    
            {    
                _instance = new Language();    
            }    
            return _instance;    
        
    }    

     /// <summary>    
    /// 读取配置文件，将文件信息保存到字典里    
    /// </summary>    
    public Language()    
    {
        CheckAndSetUserLanguage();

    }

    


    public string GetValueByEnum(LanguageEnum languageEnum)
    {
        string languageKey = Enum.GetName(typeof(LanguageEnum), languageEnum);

        return GetValue(languageKey);
    }

    /// <summary>    
    /// 获取value    
    /// </summary>    
    /// <param name="key"></param>    
    /// <returns></returns>    
    public string GetValue(string key)    
    {     
        if (languageDic.ContainsKey(key) == false)    
        {    
            return key;    
         }
        string value = key;
        languageDic.TryGetValue(key, out value);    
        return value;
    }    

    public static string getSolutionStepName(string stepName)
    {
        string stepName2 = stepName.Replace("SolutionStep.", "");
        string stepName3=Language.getInstance().GetValue(stepName2);
        if (stepName3.Equals(Language.EMPTY))
        {
            stepName3 = stepName2;
        }
        return stepName3;
    }

    public static string getSolutionStep(string parastr)
    {
        parastr = parastr.Replace("SolutionStep.", "");
        return parastr;
    }

    public static string getDifficultyLevelName(string levelName)
    {
        return levelName;
    }

    public static string getAlert(string alertContent)
    {
        return alertContent;
    }


    /**
       获得校正后的系统语言
       因为ios9调整了系统语言，简体中文和繁体中文在ios9上Application.systemLanguage获取的值都是Chinese
       无法区分简体中文和繁体中文

       ios 7
       简体                zh-Hans
       繁体                zh-Hant

       ios 8.1
       简体中文            zh-Hans                ChineseSimplified
       繁体中文(香港)        zh-HK                ChineseTraditional
       繁体中文(台湾)        zh-Hant                ChineseTraditional

       ios 9.1
       简体中文            zh-Hans-CN            Chinese
       繁体中文(香港)        zh-HK                ChineseTraditional
       繁体中文(台湾)        zh-TW                Chinese
   **/
    private  void CheckAndSetUserLanguage() 
    {
        DebugEx.Log("Application.systemLanguage:" + Application.systemLanguage);
        switch(Application.systemLanguage) 
        {
            case SystemLanguage.Chinese:
                #if UNITY_IOS
                    string name = iOSUtil.getCurIOSLang();  
                    if (name.StartsWith("zh-Hans")) {
                    language = SimpleChinese;
                   
                    break;
                    }
                #endif
				language = English;

                break;
            case SystemLanguage.ChineseSimplified:
                language = SimpleChinese;
                break;
			case SystemLanguage.ChineseTraditional:
				language = English;
				break;
			case SystemLanguage.English:
				language = English;
				break;
			default:
				language = English;
				break;

        }


#if UNITY_EDITOR
        //language = SimpleChinese;
        language = English;
#endif

    }

}

public enum LanguageEnum
{
    empty,
    SQUARE,
    TRIANGLE,
    HEXAGON,
    BEGINNER,
    EASY,
    MEDIUM,
    HARD,
    IMPOSSIBLE,
}
