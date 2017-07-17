using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Main : MonoBehaviour
{

    GameObject mParticleSystem;

    GameObject mHMD;
    public GameObject mLeftController;
    public GameObject mRightController;

    private void Start ()
    {
        mHMD = GameObject.Find("Camera (head)");

        if (mLeftController == null)
        {
            mLeftController = new GameObject("STATIC LEFT CONTROLLER");
            mLeftController.transform.position = new Vector3(-3,0,5);
        }
        if (mRightController == null)
        {
            mRightController = new GameObject("STATIC RIGHT CONTROLLER");
            mRightController.transform.position = new Vector3(3, 0, 5);
        }

        mRightController.AddComponent<AttractorWand>();

    }

    private void Update()
    {

    }

    private void OnDestroy()
    {

    }

}
