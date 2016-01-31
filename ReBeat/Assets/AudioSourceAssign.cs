using UnityEngine;
using System.Collections;

public class AudioSourceAssign : MonoBehaviour {

	// Use this for initialization
	void Start () {

        AudioSource audio = GetComponent<AudioSource>();
        ApplicationModel.Audio = audio;
        if(ApplicationModel.KonamiCodeActivated)
        {
            ApplicationModel.StopPlayingSecretStuff();
            ApplicationModel.KonamiCodeActivated = false;
        }
    }
	
    void Awake()
    {
        DontDestroyOnLoad(GetComponent<AudioSource>());
    }

	// Update is called once per frame
	void Update () {
	
	}
}
