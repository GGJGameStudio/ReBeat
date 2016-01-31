using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class MainMenuMain : MonoBehaviour {

    KeyCode[] KonamiCodeSequence = new KeyCode[] { KeyCode.UpArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.B, KeyCode.A };
    int KonamiIndex = 0;
    

	// Use this for initialization
	void Start () {
        ApplicationModel.KonamiCodeActivated = false;
        KonamiIndex = 0;
        ApplicationModel.WorldList = new System.Collections.Generic.Dictionary<int, int>();
        string worldDetail = ((TextAsset)Resources.Load("Worlds/WorldList")).text;
        string[] worldDetailTab = worldDetail.Split('\n');
        foreach(string detail in worldDetailTab)
        {
            string[] detailTab = detail.Split(':');
            ApplicationModel.WorldList.Add(Int32.Parse(detailTab[0]), Int32.Parse(detailTab[1]));
        }
    }


    // Update is called once per frame
    void Update () {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KonamiCodeSequence[KonamiIndex]))
            {
                KonamiIndex++;
                if(KonamiIndex >= KonamiCodeSequence.Length)
                {
                    ApplicationModel.KonamiCodeActivated = !ApplicationModel.KonamiCodeActivated;
                    KonamiIndex = 0;
                    StartGame();
                }
            }
            else
            {
                KonamiIndex = 0;
            }
        }
	}


    public void IncrementWorld()
    {
        if (ApplicationModel.WorldList.ContainsKey(ApplicationModel.World + 1))
        {
            ApplicationModel.World++;
            ApplicationModel.MapsetNumber = 1;
        }
    }

    public void DecrementWorld()
    {

        if (ApplicationModel.WorldList.ContainsKey(ApplicationModel.World - 1))
        {
            ApplicationModel.World--;
            ApplicationModel.MapsetNumber = 1;
        }
    }

    public void IncrementSet()
    {

        if (ApplicationModel.WorldList[ApplicationModel.World] >= ApplicationModel.MapsetNumber +1)
            ApplicationModel.MapsetNumber++;
    }

    public void DecrementSet()
    {

        if (1 <= ApplicationModel.MapsetNumber - 1)
            ApplicationModel.MapsetNumber--;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
