using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vatsug : MonoBehaviour {

    private int mHP = 5000000;


	// Use this for initialization
	void Start () {

        
        
        GameObject StraitenOutFishObject = new GameObject();
        StraitenOutFishObject.AddComponent<MeshRenderer>().material = (Material)Resources.Load("nnj3de_crucarp/Materials/cruscarp", typeof(Material));
        StraitenOutFishObject.transform.Rotate(new Vector3(-180, 0, 0));
        StraitenOutFishObject.AddComponent<MeshFilter>().mesh = (Mesh)Resources.Load("nnj3de_crucarp/cruscarp", typeof(Mesh));
        StraitenOutFishObject.transform.localScale = new Vector3(300, 200, 200);
        StraitenOutFishObject.transform.parent = gameObject.transform;
        StraitenOutFishObject.transform.localPosition = new Vector3(0, 0, -0.25f);
        StraitenOutFishObject.AddComponent<GPUParticleSphereCollider>().Radius = 0.5f;

    }
	
	// Update is called once per frame
	void Update () {
        if (mHP < 0)
        {
            Factory.CreateMichaelBayEffect(Hub.Instance.ActiveLevel, GetComponent<MeshFilter>().mesh, transform, GetComponent<Renderer>().material.color);
            Destroy(gameObject);
        }
        else
        {
            mHP -= GetComponent<GPUParticleSphereCollider>().CollisionsThisFrame;
        }
    }
}
