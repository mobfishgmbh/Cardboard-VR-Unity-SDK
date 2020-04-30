using System;
using UnityEngine;

namespace MobfishCardboard
{
    public static class CardboardManager
    {
        private static bool initiated;

        public static DeviceParams deviceParameter { get; private set; }
        public static RenderTexture viewTextureLeft { get; private set; }
        public static RenderTexture viewTextureRight { get; private set; }
        public static Mesh viewMeshLeft { get; private set; }
        public static Mesh viewMeshRight { get; private set; }
        public static CardboardMesh viewMeshLeftRaw { get; private set; }
        public static CardboardMesh viewMeshRightRaw { get; private set; }
        public static Matrix4x4 projectionMatrixLeft { get; private set; }
        public static Matrix4x4 projectionMatrixRight { get; private set; }
        public static Matrix4x4 eyeFromHeadMatrixLeft { get; private set; }
        public static Matrix4x4 eyeFromHeadMatrixRight { get; private set; }

        public static bool profileAvailable { get; private set; }
        public static bool enableVRView { get; private set; }

        public static event Action deviceParamsChangeEvent;
        public static event Action renderTextureResetEvent;
        public static event Action enableVRViewChangedEvent;

        public static void InitCardboard()
        {
            if (!initiated)
            {
                CardboardHeadTracker.CreateTracker();
                CardboardHeadTracker.ResumeTracker();

                CardboardQrCode.RegisterObserver();
                Application.quitting += ApplicationQuit;

                initiated = true;
            }
            RefreshParameters();
        }

        private static void ApplicationQuit()
        {
            CardboardQrCode.DeRegisterObserver();
        }

        public static void RefreshParameters()
        {
            CardboardQrCode.RetrieveDeviceParam();
            InitDeviceProfile();
            InitCameraProperties();

            deviceParamsChangeEvent?.Invoke();
        }

        public static void SetVRViewEnable(bool shouldEnable)
        {
            enableVRView = shouldEnable;
            enableVRViewChangedEvent?.Invoke();

            if (!profileAvailable && enableVRView)
            {
                CardboardQrCode.StartScanQrCode();
            }
        }

        private static void InitDeviceProfile()
        {
            (IntPtr, int) par = CardboardQrCode.GetDeviceParamsPointer();

            if (par.Item2 == 0 && !Application.isEditor)
            {
                profileAvailable = false;
                if (enableVRView)
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

            eyeFromHeadMatrixLeft = CardboardLensDistortion.GetEyeFromHeadMatrix(CardboardEye.kLeft);
            eyeFromHeadMatrixRight = CardboardLensDistortion.GetEyeFromHeadMatrix(CardboardEye.kRight);

            (CardboardMesh, CardboardMesh) eyeMeshes = CardboardLensDistortion.GetEyeMeshes();
            viewMeshLeftRaw = eyeMeshes.Item1;
            viewMeshRightRaw = eyeMeshes.Item2;

            viewMeshLeft = CardboardUtility.ConvertCardboardMesh_Triangle(eyeMeshes.Item1);
            viewMeshRight = CardboardUtility.ConvertCardboardMesh_Triangle(eyeMeshes.Item2);
        }

        public static void SetRenderTexture(RenderTexture newLeft, RenderTexture newRight)
        {
            if (viewTextureLeft != null)
                viewTextureLeft.Release();
            if (viewTextureRight != null)
                viewTextureRight.Release();

            viewTextureLeft = newLeft;
            viewTextureRight = newRight;

            renderTextureResetEvent?.Invoke();
        }
    }
}