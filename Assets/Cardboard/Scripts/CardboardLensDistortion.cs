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

        //todo is this correct?
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
            if (eye == CardboardEye.kLeft)
            {
                mesh = new CardboardMesh()
                {
                    vertices = new[] {-0.65f, -0.25f, -0.65f, 0.25f, -0.125f, -0.25f, -0.125f, 0.25f},
                    n_vertices = 4,
                    indices = new[] {0, 1, 2, 3},
                    n_indices = 4,
                    uvs = new[] {0f, 0f, 0f, 1f, 1f, 0f, 1f, 1f}
                };
            }
            else
            {
                mesh = new CardboardMesh()
                {
                    vertices = new[] {0.125f, -0.25f, 0.125f, 0.25f, 0.65f, -0.25f, 0.65f, 0.25f},
                    n_vertices = 4,
                    indices = new[] {0, 1, 2, 3},
                    n_indices = 4,
                    uvs = new[] {0f, 0f, 0f, 1f, 1f, 0f, 1f, 1f}
                };
            }
        }

        private static void CardboardLensDistortion_getEyeMatrices(
            IntPtr lens_Distortion, float[] projection_matrix, float[] eye_from_head_matrix, CardboardEye eye)
        {
            projection_matrix.Initialize();
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
            //todo during scan qr, malloc error, something not freed here?
            //CardboardMesh tempA = new CardboardMesh();
            //GCHandle tempAGch = GCHandle.Alloc(tempA);
            //CardboardMesh tempB = new CardboardMesh();
            //GCHandle tempBGch = GCHandle.Alloc(tempB);
            //leftEyeMesh = GCHandle.ToIntPtr(tempAGch);
            //rightEyeMesh = GCHandle.ToIntPtr(tempBGch);

            CardboardLensDistortion_getDistortionMesh(_lensDistortion, CardboardEye.kLeft, ref leftEyeMesh);
            CardboardLensDistortion_getDistortionMesh(_lensDistortion, CardboardEye.kRight, ref rightEyeMesh);

            //tempAGch.Free();
            //tempBGch.Free();

            Debug.Log("Feature Test CardboardLensDistortion.GetDistortionMesh() result leftEye n_indics=" +
                leftEyeMesh.n_indices + " n_vertices=" + leftEyeMesh.n_vertices +
                " actual indics=" + leftEyeMesh.indices.Length + " actual vertices=" + rightEyeMesh.vertices.Length);
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

            Matrix4x4 result = new Matrix4x4();

            float[] targetMatrix = eye == CardboardEye.kLeft ? projectionMatrixLeft : projectionMatrixRight;
            for (int i = 0; i < targetMatrix.Length; i++)
            {
                result[i] = targetMatrix[i];
            }
            return result;
        }

        public static Matrix4x4 GetEyeFromHeadMatrix(CardboardEye eye)
        {
            if (eyeFromHeadMatrixLeft == null || eyeFromHeadMatrixRight == null)
                RefreshProjectionMatrix();

            Matrix4x4 result = new Matrix4x4();

            float[] targetMatrix = eye == CardboardEye.kLeft ? eyeFromHeadMatrixLeft : eyeFromHeadMatrixRight;
            for (int i = 0; i < targetMatrix.Length; i++)
            {
                result[i] = targetMatrix[i];
            }
            return result;
        }


        public static float[] GetEyeFromHeadRaw(CardboardEye eye)
        {
            if (eyeFromHeadMatrixLeft == null || eyeFromHeadMatrixRight == null)
                RefreshProjectionMatrix();

            return eye == CardboardEye.kLeft ? eyeFromHeadMatrixLeft : eyeFromHeadMatrixRight;
        }
    }
}