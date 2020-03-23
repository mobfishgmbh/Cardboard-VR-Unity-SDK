using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MobfishCardboard;
using UnityEngine;
using UnityEngine.UI;

public class VRCamera: MonoBehaviour
{
    public Button scanQRButton;
    public Text debugText;

    private void Awake()
    {
        scanQRButton.onClick.AddListener(ScanQRCode);
    }

    // Start is called before the first frame update
    void Start()
    {
        CardboardHeadTracker.CreateTracker();
        CardboardHeadTracker.ResumeTracker();
        CardboardQrCode.RetrieveDeviceParam();
    }

    // Update is called once per frame
    void Update()
    {
        CardboardHeadTracker.UpdatePose();
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
    }
}