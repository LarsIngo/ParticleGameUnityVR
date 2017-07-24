using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMain : MonoBehaviour
{

    /// +++ MEMBERS +++ ///
    
    UnityEngine.UI.Text mTextDisplay;

    GameObject mEnemy;

    bool mFirstEnterDone;

    /// --- MEMBERS --- ///

    // Use this for initialization
    void Start()
    {
        
        // RESET.
        mFirstEnterDone = true;

        // WAND.
        GameObject rightWand = Factory.CreateAttractorWand(20, true);

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
