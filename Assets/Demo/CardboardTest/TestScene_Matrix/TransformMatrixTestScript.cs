using System.Collections;
using System.Collections.Generic;
using MobfishCardboard;
using UnityEngine;

namespace MobfishCardboardDemo.SceneTest
{
    public class TransformMatrixTestScript: MonoBehaviour
    {
        public Transform testTransform;

        // Matrix, col 0 to col 3:
        // right, up, forward, all of them are normalized
        void Start()
        {
            Debug.LogFormat("Origin Pos={0}, Quat={1}, \r\neuler={2}, \r\nup={3}, forward={4}",
                testTransform.localPosition.ToString("F4"),
                testTransform.localRotation.ToString("F4"),
                testTransform.localEulerAngles.ToString("F4"),
                testTransform.up.ToString("F4"),
                testTransform.forward.ToString("F4"));

            Matrix4x4 testMat = CardboardUtility.GetTransformTRSMatrix(testTransform);

            Debug.LogFormat("matrix=\r\n{0}", testMat);

            Pose newPose = CardboardUtility.GetPoseFromTRSMatrix(testMat);

            Debug.LogFormat("newPose Pos={0}, Quat={1}, euler={2}",
                newPose.position.ToString("F4"),
                newPose.rotation.ToString("F4"),
                newPose.rotation.eulerAngles.ToString("F4"));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}