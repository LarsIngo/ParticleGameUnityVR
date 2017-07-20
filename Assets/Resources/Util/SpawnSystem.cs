using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : MonoBehaviour {

    struct DelayInfo
    {
        public GameObject go;
        public float currentDelay;

        public DelayInfo(GameObject otherGo, float otherDelay) { go = otherGo; currentDelay = otherDelay; }
    }
    List<DelayInfo> mDelayInfoList = new List<DelayInfo>();

    /// <summary>
    /// Adds a gameobject to a list with a delay and sets it to inactive until the delay reaches zero in the update function.
    /// </summary>
    /// <param name="go"></param>
    /// <param name="delay"></param>
    public void AddGameObjectWithDelay(GameObject go, float delay)
    {
        go.SetActive(false);
        DelayInfo d = new DelayInfo();
        d.currentDelay = delay;
        d.go = go;
        mDelayInfoList.Add(d);

        return;
    }
   
    /// <summary>
    /// removes all elements that were suposed to be delayed
    /// </summary>
    public void Flush()
    {
        mDelayInfoList.Clear();
        
        return;
    }

	// Use this for initialization
	void Awake () {
        
    }
	
	// Update is called once per frame
	void Update () {
        
		for(int i = 0; i < mDelayInfoList.Count; ++i)
        {
            DelayInfo delayInfo = mDelayInfoList[i];

            if (delayInfo.currentDelay <= 0.0f)
            {
                delayInfo.go.SetActive(true);
                mDelayInfoList[i] = delayInfo;
                mDelayInfoList.RemoveAt(i);
                i--;
            }
            else
            {
                delayInfo.currentDelay -= Time.deltaTime;
                mDelayInfoList[i] = delayInfo;
            }
        }
	}
}
