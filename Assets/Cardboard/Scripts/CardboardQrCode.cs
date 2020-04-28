#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
#define NATIVE_PLUGIN_EXIST
#endif

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MobfishCardboard
{
    public static class CardboardQrCode
    {
        private static IntPtr _encodedDeviceParams;
        private static int _paramsSize;
        private static byte[] encodedBytes;
        private static DeviceParams decodedParams;

        private delegate void QRCodeScannedCallbackType();

        #if NATIVE_PLUGIN_EXIST
        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardQrCode_scanQrCodeAndSaveDeviceParams();

        //https://developers.google.com/cardboard/reference/c/group/qrcode-scanner#cardboardqrcode_getsaveddeviceparams
        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardQrCode_getSavedDeviceParams(ref IntPtr encoded_device_params, ref int size);

        //New method for libCardboardUtility, iOS only.
        #if UNITY_IOS

        [DllImport(CardboardUtility.DLLName)]
        private static extern void registerObserver(QRCodeScannedCallbackType _callback);
        [DllImport(CardboardUtility.DLLName)]
        private static extern void deRegisterObserver();

        #endif

        #else

        private static void CardboardQrCode_scanQrCodeAndSaveDeviceParams()
        {
        }

        private static void CardboardQrCode_getSavedDeviceParams(ref IntPtr encoded_device_params, ref int size)
        {
            size = 0;
        }

        #endif

        [AOT.MonoPInvokeCallback(typeof(QRCodeScannedCallbackType))]
        private static void QRCodeScannedCallback()
        {
            Debug.Log("QRCodeScannedCallback received in Unity!!");
            CardboardManager.RefreshParameters();
        }

        public static void StartScanQrCode()
        {
            CardboardQrCode_scanQrCodeAndSaveDeviceParams();
        }

        public static void RegisterObserver()
        {
            #if NATIVE_PLUGIN_EXIST && UNITY_IOS
            registerObserver(QRCodeScannedCallback);
            #endif
        }

        public static void DeRegisterObserver()
        {
            #if NATIVE_PLUGIN_EXIST && UNITY_IOS
            deRegisterObserver();
            #endif
        }

        public static void RetrieveDeviceParam()
        {
            CardboardQrCode_getSavedDeviceParams(ref _encodedDeviceParams, ref _paramsSize);

            Debug.Log("Feature Test RetrieveDeviceParam size=" + _paramsSize);
            encodedBytes = ReadByteArray(_encodedDeviceParams, _paramsSize);

            if (_paramsSize > 0)
                decodedParams = DeviceParams.Parser.ParseFrom(encodedBytes);

            Debug.LogFormat("Feature Test RetrieveDeviceParam params length={0}, byte=\r\n {1}",
                encodedBytes.Length, string.Join(" , ", encodedBytes));
            Debug.LogFormat("Feature Test decode device params: \r\n{0}",
                CardboardUtility.DeviceParamsToString(decodedParams));
        }

        public static (IntPtr, int) GetDeviceParamsPointer()
        {
            return (_encodedDeviceParams, _paramsSize);
        }

        // public static (byte[], int) GetDeviceParamsByte()
        // {
        //     return (encodedBytes, _paramsSize);
        // }

        public static DeviceParams GetDecodedDeviceParams()
        {
            return decodedParams;
        }

        private static byte[] ReadByteArray(IntPtr pointer, int size)
        {
            var result = new byte[size];
            if (size > 0)
                Marshal.Copy(pointer, result, 0, size);
            return result;
        }

    }
}