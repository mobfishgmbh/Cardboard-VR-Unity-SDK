using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MobfishCardboard;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace MobfishCardboardDemo
{
    public class VRCamera: MonoBehaviour
    {
        [Header("Cameras")]
        public Camera centerCam;
        public Camera leftCam;
        public Camera rightCam;

        [Header("QR")]
        public Button scanQRButton;
        public Button continueButton;
        public GameObject continuePanel;

        [Header("Other")]
        public Text debugText;
        public MeshFilter testEyeMeshLeft;
        public MeshFilter testEyeMeshRight;

        private RenderTextureDescriptor eyeRenderTextureDesc;
        private RenderTexture centerRenderTexture;
        private bool needUpdateProfile;

        private void Awake()
        {
            Application.targetFrameRate = 60;

            SetupRenderTexture();

            continuePanel.SetActive(false);
            continueButton.onClick.AddListener(ContinueClicked);
            scanQRButton.onClick.AddListener(ScanQRCode);
        }

        // Start is called before the first frame update
        void Start()
        {
            CardboardHeadTracker.CreateTracker();
            CardboardHeadTracker.ResumeTracker();
            CardboardDistortionRenderer.InitDestortionRenderer();

            ResetProfile();
        }

        private void SetupRenderTexture()
        {
            eyeRenderTextureDesc = new RenderTextureDescriptor()
            {
                dimension = TextureDimension.Tex2D,
                width = Screen.width,
                height = Screen.height,
                depthBufferBits = 16,
                volumeDepth = 1,
                msaaSamples = 2,
            };

            RenderTexture newLeft = new RenderTexture(eyeRenderTextureDesc);
            RenderTexture newRight = new RenderTexture(eyeRenderTextureDesc);
            leftCam.targetTexture = newLeft;
            rightCam.targetTexture = newRight;

            CardboardManager.SetRenderTexture(newLeft, newRight);
        }

        private void ResetProfile()
        {
            CardboardQrCode.RetrieveDeviceParam();
            (IntPtr, int) par = CardboardQrCode.GetDeviceParamsPointer();

            if (par.Item2 == 0 && !Application.isEditor)
            {
                ScanQRCode();
                return;
            }

            //CardboardLensDistortion.DestroyLensDistortion();
            CardboardLensDistortion.CreateLensDistortion(par.Item1, par.Item2);
            RefreshCamera();

            needUpdateProfile = false;

            (byte[], int) paramDetailVar = CardboardQrCode.GetDeviceParamsByte();
            NativeDataExtract.Save_EncodedParam(paramDetailVar.Item1, paramDetailVar.Item2);
        }

        private void ContinueClicked()
        {
            continuePanel.SetActive(false);
            ResetProfile();
        }


        private void DoRenderTest()
        {
            CardboardEyeTextureDescription cetdLeft = new CardboardEyeTextureDescription()
            {
                texture = centerRenderTexture.GetNativeTexturePtr(),
                eye_from_head = CardboardLensDistortion.GetEyeFromHeadRaw(CardboardEye.kLeft),
                left_u = 0,
                right_u = 1,
                bottom_v = 0,
                top_v = 1,
                layer = 0
            };
            CardboardDistortionRenderer.RenderEyeToDisplay(cetdLeft, cetdLeft);
        }

        private void RefreshCamera()
        {
            CardboardLensDistortion.RetrieveEyeMeshes();
            CardboardLensDistortion.RefreshProjectionMatrix();

            Matrix4x4 leftMatrix = CardboardLensDistortion.GetProjectionMatrix(CardboardEye.kLeft);
            if (!leftMatrix.Equals(Matrix4x4.zero))
                leftCam.projectionMatrix = leftMatrix;
            Matrix4x4 rightMatrix = CardboardLensDistortion.GetProjectionMatrix(CardboardEye.kRight);
            if (!rightMatrix.Equals(Matrix4x4.zero))
                rightCam.projectionMatrix = rightMatrix;

            (CardboardMesh, CardboardMesh) eyeMeshes = CardboardLensDistortion.GetEyeMeshes();
            CardboardDistortionRenderer.SetEyeMeshes(eyeMeshes.Item1, eyeMeshes.Item2);
            CardboardManager.SetEyeMesh(
                CardboardUtility.ConvertCardboardMesh_Triangle(eyeMeshes.Item1),
                CardboardUtility.ConvertCardboardMesh_Triangle(eyeMeshes.Item2));
            testEyeMeshLeft.mesh = CardboardManager.viewMeshLeft;
            testEyeMeshRight.mesh = CardboardManager.viewMeshRight;

            NativeDataExtract.Save_MeshJson(eyeMeshes.Item1);
            NativeDataExtract.Save_MeshJson(eyeMeshes.Item2);
        }

        // Update is called once per frame
        void Update()
        {
            CardboardHeadTracker.UpdatePoseGyro();
            if (!Application.isEditor)
                transform.localRotation = CardboardHeadTracker.trackerUnityRotation;
            Update_DebugInfo();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            Debug.Log("OnApplicationFocus called, hasFocus=" + hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            Debug.Log("OnApplicationPause called, pauseStatus=" + pauseStatus);
        }

        void Update_DebugInfo()
        {
            debugText.text = string.Format("device rot={0}, \r\nUnity rot={1}",
                CardboardHeadTracker.trackerRawRotation.eulerAngles,
                CardboardHeadTracker.trackerUnityRotation.eulerAngles);
        }

        private void ScanQRCode()
        {
            CardboardQrCode.StartScanQrCode();
            needUpdateProfile = true;
            continuePanel.SetActive(true);
        }
    }
}