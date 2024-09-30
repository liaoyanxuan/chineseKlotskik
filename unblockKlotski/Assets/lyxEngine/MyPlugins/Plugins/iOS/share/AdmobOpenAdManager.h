//
//  AdmobOpenAdManager.h
//  Unity-iPhone
//
//  Created by 廖衍旋 on 2021/8/7.
//
#import <GoogleMobileAds/GoogleMobileAds.h>

#ifndef AdmobOpenAdManager_h
#define AdmobOpenAdManager_h
//NSObject:基类；
//GDTRewardedVideoAdDelegate接口（协议）
@interface AdmobOpenAdManager:NSObject<GADFullScreenContentDelegate>
+(instancetype)sharedInstance; //静态属性

@property(strong, nonatomic) GADAppOpenAd* appOpenAd;
@property(weak, nonatomic) NSDate *loadTime;

- (void)requestAppOpenAd;
- (void)tryToPresentAd;

@end

#endif /* AdmobOpenAdManager_h */
