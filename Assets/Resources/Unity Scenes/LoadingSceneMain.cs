﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneMain : MonoBehaviour {

	// Use this for initialization
	void Start () {

        StageInfo stageInfo = new StageInfo(0, 0, "Tutorial");
        stageInfo.mName = "Tutorial";
        stageInfo.mThumbnail = "MenuIconTextures/tutorialicon";
        Hub.Instance.mStageInfoList.Add(stageInfo);

        stageInfo = new StageInfo(1, 0, "Attractor_lvl_1");
        stageInfo.mName = "My first wand!";
        stageInfo.mThumbnail = "MenuIconTextures/wand1";
        stageInfo.mBronze = 30;
        stageInfo.mSilver = 60;
        stageInfo.mGold = 75;

        Hub.Instance.mStageInfoList.Add(stageInfo);

        stageInfo = new StageInfo(1, 1, "Attractor_lvl_2");
        stageInfo.mName = "Double Trouble!";
        stageInfo.mThumbnail = "MenuIconTextures/wand2";
        stageInfo.mStarRequirement = 2;

        stageInfo.mBronze = 30;
        stageInfo.mSilver = 60;
        stageInfo.mGold = 75;

        Hub.Instance.mStageInfoList.Add(stageInfo);

        

        stageInfo = new StageInfo(1, 2, "Attractor_lvl_4");
        stageInfo.mName = "Level 4";
        stageInfo.mThumbnail = "MenuIconTextures/wand3";
        stageInfo.mStarRequirement = 7;

        stageInfo.mBronze = 30;
        stageInfo.mSilver = 60;
        stageInfo.mGold = 75;

        Hub.Instance.mStageInfoList.Add(stageInfo);

        stageInfo = new StageInfo(1, 3, "Attractor_lvl_5");
        stageInfo.mName = "Level 5";
        stageInfo.mThumbnail = "MenuIconTextures/wand3";
        stageInfo.mStarRequirement = 0;

        stageInfo.mBronze = 30;
        stageInfo.mSilver = 60;
        stageInfo.mGold = 75;

        Hub.Instance.mStageInfoList.Add(stageInfo);

        stageInfo = new StageInfo(1, 4, "Attractor_lvl_6");
        stageInfo.mName = "Level 6";
        stageInfo.mThumbnail = "MenuIconTextures/wand3";
        stageInfo.mStarRequirement = 0;

        stageInfo.mBronze = 30;
        stageInfo.mSilver = 60;
        stageInfo.mGold = 75;

        Hub.Instance.mStageInfoList.Add(stageInfo);

        stageInfo = new StageInfo(1, 5, "Attractor_lvl_7");
        stageInfo.mName = "Level 7";
        stageInfo.mThumbnail = "MenuIconTextures/wand3";
        stageInfo.mStarRequirement = 0;

        stageInfo.mBronze = 30;
        stageInfo.mSilver = 60;
        stageInfo.mGold = 75;

        Hub.Instance.mStageInfoList.Add(stageInfo);

        stageInfo = new StageInfo(1, 6, "Attractor_lvl_8");
        stageInfo.mName = "Level 8";
        stageInfo.mThumbnail = "MenuIconTextures/wand3";
        stageInfo.mStarRequirement = 0;

        stageInfo.mBronze = 30;
        stageInfo.mSilver = 60;
        stageInfo.mGold = 75;

        Hub.Instance.mStageInfoList.Add(stageInfo);

        stageInfo = new StageInfo(1, 7, "Attractor_lvl_9");
        stageInfo.mName = "Level 9";
        stageInfo.mThumbnail = "MenuIconTextures/wand3";
        stageInfo.mStarRequirement = 0;

        stageInfo.mBronze = 30;
        stageInfo.mSilver = 60;
        stageInfo.mGold = 75;

        Hub.Instance.mStageInfoList.Add(stageInfo);


        stageInfo = new StageInfo(2, 0, "Vatsug1");
        stageInfo.mName = "Escape";
        stageInfo.mThumbnail = "MenuIconTextures/vatsugicon1";


        stageInfo.mBronze = 3;
        stageInfo.mSilver = 6;
        stageInfo.mGold = 9;

        Hub.Instance.mStageInfoList.Add(stageInfo);

        stageInfo = new StageInfo(2, 1, "Vatsug");
        stageInfo.mName = "Boat ride";
        stageInfo.mThumbnail = "MenuIconTextures/vatsugicon";


        stageInfo.mBronze = 3;
        stageInfo.mSilver = 6;
        stageInfo.mGold = 9;
        stageInfo.mStarRequirement = 3;

        Hub.Instance.mStageInfoList.Add(stageInfo);

        stageInfo = new StageInfo(2, 2, "Vatsug2");
        stageInfo.mName = "Stranded";
        stageInfo.mThumbnail = "MenuIconTextures/vatsugicon2";
        stageInfo.mBronze = 3;
        stageInfo.mSilver = 6;
        stageInfo.mGold = 9;
        stageInfo.mStarRequirement = 6;

        Hub.Instance.mStageInfoList.Add(stageInfo);


        stageInfo = new StageInfo(3, 0, "Attractor_lvl_3");
        stageInfo.mName = "A real challenge!";
        stageInfo.mThumbnail = "MenuIconTextures/wand3";
        stageInfo.mStarRequirement = 5;

        stageInfo.mBronze = 1;
        stageInfo.mSilver = 5;
        stageInfo.mGold = 10;

        Hub.Instance.mStageInfoList.Add(stageInfo);

        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
