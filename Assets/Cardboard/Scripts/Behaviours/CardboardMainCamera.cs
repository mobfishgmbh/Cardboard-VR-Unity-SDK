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

        private RenderTextureDescriptor eyeRenderTextureDesc;
        private bool overlayIsOpen;

        private void Awake()
        {
            #if UNITY_IOS
            Application.targetFrameRate = 60;

            #endif

            SetupRenderTexture();

            CardboardManager.InitCardboard();
        }

        // Start is called before the first frame update
        void Start()
        {
            RefreshCamera();
            CardboardManager.deviceParamsChangeEvent += RefreshCamera;
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

        private void RefreshCamera()
        {
            if (!CardboardManager.profileAvailable)
            {
                return;
            }

            if (!CardboardManager.projectionMatrixLeft.Equals(Matrix4x4.zero))
                leftCam.projectionMatrix = CardboardManager.projectionMatrixLeft;
            if (!CardboardManager.projectionMatrixRight.Equals(Matrix4x4.zero))
                rightCam.projectionMatrix = CardboardManager.projectionMatrixRight;

            if (CardboardManager.deviceParameter != null)
            {
                leftCam.transform.localPosition =
                    new Vector3(-CardboardManager.deviceParameter.InterLensDistance / 2, 0, 0);
                rightCam.transform.localPosition =
                    new Vector3(CardboardManager.deviceParameter.InterLensDistance / 2, 0, 0);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}