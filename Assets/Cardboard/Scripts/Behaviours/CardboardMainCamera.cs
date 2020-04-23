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
                width = Screen.width / 2,
                height = Screen.height,
                depthBufferBits = 16,
                volumeDepth = 1,
                msaaSamples = 1,
                vrUsage = VRTextureUsage.OneEye
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

            RefreshCamera_Eye(leftCam,
                CardboardManager.projectionMatrixLeft, CardboardManager.eyeFromHeadMatrixLeft);
            RefreshCamera_Eye(rightCam,
                CardboardManager.projectionMatrixRight, CardboardManager.eyeFromHeadMatrixRight);


            if (CardboardManager.deviceParameter != null)
            {
                leftCam.transform.localPosition =
                    new Vector3(-CardboardManager.deviceParameter.InterLensDistance / 2, 0, 0);
                rightCam.transform.localPosition =
                    new Vector3(CardboardManager.deviceParameter.InterLensDistance / 2, 0, 0);
            }
        }

        private static void RefreshCamera_Eye(Camera eyeCam, Matrix4x4 projectionMat, Matrix4x4 eyeFromHeadMat)
        {
            if (!projectionMat.Equals(Matrix4x4.zero))
                eyeCam.projectionMatrix = projectionMat;

            // if (!eyeFromHeadMat.Equals(Matrix4x4.zero))
            // {
            //     Transform camTransform = eyeCam.transform;
            //     Matrix4x4 newPoseMat = eyeFromHeadMat * CardboardUtility.GetTransformTRSMatrix(camTransform);
            //
            //     Pose newPoseFinal = CardboardUtility.GetPoseFromTRSMatrix(newPoseMat);
            //     camTransform.localPosition = newPoseFinal.position;
            //     camTransform.localRotation = newPoseFinal.rotation;
            // }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}