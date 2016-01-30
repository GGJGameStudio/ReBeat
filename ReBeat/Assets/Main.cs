using UnityEngine;
using System.Collections;
using SimpleJSON;
using Assets.Model;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    for(int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Vector3 pos = new Vector3(i, j, 0) * 0.32f;
                Instantiate(Resources.Load("blanc"), pos, Quaternion.identity);
            }
        }
        

        
        

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
