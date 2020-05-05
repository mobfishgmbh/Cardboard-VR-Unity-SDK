#import "NotificationObserver.h"
#define kQRCodeScannedNotification @"QRCodeScanned"

@implementation NotificationObserver

static QRScannedCallback qrScannedCallback;
+(QRScannedCallback) qrScannedCallback {
    @synchronized (self) {
        return qrScannedCallback;
    }
}
+(void) setQrScannedCallback: (nullable QRScannedCallback) _callback{
    @synchronized (self) {
        qrScannedCallback = _callback;
    }
}

+(void) RegisterQRCallback:(nullable QRScannedCallback) _callback{
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(QRObserver:) name:kQRCodeScannedNotification object:nil];
    NotificationObserver.qrScannedCallback = _callback;
}
+(void) DeRegisterQRCallback{
    [[NSNotificationCenter defaultCenter] removeObserver:self];
    NotificationObserver.qrScannedCallback = nil;
}
+(void) QRObserver: (NSNotification *) name{
    NSLog(@"QRCodeScanned Received!!");
    if(NotificationObserver.qrScannedCallback != nil){
        NotificationObserver.qrScannedCallback(YES);
    }
}
@end
