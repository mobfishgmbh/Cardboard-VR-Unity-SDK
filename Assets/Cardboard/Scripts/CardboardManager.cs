using System;
using UnityEngine;

namespace MobfishCardboard
{
    public static class CardboardManager
    {
        public static DeviceParams deviceParameter { get; private set; }
        public static RenderTexture viewTextureLeft { get; private set; }
        public static RenderTexture viewTextureRight { get; private set; }
        public static Mesh viewMeshLeft { get; private set; }
        public static Mesh viewMeshRight { get; private set; }
        public static CardboardMesh viewMeshLeftRaw { get; private set; }
        public static CardboardMesh viewMeshRightRaw { get; private set; }
        public static Matrix4x4 projectionMatrixLeft { get; private set; }
        public static Matrix4x4 projectionMatrixRight { get; private set; }

        public static bool profileAvailable { get; private set; }

        public static event Action deviceParamsChangeEvent;

        public static void InitCardboard()
        {
            CardboardHeadTracker.CreateTracker();
            CardboardHeadTracker.ResumeTracker();

            RefreshParameters();
        }

        public static void RefreshParameters()
        {
            CardboardQrCode.RetrieveDeviceParam();
            InitDeviceProfile();
            InitCameraProperties();

            deviceParamsChangeEvent?.Invoke();
        }

        private static void InitDeviceProfile()
        {
            (IntPtr, int) par = CardboardQrCode.GetDeviceParamsPointer();

            if (par.Item2 == 0 && !Application.isEditor)
            {
                profileAvailable = false;
                CardboardQrCode.StartScanQrCode();
                return;
            }

            //CardboardLensDistortion.DestroyLensDistortion();
            deviceParameter = CardboardQrCode.GetDecodedDeviceParams();
            CardboardLensDistortion.CreateLensDistortion(par.Item1, par.Item2);
            profileAvailable = true;
        }

        private static void InitCameraProperties()
        {
            if (!profileAvailable)
                return;

            CardboardLensDistortion.RetrieveEyeMeshes();
            CardboardLensDistortion.RefreshProjectionMatrix();

            projectionMatrixLeft = CardboardLensDistortion.GetProjectionMatrix(CardboardEye.kLeft);
            projectionMatrixRight = CardboardLensDistortion.GetProjectionMatrix(CardboardEye.kRight);

            (CardboardMesh, CardboardMesh) eyeMeshes = CardboardLensDistortion.GetEyeMeshes();
            viewMeshLeftRaw = eyeMeshes.Item1;
            viewMeshRightRaw = eyeMeshes.Item2;

            viewMeshLeft = CardboardUtility.ConvertCardboardMesh_Triangle(eyeMeshes.Item1);
            viewMeshRight = CardboardUtility.ConvertCardboardMesh_Triangle(eyeMeshes.Item2);
        }

        public static void SetRenderTexture(RenderTexture newLeft, RenderTexture newRight)
        {
            viewTextureLeft = newLeft;
            viewTextureRight = newRight;
        }
    }
}