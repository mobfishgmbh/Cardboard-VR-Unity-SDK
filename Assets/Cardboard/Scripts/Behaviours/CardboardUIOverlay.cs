using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MobfishCardboard
{
    public class CardboardUIOverlay: MonoBehaviour
    {
        //Only used in dontDestroyAndSingleton
        private static CardboardUIOverlay instance;

        public Button scanQRButton;
        public Button switchVRButton;
        public Text profileParamText;
        public Button continueButton;
        public GameObject continuePanel;

        [Header("Options")]
        [SerializeField]
        private bool dontDestroyAndSingleton;

        private bool overlayIsOpen;

        private void Awake()
        {
            if (dontDestroyAndSingleton)
            {
                if (instance == null)
                {
                    DontDestroyOnLoad(gameObject);
                    instance = this;
                }
                else if (instance != this)
                {
                    Destroy(gameObject);
                    return;
                }
            }

            SetEnableQROverlay(false);
            continueButton.onClick.AddListener(ContinueClicked);
            scanQRButton.onClick.AddListener(ScanQRCode);
            switchVRButton.onClick.AddListener(SwitchVRView);
        }

        // Start is called before the first frame update
        void Start()
        {

            TriggerRefresh();
            scanQRButton.gameObject.SetActive(CardboardManager.enableVRView);
            CardboardManager.deviceParamsChangeEvent += TriggerRefresh;
        }

        private void OnDestroy()
        {
            CardboardManager.deviceParamsChangeEvent -= TriggerRefresh;
        }

        private void ScanQRCode()
        {
            CardboardQrCode.StartScanQrCode();
            SetEnableQROverlay(true);
        }

        private void SwitchVRView()
        {
            CardboardManager.SetVRViewEnable(!CardboardManager.enableVRView);

            SetEnableQROverlay(false);
            scanQRButton.gameObject.SetActive(CardboardManager.enableVRView);

            TriggerRefresh();
        }

        private void SetEnableQROverlay(bool shouldEnable)
        {
            continuePanel.SetActive(shouldEnable);
            overlayIsOpen = shouldEnable;
        }

        private void ContinueClicked()
        {
            TriggerRefresh();

            SetEnableQROverlay(false);
        }

        private void TriggerRefresh()
        {
            //CardboardManager.RefreshParameters();

            if (CardboardManager.enableVRView && !CardboardManager.profileAvailable)
            {
                SetEnableQROverlay(true);
            }

            if (CardboardManager.deviceParameter != null)
            {
                profileParamText.text =
                    CardboardManager.deviceParameter.Vendor + " " + CardboardManager.deviceParameter.Model;
            }
        }
    }
}