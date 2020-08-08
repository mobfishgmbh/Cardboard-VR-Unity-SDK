using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MobfishCardboard;

namespace MobfishCardboardDemo
{
    public class VRTestBehaviour: MonoBehaviour
    {
        public Text debugText;
        public Text debugTextStatic;
        public MeshFilter testEyeMeshLeft;
        public MeshFilter testEyeMeshRight;
        public Transform testHeadFollower;

        private void Awake()
        {
            debugTextStatic.text = string.Format("System: \r\n GraphicAPI={0}", SystemInfo.graphicsDeviceType);
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        private void OnEnable()
        {
            CardboardManager.deviceParamsChangeEvent += RefreshCameraProperty;
            CardboardManager.deviceParamsChangeEvent += DeviceParamsChanged;
        }

        private void OnDisable()
        {
            CardboardManager.deviceParamsChangeEvent -= RefreshCameraProperty;
            CardboardManager.deviceParamsChangeEvent -= DeviceParamsChanged;
        }

        // Update is called once per frame
        void Update()
        {
            debugText.text = string.Format("device rot={0}, \r\nUnity rot={1},\r\nUnity FPS={2}",
                CardboardHeadTracker.trackerRawRotation.eulerAngles,
                CardboardHeadTracker.trackerUnityRotation.eulerAngles,
                FramerateCount.fpsString);

            Pose headPose = CardboardManager.GetHeadPose();
            testHeadFollower.localRotation = headPose.rotation;
            testHeadFollower.localPosition = headPose.position;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            Debug.Log("OnApplicationFocus called, hasFocus=" + hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            Debug.Log("OnApplicationPause called, pauseStatus=" + pauseStatus);
        }

        private void DeviceParamsChanged()
        {
            // debugTextStatic.text = string.Format("eyefromhead=\r\n{0}", CardboardManager.eyeFromHeadMatrixLeft);
            Debug.Log("qr code scan count=" + CardboardQrCode.GetQRCodeScanCount());
        }

        private void RefreshCameraProperty()
        {
            testEyeMeshLeft.mesh = CardboardManager.viewMeshLeft;
            testEyeMeshRight.mesh = CardboardManager.viewMeshRight;

            // if (!Application.isEditor)
            // {
            //     NativeDataExtract.Save_MeshJson(CardboardManager.viewMeshLeftRaw);
            //     NativeDataExtract.Save_MeshJson(CardboardManager.viewMeshRightRaw);
            // }

            // (byte[], int) paramDetailVar = CardboardQrCode.GetDeviceParamsByte();
            // NativeDataExtract.Save_EncodedParam(paramDetailVar.Item1, paramDetailVar.Item2);
        }
    }


}