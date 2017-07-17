using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractorWand : MonoBehaviour {

    GameObject wand;
    GameObject rod;

    GameObject tip;

	// Use this for initialization
	void Start () {

        //We create the various parts.
        wand = new GameObject("Wand");
        wand.transform.parent = gameObject.transform;
        MeshRenderer renderer = wand.AddComponent<MeshRenderer>();
        MeshFilter filter = wand.AddComponent<MeshFilter>();

        rod = new GameObject("Rod");
        rod.transform.parent = wand.transform;

        tip = new GameObject("Tip");
        tip.transform.parent = wand.transform;



    }

    // Update is called once per frame
    void Update () {
		
	}
}
