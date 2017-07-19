using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageScreen : MonoBehaviour {

    GameObject screen;

	// Use this for initialization
	void Awake () {

        screen = GameObject.CreatePrimitive(PrimitiveType.Quad);
        screen.transform.parent = transform;
        screen.tag = "StageScreen";
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
