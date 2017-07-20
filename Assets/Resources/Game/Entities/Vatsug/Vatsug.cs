using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vatsug : MonoBehaviour {

    private Vector3 prevPos;
    
    // Use this for initialization
    void Start () {

        Debug.Assert(gameObject.GetComponent<GPUParticleSphereCollider>());

        prevPos = new Vector3(0, 0, 0);
        

    }
	
	// Update is called once per frame
	void Update () {

        /*if (VrInput.LeftTrigger() == 1.0f || VrInput.RightTrigger() == 1.0f)
        {
            Factory.CreateMichaelBayEffect(Hub.Instance.ActiveLevel, GetComponent<MeshFilter>().mesh, transform, new Color(1, 1, 1, 1));
            Destroy(gameObject);
        }
        else
        {
            mHP -= gameObject.GetComponent<GPUParticleSphereCollider>().CollisionsThisFrame;
        }*/


        Vector3 newPos = new Vector3(Mathf.Tan(Time.time / 8.0f), Mathf.Cos(Time.time) * 3.0f + 1.0f, Mathf.Sin(Time.time / 2) * 3.0f);

        gameObject.transform.position = prevPos;
        gameObject.transform.LookAt(newPos);
        prevPos = newPos;
    }
}
