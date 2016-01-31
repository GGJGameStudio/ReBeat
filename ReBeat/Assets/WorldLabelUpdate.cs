using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WorldLabelUpdate : MonoBehaviour {

    Text input;
	// Use this for initialization
	void Start () {
        input = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        input.text = "World " + ApplicationModel.World;
	}
}
