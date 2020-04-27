#import "CustomLibrary.h"

extern "C" {
QRScannedCallback savedCallback;

    void registerObserver(QRScannedCallback _callback) {
        savedCallback = _callback;
        [NotificationObserver RegisterQRCallback:_callback];
    }
    void deRegisterObserver(){
        [NotificationObserver DeRegisterQRCallback];
    }
}
