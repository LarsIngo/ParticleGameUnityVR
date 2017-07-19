using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage  {

    static public int count = 0;

    public string name;

    public Stage()
    {

        name = "Stage:" + count;

    }
    public Stage(string name)
    {

        this.name = name;

    }


}
