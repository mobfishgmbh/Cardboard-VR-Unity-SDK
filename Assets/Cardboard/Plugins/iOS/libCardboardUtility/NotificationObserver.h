#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN
typedef void (*QRScannedCallback)(bool);

@interface NotificationObserver : NSObject

+(QRScannedCallback) qrScannedCallback;
+(void) setQrScannedCallback: (nullable QRScannedCallback) _callback;

+(void) RegisterQRCallback: (nullable QRScannedCallback) _callback;
+(void) DeRegisterQRCallback;
+(void) QRObserver: (NSNotification *) name;

@end

NS_ASSUME_NONNULL_END
