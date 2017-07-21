using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneMain : MonoBehaviour {

    /// +++ MEMBERS +++ ///

    /// <summary>
    /// Storing all the created scenes.
    /// </summary>
    private List<GameObject> mScreenList;

    /// <summary>
    /// Storing all the created scenes.
    /// </summary>
    private UnityEngine.UI.Text mStarText;

    /// --- MEMBERS --- ///

    // Use this for initialization
    void Start () {

        //Equip a wand.
        GameObject menuWand = Factory.CreateMenuWand(true);

        mScreenList = new List<GameObject>();

        GameObject starText = Factory.CreateWorldText(Hub.Instance.stars.ToString(), Color.black);
        starText.transform.position += Vector3.forward * 150 + Vector3.up * 150;
        starText.transform.localScale *= 100;

        GameObject starImage = Factory.CreateWorldImage("Textures/Star", true);
        starImage.transform.position += Vector3.forward * 155 + Vector3.up * 150;
        starImage.transform.localScale *= 100;

        GameObject menu = Factory.CreateWorldImage("Textures/menu", true);
        menu.transform.position += Vector3.forward * 1 + Vector3.up * 2 + Vector3.right * -3;
        menu.transform.Rotate(0, -50, 0);

        GameObject select = Factory.CreateWorldImage("Textures/select", true);
        select.transform.position += Vector3.forward * 1 + Vector3.up * 0.9f + Vector3.right * -3;
        select.transform.Rotate(0, -50, 0);


        mStarText = starText.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        mStarText.text = Hub.Instance.stars.ToString();

        for (int i = 0; i < Hub.Instance.mStageInfoList.Count; i++)
        {

            GameObject screen = Factory.CreateStageScreen(Hub.Instance.mStageInfoList[i]);

            screen.transform.position += Vector3.forward * 2 + Vector3.up * 1.5f;

            //Move it to the world location
            screen.transform.position += Vector3.right * Hub.Instance.mStageInfoList[i].mWorld + Vector3.right * Hub.Instance.mStageInfoList[i].mWorld * 0.1f;

            //Move it to the stage location
            screen.transform.position -= Vector3.up * Hub.Instance.mStageInfoList[i].mStage + Vector3.up * Hub.Instance.mStageInfoList[i].mStage * 0.5f;

            mScreenList.Add(screen);

        }

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

        Time.timeScale = 1.0f;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Alpha1))
            UnityEngine.SceneManagement.SceneManager.LoadScene(Hub.Instance.mStageInfoList[0].mSceneName);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            UnityEngine.SceneManagement.SceneManager.LoadScene(Hub.Instance.mStageInfoList[1].mSceneName);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            UnityEngine.SceneManagement.SceneManager.LoadScene(Hub.Instance.mStageInfoList[2].mSceneName);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            UnityEngine.SceneManagement.SceneManager.LoadScene(Hub.Instance.mStageInfoList[3].mSceneName);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            UnityEngine.SceneManagement.SceneManager.LoadScene(Hub.Instance.mStageInfoList[4].mSceneName);

        if (VrInput.RightGripPressed())
        {

            for (int i = 0; i < mScreenList.Count; i++)
            {

                mScreenList[i].transform.position -= VrInput.deltaRight.x * Vector3.right * 5 + VrInput.deltaRight.y * Vector3.up * 5;

            }

        }

    }

}
