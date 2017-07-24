using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vatsug1 : MonoBehaviour
{

    private Vector3 prevPos;
    private AudioSource sound;
    private float soundDelay;
    private bool swap;
    private Vector3 mul;
    private Vector3 offs;
    private int algoritm;
    private float timeoffset;
    

    // Use this for initialization
    void Start()
    {
        gameObject.transform.position = new Vector3(100, 100, 100);
        prevPos = new Vector3(0, 0, 0);
        sound = gameObject.GetComponentInChildren<AudioSource>();
        Debug.Assert(sound);
        
        mul = new Vector3(1.0f, 0.4f, 1.0f);
        offs = new Vector3(0, Random.Range(1.0f, 3.0f), 0);

        algoritm = Random.Range(0, 2);

        timeoffset = Random.Range(0.0f, Mathf.PI * 2 - 0.01f);

        if (Random.Range(0, 2) == 1)
        {
            swap = true;
        }

        soundDelay = Random.Range(0.0f, 30.0f);
    }

    // Update is called once per frame
    void Update()
    {

        float t = Time.time + timeoffset;
        soundDelay -= Time.deltaTime;
        if (swap)
        {
            t *= -1;
        }


        Vector3 newPos = new Vector3(0, 0, 0);
        if (algoritm == 0)
            newPos = new Vector3(Mathf.Cos(t * 2) * mul.x,
                 Mathf.Sin(t / 2) * mul.y, Mathf.Sin(t * 2) * mul.z) + offs;
        else if (algoritm == 1)
        {
            newPos = new Vector3(Mathf.Sin(t / 2),
                 Mathf.Cos(t * 2) * mul.y, Mathf.Cos(t / 2)) + offs;

        }
       

        
        gameObject.transform.position = prevPos;
        gameObject.transform.LookAt(newPos);
        prevPos = newPos;

        if (soundDelay <= 0.0f)
        {
            sound.Play();
            soundDelay = Random.Range(6.0f, 26.0f);
        }
    }
}
