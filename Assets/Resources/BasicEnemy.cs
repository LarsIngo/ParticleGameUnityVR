using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {

    private int mHealth;

    public int Health { get { return mHealth; } set { mHealth = value; startHealth = value; } }

    int startHealth;
	// Use this for initialization
	void Start () {

        Health = 10000;
        startHealth = mHealth;
        TempVisuals(gameObject, PrimitiveType.Sphere, Color.green);

        gameObject.AddComponent<GPUParticleSphereCollider>();

	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(Health);
        if(Health < 0)
        {

            Destroy(gameObject);

        }

        GetComponent<Renderer>().material.color = Color.Lerp(Color.green, Color.red, 1 - ((float)Health / startHealth));

        mHealth -= GetComponent<GPUParticleSphereCollider>().CollisionsThisFrame;
        	
	}

    void TempVisuals(GameObject target, PrimitiveType primitive, Color color)
    {

        GameObject tmp = GameObject.CreatePrimitive(primitive);
        MeshRenderer renderer = target.AddComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = color;
        renderer.material = mat;
        MeshFilter filter = target.AddComponent<MeshFilter>();
        filter.mesh = tmp.GetComponent<MeshFilter>().mesh;
        Destroy(tmp);

    }

}
