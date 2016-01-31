using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreMenuMain : MonoBehaviour {

    Text input;

	// Use this for initialization
	void Start () {
        input = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        input.text = "On World " + ApplicationModel.World + "-" + ApplicationModel.MapsetNumber + ", you scored \n" + ApplicationModel.Score + " points";
	}

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
