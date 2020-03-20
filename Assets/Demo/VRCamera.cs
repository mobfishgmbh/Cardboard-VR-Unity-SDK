using System;
using System.Collections;
using System.Collections.Generic;
using MobfishCardboard;
using UnityEngine;

public class VRCamera: MonoBehaviour
{
    private void Awake()
    {
        CardboardHeadTracker.CreateTracker();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = CardboardHeadTracker.GetPose();
    }
}