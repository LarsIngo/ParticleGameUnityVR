using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWand : MonoBehaviour {

    public LineRenderer lineRenderer;

    public bool rightHand;

    GameObject target;
    Vector3 oldScale;
    // Update is called once per frame
    void Update()
    {

        Ray ray = new Ray(lineRenderer.transform.position, lineRenderer.transform.up);
        RaycastHit hit;

        Vector3 point = lineRenderer.transform.position + lineRenderer.transform.up * 10;
        if (Physics.Raycast(ray, out hit))
        {

            if(hit.collider.tag == "StageScreen" && !hit.collider.GetComponent<StageScreen>().stageInfo.mLocked && hit.collider.GetComponent<StageScreen>().stageInfo.mStarRequirement <= Hub.Instance.stars)
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

        if (VrInput.controllersFound && target != null)
        {

            bool trigger = false;
            if (rightHand)
                trigger = VrInput.RightTriggerPressed();
            else trigger = VrInput.LeftTriggerPressed();

            if (trigger)
            {

                UnityEngine.SceneManagement.SceneManager.LoadScene(target.GetComponent<StageScreen>().stageInfo.mSceneName);

            }

        }
        else if(target != null)
        {

            if (Input.GetKey(KeyCode.Space))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(target.GetComponent<StageScreen>().stageInfo.mSceneName);
            }

        }
    }
}
