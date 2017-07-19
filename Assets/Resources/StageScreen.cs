using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageScreen : MonoBehaviour {

    public Stage stage;

	// Use this for initialization
	void Awake () {

        transform.parent = transform;
        tag = "StageScreen";
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
