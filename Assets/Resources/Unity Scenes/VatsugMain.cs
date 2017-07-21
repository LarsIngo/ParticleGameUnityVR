using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VatsugMain : MonoBehaviour
{

    /// +++ MEMBERS +++ ///
    
    
    UnityEngine.UI.Text highscore;

    float timer = 0;

    /// --- MEMBERS --- ///

    // Use this for initialization
    void Start()
    {
        GameObject enemy = new GameObject("TheOneAndOnlyVatsug");
        Factory.CreateVatsug(enemy.transform);
        enemy.AddComponent<Vatsug>();

        //this.mSpawnSystem.GetComponent<SpawnSystem>().AddGameObjectWithDelay(enemy, 1.0f);

        GameObject boat = Factory.CreateBoat();

        GameObject moon = Factory.CreateMoon();

        GameObject water = Factory.CreateWater();


        //Equip a wand.
        //GameObject rightWand = Factory.CreateVatsugWand(this, 90.0f, 35.0f, 5.0f, 15.0f, true);
        GameObject leftWand = Factory.CreateAttractorWand(20, false);
        //this.mSpawnSystem.GetComponent<SpawnSystem>().AddGameObjectWithDelay(rightWand, 5.0f);
        //this.mSpawnSystem.GetComponent<SpawnSystem>().AddGameObjectWithDelay(leftWand, 5.0f);

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
        if (VrInput.Menu() || Input.GetKeyDown(KeyCode.Escape))
        {
            AudioSource audioSource = Hub.backgroundMusic.GetComponent<AudioSource>();
            audioSource.clip = Resources.Load<AudioClip>("Music/MachinimaSound.com_-_The_Arcade");
            audioSource.Play();

            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");



        }
    }





}
