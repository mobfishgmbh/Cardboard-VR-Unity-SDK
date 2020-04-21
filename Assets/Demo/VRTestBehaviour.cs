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
        public MeshFilter testEyeMeshLeft;
        public MeshFilter testEyeMeshRight;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnEnable()
        {
            CardboardManager.cardboardProfileChanged += RefreshCameraProperty;
        }

        private void OnDisable()
        {
            CardboardManager.cardboardProfileChanged -= RefreshCameraProperty;
        }

        // Update is called once per frame
        void Update()
        {
            debugText.text = string.Format("device rot={0}, \r\nUnity rot={1}",
                CardboardHeadTracker.trackerRawRotation.eulerAngles,
                CardboardHeadTracker.trackerUnityRotation.eulerAngles);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            Debug.Log("OnApplicationFocus called, hasFocus=" + hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            Debug.Log("OnApplicationPause called, pauseStatus=" + pauseStatus);
        }

        private void RefreshCameraProperty()
        {
            testEyeMeshLeft.mesh = CardboardManager.viewMeshLeft;
            testEyeMeshRight.mesh = CardboardManager.viewMeshRight;

            if (!Application.isEditor)
            {
                NativeDataExtract.Save_MeshJson(CardboardManager.viewMeshLeftRaw);
                NativeDataExtract.Save_MeshJson(CardboardManager.viewMeshRightRaw);
            }

            // (byte[], int) paramDetailVar = CardboardQrCode.GetDeviceParamsByte();
            // NativeDataExtract.Save_EncodedParam(paramDetailVar.Item1, paramDetailVar.Item2);
        }
    }

    
}