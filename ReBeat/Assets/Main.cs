using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using SimpleJSON;
using Assets.Model;

public class Main : MonoBehaviour {

    private int mapsize = 10;
    private int tilesize = 32;
    private float speed = 8;
    private float leveltime = 5;

    private GameObject player;
    private Position playerPos;
    private Position nextPos;
    private Direction playerDir;

    private float moveTimer = 0;
    private float levelTimer = 0;

    private GameObject time;
    private GameObject timeBase;
    private List<GameObject> timelineobjects = new List<GameObject>();

    public int score = 0;


    // Use this for initialization
    void Start () {

        //charger map
        if (ApplicationModel.Level == 1)
        {
            /*ApplicationModel.Mapset = new Mapset();
            ApplicationModel.Mapset.Levels = new Level[2];
            for (int lvl = 0; lvl < ApplicationModel.Mapset.Levels.Length; lvl++)
            {
                ApplicationModel.Mapset.Levels[lvl] = new Level();
                ApplicationModel.Mapset.Levels[lvl].Environment = new BaseTile[mapsize, mapsize];
                ApplicationModel.Mapset.Levels[lvl].Collectibles = new BaseCollectible[mapsize, mapsize];

                for (int i = 0; i < mapsize; i++)
                {
                    for (int j = 0; j < mapsize; j++)
                    {
                        ApplicationModel.Mapset.Levels[lvl].Environment[i, j] = new BaseTile();
                        ApplicationModel.Mapset.Levels[lvl].Collectibles[i, j] = new BaseCollectible();
                        Vector3 pos = new Position(i, j).ToWorldPos(tilesize, mapsize);
                        if (i == 0 || j == 0 || i == mapsize - 1 || j == mapsize - 1)
                        {
                            ApplicationModel.Mapset.Levels[lvl].Environment[i, j].Type = TileType.Wall;
                        }
                        else
                        {
                            ApplicationModel.Mapset.Levels[lvl].Environment[i, j].Type = TileType.Blank;
                        }

                        if (i % 5 == 1 && j % 5 == 1)
                        {
                            ApplicationModel.Mapset.Levels[lvl].Collectibles[i, j].Type = CollectibleType.Coin;
                        }
                        else
                        {
                            ApplicationModel.Mapset.Levels[lvl].Collectibles[i, j].Type = CollectibleType.BigCoin;
                        }


                    }
                }
                Vector3 pos = new Vector3(i, j, 0) * 0.32f;
                Instantiate(Resources.Load("blanc"), pos, Quaternion.identity);
            }*/
            ApplicationModel.Mapset = JSONParser.Parse(((TextAsset)Resources.Load("Worlds/1/Set_1")).text);
        }


        Position startPos = new Position(ApplicationModel.Mapset.Levels[ApplicationModel.Level-1].StartX, ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].StartY);
        Direction startDir = ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].StartDirection;


        if (ApplicationModel.Level > 1)
        {
            ApplicationModel.Mapset.Levels[ApplicationModel.Level-1].StackLevelCollectibles(ApplicationModel.Mapset.Levels[ApplicationModel.Level-2]);
        }
        
        ApplicationModel.Inputs.Add(new List<int>());

        for (int i = 0; i < ApplicationModel.Mapset.Levels[ApplicationModel.Level-1].Environment.GetLength(0); i++)
        {
            for (int j = 0; j < ApplicationModel.Mapset.Levels[ApplicationModel.Level-1].Environment.GetLength(1); j++)
            {
                Vector3 pos = new Position(i, j).ToWorldPos(tilesize, mapsize);
                GameObject obj = null;
                switch (ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Environment[i, j].Type)
                {
                    case TileType.Blank:
                        obj = (GameObject) Instantiate(Resources.Load("blanc"), pos, Quaternion.identity);
                        break;
                    case TileType.Wall:
                        obj = (GameObject) Instantiate(Resources.Load("wall"), pos, Quaternion.identity);
                        break;
                    default:
                        break;
                }
                ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Environment[i, j].GameObject = obj;

                obj = null;
                switch (ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Collectibles[i, j].Type)
                {
                    case CollectibleType.Coin:
                        obj = (GameObject) Instantiate(Resources.Load("coin"), pos, Quaternion.identity);
                        break;
                    default:
                        break;
                }
                ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Collectibles[i, j].GameObject = obj;
            }
        }

        startPos = new Position(startPos);
        playerPos = new Position(startPos);
        playerDir = startDir;
        nextPos = playerPos.Add(Position.DirToPos(playerDir));

        Vector3 playerstartpos = startPos.ToWorldPos(tilesize, mapsize);
        player = (GameObject) Instantiate(Resources.Load("sprite-triangle"), playerstartpos, Quaternion.identity);
        player.transform.localScale = new Vector3(0.3f, 0.3f, 1);

        var ui = new GameObject();

        Vector3 timelinepos = new Position(mapsize / 2, -2).ToWorldPos(tilesize, mapsize);
        var timeline = (GameObject)Instantiate(Resources.Load("blanc"), timelinepos, Quaternion.identity);
        timeline.transform.localScale = new Vector3(mapsize, 1, 1);
        //timeline.transform.Translate(new Vector3(-0.5f * tilesize / 100, 0, 0));
        timeline.transform.parent = ui.transform;

        Vector3 timepos = new Position(0, -2).ToWorldPos(tilesize, mapsize);
        time = (GameObject)Instantiate(Resources.Load("wall"), timepos, Quaternion.identity);
        time.transform.localScale = new Vector3(0.1f, 1, 1);
        //time.transform.Translate(new Vector3(-0.5f * tilesize / 100, 0, 0));
        time.transform.parent = ui.transform;

        timeBase = new GameObject();
        timeBase.transform.position = new Vector3(time.transform.position.x, time.transform.position.y, time.transform.position.z);
        timeBase.transform.parent = ui.transform;

        ui.transform.position = new Vector3(-0.5f * tilesize / 100, 0, 0);
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
                    switch (ApplicationModel.Mapset.Levels[level-1].Mechanic)
                    {
                        case LevelMechanic.TurnRight:
                            playerDir = Position.turnRight(playerDir);
                            break;
                        case LevelMechanic.TurnLeft:
                            playerDir = Position.turnLeft(playerDir);
                            break;
                        default:
                            break;
                    }
                }
                
                level++;
            }


            nextPos = playerPos.Add(Position.DirToPos(playerDir));
            if (ApplicationModel.Mapset.Levels[ApplicationModel.Level-1].Environment[nextPos.X, nextPos.Y].Type == TileType.Wall)
            {
                nextPos = playerPos;
            } else
            {
                if (ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Collectibles[nextPos.X, nextPos.Y].Type == CollectibleType.Coin)
                {
                    score += 10;
                    ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Collectibles[nextPos.X, nextPos.Y].Type = CollectibleType.Nothing;
                    Destroy(ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Collectibles[nextPos.X, nextPos.Y].GameObject);
                }
            }
        }

        if (levelTimer > leveltime * speed)
        {
            ApplicationModel.Level++;
            if (ApplicationModel.Level > 2)
            {
                ApplicationModel.Level = 1;
                ApplicationModel.Inputs.Clear();
            }
            SceneManager.LoadScene(0);
        }


        updateTimeLine(levelTimer);
    }

    private void updateTimeLine(float levelTimer)
    {
        timelineobjects.ForEach(o => GameObject.Destroy(o));
        timelineobjects.Clear();

        time.transform.position = Vector3.Lerp(timeBase.transform.position, new Vector3(timeBase.transform.position.x + mapsize * tilesize / 100f, timeBase.transform.position.y, 0), levelTimer / (leveltime * speed));
        
        foreach (List<int> levelinputs in ApplicationModel.Inputs)
        {
            foreach (int input in levelinputs)
            {
                var timelineobjectpos = Vector3.Lerp(timeBase.transform.position, new Vector3(timeBase.transform.position.x + mapsize * tilesize / 100f, timeBase.transform.position.y, 0), input / (leveltime * speed));
                GameObject timelineobject = (GameObject)Instantiate(Resources.Load("perso"), timelineobjectpos, Quaternion.identity);
                timelineobject.transform.localScale = new Vector3(0.2f, 1, 1);
                timelineobjects.Add(timelineobject);
            } 
        }

        
    }
}
