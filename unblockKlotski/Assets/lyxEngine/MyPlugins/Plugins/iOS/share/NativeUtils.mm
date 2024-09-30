//
//  NativeUtils.mm
//
#import <Foundation/Foundation.h>
#import <MessageUI/MessageUI.h>

#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <AdSupport/AdSupport.h>

#import <StoreKit/StoreKit.h>
//Hide error of undef function which is available in unity generated xCode project
//#define FMDEBUG
//#ifdef FMDEBUG
//inline UIViewController* UnityGetGLViewController()
//{
//    return nil;
//}
//#endif
#import "NativeUtils.h"
#import "AdmobOpenAdManager.h"


@implementation NativeUtils

static NativeUtils * _sharedInstance;

+ (instancetype)sharedInstance {
    
    static NativeUtils *_sharedInstance;
    static dispatch_once_t token;
    dispatch_once(&token,^{
        _sharedInstance=[[NativeUtils alloc] init];
       
    });
    return _sharedInstance;
}


+(NSString*) charToNSString: (char*)text {
    return text ? [[NSString alloc] initWithUTF8String:text] : [[NSString alloc] initWithUTF8String:""];
}

- (void)printLogs:(NSString *)format, ...
{
    BOOL isDebug=false;
    if(isDebug)
    {
        va_list args;
        va_start(args, format);
        
        NSString *formattedString = [[NSString alloc] initWithFormat:format arguments:args];
        NSLog(@"%@", formattedString);
        
        va_end(args);
        
    }
}


//分享图片 分享文字
-(void) SocialSharing: (NSString*) body withURL: (NSString*) urlString withImage:(NSString*) imageDataString withSubject: (NSString*) subject {
    
    
    UIImage *image = NULL;
    if([self isStringValideBase64:imageDataString]){
        NSData *imageData = [[NSData alloc] initWithBase64EncodedString:imageDataString options:0];
        image = [[UIImage alloc] initWithData:imageData];
    }
    else{
        NSData *dataImage = [NSData dataWithContentsOfFile:imageDataString];
        image = [[UIImage alloc] initWithData:dataImage];
    }
    
    NSData *imageData2=UIImagePNGRepresentation(image);
    image = [[UIImage alloc] initWithData:imageData2];
    //分享的url
    NSURL *urlToShare = [NSURL URLWithString:urlString];
    
    //NSMutableArray *sharingItems=@[body,image,urlToShare];
    NSMutableArray *sharingItems=@[body,image];
    
    UIActivityViewController *activityViewController = [[UIActivityViewController alloc] initWithActivityItems:sharingItems applicationActivities:nil];
    
    activityViewController.popoverPresentationController.sourceView = UnityGetGLViewController().view;
    activityViewController.popoverPresentationController.sourceRect = CGRectMake(UnityGetGLViewController().view.frame.size.width/2, UnityGetGLViewController().view.frame.size.height/4, 0, 0);
    
   
    [UnityGetGLViewController() presentViewController:activityViewController animated:YES completion:nil];
      
}


-(void) CopyTextToClipboard:(NSString*) targetString
{

    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    pasteboard.string = targetString;
}

//所有参数都必须有分享网页链接
-(void) ShareWeb: (NSString*) body withURL: (NSString*) urlString withImage:(NSString*) imageDataString withSubject: (NSString*) subject  {

    if(body==NULL||body.length<0||urlString==NULL||urlString.length<0||imageDataString==NULL||imageDataString.length<0||subject==NULL||subject.length<0){
        return;
    }
    NSURL *urlToShare=[NSURL URLWithString:urlString];
    UIImage *image = NULL;
    if([self isStringValideBase64:imageDataString]){
        NSData *imageData = [[NSData alloc] initWithBase64EncodedString:imageDataString options:0];
        image = [[UIImage alloc] initWithData:imageData];
    }
    else{
        NSData *dataImage = [NSData dataWithContentsOfFile:imageDataString];
        image = [[UIImage alloc] initWithData:dataImage];
    }
    NSArray *shareItems=@[body,urlToShare,image];

    UIActivityViewController *activityViewController = [[UIActivityViewController alloc] initWithActivityItems:shareItems applicationActivities:nil];
    activityViewController.popoverPresentationController.sourceView = UnityGetGLViewController().view;
    activityViewController.popoverPresentationController.sourceRect = CGRectMake(UnityGetGLViewController().view.frame.size.width/2, UnityGetGLViewController().view.frame.size.height/4, 0, 0);
    
    if(subject && subject.length > 0)
    {
        [activityViewController setValue:subject forKey:@"subject"];
    }
    
    [UnityGetGLViewController() presentViewController:activityViewController animated:YES completion:nil];
}

-(BOOL) isStringValideBase64:(NSString*)string{
    
    NSString *regExPattern = @"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$";
    
    NSRegularExpression *regEx = [[NSRegularExpression alloc] initWithPattern:regExPattern options:NSRegularExpressionCaseInsensitive error:nil];
    NSUInteger regExMatches = [regEx numberOfMatchesInString:string options:0 range:NSMakeRange(0, [string length])];
    return regExMatches != 0;
}


//extern char* cStringCopy(const char* string);

@end
// Helper method to create C string copy
char* MakeStringCopy (const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

extern "C"
{
    void _socialSharing(char* body, char* url, char* imageDataString, char* subject) {
        
        [[NativeUtils sharedInstance] SocialSharing:[NativeUtils charToNSString:body] withURL:[NativeUtils charToNSString:url] withImage:[NativeUtils charToNSString:imageDataString] withSubject:[NativeUtils charToNSString:subject]];
    }
     void _shareWeb(char* body, char* url, char* imageDataString, char* subject) {
        
        [[NativeUtils sharedInstance] ShareWeb:[NativeUtils charToNSString:body] withURL:[NativeUtils charToNSString:url] withImage:[NativeUtils charToNSString:imageDataString] withSubject:[NativeUtils charToNSString:subject]];
    }
    

    void _copyTextToClipboard(char* targetString)
    {
        [[NativeUtils sharedInstance] CopyTextToClipboard:[NativeUtils charToNSString:targetString]];
    }

        // ios手机的当前语言 "en"、“zh"、“zh-Hans"、"zh-Hant"
   const char* _curiOSLang()
   {
        NSArray *languages = [NSLocale preferredLanguages];  
        NSString *currentLanguage = [languages objectAtIndex:0]; 
        //return cStringCopy([currentLanguage UTF8String]);
        return MakeStringCopy([currentLanguage UTF8String]);
   }
   
    const char*  _specicalDeviceUUID()
    {
        
       
        //用于特定设备的测试，默认为空
        //NSString *deviceUUID=@"eda3ff2adbd70ee7d609b91a7b19962d";
        //NSString *deviceUUID=@"a4cfe0ce54010c177a5aaaaa942c5994";
        NSString *deviceUUID = NULL;
        return  MakeStringCopy([deviceUUID UTF8String]);
   }

    const char*  _umpDeviceIdentifier()
    {
        
        [[NativeUtils sharedInstance] printLogs:@"_umpDeviceIdentifier"];
        //用于特定设备的测试，默认为空
        //NSString *deviceUUID=@"eda3ff2adbd70ee7d609b91a7b19962d";
        //NSString *deviceUUID=@"a4cfe0ce54010c177a5aaaaa942c5994";
        NSString *deviceUUID = NULL;
        return  MakeStringCopy([deviceUUID UTF8String]);
    }

    int _isChineseCNY()
    {
        NSLocale *local = [NSLocale currentLocale];
        if (@available(iOS 17.0, *)) {  //local.regionCode 在iOS17才生效
           // [[NativeUtils sharedInstance] printLogs:@"currencyCode:%@,%@,%@",local.currencyCode,local.countryCode,local.regionCode];
        } else {
            // Fallback on earlier versions
        }
        // 地区货币代码。注意，currencyCode只能在iOS   //CNY,CN,CN;USD,US,US;JPY,JP,JP;KRW,KR,KR 10及以上的版本可以使用，所以低于这个版本的系统上，会crash。  英国：GBP,GB,GB
        if (![local.currencyCode isEqualToString:@"CNY"])
        {
            return 0;
        }else
        {
            return 1;
        }
    }

    int _isNeedGDPR()
    {
        NSLocale *local = [NSLocale currentLocale];
        //[[NativeUtils sharedInstance] printLogs:@"currencyCode:%@,%@,%@",local.currencyCode,local.countryCode,local.regionCode];
        // 地区货币代码。注意，currencyCode只能在iOS   //CNY,CN,CN;USD,US,US;JPY,JP,JP;KRW,KR,KR 10及以上的版本可以使用，所以低于这个版本的系统上，会crash。  英国：GBP,GB,GB
        //中美日韩不需要
        if ([local.countryCode isEqualToString:@"CN"] ||
            [local.countryCode isEqualToString:@"US"] ||
            [local.countryCode isEqualToString:@"JP"] ||
            [local.countryCode isEqualToString:@"KR"])
        {
            [[NativeUtils sharedInstance] printLogs:@"currencyCode:Not-Need-GDPR"];
            return 0;
        }else
        {
            [[NativeUtils sharedInstance] printLogs:@"currencyCode:NeedGDPR"];
            return 1;
        }
    }

   
    int _getScaleFactor()
    {
        NSInteger scaleFactor=(NSInteger)floor([UIScreen mainScreen].scale);
        
        return (int)scaleFactor;
    }


   int _isRealAd()
   {   //默认真实
       //0表示是测试Ad；1表示真实Ad
       return 1;
   }

    int _IsDebugTest()
   {   
      
       return 0;
   }

   int _IsDebugLayoutTest()
   {   
       return 0;
   }

    int _IsFPSDebugTest()
    {
       return 0;
    }
    
    //应用内评价
    void _inAppEvaluate()
    {
        if([SKStoreReviewController respondsToSelector:@selector(requestReview)]) {
            // iOS 10.3 以后
           NSLog(@"inAppEvaluate_InVoke1");
            [SKStoreReviewController requestReview];
        } 
   
    }

    void _loadOpenAd()
   {
        NSLog(@"_loadOpenAd");
        [[AdmobOpenAdManager sharedInstance] requestAppOpenAd];
   }

    void _presentOpenAd()
   {
        NSLog(@"_presentOpenAd");
        [[AdmobOpenAdManager sharedInstance] tryToPresentAd];
   }





void _requestIDFA()
{
    if (@available(iOS 14.0, *)) {
        
          NSLog(@"requestIDFA_ATTrackingManager");
          [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
            // Tracking authorization completed. Start loading ads here.
            // [self loadAd];
              NSLog(@"requestIDFA_ATTrackingManager-status");
              // 处理授权请求的结果
               switch (status) {
                   case ATTrackingManagerAuthorizationStatusAuthorized:
                       NSLog(@"User granted tracking authorization");
                       // 在这里执行你的跟踪用户行为数据的代码
                       break;
                       
                   case ATTrackingManagerAuthorizationStatusDenied:
                       NSLog(@"User denied tracking authorization");
                       // 在这里处理用户拒绝授权的情况
                       break;
                       
                   case ATTrackingManagerAuthorizationStatusNotDetermined:
                       NSLog(@"Tracking authorization not determined yet");
                       // 在这里处理用户尚未作出选择的情况
                       break;
                       
                   case ATTrackingManagerAuthorizationStatusRestricted:
                       NSLog(@"Tracking authorization restricted");
                       // 在这里处理因设备受限而无法请求授权的情况
                       break;
               }
          
         //SendMessage: object HomeScreen not found!;HomeScreen会inactive
          UnitySendMessage("MainCamera", "requestIDFAResult", "1");
          
      }];
            
    }else
    {
      //SendMessage: object HomeScreen not found!;HomeScreen会inactive
        UnitySendMessage("MainCamera", "requestIDFAResult", "1");
    }
    
}

    //跳转到其他游戏页面
    void _goToProductPage(char* appid)
    {
        
       // [[NativeUtils sharedInstance] openAppWithIdentifier:[NativeUtils charToNSString:appid]];
        // AppId
       NSString *appID = [NativeUtils charToNSString:appid];

        // 应用地址 https://www.jianshu.com/p/010ba9153926
        NSString *appStr = [NSString stringWithFormat:@"https://itunes.apple.com/app/id%@", appID];
        
         NSLog(@"appStr:%@",appStr);
        // 跳转
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:appStr] options:@{} completionHandler:nil];

        
  
  
    }


    void _setStatusBarLight(int isBlack)
    {
        if(isBlack==1)
        {
            [UIApplication sharedApplication].statusBarStyle = UIStatusBarStyleDefault; //黑色
        }else
        {
            [UIApplication sharedApplication].statusBarStyle = UIStatusBarStyleLightContent; //白色
        }
    }

    void _setBannerHeight(int _bannerHeight)
    {
        NSLog(@"_setbannerHeight:%d",_bannerHeight);
        [NativeUtils sharedInstance].bannerHeight=_bannerHeight;
    }


    //********TCenAd-start
    int _isTCRewardAdReady()
    {
       
        return 0;
    }

    void _createAndLoadTCRewardVideoAd()
    {
         
    }
    
    void _showTCRewardVideoAd()
    {
       
    }
  //********TCenAd-end
}




 



