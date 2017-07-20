using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Moon : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Light moonLight = new Light();
        moonLight.type = LightType.Directional;
        moonLight.shadows = LightShadows.Soft;
        moonLight.color = new Color(0.25f, 0.25f, 0.25f, 1.0f);
        //GameObject moon = 
        
        moonLight.transform.LookAt(new Vector3(0, 0, 0));
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
