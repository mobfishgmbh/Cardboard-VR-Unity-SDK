using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FramerateCount: MonoBehaviour
{
    public static string fpsString { get; private set; }

    void Update()
    {
        float fps = 1.0f / Time.deltaTime;
        fpsString = Mathf.Ceil(fps).ToString();
    }
}