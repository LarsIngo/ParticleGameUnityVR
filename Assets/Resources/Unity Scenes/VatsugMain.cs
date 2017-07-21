﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VatsugMain : MonoBehaviour
{

    /// +++ MEMBERS +++ ///
    
    
    UnityEngine.UI.Text highscore;
    private float controllerSpawnDelay;
    private bool once;
    private const int nrOfVatsugs = 20;
    private GameObject[] enemies;
    private float timer;


    private StageInfo mStageInfo;


    /// --- MEMBERS --- ///


    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < Hub.Instance.mStageInfoList.Count; i++)
        {

            if (Hub.Instance.mStageInfoList[i].mSceneName == "Vatsug")
                mStageInfo = Hub.Instance.mStageInfoList[i];

        }


        enemies = new GameObject[nrOfVatsugs];
        for (int i = 0; i < nrOfVatsugs; ++i)
        {
            enemies[i] = new GameObject("TheOneAndOnlyVatsug");
            Factory.CreateVatsug(enemies[i].transform);
            enemies[i].AddComponent<Vatsug>();
        }
        //this.GetComponent<SpawnSystem>().AddGameObjectWithDelay(enemy, 1.0f);

        GameObject boat = Factory.CreateBoat();

        GameObject moon = Factory.CreateMoon();

        GameObject water = Factory.CreateWater();

        controllerSpawnDelay = 1.0f;
        once = false;


        //30 sec on the clock
        timer = 30.0f;
        //Equip a wand.
        /*GameObject rightWand = Factory.CreateAttractorWand(20, false);
        GameObject leftWand = Factory.CreateAttractorWand(20, false);
        sp.AddGameObjectWithDelay(rightWand, 2.0f);
        sp.AddGameObjectWithDelay(leftWand, 2.0f);
        */
        //rightWand.transform.parent = rightHand.transform;
        //leftWand.transform.parent = leftHand.transform;

        // SKYBOX.
        Material skyboxMat = new Material(Shader.Find("RenderFX/Skybox"));
        Debug.Assert(skyboxMat);
        string skyboxName = "Stars01";
        Texture2D front = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/frontImage");
        Texture2D back = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/backImage");
        Texture2D left = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/leftImage");
        Texture2D right = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/rightImage");
        Texture2D up = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/upImage");
        Texture2D down = Resources.Load<Texture2D>("Skyboxes/" + skyboxName + "/downImage");
        Debug.Assert(front);
        Debug.Assert(back);
        Debug.Assert(left);
        Debug.Assert(right);
        Debug.Assert(up);
        Debug.Assert(down);
        skyboxMat.SetTexture("_FrontTex", front);
        skyboxMat.SetTexture("_BackTex", back);
        skyboxMat.SetTexture("_LeftTex", left);
        skyboxMat.SetTexture("_RightTex", right);
        skyboxMat.SetTexture("_UpTex", up);
        skyboxMat.SetTexture("_DownTex", down);

        Skybox skybox = Camera.main.GetComponent<Skybox>();
        if (skybox == null)
        {
            skybox = Camera.main.gameObject.AddComponent<Skybox>();
            Camera.main.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
        }
        skybox.material = skyboxMat;


        AudioSource audioSource = Hub.backgroundMusic.GetComponent<AudioSource>();
        audioSource.clip = (AudioClip)Resources.Load("Music/MysGitarrreverb");
        audioSource.Play();

    }

    private GameObject CreateGameObject(string v)
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            int counter = 0;
            for (int i = 0; i < nrOfVatsugs; ++i)
            {
                if (enemies[i].transform.childCount == 0)
                {
                    ++counter;
                }
                Destroy(enemies[i]);
            }
            if (mStageInfo.Score < counter)
                mStageInfo.SetScore(counter);

            Factory.CreateCelebration();
            timer = 999999999.0f;

        }
        if (controllerSpawnDelay <= 0 && !once)
        {
            //Factory.CreateVatsugWand(100, 50, 0, 0, true);
            Factory.CreateAttractorWand(20, true);
            Factory.CreateAttractorWand(20, false);
            once = true;
        }
        else
        {
            controllerSpawnDelay -= Time.deltaTime;
        }
        if (VrInput.Menu() || Input.GetKeyDown(KeyCode.Escape))
        {
            AudioSource audioSource = Hub.backgroundMusic.GetComponent<AudioSource>();
            audioSource.clip = Resources.Load<AudioClip>("Music/MachinimaSound.com_-_The_Arcade");
            audioSource.Play();

            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");



        }
    }





}
