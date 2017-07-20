using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.AddComponent<MeshRenderer>().material = (Material)Resources.Load("Boat/Meshes/Materials/Boat_MAT", typeof(Material));
        gameObject.AddComponent<MeshFilter>().mesh = (Mesh)Resources.Load("Boat/Meshes/Boat_Mesh", typeof(Mesh));
        gameObject.transform.Rotate(new Vector3(-90, 90, 0));
    }

    // Update is called once per frame
    void Update () {
		
	}
}
