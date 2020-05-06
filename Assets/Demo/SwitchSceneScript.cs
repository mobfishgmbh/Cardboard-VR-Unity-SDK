using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MobfishCardboardDemo
{
    public class SwitchSceneScript: MonoBehaviour
    {
        public Button switchButton;
        public int targetSceneId;

        // Start is called before the first frame update
        void Start()
        {
            if (switchButton != null)
                switchButton.onClick.AddListener(SwitchScene);
        }

        private void SwitchScene()
        {
            if (targetSceneId >= 0)
                SceneManager.LoadScene(targetSceneId);
        }
    }
}