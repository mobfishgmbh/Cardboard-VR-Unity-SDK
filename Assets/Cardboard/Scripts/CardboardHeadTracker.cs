#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
#define NATIVE_PLUGIN_EXIST
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace MobfishCardboard
{
    public static class CardboardHeadTracker
    {
        //https://github.com/googlevr/cardboard/blob/master/hellocardboard-ios/HelloCardboardRenderer.mm
        //https://github.com/googlevr/gvr-unity-sdk/blob/v0.8.1/GoogleVR/Scripts/Pose3D.cs
        //https://github.com/googlevr/gvr-unity-sdk/blob/v0.8.1/GoogleVR/Scripts/VRDevices/GvrDevice.cs
        //https://gamedev.stackexchange.com/questions/174107/unity-gyroscope-orientation-attitude-wrong

        //todo reduce memory alloc within the file

        private const long kPrediction = 50000000;
        private static readonly Matrix4x4 flipZ = Matrix4x4.Scale(new Vector3(1, 1, -1));

        private static IntPtr _headTracker;

        #if !UNITY_2019_1_OR_NEWER || ENABLE_LEGACY_INPUT_MANAGER
        private static UnityEngine.Gyroscope gyro;
        #endif

        public static Vector3 trackerRawPosition { get; private set; }
        public static Quaternion trackerRawRotation { get; private set; }
        public static Vector3 trackerUnityPosition { get; private set; }
        public static Quaternion trackerUnityRotation { get; private set; }
        private static Quaternion trackerUnityRotationOffset;

        #if NATIVE_PLUGIN_EXIST
        [DllImport(CardboardUtility.DLLName)]
        private static extern IntPtr CardboardHeadTracker_create();

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardHeadTracker_destroy(IntPtr head_tracker);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardHeadTracker_getPose(
            IntPtr head_tracker, long timestamp_ns, float[] position, float[] orientation);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardHeadTracker_pause(IntPtr head_tracker);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardHeadTracker_resume(IntPtr head_tracker);


        #if UNITY_IOS

        [DllImport(CardboardUtility.DLLName)]
        private static extern double CACurrentMediaTime();

        #endif

        #else

        private static readonly float[] editorOrientation = new[]
        {
            Quaternion.identity.x, Quaternion.identity.y, -Quaternion.identity.z, -Quaternion.identity.w
        };

        private static IntPtr CardboardHeadTracker_create()
        {
            return IntPtr.Zero;
        }

        private static void CardboardHeadTracker_destroy(IntPtr head_tracker)
        {
        }

        private static void CardboardHeadTracker_getPose(
            IntPtr head_tracker, long timestamp_ns, float[] position, float[] orientation)
        {
            position.Initialize();
            editorOrientation.CopyTo(orientation, 0);
        }

        private static void CardboardHeadTracker_pause(IntPtr head_tracker)
        {
        }

        private static void CardboardHeadTracker_resume(IntPtr head_tracker)
        {
        }

        #endif

        private static long GetMonotonicTime()
        {
            #if NATIVE_PLUGIN_EXIST
            #if UNITY_IOS
            return Convert.ToInt64(CACurrentMediaTime() * 1e9);

            #elif UNITY_ANDROID

            //todo android and editor
            return 0;
            #endif

            #else
            return 0;

            #endif
        }

        public static void CreateTracker()
        {
            #if !UNITY_2019_1_OR_NEWER || ENABLE_LEGACY_INPUT_MANAGER

            Input.gyro.enabled = true;
            gyro = Input.gyro;

            #endif

            _headTracker = CardboardHeadTracker_create();
            Init();
        }

        private static void Init()
        {
            trackerUnityRotationOffset = Quaternion.identity;
        }

        public static void UpdatePose()
        {
            #if NATIVE_PLUGIN_EXIST
            UpdatePoseCardboard();
            #else
            UpdatePoseGyro();
            #endif
        }

        private static void UpdatePoseCardboard()
        {
            long time = GetMonotonicTime();
            time = time + kPrediction;

            float[] _position = new float[3];
            float[] _orientation = new float[4];

            CardboardHeadTracker_getPose(_headTracker, time, _position, _orientation);

            trackerRawPosition = new Vector3(_position[0], _position[1], _position[2]);
            trackerRawRotation = new Quaternion(_orientation[0], _orientation[1], _orientation[2], _orientation[3]);

            Matrix4x4 deviceRawPoseMat = Matrix4x4.TRS(trackerRawPosition, trackerRawRotation, Vector3.one);
            Matrix4x4 unityPoseMat = flipZ * deviceRawPoseMat * flipZ;

            trackerUnityPosition = unityPoseMat.GetColumn(3);
            trackerUnityRotation = new Quaternion(trackerRawRotation.x, trackerRawRotation.y, -trackerRawRotation.z,
                trackerRawRotation.w);
            trackerUnityRotation = Quaternion.Inverse(trackerUnityRotationOffset) * trackerUnityRotation;
        }

        private static void UpdatePoseGyro()
        {
            #if !UNITY_2019_1_OR_NEWER || ENABLE_LEGACY_INPUT_MANAGER

            trackerRawPosition = trackerUnityPosition = Vector3.zero;
            trackerRawRotation = gyro?.attitude ?? Quaternion.identity;
            Quaternion rawConvert = new Quaternion(trackerRawRotation.x, trackerRawRotation.y, -trackerRawRotation.z,
                -trackerRawRotation.w);
            trackerUnityRotation = Quaternion.Euler(90, 0, 0) * rawConvert;

            #endif
        }

        public static void RecenterCamera(bool horizontalOnly = true)
        {
            trackerUnityRotationOffset = new Quaternion(trackerRawRotation.x, trackerRawRotation.y,
                -trackerRawRotation.z, trackerRawRotation.w);
            if (horizontalOnly)
            {
                trackerUnityRotationOffset = Quaternion.AngleAxis(trackerUnityRotationOffset.eulerAngles.y, Vector3.up);
            }
        }

        public static void PauseTracker()
        {
            CardboardHeadTracker_pause(_headTracker);
        }

        public static void ResumeTracker()
        {
            CardboardHeadTracker_resume(_headTracker);
        }


        private static float[] ReadFloatArray(IntPtr pointer, int size)
        {
            var result = new float[size];
            Marshal.Copy(pointer, result, 0, size);
            return result;
        }
    }
}