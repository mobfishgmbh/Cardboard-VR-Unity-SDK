#import "CustomLibrary.h"

extern "C" {
    void registerObserver(QRScannedCallback _callback) {
        [NotificationObserver RegisterQRCallback:_callback];
    }
    void deRegisterObserver(){
        [NotificationObserver DeRegisterQRCallback];
    }

void loadDeviceParamertersFromURL(const char* url, QRScannedCallback _callback){
   [CardboardDeviceParamsHelper resolveAndUpdateViewerProfileFromURL:[NSURL URLWithString:[NSString stringWithUTF8String:url]] withCompletion:^(BOOL success, NSError *error) {
         _callback(success);
   }];
}

}
