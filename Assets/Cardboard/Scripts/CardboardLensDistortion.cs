using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace MobfishCardboard
{
    public static class CardboardLensDistortion
    {
        //https://github.com/googlevr/cardboard/blob/master/sdk/distortion_mesh.cc

        private static IntPtr _lensDistortion;

        [DllImport(CardboardUtility.DLLName)]
        private static extern IntPtr CardboardLensDistortion_create(
            IntPtr encoded_device_params, int size, int display_width, int display_height);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardLensDistortion_destroy(IntPtr lens_Distortion);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardLensDistortion_getDistortionMesh(
            IntPtr lens_Distortion, CardboardEye eye, ref CardboardMesh mesh);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardLensDistortion_getEyeMatrices(
            IntPtr lens_Distortion, float[] projection_matrix, float[] eye_from_head_matrix, CardboardEye eye);

        [DllImport(CardboardUtility.DLLName)]
        private static extern CardboardUv CardboardLensDistortion_distortedUvForUndistortedUv(
            IntPtr lens_Distortion, CardboardUv undistorted_uv, CardboardEye eye);

        [DllImport(CardboardUtility.DLLName)]
        private static extern CardboardUv CardboardLensDistortion_undistortedUvForDistortedUv(
            IntPtr lens_Distortion, CardboardUv distorted_uv, CardboardEye eye);
    }
}