using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MobfishCardboard;

namespace MobfishCardboardDemo
{
    public class RecenterScript : MonoBehaviour
    {
        public Button recenterButton;

        // Start is called before the first frame update
        void Start()
        {
            if (recenterButton != null)
                recenterButton.onClick.AddListener(RecenterCamera);

        }

        // Update is called once per frame
        void RecenterCamera()
        {
            CardboardManager.RecenterCamera();
        }
    }
}