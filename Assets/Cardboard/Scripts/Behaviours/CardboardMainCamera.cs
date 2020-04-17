using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace MobfishCardboard
{
    public class CardboardMainCamera: MonoBehaviour
    {
        [Header("Cameras")]
        public Camera novrCam;
        public Camera leftCam;
        public Camera rightCam;

        [Header("QR")]
        public Button scanQRButton;
        public Text profileParamText;
        public Button refreshButton;
        public Button continueButton;
        public GameObject continuePanel;

        private RenderTextureDescriptor eyeRenderTextureDesc;
        private bool overlayIsOpen;

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
            refreshButton.onClick.AddListener(RefreshCamera);
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
            RefreshCamera();

            SetEnableQROverlay(false);
        }

        private void RefreshCamera()
        {
            CardboardManager.RefreshParameters();

            if (!CardboardManager.profileAvailable)
            {
                SetEnableQROverlay(true);
                return;
            }

            if (!CardboardManager.projectionMatrixLeft.Equals(Matrix4x4.zero))
                leftCam.projectionMatrix = CardboardManager.projectionMatrixLeft;
            if (!CardboardManager.projectionMatrixRight.Equals(Matrix4x4.zero))
                rightCam.projectionMatrix = CardboardManager.projectionMatrixRight;
        }

        // Update is called once per frame
        void Update()
        {
            CardboardHeadTracker.UpdatePose();
            if (!Application.isEditor)
                transform.localRotation = CardboardHeadTracker.trackerUnityRotation;

            //todo not good here, find a way around
            if (overlayIsOpen && CardboardManager.deviceParameter != null)
            {
                profileParamText.text =
                    CardboardManager.deviceParameter.Model + " " + CardboardManager.deviceParameter.Vendor;
            }
        }

        private void ScanQRCode()
        {
            CardboardQrCode.StartScanQrCode();
            SetEnableQROverlay(true);
        }

        private void SetEnableQROverlay(bool shouldEnable)
        {
            continuePanel.SetActive(shouldEnable);
            overlayIsOpen = shouldEnable;
        }
    }
}