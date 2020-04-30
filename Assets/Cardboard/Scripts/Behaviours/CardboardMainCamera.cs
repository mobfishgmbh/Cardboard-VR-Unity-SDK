using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

namespace MobfishCardboard
{
    public class CardboardMainCamera: MonoBehaviour
    {
        [Header("Cameras")]
        [SerializeField]
        private Camera novrCam;
        [SerializeField]
        private Camera leftCam;
        [SerializeField]
        private Camera rightCam;
        [SerializeField]
        private GameObject vrCamGroup;
        [SerializeField]
        private GameObject novrCamGroup;

        [Header("Options")]
        [SerializeField]
        private bool defaultEnableVRView;
        [Tooltip("Set this game object DontDestroyOnLoad. If it's not needed or any parent GameObject already have DontDestroyOnLoad, disable it")]
        [SerializeField]
        private bool dontDestroyOnLoad;

        private RenderTextureDescriptor eyeRenderTextureDesc;
        private bool overlayIsOpen;

        private void Awake()
        {
            #if UNITY_IOS
            Application.targetFrameRate = 60;

            #endif

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            SetupRenderTexture();

            CardboardManager.InitCardboard();
            CardboardManager.SetVRViewEnable(defaultEnableVRView);
        }

        // Start is called before the first frame update
        void Start()
        {
            RefreshCamera();
            CardboardManager.deviceParamsChangeEvent += RefreshCamera;
            SwitchVRCamera();
            CardboardManager.enableVRViewChangedEvent += SwitchVRCamera;
        }

        private void OnDestroy()
        {
            CardboardManager.deviceParamsChangeEvent -= RefreshCamera;
            CardboardManager.enableVRViewChangedEvent -= SwitchVRCamera;
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

        private void SwitchVRCamera()
        {
            vrCamGroup.SetActive(CardboardManager.enableVRView);
            novrCamGroup.SetActive(!CardboardManager.enableVRView);
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


            // if (CardboardManager.deviceParameter != null)
            // {
            //     leftCam.transform.localPosition =
            //         new Vector3(-CardboardManager.deviceParameter.InterLensDistance / 2, 0, 0);
            //     rightCam.transform.localPosition =
            //         new Vector3(CardboardManager.deviceParameter.InterLensDistance / 2, 0, 0);
            // }
        }

        private static void RefreshCamera_Eye(Camera eyeCam, Matrix4x4 projectionMat, Matrix4x4 eyeFromHeadMat)
        {
            if (!projectionMat.Equals(Matrix4x4.zero))
                eyeCam.projectionMatrix = projectionMat;

            //https://github.com/googlevr/cardboard/blob/master/sdk/lens_distortion.cc
            if (!eyeFromHeadMat.Equals(Matrix4x4.zero))
            {
                Pose eyeFromHeadPoseGL = CardboardUtility.GetPoseFromTRSMatrix(eyeFromHeadMat);
                eyeFromHeadPoseGL.position.x = -eyeFromHeadPoseGL.position.x;
                eyeCam.transform.localPosition = eyeFromHeadPoseGL.position;
                eyeCam.transform.localRotation = eyeFromHeadPoseGL.rotation;
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}