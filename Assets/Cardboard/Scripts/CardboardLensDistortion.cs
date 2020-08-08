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
        private static extern CardboardUv CardboardLensDistortion_distortedUvForUndistortedUv(
            IntPtr lens_Distortion, CardboardUv undistorted_uv, CardboardEye eye);

        [DllImport(CardboardUtility.DLLName)]
        private static extern CardboardUv CardboardLensDistortion_undistortedUvForDistortedUv(
            IntPtr lens_Distortion, CardboardUv distorted_uv, CardboardEye eye);


        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardLensDistortion_getProjectionMatrix(IntPtr lens_Distortion, CardboardEye eye, float z_near, float z_far, float[] projection_matrix);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardLensDistortion_getEyeFromHeadMatrix(IntPtr lens_Distortion, CardboardEye eye, float[] eye_from_head_matrix);

        [DllImport(CardboardUtility.DLLName)]
        private static extern void CardboardLensDistortion_getFieldOfView(IntPtr lens_Distortion, CardboardEye eye, ref float field_of_view);

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

        private static void CardboardLensDistortion_getProjectionMatrix(IntPtr lens_Distortion, CardboardEye eye,
            float z_near, float z_far, float[] projection_matrix)
        {
            CardboardUtility.Matrix4x4ToArray(Matrix4x4.Perspective(70, 0.8f, 0.5f, 1000)).CopyTo(projection_matrix, 0);
        }

        private static void CardboardLensDistortion_getEyeFromHeadMatrix(IntPtr lens_Distortion, CardboardEye eye,
            float[] eye_from_head_matrix)
        {
            eye_from_head_matrix.Initialize();
        }

        private static void CardboardLensDistortion_getFieldOfView(IntPtr lens_Distortion, CardboardEye eye,
            ref float field_of_view)
        {
            field_of_view = 60.0f;
        }

        #endif

        public static void CreateLensDistortion(IntPtr encoded_device_params, int params_size)
        {
            Vector2Int resolution = CardboardUtility.GetAdjustedScreenResolution();
            _lensDistortion = CardboardLensDistortion_create(
                encoded_device_params, params_size, resolution.x, resolution.y);
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
            CardboardLensDistortion_getProjectionMatrix(_lensDistortion, CardboardEye.kLeft, 0.1f, 100f,
                projectionMatrixLeft);
            CardboardLensDistortion_getProjectionMatrix(_lensDistortion, CardboardEye.kRight, 0.1f, 100f,
                projectionMatrixRight);

            CardboardLensDistortion_getEyeFromHeadMatrix(_lensDistortion, CardboardEye.kLeft, eyeFromHeadMatrixLeft);
            CardboardLensDistortion_getEyeFromHeadMatrix(_lensDistortion, CardboardEye.kRight, eyeFromHeadMatrixRight);
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