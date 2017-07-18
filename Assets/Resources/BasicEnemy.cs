using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour {

    public int health;

	// Use this for initialization
	void Start () {

        health = 100;

        TempVisuals(gameObject, PrimitiveType.Sphere, Color.green);

	}
	
	// Update is called once per frame
	void Update () {

        if(health < 0)
        {
            Factory.CreateMichaelBayEffect(this.GetComponentInChildren<MeshFilter>().mesh);
            Destroy(this);

        }
		
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
