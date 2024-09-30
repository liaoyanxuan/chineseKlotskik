
using Hyperbyte;


public class TestManager :  Singleton<TestManager>
{
#if UNITY_EDITOR
    public  bool isGDPR=true;  //GDPR展示
    public  bool isChianCNY=false;     //中国大陆地区
    public bool isShowNewGuide = true;  //展示新手引导
    public bool isDebugTest = true;  //展示新手引导
    public bool showBanner = true;  //展示Banner
    public bool getRewardDirect = true; //直接获取Reward
    public bool isNeedNameVerified = true; //是否需要实名
#endif
}
