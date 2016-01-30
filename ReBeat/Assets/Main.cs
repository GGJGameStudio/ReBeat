using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Main : MonoBehaviour {

    private int mapsize = 20;
    private int tilesize = 32;
    private float speed = 8;
    private Position startPos = new Position(1, 5);
    private Direction startDir = Direction.Right;

    private GameObject player;
    private Position playerPos;
    private Position nextPos;
    private Direction playerDir;

    private float moveTimer = 0;
    private float levelTimer = 0;




    // Use this for initialization
    void Start () {

        //charger map
        if (ApplicationModel.Level == 1)
        {
            //TODO
        }

        ApplicationModel.Inputs.Add(new List<int>());


        for (int i = 0; i < mapsize; i++)
        {
            for (int j = 0; j < mapsize; j++)
            {
                Vector3 pos = new Position(i, j).ToWorldPos(tilesize, mapsize);
                if (i == 0 || j == 0 || i == mapsize - 1 || j == mapsize - 1)
                {
                    Instantiate(Resources.Load("wall"), pos, Quaternion.identity);
                } else
                {
                    Instantiate(Resources.Load("blanc"), pos, Quaternion.identity);
                }
            }
        }

        startPos = new Position(startPos);
        playerPos = new Position(startPos);
        playerDir = Direction.Right;
        nextPos = playerPos.Add(Position.DirToPos(playerDir));

        Vector3 playerstartpos = startPos.ToWorldPos(tilesize, mapsize);
        player = (GameObject) Instantiate(Resources.Load("perso"), playerstartpos, Quaternion.identity);

        Vector3 coinpos = new Position(1, 1).ToWorldPos(tilesize, mapsize);
        Instantiate(Resources.Load("coin"), coinpos, Quaternion.identity);



    }
	
	// Update is called once per frame
	void Update () {
        moveTimer += Time.deltaTime * speed;
        levelTimer += Time.deltaTime * speed;

        player.transform.position = Vector3.Lerp(playerPos.ToWorldPos(tilesize, mapsize), nextPos.ToWorldPos(tilesize, mapsize), moveTimer);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            int nextTimeSlot = Mathf.CeilToInt(levelTimer);
            ApplicationModel.Inputs[ApplicationModel.Level-1].Add(nextTimeSlot);
        }

        if (moveTimer > 1)
        {
            playerPos = nextPos;
            moveTimer = moveTimer - 1;

            int currentTimeSlot = Mathf.FloorToInt(levelTimer);

            int level = 1;
            foreach(List<int> levelinputs in ApplicationModel.Inputs)
            {
                if (levelinputs.Contains(currentTimeSlot))
                {
                    if (level == 1)
                    {
                        playerDir = Position.turnRight(playerDir);
                    }

                    if (level == 2)
                    {
                        playerDir = Position.turnLeft(playerDir);
                    }
                }
                
                level++;
            }


            nextPos = playerPos.Add(Position.DirToPos(playerDir));
        }

        if (levelTimer > 40)
        {
            ApplicationModel.Level++;
            if (ApplicationModel.Level > 2)
            {
                ApplicationModel.Level = 1;
                ApplicationModel.Inputs.Clear();
            }
            SceneManager.LoadScene(0);
        }

    }
    
}
