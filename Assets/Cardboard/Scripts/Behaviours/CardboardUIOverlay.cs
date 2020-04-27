using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MobfishCardboard
{
    public class CardboardUIOverlay: MonoBehaviour
    {
        public Button scanQRButton;
        public Text profileParamText;
        public Button refreshButton;
        public Button continueButton;
        public GameObject continuePanel;

        private bool overlayIsOpen;

        private void Awake()
        {
            SetEnableQROverlay(false);
            continueButton.onClick.AddListener(ContinueClicked);
            scanQRButton.onClick.AddListener(ScanQRCode);
            refreshButton.onClick.AddListener(RefreshClicked);
        }

        // Start is called before the first frame update
        void Start()
        {
            if (!CardboardManager.profileAvailable)
            {
                SetEnableQROverlay(true);
            }
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

        private void RefreshClicked()
        {
            TriggerRefresh();
        }

        private void TriggerRefresh()
        {
            //CardboardManager.RefreshParameters();

            if (!CardboardManager.profileAvailable)
            {
                SetEnableQROverlay(true);
            }

            if (CardboardManager.deviceParameter != null)
            {
                profileParamText.text =
                    CardboardManager.deviceParameter.Model + " " + CardboardManager.deviceParameter.Vendor;
            }
        }
    }
}