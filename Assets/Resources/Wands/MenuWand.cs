using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWand : MonoBehaviour {

    GameObject mWandGO;

    GameObject mRodGO;

    GameObject mTipGO;
    LineRenderer lineRenderer;

    public bool rightHand;
    public float power;

    // Use this for initialization
    void Awake()
    {

        power = 20;

        //We create the various parts.
        InitGameObjects();

        mWandGO.transform.Rotate(90, 0, 0);
        mWandGO.transform.position += Vector3.forward * 0.2f;

        lineRenderer = mTipGO.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.red;
        lineRenderer.widthMultiplier = 0.01f;
        lineRenderer.positionCount = 2;

    }

    void InitGameObjects()
    {

        //The wand is the parent object to all the parts.
        mWandGO = new GameObject("Wand");
        mWandGO.transform.parent = gameObject.transform;

        //The rod
        //We set its transform.
        mRodGO = new GameObject("Rod");
        mRodGO.transform.parent = mWandGO.transform;
        mRodGO.transform.localScale += Vector3.up * 8;
        mRodGO.transform.localScale *= 0.2f;
        TempVisuals(mRodGO, PrimitiveType.Cylinder, Color.yellow);

        //The tip
        //We set its transform
        mTipGO = new GameObject("Tip");
        mTipGO.transform.parent = mWandGO.transform;
        mTipGO.transform.position += Vector3.up * 2;
        mTipGO.transform.localScale *= 0.5f;
        TempVisuals(mTipGO, PrimitiveType.Sphere, Color.red);

        mWandGO.transform.localScale *= 0.1f;

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

    GameObject target;
    Vector3 oldScale;
    // Update is called once per frame
    void Update()
    {

        Ray ray = new Ray(mTipGO.transform.position, transform.forward);
        RaycastHit hit;

        Vector3 point = mTipGO.transform.position + transform.forward * 10;
        if (Physics.Raycast(ray, out hit))
        {

            if(hit.collider.tag == "StageScreen")
            {

                point = hit.point;

                if (target != hit.collider.gameObject)
                {

                    if (target != null)
                    {

                        target.transform.localScale = oldScale;

                    }

                    target = hit.collider.gameObject;
                    oldScale = hit.collider.transform.localScale;

                    target.transform.localScale *= 1.1f;

                }

            }

        }
        else
        {

            if (target != null)
            {

                target.transform.localScale = oldScale;
                target = null;

            }

        }

        lineRenderer.SetPosition(0, ray.origin);
        lineRenderer.SetPosition(1, point);

        if (VrInput.controllersFound)
        {

            float trigger = 0;
            if (rightHand)
                trigger = VrInput.RightTrigger();
            else trigger = VrInput.LeftTrigger();

            if (trigger == 1.0f)
            {



            }

        }
        else
        {

            if (Input.GetKey(KeyCode.Space))
            {



            }

        }
    }
}
