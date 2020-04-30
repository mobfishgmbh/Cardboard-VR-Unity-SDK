#import "NotificationObserver.h"
#import "device_params_helper.h"
#ifdef __cplusplus
extern "C" {
#endif

void registerObserver(QRScannedCallback _callback);
void deRegisterObserver();

void loadDeviceParamertersFromURL(const char* url, QRScannedCallback _callback);
 
#ifdef __cplusplus
}
#endif
