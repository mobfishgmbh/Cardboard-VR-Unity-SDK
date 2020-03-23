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

        private const ulong kPrediction = 50000000;

        private static IntPtr _headTracker;

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

        public static Quaternion GetPose()
        {
            double time = CACurrentMediaTime() * 1e9;
            time += kPrediction;

            float[] _position = new float[3];
            float[] _orientation = new float[4];

            CardboardHeadTracker_getPose(_headTracker, time, _position, _orientation);

            return new Quaternion(_orientation[0], _orientation[1], _orientation[2], _orientation[3]);
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