using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MobfishCardboard;
using UnityEngine;
using UnityEngine.UI;

public class VRCamera: MonoBehaviour
{
    [Header("Cameras")]
    public Camera leftCam;
    public Camera rightCam;

    [Header("Other")]
    public Button scanQRButton;
    public Text debugText;
    public MeshFilter leftEyeMesh;
    public MeshFilter rightEyeMesh;

    private void Awake()
    {
        scanQRButton.onClick.AddListener(ScanQRCode);
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        CardboardHeadTracker.CreateTracker();
        CardboardHeadTracker.ResumeTracker();
        CardboardQrCode.RetrieveDeviceParam();
        (IntPtr, int) par = CardboardQrCode.GetDeviceParamsPointer();
        CardboardLensDistortion.CreateLensDistortion(par.Item1, par.Item2);
        RefreshCamera();
    }

    private void RefreshCamera()
    {
        leftCam.projectionMatrix = CardboardLensDistortion.GetProjectionMatrix(CardboardEye.kLeft);
        rightCam.projectionMatrix = CardboardLensDistortion.GetProjectionMatrix(CardboardEye.kRight);
        leftEyeMesh.mesh = CardboardLensDistortion.GetDistortionMesh(CardboardEye.kLeft);
        rightEyeMesh.mesh = CardboardLensDistortion.GetDistortionMesh(CardboardEye.kRight);
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