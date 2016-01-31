using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using SimpleJSON;
using Assets.Model;

public class Main : MonoBehaviour {

    private int mapsize = 10;
    private int tilesize = 144;
    private float speed = 6;
    private float leveltime = 42;

    private GameObject player;
    private Position teleportPreviousPosition;
    private Position playerPos;
    private Position nextPos;
    private Direction playerDir;

    private bool walking = true;

    private float moveTimer = 0;
    private float levelTimer = 0;
    private List<GameObject> timelineobjects = new List<GameObject>();
    private List<GameObject> timelineinputobjects = new List<GameObject>();

    public int score = 0;


    // Use this for initialization
    void Start () {
        
        //charger map
        if (ApplicationModel.Level == 1)
        {
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
                /*switch (ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Environment[i, j].Type)
                {
                    case TileType.Blank:
                        obj = (GameObject) Instantiate(Resources.Load("blanc"), pos, Quaternion.identity);
                        break;
                    case TileType.Wall:
                        obj = (GameObject) Instantiate(Resources.Load("wall"), pos, Quaternion.identity);
                        break;
                    default:
                        break;
                }*/
                string envTex = "Environment/Prefabs/" + ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Environment[i, j].UnityResource;

                obj = (GameObject)Instantiate(Resources.Load(envTex), pos, Quaternion.identity);
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
        teleportPreviousPosition = new Position(startPos);
        playerDir = startDir;
        nextPos = playerPos.Add(Position.DirToPos(playerDir));
        walking = true;

        Vector3 playerstartpos = startPos.ToWorldPos(tilesize, mapsize);
        player = (GameObject) Instantiate(Resources.Load("sprite-triangle"), playerstartpos, Quaternion.identity);

        var camera = GetComponent<Camera>();
        for (int l = 0; l < ApplicationModel.Level; l++)
        {
            Vector3 timelinepos = new Vector3(5 + l, 0, 0);
            var timeline = (GameObject)Instantiate(Resources.Load("Timeline/prefab/timeline"), timelinepos, Quaternion.identity);
            
            var timeBasePosition = new Vector3(5 + l, -2, 0);
            var basetime = (GameObject)Instantiate(Resources.Load("Timeline/prefab/player"), timeBasePosition, Quaternion.identity);

            for (int t = 0; t <= leveltime * speed; t++)
            {
                //graduation
                var timelineobjectpos = timeBasePosition + Vector3.up * t * tilesize / 100;
                GameObject timelineobject = null;
                if (t != leveltime * speed)
                {
                    timelineobject = (GameObject)Instantiate(Resources.Load("Timeline/prefab/graduation"), timelineobjectpos, Quaternion.identity);
                } else
                {
                    timelineobject = (GameObject)Instantiate(Resources.Load("Timeline/prefab/fin"), timelineobjectpos, Quaternion.identity);
                }
                timelineobjects.Add(timelineobject);

                if (l == ApplicationModel.Level - 1)
                {
                    //input (caché)
                    GameObject timelineInputobject = (GameObject)Instantiate(Resources.Load("Timeline/prefab/input"), timelineobjectpos, Quaternion.identity);
                    timelineinputobjects.Add(timelineInputobject);
                    timelineInputobject.SetActive(false);
            }
        }
        }

        int level = 0;
        foreach (List<int> levelinputs in ApplicationModel.Inputs)
        {
            foreach (int input in levelinputs)
            {
                var timeBasePosition = new Vector3(5 + level, -2, 0);

                var timelineobjectpos = timeBasePosition + Vector3.up * input * tilesize / 100;
                GameObject timelineobject = (GameObject)Instantiate(Resources.Load("Timeline/prefab/input"), timelineobjectpos, Quaternion.identity);
                timelineobjects.Add(timelineobject);
            }

            level++;
        }

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

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ApplicationModel.Level = 1;
            ApplicationModel.Inputs.Clear();
            SceneManager.LoadScene(0);
        }

        if (moveTimer > 1)
        {
            playerPos = nextPos;
            teleportPreviousPosition = playerPos;
            moveTimer = moveTimer - 1;

            int currentTimeSlot = Mathf.FloorToInt(levelTimer);

            int level = 1;
            foreach(List<int> levelinputs in ApplicationModel.Inputs)
            {
                if (levelinputs.Contains(currentTimeSlot))
                {
                    AudioSource audio = GetComponent<AudioSource>();
                    AudioClip clip = null;
                    switch (ApplicationModel.Mapset.Levels[level-1].Mechanic)
                    {
                        case LevelMechanic.TurnRight:
                            playerDir = Position.turnRight(playerDir);
                            clip = (AudioClip)Resources.Load("Sound/stem04");
                            break;
                        case LevelMechanic.TurnLeft:
                            playerDir = Position.turnLeft(playerDir);
                            clip = (AudioClip)Resources.Load("Sound/stem03");
                            break;
                        case LevelMechanic.SlideLeft:
                            playerPos = Position.TeleportLeft(playerPos, playerDir, 3);
                            playerPos = checkTeleportPosition(playerPos, Position.turnLeft(playerDir));
                            clip = (AudioClip)Resources.Load("Sound/stem05");
                            break;
                        case LevelMechanic.SlideRight:
                            playerPos = Position.TeleportRight(playerPos, playerDir, 3);
                            playerPos = checkTeleportPosition(playerPos, Position.turnRight(playerDir));
                            clip = (AudioClip)Resources.Load("Sound/stem05");
                            break;
                        case LevelMechanic.TeleportFwd:
                            playerPos = Position.TeleportFwd(playerPos, playerDir, 3);
                            playerPos = checkTeleportPosition(playerPos, playerDir);
                            clip = (AudioClip)Resources.Load("Sound/stem01");
                            break;
                        case LevelMechanic.TeleportBwd:
                            playerPos = Position.TeleportBwd(playerPos, playerDir, 3);
                            playerPos = checkTeleportPosition(playerPos, Position.turnLeft(Position.turnLeft(playerDir)));
                            clip = (AudioClip)Resources.Load("Sound/stem01");
                            break;
                        case LevelMechanic.StartStop:
                            walking = !walking;
                            clip = (AudioClip)Resources.Load("Sound/stem02");
                            break;
                        default:
                            break;
                    }
                    if (clip != null)
                    {
                        audio.PlayOneShot(clip);
                    }
                }
                
                level++;
            }

            if (ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Collectibles[nextPos.X, nextPos.Y].Type == CollectibleType.Coin)
            {
                score += 10;
                ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Collectibles[nextPos.X, nextPos.Y].Type = CollectibleType.Nothing;
                Destroy(ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Collectibles[nextPos.X, nextPos.Y].GameObject);
            }

            nextPos = playerPos.Add(Position.DirToPos(playerDir));
            if (!walking || ApplicationModel.Mapset.Levels[ApplicationModel.Level-1].Environment[nextPos.X, nextPos.Y].Type == TileType.Wall)
            {
                nextPos = playerPos;
            }
        }

        updateTimeLine();
        updateRotation();

        if (levelTimer > leveltime * speed)
        {
            ApplicationModel.Level++;
            if (ApplicationModel.Level > ApplicationModel.Mapset.Levels.Length)
            {
                ApplicationModel.Level = 1;
                ApplicationModel.Inputs.Clear();
            }
            SceneManager.LoadScene(0);
        }
    }

    private Position checkTeleportPosition(Position position, Direction dir)
    {
        BaseEnvironment[,] env = ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Environment;
        bool stop = false;
        Position newPosition = getRelativePosition(position);
        while (!stop)
        {
            if (env[newPosition.X, newPosition.Y].Type != TileType.Wall)
                stop = true;
            else
                newPosition = getRelativePosition(newPosition.Add(Position.DirToPos(dir).Multiply(-1)));
        }

        return newPosition;
    }

    private Position getRelativePosition(Position position)
    {
        BaseEnvironment[,] env = ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Environment;
        return new Position((position.X + env.GetLength(0)) % env.GetLength(0), (position.Y + env.GetLength(1)) % env.GetLength(1));
    }

    private void updateRotation()
    {
        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Position.DirToRot(playerDir), 0.2f);
    }

    private void updateTimeLine()
    {
        foreach (GameObject o in timelineobjects)
        {
            o.transform.Translate(Vector3.down * speed * tilesize * Time.deltaTime / 100);
            if (o.transform.position.y > 3 || o.transform.position.y < -3)
            {
                o.SetActive(false);
            }
            else
            {
                o.SetActive(true);
            }
        }
        int i = 0;
        foreach (GameObject o in timelineinputobjects)
        {
            o.transform.Translate(Vector3.down * speed * tilesize * Time.deltaTime / 100);
            if (o.transform.position.y < -3)
            {
                o.SetActive(false);
            }
            else if (ApplicationModel.Inputs[ApplicationModel.Level - 1].Contains(i) && o.transform.position.y < -2)
            {
                o.SetActive(true);
            }
            i++;
        }
    }
}
