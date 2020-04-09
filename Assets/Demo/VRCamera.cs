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
            eyeRenderTextureDesc = new RenderTextureDescriptor()
            {
                dimension = TextureDimension.Tex2D,
                width = Screen.width / 2,
                height = Screen.height / 2,
                depthBufferBits = 16,
                volumeDepth = 1,
                msaaSamples = 2
            };
            centerRenderTexture = new RenderTexture(eyeRenderTextureDesc);
            centerCam.targetTexture = centerRenderTexture;

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

        private void ResetProfile()
        {
            CardboardQrCode.RetrieveDeviceParam();
            (IntPtr, int) par = CardboardQrCode.GetDeviceParamsPointer();

            if (par.Item2 == 0)
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
            leftCam.projectionMatrix = CardboardLensDistortion.GetProjectionMatrix(CardboardEye.kLeft);
            rightCam.projectionMatrix = CardboardLensDistortion.GetProjectionMatrix(CardboardEye.kRight);
            (CardboardMesh, CardboardMesh) eyeMeshes = CardboardLensDistortion.GetEyeMeshes();
            CardboardDistortionRenderer.SetEyeMeshes(eyeMeshes.Item1, eyeMeshes.Item2);
            testEyeMeshLeft.mesh = CardboardUtility.ConvertCardboardMesh_LineStrip(eyeMeshes.Item1);
            testEyeMeshRight.mesh = CardboardUtility.ConvertCardboardMesh_LineStrip(eyeMeshes.Item2);

            NativeDataExtract.Save_MeshJson(eyeMeshes.Item1);
            NativeDataExtract.Save_MeshJson(eyeMeshes.Item2);
        }

        // Update is called once per frame
        void Update()
        {
            CardboardHeadTracker.UpdatePoseGyro();
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