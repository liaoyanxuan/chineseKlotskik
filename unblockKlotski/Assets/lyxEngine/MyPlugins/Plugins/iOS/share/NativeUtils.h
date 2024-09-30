//
//  NativeUtils.h
//  Unity-iPhone
//
//  Created by 廖衍旋 on 2022/7/23.
//

#ifndef NativeUtils_h
#define NativeUtils_h



@interface NativeUtils : NSObject
+ (instancetype)sharedInstance;

@property(strong, nonatomic) NSString* bannerEcpm;
@property(strong, nonatomic) NSString* rewardEcpm;
@property(strong, nonatomic) NSString* interEcpm;


@property NSInteger bannerHeight;

-(void) shareText: (NSString*) body withURL: (NSString*) urlString withImage:(NSString*) imageDataString withSubject: (NSString*) subject;
-(void) ShareWeb: (NSString*) body withURL: (NSString*) urlString withImage:(NSString*) imageDataString withSubject: (NSString*) subject;
-(BOOL) isStringValideBase64:(NSString*)string;

-(void)printLogs:(NSString *)format, ...;

@end

#endif /* NativeUtils_h */
