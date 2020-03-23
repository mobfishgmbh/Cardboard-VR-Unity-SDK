using System;
using System.Collections;
using System.Collections.Generic;
using MobfishCardboard;
using UnityEngine;
using UnityEngine.UI;

public class VRCamera: MonoBehaviour
{
    public Button scanQRButton;

    private void Awake()
    {
        scanQRButton.onClick.AddListener(ScanQRCode);
    }

    // Start is called before the first frame update
    void Start()
    {
        CardboardHeadTracker.CreateTracker();
        CardboardHeadTracker.ResumeTracker();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = CardboardHeadTracker.GetPose();
    }

    private void ScanQRCode()
    {
        CardboardQrCode.StartScanQrCode();
    }
}