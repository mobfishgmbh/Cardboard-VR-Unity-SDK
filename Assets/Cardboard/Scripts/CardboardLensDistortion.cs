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
    public static class CardboardLensDistortion
    {
        //https://github.com/googlevr/cardboard/blob/master/sdk/distortion_mesh.cc

        private static IntPtr _lensDistortion;

        private static CardboardMesh leftEyeMesh;
        private static CardboardMesh rightEyeMesh;

        private static float[] projectionMatrixLeft;
        private static float[] projectionMatrixRight;
        private static float[] eyeFromHeadMatrixLeft;
        private static float[] eyeFromHeadMatrixRight;

        #if NATIVE_PLUGIN_EXIST
        [DllImport(CardboardUtility.DLLName)]
        private static extern IntPtr CardboardLensDistortion_create(
            IntPtr encoded_device_params, int size, int display_width, int display_height);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardLensDistortion_destroy(IntPtr lens_Distortion);

        [DllImport(CardboardUtility.DLLName, CallingConvention = CallingConvention.Cdecl)]
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

        #else

        private static IntPtr CardboardLensDistortion_create(
            IntPtr encoded_device_params, int size, int display_width, int display_height)
        {
            return IntPtr.Zero;
        }

        private static void CardboardLensDistortion_destroy(IntPtr lens_Distortion)
        {
        }

        private static void CardboardLensDistortion_getDistortionMesh(
            IntPtr lens_Distortion, CardboardEye eye, ref CardboardMesh mesh)
        {
            mesh = CardboardUtility.CreateMockupCardboardMesh(eye);
        }

        private static void CardboardLensDistortion_getEyeMatrices(
            IntPtr lens_Distortion, float[] projection_matrix, float[] eye_from_head_matrix, CardboardEye eye)
        {
            CardboardUtility.Matrix4x4ToArray(Matrix4x4.Perspective(70, 0.8f, 0.5f, 1000)).CopyTo(projection_matrix, 0);
            eye_from_head_matrix.Initialize();
        }

        private static CardboardUv CardboardLensDistortion_distortedUvForUndistortedUv(
            IntPtr lens_Distortion, CardboardUv undistorted_uv, CardboardEye eye)
        {
            return undistorted_uv;
        }

        private static CardboardUv CardboardLensDistortion_undistortedUvForDistortedUv(
            IntPtr lens_Distortion, CardboardUv distorted_uv, CardboardEye eye)
        {
            return distorted_uv;
        }

        #endif

        public static void CreateLensDistortion(IntPtr encoded_device_params, int params_size)
        {
            _lensDistortion = CardboardLensDistortion_create(
                encoded_device_params, params_size, Screen.width, Screen.height);
        }

        public static void DestroyLensDistortion()
        {
            CardboardLensDistortion_destroy(_lensDistortion);
        }

        public static void RetrieveEyeMeshes()
        {
            CardboardLensDistortion_getDistortionMesh(_lensDistortion, CardboardEye.kLeft, ref leftEyeMesh);
            CardboardLensDistortion_getDistortionMesh(_lensDistortion, CardboardEye.kRight, ref rightEyeMesh);

            Debug.LogFormat(
                "CardboardLensDistortion.RetrieveEyeMeshes() result leftEye n_indics={0} n_vertices={1} " +
                "actual indics={2} actual vertices={3}",
                leftEyeMesh.n_indices, leftEyeMesh.n_vertices,
                leftEyeMesh.indices.Length, rightEyeMesh.vertices.Length);
        }

        public static (CardboardMesh, CardboardMesh) GetEyeMeshes()
        {
            return (leftEyeMesh, rightEyeMesh);
        }

        public static void RefreshProjectionMatrix()
        {
            projectionMatrixLeft = new float[16];
            eyeFromHeadMatrixLeft = new float[16];
            projectionMatrixRight = new float[16];
            eyeFromHeadMatrixRight = new float[16];
            CardboardLensDistortion_getEyeMatrices(_lensDistortion, projectionMatrixLeft, eyeFromHeadMatrixLeft,
                CardboardEye.kLeft);
            CardboardLensDistortion_getEyeMatrices(_lensDistortion, projectionMatrixRight, eyeFromHeadMatrixRight,
                CardboardEye.kRight);
        }

        public static Matrix4x4 GetProjectionMatrix(CardboardEye eye)
        {
            if (projectionMatrixLeft == null || projectionMatrixRight == null)
                RefreshProjectionMatrix();

            float[] targetMatrix = eye == CardboardEye.kLeft ? projectionMatrixLeft : projectionMatrixRight;

            return CardboardUtility.ArrayToMatrix4x4(targetMatrix);
        }

        public static Matrix4x4 GetEyeFromHeadMatrix(CardboardEye eye)
        {
            if (eyeFromHeadMatrixLeft == null || eyeFromHeadMatrixRight == null)
                RefreshProjectionMatrix();

            float[] targetMatrix = eye == CardboardEye.kLeft ? eyeFromHeadMatrixLeft : eyeFromHeadMatrixRight;

            return CardboardUtility.ArrayToMatrix4x4(targetMatrix);
        }


        public static float[] GetEyeFromHeadRaw(CardboardEye eye)
        {
            if (eyeFromHeadMatrixLeft == null || eyeFromHeadMatrixRight == null)
                RefreshProjectionMatrix();

            return eye == CardboardEye.kLeft ? eyeFromHeadMatrixLeft : eyeFromHeadMatrixRight;
        }
    }
}