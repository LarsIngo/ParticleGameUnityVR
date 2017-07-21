using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneMain : MonoBehaviour {

	// Use this for initialization
	void Start () {

        StageInfo stageInfo = new StageInfo(0, 0, "Attractor_lvl_1");
        stageInfo.mName = "My first wand!";
        stageInfo.mThumbnail = "Textures/Attractor_lvl_1";
        stageInfo.mBronze = 30;
        stageInfo.mSilver = 60;
        stageInfo.mGold = 75;

        Hub.Instance.mStageInfoList.Add(stageInfo);

        stageInfo = new StageInfo(0, 1, "Attractor_lvl_2");
        stageInfo.mName = "Double Trouble!";
        stageInfo.mThumbnail = "Textures/Attractor_lvl_2";
        stageInfo.mStarRequirement = 2;

        stageInfo.mBronze = 30;
        stageInfo.mSilver = 60;
        stageInfo.mGold = 75;

        Hub.Instance.mStageInfoList.Add(stageInfo);


        stageInfo = new StageInfo(2, 0, "Vatsug");
        stageInfo.mName = "Vatsug";
        stageInfo.mThumbnail = "Textures/vatsugicon";


        stageInfo.mBronze = 30;
        stageInfo.mSilver = 20;
        stageInfo.mGold = 10;

        Hub.Instance.mStageInfoList.Add(stageInfo);

        stageInfo = new StageInfo(2, 1, "Vatsug2");
        stageInfo.mName = "Vatsug2";
        stageInfo.mThumbnail = "Textures/vatsugicon2";

        Hub.Instance.mStageInfoList.Add(stageInfo);


        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");

        stageInfo = new StageInfo(0, 2, "Attractor_lvl_3");
        stageInfo.mName = "A real challenge!";
        stageInfo.mThumbnail = "Textures/Attractor_lvl_2";
        stageInfo.mStarRequirement = 5;

        stageInfo.mBronze = 1;
        stageInfo.mSilver = 5;
        stageInfo.mGold = 10;

        Hub.Instance.mStageInfoList.Add(stageInfo);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
