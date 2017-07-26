﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMain : MonoBehaviour
{

    /// +++ MEMBERS +++ ///
    
    UnityEngine.UI.Text mTextDisplay;

    private GameObject rightWand;
    private GameObject activateWandTooltip;
    private LineRenderer lineRenderer;

    private Material tex1;
    private Material tex2;

    private GameObject mEnemy;

    float texTimer = 0.8f;
    bool texSwap = false;

    bool mFirstEnterDone;

    /// --- MEMBERS --- ///

    // Use this for initialization
    void Start()
    {
        
        // RESET.
        mFirstEnterDone = true;

        // WAND.
        rightWand = Factory.CreateAttractorWand(20, true);

        activateWandTooltip = Factory.CreateWorldImage("MenuIconTextures/ViveTriggerHoldPressed1");
        
        tex1 = new Material(Shader.Find("Unlit/Texture"));
        tex2 = new Material(Shader.Find("Unlit/Texture"));
        tex1.mainTexture = Resources.Load("MenuIconTextures/ViveTriggerHoldPressed1") as Texture2D;
        tex2.mainTexture = Resources.Load("MenuIconTextures/ViveTriggerHoldPressed2") as Texture2D;
        activateWandTooltip.transform.position = new Vector3(-0.05f, -0.55f, 0.2f);
        activateWandTooltip.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        activateWandTooltip.transform.LookAt(activateWandTooltip.transform.position - Camera.main.transform.position);
        activateWandTooltip.transform.parent = rightWand.transform;

        lineRenderer = rightWand.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.white;
        lineRenderer.widthMultiplier = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(0, 0, 0));
        lineRenderer.SetPosition(1, new Vector3(0, -0.5f, 0.2f));


        // TIMER.
        GameObject displayTextObject = Factory.CreateWorldText("Welcome to Partycles", Color.green);
        displayTextObject.transform.position += Vector3.forward * 100 + Vector3.up * 50;
        displayTextObject.transform.localScale *= 100;
        GameObject instructionText = Factory.CreateWorldText("Throw particles at the ball!", Color.white);
        instructionText.transform.position += Vector3.forward * 100 + Vector3.up * 20;
        instructionText.transform.localScale *= 50;

        // ENEMIES.
        SpawnEnemies();

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

    }

    float endTimer = 0;
    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, rightWand.transform.position);
        lineRenderer.SetPosition(1, rightWand.transform.position + new Vector3(0, -0.5f, 0.2f));
        activateWandTooltip.transform.LookAt(activateWandTooltip.transform.position - Camera.main.transform.position);

        texTimer -= Time.deltaTime;

        if (!texSwap && texTimer <= 0.0f)
        {
            activateWandTooltip.GetComponent<MeshRenderer>().material = tex1;
            texTimer = 0.8f;
            texSwap = true;
        }
        else if (texSwap && texTimer <= 0.0f)
        {
            activateWandTooltip.GetComponent<MeshRenderer>().material = tex2;
            texTimer = 1.6f;
            texSwap = false;
        }
        

        if (!mEnemy)
        {
            if (mFirstEnterDone)
            {
                mFirstEnterDone = false;

                // Celebration! :D :D :D
                Factory.CreateCelebration();
                
            }

            endTimer += Time.deltaTime;
            if (endTimer > 6)
                UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");

        }
        

        // Check input to leave scene.
        if (VrInput.Menu() || Input.GetKeyDown(KeyCode.Escape))
        {

            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");

        }

    }

    void SpawnEnemies()
    {
        // ENEMIES.
        mEnemy = Factory.CreateBasicEnemy(Vector3.forward * 3 + Vector3.right * 0, 750);
    }

}
