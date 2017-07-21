using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VatsugMain2 : MonoBehaviour
{

    /// +++ MEMBERS +++ ///
    
    UnityEngine.UI.Text mCurrentScore;
    UnityEngine.UI.Text highscore;
    
    float controllerSpawnDelay;
    bool once;


    private float timer;
    private const int nrOfVatsugs = 20;
    private int counter = 0;
    List<GameObject> enemies;

    private StageInfo mStageInfo;

    /// --- MEMBERS --- ///

    // Use this for initialization
    void Start()
    {
        // TIMER.
        GameObject timerText = Factory.CreateWorldText("Highscore", Color.white);
        timerText.transform.position += Vector3.forward * 100 + Vector3.up * 50;
        timerText.transform.localScale *= 100;
        mCurrentScore = timerText.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        

        // Highscore.
        GameObject Highscore = Factory.CreateWorldText("Highscore:" + mStageInfo.highscore, Color.white);
        Highscore.transform.position += Vector3.forward * 100 + Vector3.up * 30;
        Highscore.transform.localScale *= 20;

        timer = 30.0f;
        for (int i = 0; i < Hub.Instance.mStageInfoList.Count; i++)
        {

            if (Hub.Instance.mStageInfoList[i].mSceneName == "Vatsug2")
                mStageInfo = Hub.Instance.mStageInfoList[i];

        }
        enemies = new List<GameObject>();
        for (int i = 0; i < nrOfVatsugs; ++i)
        {
            enemies.Add(new GameObject("TheOneAndOnlyVatsug2" + i));
            Factory.CreateVatsug2(enemies[i].transform);
            enemies[i].AddComponent<Vatsug2>();
        }
        //this.mSpawnSystem.GetComponent<SpawnSystem>().AddGameObjectWithDelay(enemy, 1.0f);

        GameObject boat = Factory.CreateBoat();
        boat.transform.position = new Vector3(-1.5f, 0, -1.5f);
        boat.transform.eulerAngles = new Vector3(45, 30, 45);

        GameObject moon = Factory.CreateMoon();

        GameObject water = Factory.CreateWater();

        Factory.CreateIsland();

        controllerSpawnDelay = 1.0f;
        once = false;

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
        
        foreach (GameObject go in enemies)
        {
            if (go.transform.childCount == 0)
            {
                ++counter;
                Destroy(go);
                enemies.Remove(go);
                mCurrentScore.text = "Birds sacrificed: " + counter;
            }
            
        }
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            foreach (GameObject go in enemies)
            {
                if (go.transform.childCount == 0)
                {
                    ++counter;
                }
                Destroy(go);
                enemies.Remove(go);
            }


            if (mStageInfo.Score < counter)
                mStageInfo.SetScore(counter);

            Factory.CreateCelebration();
            timer = 999999999.0f;
        }
        if (controllerSpawnDelay <= 0 && !once)
        {
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
