using System;
using UnityEngine;

namespace MobfishCardboard
{
    public static class CardboardManager
    {
        private static bool initiated;
        private static bool retriving;
        private static Pose headPoseTemp;

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
                #if UNITY_ANDROID
                CardboardAndroidInitialization.InitAndroid();

                #endif

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

        private static void LoadDefaultProfile()
        {
            if (profileAvailable)
                return;

            SetCardboardProfile(CardboardUtility.defaultCardboardUrl);
        }

        //This method can be used to change current device paramters
        public static void SetCardboardProfile(string url)
        {
            CardboardQrCode.SetCardboardProfile(url);
        }

        //This method will set cardbaord profile ONLY when cardboard qr has not yet been scanned by camera.
        //This behaviour is the same as the legacy cardboard gvr_set_default_viewer_profile
        public static void SetCardboardInitialProfile(string url)
        {
            CardboardQrCode.SetCardboardInitialProfile(url);
        }

        public static void ScanQrCode()
        {
            CardboardQrCode.StartScanQrCode();
        }

        public static void RefreshParameters()
        {
            CardboardQrCode.RetrieveDeviceParam();

            if (retriving)
                return;

            retriving = true;

            InitDeviceProfile();
            InitCameraProperties();

            retriving = false;

            deviceParamsChangeEvent?.Invoke();
        }

        public static void SetVRViewEnable(bool shouldEnable)
        {
            enableVRView = shouldEnable;
            enableVRViewChangedEvent?.Invoke();
        }

        public static void RecenterCamera(bool horizontalOnly = true)
        {
            CardboardHeadTracker.RecenterCamera(horizontalOnly);
        }

        public static Pose GetHeadPose(bool withUpdate = false)
        {
            if (withUpdate)
            {
                CardboardHeadTracker.UpdatePose();
            }

            headPoseTemp.position = CardboardHeadTracker.trackerUnityPosition;
            headPoseTemp.rotation = CardboardHeadTracker.trackerUnityRotation;

            return headPoseTemp;
        }

        private static void InitDeviceProfile()
        {
            (IntPtr, int) par = CardboardQrCode.GetDeviceParamsPointer();

            if (par.Item2 == 0 && !Application.isEditor)
            {
                profileAvailable = false;
                LoadDefaultProfile();
                par = CardboardQrCode.GetDeviceParamsPointer();
            }

            // if (par.Item2 == 0 && !Application.isEditor)
            // {
            //     CardboardQrCode.RetrieveCardboardDeviceV1Params();
            //     par = CardboardQrCode.GetDeviceParamsPointer();
            // }

            if (par.Item2 > 0 || Application.isEditor)
            {
                deviceParameter = CardboardQrCode.GetDecodedDeviceParams();
                //todo do we need to destroy it before create it?

                CardboardLensDistortion.CreateLensDistortion(par.Item1, par.Item2);
                profileAvailable = true;
            }
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