using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo  {

    static public int count = 0;

    public string name;
    public string thumbnail;

    public StageInfo()
    {

        name = "Stage:" + count;
        thumbnail = "Textures/Default.jpg";

    }
    public StageInfo(string name)
    {

        this.name = name;

    }


}
