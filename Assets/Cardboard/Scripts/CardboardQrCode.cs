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

        private delegate void QRCodeScannedCallbackType(bool success);

        #if NATIVE_PLUGIN_EXIST
        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardQrCode_scanQrCodeAndSaveDeviceParams();

        //https://developers.google.com/cardboard/reference/c/group/qrcode-scanner#cardboardqrcode_getsaveddeviceparams
        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardQrCode_getSavedDeviceParams(ref IntPtr encoded_device_params, ref int size);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardQrCode_getCardboardV1DeviceParams(ref IntPtr encoded_device_params,
            ref int size);

        [DllImport(CardboardUtility.DLLName)]
        private static extern int CardboardQrCode_getQrCodeScanCount();

        //New method for libCardboardUtility, iOS only.
        #if UNITY_IOS
        [DllImport(CardboardUtility.DLLName)]
        private static extern void registerObserver(QRCodeScannedCallbackType _callback);
        [DllImport(CardboardUtility.DLLName)]
        private static extern void deRegisterObserver();
        [DllImport(CardboardUtility.DLLName)]
        private static extern void loadDeviceParamertersFromURL(string url, QRCodeScannedCallbackType _callback);

        #endif

        #if UNITY_ANDROID

        private static string CARDBOARD_PARAMS_UTIL_CLASS = "com.google.cardboard.sdk.qrcode.CardboardParamsUtils";
        private static string CARDBOARD_PARAMS_METHOD_SETFOLDER = "setNewFileParent";
        private static string CARDBOARD_PARAMS_METHOD_SETPROFILE = "writeDeviceParamsToStorage";

        // Called in CardboardAndroidInitialization
        public static void SetAndroidQRCodeLocation()
        {
            AndroidJavaClass utilClass = new AndroidJavaClass(CARDBOARD_PARAMS_UTIL_CLASS);
            utilClass.CallStatic(CARDBOARD_PARAMS_METHOD_SETFOLDER, Application.persistentDataPath);
        }

        public static void SetDeviceParamertersFromURL(string url)
        {
            AndroidJavaClass utilClass = new AndroidJavaClass(CARDBOARD_PARAMS_UTIL_CLASS);
            utilClass.CallStatic<bool>(CARDBOARD_PARAMS_METHOD_SETPROFILE, url);
        }

        #endif

        #else
        private static void CardboardQrCode_scanQrCodeAndSaveDeviceParams()
        {
        }

        private static void CardboardQrCode_getSavedDeviceParams(ref IntPtr encoded_device_params, ref int size)
        {
            size = 0;
        }

        private static void CardboardQrCode_getCardboardV1DeviceParams(ref IntPtr encoded_device_params, ref int size)
        {
            size = 0;
        }

        private static int CardboardQrCode_getQrCodeScanCount()
        {
            return 1;
        }

        #endif

        [AOT.MonoPInvokeCallback(typeof(QRCodeScannedCallbackType))]
        private static void QRCodeScannedCallback(bool success)
        {
            Debug.Log("QRCodeScannedCallback received in Unity!!");
            CardboardManager.RefreshParameters();

            if (GetQRCodeScanCount() > 0 && !PlayerPrefs.HasKey(CardboardUtility.KEY_CARDBOARD_CAMERA_SCANNED))
            {
                PlayerPrefs.SetInt(CardboardUtility.KEY_CARDBOARD_CAMERA_SCANNED, 1);
                PlayerPrefs.Save();
            }
        }

        private static void AppFocusChanged(bool hasFocus)
        {
            if (hasFocus)
                QRCodeScannedCallback(true);
        }

        [AOT.MonoPInvokeCallback(typeof(QRCodeScannedCallbackType))]
        private static void LoadDeviceParamCallback(bool success)
        {
            Debug.Log("LoadDeviceParamCallback called in Unity");
            CardboardManager.RefreshParameters();
        }

        public static void StartScanQrCode()
        {
            Debug.Log("CardboardQrCode.StartScanQrCode() called");
            GetQRCodeScanCount();
            CardboardQrCode_scanQrCodeAndSaveDeviceParams();
            Debug.Log("CardboardQrCode.StartScanQrCode() call finish");
        }

        public static void RegisterObserver()
        {
            #if NATIVE_PLUGIN_EXIST
            #if UNITY_IOS
            registerObserver(QRCodeScannedCallback);
            #else
            Application.focusChanged += AppFocusChanged;
            #endif

            #endif
        }

        public static void DeRegisterObserver()
        {
            #if NATIVE_PLUGIN_EXIST
            #if UNITY_IOS
            deRegisterObserver();
            #else
            Application.focusChanged -= AppFocusChanged;
            #endif

            #endif
        }

        public static void SetCardboardInitialProfile(string url)
        {
            if (!PlayerPrefs.HasKey(CardboardUtility.KEY_CARDBOARD_CAMERA_SCANNED) ||
                PlayerPrefs.GetInt(CardboardUtility.KEY_CARDBOARD_CAMERA_SCANNED) == 0)
                SetCardboardProfile(url);
        }

        public static void SetCardboardProfile(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            Debug.LogFormat("CardboardQrCode.SetCardboardProfile() called, url: \r\n{0}",
                url);

            #if NATIVE_PLUGIN_EXIST
            #if UNITY_IOS
            loadDeviceParamertersFromURL(url, LoadDeviceParamCallback);
            #elif UNITY_ANDROID
            SetDeviceParamertersFromURL(url);
            LoadDeviceParamCallback(true);
            #endif

            #endif
        }

        public static void RetrieveDeviceParam()
        {
            CardboardQrCode_getSavedDeviceParams(ref _encodedDeviceParams, ref _paramsSize);

            Debug.Log("CardboardQrCode.RetrieveDeviceParam() size=" + _paramsSize);
            encodedBytes = ReadByteArray(_encodedDeviceParams, _paramsSize);

            if (_paramsSize > 0)
                decodedParams = DeviceParams.Parser.ParseFrom(encodedBytes);

            // Debug.LogFormat("CardboardQrCode.RetrieveDeviceParam() params length={0}, byte=\r\n {1}",
            //     encodedBytes.Length, string.Join(" , ", encodedBytes));
            Debug.LogFormat("CardboardQrCode.RetrieveDeviceParam() decode device params: \r\n{0}",
                CardboardUtility.DeviceParamsToString(decodedParams));
        }

        public static void RetrieveCardboardDeviceV1Params()
        {
            CardboardQrCode_getCardboardV1DeviceParams(ref _encodedDeviceParams, ref _paramsSize);

            Debug.Log("CardboardQrCode.RetrieveCardboardDeviceV1Params() size=" + _paramsSize);
            encodedBytes = ReadByteArray(_encodedDeviceParams, _paramsSize);

            if (_paramsSize > 0)
                decodedParams = DeviceParams.Parser.ParseFrom(encodedBytes);

            Debug.LogFormat("CardboardQrCode.RetrieveCardboardDeviceV1Params() decode device params: \r\n{0}",
                CardboardUtility.DeviceParamsToString(decodedParams));
        }

        public static int GetQRCodeScanCount()
        {
            return CardboardQrCode_getQrCodeScanCount();
        }

        public static (IntPtr, int) GetDeviceParamsPointer()
        {
            return (_encodedDeviceParams, _paramsSize);
        }

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