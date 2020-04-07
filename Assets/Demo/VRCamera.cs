using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MobfishCardboard;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class VRCamera : MonoBehaviour
{
    [Header("Cameras")]
    public Camera centerCam;
    public Camera leftCam;
    public Camera rightCam;

    [Header("Other")]
    public Button scanQRButton;
    public Text debugText;
    public MeshFilter testEyeMeshLeft;
    public MeshFilter testEyeMeshRight;

    private RenderTextureDescriptor eyeRenderTextureDesc;
    private RenderTexture centerRenderTexture;

    private void Awake()
    {
        scanQRButton.onClick.AddListener(ScanQRCode);
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
    }

    // Start is called before the first frame update
    void Start()
    {
        CardboardHeadTracker.CreateTracker();
        CardboardHeadTracker.ResumeTracker();
        CardboardQrCode.RetrieveDeviceParam();
        CardboardDistortionRenderer.InitDestortionRenderer();
        (IntPtr, int) par = CardboardQrCode.GetDeviceParamsPointer();
        CardboardLensDistortion.CreateLensDistortion(par.Item1, par.Item2);
        RefreshCamera();
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
        testEyeMeshLeft.mesh = CardboardUtility.ConvertCardboardMesh(eyeMeshes.Item1);
        testEyeMeshRight.mesh = CardboardUtility.ConvertCardboardMesh(eyeMeshes.Item2);
    }

    // Update is called once per frame
    void Update()
    {
        CardboardHeadTracker.UpdatePoseGyro();
        transform.localRotation = CardboardHeadTracker.trackerUnityRotation;
        Update_DebugInfo();
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
        CardboardQrCode.RetrieveDeviceParam();
        RefreshCamera();
    }
}