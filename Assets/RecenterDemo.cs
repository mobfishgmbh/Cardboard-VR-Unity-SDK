using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MobfishCardboard;

public class RecenterDemo : MonoBehaviour
{
	public float recenterInterval = 3;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(recenterRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator recenterRoutine(){
    	WaitForSeconds wait = new WaitForSeconds(recenterInterval);

    	while(true){
    		yield return wait;
    		CardboardHeadTracker.RecenterCamera();	
    	}
    	
    }
}
