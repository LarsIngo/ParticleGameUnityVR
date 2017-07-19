using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo  {

    static public int count = 0;

    public string name;
    public string thumbnail;
    public Hub.STATE stageState;

    public StageInfo()
    {

        name = "Stage:" + count;
        thumbnail = "Textures/Default";
        stageState = Hub.STATE.DEFAULT;

    }

}
