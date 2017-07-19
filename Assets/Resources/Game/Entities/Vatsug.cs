using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vatsug : MonoBehaviour {

    private int mHP = 5000000;

    private Vector3 prevPos;

    private GPUParticleSphereCollider particleColider;
    // Use this for initialization
    void Start () {

        prevPos = new Vector3(0, 0, 0);

        GameObject StraitenOutFishObject = new GameObject();
        StraitenOutFishObject.AddComponent<MeshRenderer>().material = (Material)Resources.Load("nnj3de_crucarp/Materials/cruscarp", typeof(Material));
        StraitenOutFishObject.transform.Rotate(new Vector3(-90, 0, 0));
        StraitenOutFishObject.AddComponent<MeshFilter>().mesh = (Mesh)Resources.Load("nnj3de_crucarp/cruscarp", typeof(Mesh));
        StraitenOutFishObject.transform.localScale = new Vector3(300, 200, 200);
        StraitenOutFishObject.transform.parent = gameObject.transform;
        StraitenOutFishObject.transform.localPosition = new Vector3(0, 0, -0.25f);
        particleColider = StraitenOutFishObject.AddComponent<GPUParticleSphereCollider>();
        particleColider.Radius = 0.25f;

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
            mHP -= particleColider.CollisionsThisFrame;
        }

        float x = Mathf.Tan(Time.time / 8.0f);
        float y = Mathf.Cos(Time.time) * 3.0f + 1.0f;
        float z = Mathf.Sin(Time.time / 2) * 3.0f;// Mathf.Cos(Time.time / 3) * 3.0f;

        Vector3 newPos = new Vector3(x, y, z);

        gameObject.transform.position = prevPos;
        gameObject.transform.LookAt(newPos);
        prevPos = newPos;
    }
}
