//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Attractor_lvl_1 : Level
//{
//    /// +++ MEMBERS +++ ///

//    StageInfo stageInfo;

//    GameObject enemy1;
//    GameObject enemy2;
//    GameObject enemy3;

//    UnityEngine.UI.Text highscore;

//    float timer = 0;

//    /// --- MEMBERS --- ///


//    /// +++ FUNCTIONS +++ ///

//    /// <summary>
//    /// Constructor.
//    /// </summary>
//    /// <param name="name">Name of level, must be unique.</param>
//    public Attractor_lvl_1(string name) : base(name)
//    {



//    }


//    /// <summary>
//    /// Override awake function.
//    /// </summary>
//    public override void Awake()
//    {

//        //Equip a wand.
//        //GameObject rightWand = Factory.CreateAttractorWand(this, 20, true);

//        //GameObject timerText = Factory.CreateWorldText(this, "Highscore", Color.white);

//        //timerText.transform.position += Vector3.forward * 100 + Vector3.up * 50;
//        //timerText.transform.localScale *= 100;

//        //highscore = timerText.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();

//        ////Spawn enemies.
//        //SpawnEnemies();
//        //timer = 0;
//    }

//    /// <summary>
//    /// Override update function.
//    /// </summary>
//    public override void Update()
//    {

//        //if ((enemy1 || enemy2 || enemy3))
//        //    timer += Time.deltaTime;
//        //else
//        //{

//        //    if (stageInfo.Score > timer)
//        //        stageInfo.SetScore(timer);

//        //}
//        //highscore.text = timer.ToString("0.00");

//    }

//    /// <summary>
//    /// Override sleep function.
//    /// </summary>
//    public override void Sleep()
//    {
        

//    }



//    /// --- FUNCTIONS --- ///
//}
