using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorHandMovement : MonoBehaviour {

    public bool rightHand;

	// Update is called once per frame
	void Update () {

        if (rightHand)
        {

            transform.position = VrInput.rightHand.transform.position;
            transform.rotation = VrInput.rightHand.transform.rotation;

        }
        else
        {

            transform.position = VrInput.leftHand.transform.position;
            transform.rotation = VrInput.leftHand.transform.rotation;

        }

    }
}
