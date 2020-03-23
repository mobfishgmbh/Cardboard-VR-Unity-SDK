using System;
using System.Runtime.InteropServices;

namespace MobfishCardboard
{
    public static class CardboardQrCode
    {
        private static IntPtr _encodedDeviceParams;

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardQrCode_scanQrCodeAndSaveDeviceParams();

        //todo is this correct?
        //https://developers.google.com/cardboard/reference/c/group/qrcode-scanner#cardboardqrcode_getsaveddeviceparams
        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardQrCode_getSavedDeviceParams(ref IntPtr encoded_device_params, ref int size);

        public static void StartScanQrCode()
        {
            CardboardQrCode_scanQrCodeAndSaveDeviceParams();
        }
    }
}