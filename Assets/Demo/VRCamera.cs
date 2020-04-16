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
        public Camera novrCam;
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

        private void Awake()
        {
            #if UNITY_IOS
            Application.targetFrameRate = 60;

            #endif

            SetupRenderTexture();

            CardboardManager.InitCardboard();

            SetEnableQROverlay(false);
            continueButton.onClick.AddListener(ContinueClicked);
            scanQRButton.onClick.AddListener(ScanQRCode);
        }

        // Start is called before the first frame update
        void Start()
        {
            RefreshCamera();
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

        private void ContinueClicked()
        {
            CardboardManager.InitDeviceProfile();
            CardboardManager.InitCameraProperties();
            RefreshCamera();

            SetEnableQROverlay(false);
        }

        private void RefreshCamera()
        {
            if (!CardboardManager.profileAvailable)
            {
                SetEnableQROverlay(true);
                return;
            }

            if (!CardboardManager.projectionMatrixLeft.Equals(Matrix4x4.zero))
                leftCam.projectionMatrix = CardboardManager.projectionMatrixLeft;
            if (!CardboardManager.projectionMatrixRight.Equals(Matrix4x4.zero))
                rightCam.projectionMatrix = CardboardManager.projectionMatrixRight;

            testEyeMeshLeft.mesh = CardboardManager.viewMeshLeft;
            testEyeMeshRight.mesh = CardboardManager.viewMeshRight;

            NativeDataExtract.Save_MeshJson(CardboardManager.viewMeshLeftRaw);
            NativeDataExtract.Save_MeshJson(CardboardManager.viewMeshRightRaw);

            (byte[], int) paramDetailVar = CardboardQrCode.GetDeviceParamsByte();
            NativeDataExtract.Save_EncodedParam(paramDetailVar.Item1, paramDetailVar.Item2);
        }

        // Update is called once per frame
        void Update()
        {
            CardboardHeadTracker.UpdatePose();
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
            SetEnableQROverlay(true);
        }

        private void SetEnableQROverlay(bool shouldEnable)
        {
            continuePanel.SetActive(shouldEnable);
        }
    }
}