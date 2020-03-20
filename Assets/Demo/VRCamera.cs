using System;
using System.Collections;
using System.Collections.Generic;
using MobfishCardboard;
using UnityEngine;

public class VRCamera: MonoBehaviour
{
    private void Awake()
    {
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
}