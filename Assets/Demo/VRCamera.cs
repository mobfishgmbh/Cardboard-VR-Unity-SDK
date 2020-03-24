using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MobfishCardboard;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class VRCamera: MonoBehaviour
{
    [Header("Cameras")]
    public Camera centerCam;
    public Camera leftCam;
    public Camera rightCam;

    [Header("Other")]
    public Button scanQRButton;
    public Text debugText;

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
            depthBufferBits = 16
        };
        centerRenderTexture = new RenderTexture(eyeRenderTextureDesc);
        centerCam.targetTexture = centerRenderTexture;
    }

    private void OnPostRender()
    {

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

    private void RefreshCamera()
    {
        leftCam.projectionMatrix = CardboardLensDistortion.GetProjectionMatrix(CardboardEye.kLeft);
        rightCam.projectionMatrix = CardboardLensDistortion.GetProjectionMatrix(CardboardEye.kRight);
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