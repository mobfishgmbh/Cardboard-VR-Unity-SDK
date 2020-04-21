using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobfishCardboard
{
    public class SimulateHeadTransform: MonoBehaviour
    {
        [SerializeField]
        private Transform targetTransform;

        private void Awake()
        {
            if (targetTransform == null)
                targetTransform = GetComponent<Transform>();

            if (!Application.isEditor)
                enabled = false;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                Vector3 currentEulerAngle = transform.localEulerAngles;
                float targetRotX = currentEulerAngle.x - Input.GetAxis("Mouse Y");
                if (targetRotX < 90 || targetRotX > -90)
                {
                    currentEulerAngle.x = targetRotX;
                }
                float targetRotY = currentEulerAngle.y + Input.GetAxis("Mouse X");
                if (targetRotY > 360)
                    targetRotY -= 360;
                else if (targetRotY < -360)
                    targetRotY += 360;
                currentEulerAngle.y = targetRotY;

                transform.localEulerAngles = currentEulerAngle;
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                Vector3 currentEulerAngle = transform.localEulerAngles;
                float targetRotZ = currentEulerAngle.z - Input.GetAxis("Mouse X");

                currentEulerAngle.z = targetRotZ;
                transform.localEulerAngles = currentEulerAngle;
            }

        }
    }
}