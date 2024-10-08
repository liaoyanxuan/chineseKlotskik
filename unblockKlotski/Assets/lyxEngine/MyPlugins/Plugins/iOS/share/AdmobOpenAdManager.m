//
//  AdmobOpenAdManager.m
//  Unity-iPhone
//
//  Created by 廖衍旋 on 2021/8/7.
//

#import <Foundation/Foundation.h>
#import "AdmobOpenAdManager.h"
#import <GoogleMobileAds/GoogleMobileAds.h>

@implementation AdmobOpenAdManager

+(instancetype)sharedInstance
{
    static AdmobOpenAdManager *instance;
    static dispatch_once_t token;
    dispatch_once(&token,^{
        instance=[[AdmobOpenAdManager alloc] init];
       
    });
    return instance;
}

- (void)requestAppOpenAd {

}

- (void)tryToPresentAd {
  if (self.appOpenAd && [self wasLoadTimeLessThanNHoursAgo:4]) {
    UIViewController *rootController = UnityGetGLViewController();
    UIViewController *presentedViewController=rootController.presentedViewController;
    //  [self.appOpenAd presentFromRootViewController:presentedViewController];
      
      if(presentedViewController==nil)
      {
          [self.appOpenAd presentFromRootViewController:rootController];
      }
      else //if([presentedViewController isKindOfClass:[UnityViewControllerBase class]] )
      {
         
          
          NSString *presentedclassName=NSStringFromClass([presentedViewController class]);
          
          NSLog(@"class name>> %@",presentedclassName);
          if([presentedclassName isEqualToString:@"UnityPortraitOnlyViewController"])
          {
              [presentedViewController  dismissViewControllerAnimated:NO completion:^ {
                 
                  [self.appOpenAd presentFromRootViewController:rootController];
              }];
          }

      }
      
  } else {
    // If you don't have an ad ready, request one.
    [self requestAppOpenAd];
  }
}


#pragma mark - GADFullScreenContentDelegate

/// Tells the delegate that the ad failed to present full screen content.
- (void)ad:(nonnull id<GADFullScreenPresentingAd>)ad
    didFailToPresentFullScreenContentWithError:(nonnull NSError *)error {
  NSLog(@"didFailToPresentFullScreenContentWithError: %@", error);
  [self requestAppOpenAd];

}

/// Tells the delegate that the ad presented full screen content.
- (void)adDidPresentFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
  NSLog(@"adDidPresentFullScreenContent");
}

/// Tells the delegate that the ad dismissed full screen content.
- (void)adDidDismissFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
  NSLog(@"adDidDismissFullScreenContent");
  [self requestAppOpenAd];
}

- (BOOL)wasLoadTimeLessThanNHoursAgo:(int)n {
  NSDate *now = [NSDate date];
  NSTimeInterval timeIntervalBetweenNowAndLoadTime = [now timeIntervalSinceDate:self.loadTime];
  double secondsPerHour = 3600.0;
  double intervalInHours = timeIntervalBetweenNowAndLoadTime / secondsPerHour;
  return intervalInHours < n;
}


@end
