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

        private static IntPtr leftEyeMesh;
        private static IntPtr rightEyeMesh;

        [DllImport(CardboardUtility.DLLName)]
        private static extern IntPtr CardboardLensDistortion_create(
            IntPtr encoded_device_params, int size, int display_width, int display_height);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardLensDistortion_destroy(IntPtr lens_Distortion);

        //todo is this correct?
        [DllImport(CardboardUtility.DLLName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void CardboardLensDistortion_getDistortionMesh(
            IntPtr lens_Distortion, CardboardEye eye, ref IntPtr mesh);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardLensDistortion_getEyeMatrices(
            IntPtr lens_Distortion, float[] projection_matrix, float[] eye_from_head_matrix, CardboardEye eye);

        [DllImport(CardboardUtility.DLLName)]
        private static extern CardboardUv CardboardLensDistortion_distortedUvForUndistortedUv(
            IntPtr lens_Distortion, CardboardUv undistorted_uv, CardboardEye eye);

        [DllImport(CardboardUtility.DLLName)]
        private static extern CardboardUv CardboardLensDistortion_undistortedUvForDistortedUv(
            IntPtr lens_Distortion, CardboardUv distorted_uv, CardboardEye eye);

        public static void CreateLensDistortion(IntPtr encoded_device_params, int params_size)
        {
            _lensDistortion = CardboardLensDistortion_create(
                encoded_device_params, params_size, Screen.width, Screen.height);
        }

        // public static void GetDistortionMesh(CardboardEye eye)
        // {
        //     CardboardMesh result = new CardboardMesh();
        //     CardboardLensDistortion_getDistortionMesh(_lensDistortion, eye, ref result);
        //     Debug.Log("Feature Test CardboardLensDistortion.GetDistortionMesh() result n_indics=" +
        //         result.n_indices + " n_vertices=" + result.n_vertices);
        //     // return CardboardUtility.ConvertCardboardMesh(result);
        // }

        public static void RetrieveEyeMeshes()
        {
            CardboardLensDistortion_getDistortionMesh(_lensDistortion, CardboardEye.kLeft, ref leftEyeMesh);
            CardboardLensDistortion_getDistortionMesh(_lensDistortion, CardboardEye.kRight, ref rightEyeMesh);
        }

        public static Matrix4x4 GetProjectionMatrix(CardboardEye eye)
        {
            float[] projectionMatrix = new float[16];
            float[] eyeFromHeadMatrix = new float[16];
            CardboardLensDistortion_getEyeMatrices(_lensDistortion, projectionMatrix, eyeFromHeadMatrix, eye);

            Matrix4x4 result = new Matrix4x4();
            for (int i = 0; i < projectionMatrix.Length; i++)
            {
                result[i] = projectionMatrix[i];
            }
            return result;
        }
    }
}