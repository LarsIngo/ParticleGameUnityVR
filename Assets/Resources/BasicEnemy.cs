using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {

    private int mHealth;

    public int Health { get { return mHealth; } set { mHealth = value; startHealth = value; } }

    int startHealth;
	// Use this for initialization
	void Start () {

        Health = 100;
        startHealth = mHealth;
        TempVisuals(gameObject, PrimitiveType.Sphere, Color.green);

        

	}
	
	// Update is called once per frame
	void Update () {

        if(Health < 0)
        {

            Destroy(this);

        }

        GetComponent<Renderer>().material.color = Color.Lerp(Color.green, Color.red, Health / startHealth);
        		
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
