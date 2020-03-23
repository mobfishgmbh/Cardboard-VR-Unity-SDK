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

        private const ulong kPrediction = 50000000;
        private static readonly Matrix4x4 flipZ = Matrix4x4.Scale(new Vector3(1, 1, -1));

        private static IntPtr _headTracker;

        public static Vector3 trackerRawPosition { get; private set; }
        public static Quaternion trackerRawRotation { get; private set; }
        public static Vector3 trackerUnityPosition { get; private set; }
        public static Quaternion trackerUnityRotation { get; private set; }

        [DllImport(CardboardUtility.DLLName)]
        private static extern IntPtr CardboardHeadTracker_create();

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardHeadTracker_destroy(IntPtr head_tracker);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardHeadTracker_getPose(
            IntPtr head_tracker, double timestamp_ns, float[] position, float[] orientation);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardHeadTracker_pause(IntPtr head_tracker);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardHeadTracker_resume(IntPtr head_tracker);

        [DllImport(CardboardUtility.DLLName)]
        private static extern double CACurrentMediaTime();

        public static void CreateTracker()
        {
            _headTracker = CardboardHeadTracker_create();
        }

        public static void UpdatePose()
        {
            double time = CACurrentMediaTime() * 1e9;
            time += kPrediction;

            float[] _position = new float[3];
            float[] _orientation = new float[4];

            CardboardHeadTracker_getPose(_headTracker, time, _position, _orientation);

            trackerRawPosition = new Vector3(_position[0], _position[1], _position[2]);
            trackerRawRotation = new Quaternion(_orientation[0], _orientation[1], _orientation[2], _orientation[3]);

            Matrix4x4 deviceRawPoseMat = Matrix4x4.TRS(trackerRawPosition, trackerRawRotation, Vector3.one);
            Matrix4x4 unityPoseMat = flipZ * deviceRawPoseMat * flipZ;

            trackerUnityPosition = unityPoseMat.GetColumn(3);
            trackerUnityRotation = Quaternion.LookRotation(unityPoseMat.GetColumn(2), unityPoseMat.GetColumn(1));
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