using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using aliyun_api_gateway_sdk;
using ConnUtils;
using InternetTime;
using sw.util;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class RealNameVerifiedScript : MonoBehaviour
{

    //成功验证后的回调
    public Action sussVerifedCallBack;

  [SerializeField]
	private InputField nameInput;

    [SerializeField]
    private InputField codeInput;

    [SerializeField]
    private Button confirmBtn;


    [SerializeField]
    private CountDownWidget countDownWidget;

    [SerializeField]
    private Text resultTxtTips;


    [SerializeField]
    private Button exitBtn;


  
    private int tryTimes = 0;
    public const string NameCodeVerifiedTryTimes = "NameCodeVerifiedTryTimes";
    public const string localnamecodefile = "lyxsudokuplnamecode";

    private VerifedNameCodeInfo verifedNameCodeInfo;


    private const int MAX_TRYTIME_A_DAY= 3; //事不过三

    private const int TimeNextDay = 1;
    private DateTime tomorrow;

    void Awake()
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {
        //同一天判断，新的一天次数清零
        //// 
        ////时间同步服务器，NTP 是网络时间协议（Network Time Protocol），它用来同步网络设备【如计算机、手机】的时间的协议
        ///
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            showResultTips("实名认证需要打开网络");
        }


        confirmBtn.onClick.AddListener(confirmClick);
        exitBtn.onClick.AddListener(exitGameClick);

        Invoke("resetDataInternal", 0.5f);

        //  Invoke("showPermission", 1f);
    }


    private void showPermission()
    {
        AndroidUtil.requireVerifyStoragePermissions();
    }

    private DateTime dateTimeNow;
    public bool isSussVerifed()
    {
        string verifedNameCodeInfoStr = getSaveDataStr();
        if (verifedNameCodeInfoStr.Equals(String.Empty))
        {
            return false;
        }
        else
        {
            verifedNameCodeInfo = JsonUtility.FromJson<VerifedNameCodeInfo>(verifedNameCodeInfoStr);
        }

        return verifedNameCodeInfo.isSussVerifed;
    }

    private void resetDataInternal()
    {
        resetData();
    }


    public void resetData(bool append=false)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            showResultTips("实名认证需要打开网络");
            append = true;
        }

        dateTimeNow = getSNTPNowTime();  //获取当前时间，服务器时间
        //从数据库中获取日期尝试次数
        initVerifedNameCodeInfo();

        DateTime verifiedTime=new DateTime(verifedNameCodeInfo.timestamp);
        if (verifiedTime.Day == dateTimeNow.Day && verifiedTime.Month== dateTimeNow.Month)  //表示今天
        {
           
        }
        else
        {
            //新的一天
            verifedNameCodeInfo.timestamp = dateTimeNow.Ticks;
            verifedNameCodeInfo.trytimes = 0; 
        }


        tryTimes = verifedNameCodeInfo.trytimes;
        if (tryTimes >= MAX_TRYTIME_A_DAY)
        {
            showResultTips("今日 " + MAX_TRYTIME_A_DAY + " 次实名认证次数已使用完，请明日再试", append);
            confirmBtn.interactable = false;
        }
       
    }

    //数据库获取验证信息
    private void initVerifedNameCodeInfo()
    {
        string verifedNameCodeInfoStr = getSaveDataStr();
        if (verifedNameCodeInfoStr.Equals(String.Empty))
        {
            verifedNameCodeInfo = new VerifedNameCodeInfo();
            verifedNameCodeInfo.timestamp = dateTimeNow.Ticks;
            verifedNameCodeInfo.trytimes = 0;
        }
        else
        {
            verifedNameCodeInfo = JsonUtility.FromJson<VerifedNameCodeInfo>(verifedNameCodeInfoStr);
        }

    }

    //获得数据 IO操作：try--catch
    private string getSaveDataStrPermission()
    {
        string filePath = Path.Combine(FileManager.GetExternalFilesDir(), localnamecodefile);

        Debug.Log("getSaveDataStr:"+ filePath);
        string resultJson = string.Empty;
        //从设备获取（external SD），设备获取不到再从内存（PlayerPrefs）获取
        resultJson = FileManager.readTextFromExternal(filePath);
        if (resultJson.Equals(string.Empty))
        {
            resultJson=PlayerPrefs.GetString(NameCodeVerifiedTryTimes, string.Empty);
        }
      
        return resultJson;
    }

    //保存数据 IO操作：try--catch
    private void setSaveDataStrPermission(string jsonStr)
    {
        string filePath = Path.Combine(FileManager.GetExternalFilesDir(), localnamecodefile);
        FileManager.writeTextToExternal(FileManager.GetExternalFilesDir(),filePath, jsonStr);

        PlayerPrefs.SetString(NameCodeVerifiedTryTimes, jsonStr);
    }


    //获得数据 IO操作：try--catch
    private string getSaveDataStr()
    {
        
       //（PlayerPrefs）获取
        string resultJson = PlayerPrefs.GetString(NameCodeVerifiedTryTimes, string.Empty);
        return resultJson;
    }

    //保存数据 IO操作：try--catch
    private void setSaveDataStr(string jsonStr)
    {
     
        PlayerPrefs.SetString(NameCodeVerifiedTryTimes, jsonStr);
    }

    //获取今日日期,时间同步接口--API
    private DateTime getSNTPNowTime()
    {
        // SNTPClient sntpClent = new SNTPClient("ntp.fudan.edu.cn");   //"time.nist.gov","time.windows.com"
        //  return sntpClent.Connect(false);
        DateTime networkTime = DateTime.Now;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
           
        }
        else
        {
            networkTime = NetTime.GetNetworkTime();
        }
       
        return networkTime;
    }




    private void showResultTips(string resultTipsStr,bool append=false)
    {
        if (append)
        {
            resultTxtTips.text = resultTxtTips.text+"\n" +resultTipsStr;
        }
        else
        {
            resultTxtTips.text = resultTipsStr;
        }
        
    }

    //校验失败的情况下，更新数据库
    private void addTryTimesAndCold(string tips)
    {

        tryTimes++;
        verifedNameCodeInfo.trytimes= tryTimes;

        string jsonStr=JsonUtility.ToJson(verifedNameCodeInfo);
      
        setSaveDataStr(jsonStr);

        tips = tips + "\n（今日剩余" + (MAX_TRYTIME_A_DAY - tryTimes).ToString() + "次认证次数）";
        showResultTips(tips);

        resetData(true);


        if (tryTimes < MAX_TRYTIME_A_DAY)
        {
            countDownWidget.startCountDown(tryTimes);
        }
    }


  
    void exitGameClick()
    {
        Application.Quit();
    }

    void confirmClick()
    {
        StartCoroutine(confirmClickCoroutine());
    }

        // Update is called once per frame
    IEnumerator confirmClickCoroutine()
    {

        Debug.Log("confirmClick");
        confirmBtn.interactable = false;

        //本地--正则校验
        //1.名字校验（中文字符）
        //2.身份证校验；
        //3.未成年校验。

        //认证过程需打开网络，请打开网络

        //本地通过，服务器校验。
        string idCode = codeInput.text.Trim();
        bool isIdCarValid=IdentityCardValidation.CheckIdCard(idCode);
        if (isIdCarValid == false)
        {
            showResultTips("身份证有误，请输入正确身份证");
            confirmBtn.interactable = true;
            yield break;
        }

        string realName = nameInput.text.Trim();
        bool isNAmeValide = IdentityCardValidation.CheckName(realName);
        if (isNAmeValide == false)
        {
            showResultTips("姓名有误，请输入正确姓名");
            confirmBtn.interactable = true;
            yield break;
        }


        if (isAdult(idCode) ==false)
        {
            showResultTips("暂时谢绝未成年人进入");
            confirmBtn.interactable = true;
            yield break;
        }

        ////   addTryTimesAndCold("姓名与身份证号不匹配");
        ////   return;

        // string resultCode = '{ "error_code":0,"reason":"成功","result":{ "realname":"廖**","idcard":"440111************","isok":true,"IdCardInfor":{ "province":"广东省","city":"广州市","district":"白云区","area":"广东省广州市白云区","sex":"男","birthday":"1982-5-2"} } }';

        // NameCodeResult jsonresult =IDCodeAndNameVerifeidApi.doGet();

        showResultTips("认证中...,请耐心等待");

        yield return null;

        NameCodeResult jsonresult = IDCodeAndNameVerifeidApi.doGet(idCode, realName);
      
        if (jsonresult == null)
        {
            showResultTips("认证请求失败，请检查网络后重试或稍后再试") ;
            confirmBtn.interactable = true;
            yield break;
        }


        //////////////////////// HttpStatusCode.OK //////////////////////////////////////
        if (Convert.ToInt32(jsonresult.error_code) != 0)
        {
            //1分钟后重试（校验不通过，冻结1分钟）；总共3次校验机会
            addTryTimesAndCold("无法查询到该姓名与身份证号");
            yield break;
        }


        if (jsonresult.result.isok == false)
        {
              //1分钟后重试（校验不通过，冻结1分钟）；总共3次校验机会(本地存储校验时间戳)
            addTryTimesAndCold("姓名与身份证号不匹配");
            yield break;
        }

        Toast.instance.ShowMessage("实名匹配成功");   //检查是否未成年人（校验不通过，冻结1分钟）；未成年提示谢绝，算一次校验(本地存储校验时间戳)，总共3次校验不匹配机会
        //客户端标记：实名认证次数，终端次数
        verifedNameCodeInfo.isSussVerifed = true;
        string jsonStr = JsonUtility.ToJson(verifedNameCodeInfo);
        setSaveDataStr(jsonStr);
        this.gameObject.SetActive(false);
       
        if (sussVerifedCallBack != null)
        {
            sussVerifedCallBack();
        }

        yield return null;
    }

    //是否未成年人
    private bool isAdult(string idCard)
    {
        if (GetAgeByIdCard(idCard) >= 18)
        {
            return true;
        }

        return false;
    }

    public  bool GetGenderByIdCard(string idCard)
    {
        if (string.IsNullOrWhiteSpace(idCard))
        {
            return false;
        }
        return Convert.ToBoolean(int.Parse(idCard.Substring(16, 1)) % 2);
    }

    public  int GetAgeByIdCard(string idCard)
    {
        int age = 0;
        if (!string.IsNullOrWhiteSpace(idCard))
        {
            var subStr = string.Empty;
            if (idCard.Length == 18)
            {
                subStr = idCard.Substring(6, 8).Insert(4, "-").Insert(7, "-");
            }
            else if (idCard.Length == 15)
            {
                subStr = ("19" + idCard.Substring(6, 6)).Insert(4, "-").Insert(7, "-");
            }
            TimeSpan ts = dateTimeNow.Subtract(Convert.ToDateTime(subStr));
            age = ts.Days / 365;
        }
        return age;
    }


}

[Serializable]
public class NameCodeResult
{
    public int error_code;
    public string reason;
    public Result result;
}

[Serializable]
public class Result
{
    public string realname;
    public string idcard;
    public bool isok;
    public IdCardInfor IdCardInfor;
}
//{ "province":"广东省","city":"广州市","district":"白云区","area":"广东省广州市白云区","sex":"男","birthday":"1982-5-2"}
[Serializable]
public class IdCardInfor
{
    public string province;
    public string city;
    public string district;
    public string area;
    public string sex;
    public string birthday;
}


/////////////////////////////////////////////////
///[Serializable]
public class VerifedNameCodeInfo
{
    public long timestamp;
    public int trytimes;
    public bool isSussVerifed = false;
}
