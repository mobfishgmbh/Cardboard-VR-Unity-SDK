#if UNITY_ANDROID && !UNITY_EDITOR
#define NATIVE_PLUGIN_EXIST
#endif

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MobfishCardboard
{
    public static class CardboardAndroidInitialization
    {
        public const string UNITY_ANDROID_PLAYER = "com.unity3d.player.UnityPlayer";
        public const string UNITY_ANDROID_ACTIVITY_CURRENT = "currentActivity";
        public const string UNITY_ANDROID_CONTEXT = "getApplicationContext";

        #if NATIVE_PLUGIN_EXIST
        [DllImport(CardboardUtility.DLLName_UNITYJNI)]
        private static extern void CardboardUnity_initializeAndroid(IntPtr context);

        #else
        private static void CardboardUnity_initializeAndroid(IntPtr context)
        {
        }

        #endif

        public static void InitAndroid()
        {
            #if NATIVE_PLUGIN_EXIST
            AndroidJavaClass unityClass = new AndroidJavaClass(UNITY_ANDROID_PLAYER);
            AndroidJavaObject activityObject = unityClass.GetStatic<AndroidJavaObject>(UNITY_ANDROID_ACTIVITY_CURRENT);
            AndroidJavaObject contextObject = activityObject.Call<AndroidJavaObject>(UNITY_ANDROID_CONTEXT);

            CardboardUnity_initializeAndroid(activityObject.GetRawObject());

            CardboardQrCode.SetAndroidQRCodeLocation();
            #endif

        }
    }
}