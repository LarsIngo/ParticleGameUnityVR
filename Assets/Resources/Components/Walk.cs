using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : MonoBehaviour {

    public Vector3 direction;
    public float speed;
	
	// Update is called once per frame
	void Update () {

        transform.position += direction * speed * Time.deltaTime;

	}
}
