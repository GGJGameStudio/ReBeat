using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using SimpleJSON;
using Assets.Model;
using UnityEngine.UI;

public class Main : MonoBehaviour {

    private int mapsize = 20;
    private int resourcetilesize = 144;
    private int tilesize = 48;
    private float speed = 3;
    private float leveltime = 12;
    private float tolerance = 1f;
    private float startDelay = 2;
    private float endLevelDelay = 1f;

    private GameObject player;
    private Position teleportPreviousPosition;
    private Position playerPos;
    private Position nextPos;
    private Direction playerDir;

    private bool walking = true;

    private float moveTimer;
    private float levelTimer;
    private List<GameObject> timelineobjects = new List<GameObject>();
    private List<GameObject> timelineinputobjects = new List<GameObject>();
    

    private float uiOffsetX = 5.25f;
    private float uiOffsetY = 0.25f;
    private float uiOffsetPlayerY = -2;
    private float uiSizeY = 3;
    private float uiGapXBase = 3f;

    public GameObject textUI;

    private bool wololo = false;

    // Use this for initialization
    void Start ()
    {
        moveTimer = -speed * startDelay;
        levelTimer = -speed * startDelay;
        wololo = false;
        //charger map
        if (ApplicationModel.Level == 1)
        {
            ApplicationModel.Mapset = JSONParser.Parse(((TextAsset)Resources.Load("Worlds/" + ApplicationModel.World + "/Set_" + ApplicationModel.MapsetNumber)).text);
            ApplicationModel.Score = 0;
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
                string envTex = "Environment/Prefabs/" + ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Environment[i, j].UnityResource;

                obj = (GameObject)Instantiate(Resources.Load(envTex), pos, Quaternion.identity);
                obj.transform.localScale = new Vector3((float)tilesize / resourcetilesize, (float)tilesize / resourcetilesize);
                ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Environment[i, j].GameObject = obj;

                obj = null;

                if (ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Collectibles[i, j].Type != CollectibleType.Nothing && ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Collectibles[i, j].Type != CollectibleType.StartPosition)
                {
                    string colTex = "Collectibles/Prefabs/" + ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Collectibles[i, j].UnityResource;
                    obj = (GameObject)Instantiate(Resources.Load(colTex), pos, Quaternion.identity);
                    obj.transform.localScale = new Vector3((float)tilesize / resourcetilesize, (float)tilesize / resourcetilesize);
                    ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Collectibles[i, j].GameObject = obj;
                }
            }
        }

        startPos = new Position(startPos);
        playerPos = new Position(startPos);
        teleportPreviousPosition = new Position(startPos);
        playerDir = startDir;
        nextPos = playerPos.Add(Position.DirToPos(playerDir));
        walking = true;

        Vector3 playerstartpos = startPos.ToWorldPos(tilesize, mapsize);
        if (ApplicationModel.KonamiCodeActivated)
        {
            player = (GameObject)Instantiate(Resources.Load("Player/Prefabs/caramel_0"), playerstartpos, Quaternion.identity);
            player.transform.localScale = new Vector3(3 * (float)tilesize / resourcetilesize, 3 * (float)tilesize / resourcetilesize);

            GameObject background = (GameObject)Instantiate(Resources.Load("Player/Prefabs/karamel_0"), Vector3.zero, Quaternion.identity);
            background.transform.localScale = new Vector3(10f, 10f, 1);
            background.GetComponent<SpriteRenderer>().sortingOrder = -1;

        } else
        {
            player = (GameObject)Instantiate(Resources.Load("Player/Prefabs/penrose01"), playerstartpos, Quaternion.identity);
            player.transform.localScale = new Vector3((float)tilesize / resourcetilesize, (float)tilesize / resourcetilesize);
        }
        
        
        var camera = GetComponent<Camera>();
        for (int l = 0; l < ApplicationModel.Level; l++)
        {
            Vector3 timelinepos = new Vector3(uiOffsetX + l * uiGapXBase / (ApplicationModel.Level + 1), uiOffsetY, 0);
            var timeline = (GameObject)Instantiate(Resources.Load("Timeline/prefab/timeline"), timelinepos, Quaternion.identity);
            
            var timeBasePosition = new Vector3(uiOffsetX + l * uiGapXBase / (ApplicationModel.Level + 1), uiOffsetY + uiOffsetPlayerY, 0);
            var basetime = (GameObject)Instantiate(Resources.Load("Timeline/prefab/player"), timeBasePosition, Quaternion.identity);

            var iconePosition = new Vector3(uiOffsetX + l * uiGapXBase / (ApplicationModel.Level + 1), uiOffsetY + uiOffsetPlayerY + 5.25f, 0);
            GameObject icone = null;
            switch(ApplicationModel.Mapset.Levels[l].Mechanic)
            {
                case LevelMechanic.TurnLeft:
                    icone = (GameObject)Instantiate(Resources.Load("Icones/prefab/gauche"), iconePosition, Quaternion.identity);
                    break;
                case LevelMechanic.TurnRight:
                    icone = (GameObject)Instantiate(Resources.Load("Icones/prefab/droite"), iconePosition, Quaternion.identity);
                    break;
                case LevelMechanic.TeleportFwd:
                    icone = (GameObject)Instantiate(Resources.Load("Icones/prefab/tp"), iconePosition, Quaternion.identity);
                    break;
                case LevelMechanic.StartStop:
                    icone = (GameObject)Instantiate(Resources.Load("Icones/prefab/stop"), iconePosition, Quaternion.identity);
                    break;
            }
            if (icone != null)
            {
                icone.transform.localScale = new Vector3(0.3f, 0.3f, 1);
            }

            for (int t = -(int)levelTimer; t <= -levelTimer + leveltime * speed; t++)
            {
                //graduation
                var timelineobjectpos = timeBasePosition + Vector3.up * t * tilesize / 100;
                GameObject timelineobject = null;
                if (t != -levelTimer + leveltime * speed)
                {
                    timelineobject = (GameObject)Instantiate(Resources.Load("Timeline/prefab/graduation"), timelineobjectpos, Quaternion.identity);
                }
                else
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
                var timeBasePosition = new Vector3(uiOffsetX + level * uiGapXBase / (ApplicationModel.Level + 1), uiOffsetY + uiOffsetPlayerY, 0);

                var timelineobjectpos = timeBasePosition + Vector3.up * (-levelTimer + input) * tilesize / 100;
                GameObject timelineobject = (GameObject)Instantiate(Resources.Load("Timeline/prefab/input"), timelineobjectpos, Quaternion.identity);
                timelineobjects.Add(timelineobject);
            }

            level++;
        }

        Text scoreText = textUI.AddComponent<Text>();
        scoreText.text = ApplicationModel.Score.ToString();
        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        scoreText.font = ArialFont;
        scoreText.fontSize = 30;
        scoreText.material = ArialFont.material;
        textUI.transform.position = new Vector3(uiOffsetX + 6, uiOffsetY + uiOffsetPlayerY - 5, 0);

    }
	
	// Update is called once per frame
	void Update () {
        moveTimer += Time.deltaTime * speed;
        levelTimer += Time.deltaTime * speed;

        if (!wololo && levelTimer > 0)
        {
            wololo = true;

            AudioSource audio = GetComponent<AudioSource>();
            AudioClip clip = (AudioClip)Resources.Load("Sound/wololo");
            if (clip != null)
            {
                audio.PlayOneShot(clip);
            }

        }

        player.transform.position = Vector3.Lerp(playerPos.ToWorldPos(tilesize, mapsize), nextPos.ToWorldPos(tilesize, mapsize), moveTimer);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            int timeSlot = Mathf.CeilToInt(levelTimer);
            if (timeSlot - levelTimer < tolerance && levelTimer > 0 && levelTimer < leveltime * speed)
            {
                ApplicationModel.Inputs[ApplicationModel.Level - 1].Add(timeSlot);
            }
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
                ApplicationModel.Score += 10;
                ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Collectibles[nextPos.X, nextPos.Y].Type = CollectibleType.Nothing;
                Destroy(ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Collectibles[nextPos.X, nextPos.Y].GameObject);
            }

            if (ApplicationModel.Mapset.Levels[ApplicationModel.Level - 1].Collectibles[nextPos.X, nextPos.Y].Type == CollectibleType.BigCoin)
            {
                ApplicationModel.Score += 100;
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

        textUI.GetComponent<Text>().text = ApplicationModel.Score.ToString();

        if (levelTimer > leveltime * speed && player.activeSelf)
        {

            AudioSource audio = GetComponent<AudioSource>();
            AudioClip clip = (AudioClip)Resources.Load("Sound/Gewonnen");
            audio.PlayOneShot(clip);
            GameObject animation = (GameObject)Instantiate(Resources.Load("Player/Prefabs/explo_0"), player.transform.position, Quaternion.identity);
            animation.transform.localScale = new Vector3(0.6f * tilesize / resourcetilesize, 0.6f * tilesize / resourcetilesize);
            Destroy(animation, 0.3f);
            player.SetActive(false);
        }

        if (levelTimer > (leveltime + endLevelDelay ) * speed )
        {
            ApplicationModel.Level++;
            if (ApplicationModel.Level > ApplicationModel.Mapset.Levels.Length)
            {
                ApplicationModel.Level = 1;
                ApplicationModel.Inputs.Clear();
                SceneManager.LoadScene(2);
            }
            else
            {
                SceneManager.LoadScene(1);
            }
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
            if (o.transform.position.y > uiOffsetY + uiSizeY || o.transform.position.y < uiOffsetY - uiSizeY)
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
            if (o.transform.position.y < uiOffsetY - uiSizeY)
            {
                o.SetActive(false);
            }
            else if (ApplicationModel.Inputs[ApplicationModel.Level - 1].Contains(i) && o.transform.position.y < uiOffsetY + uiOffsetY - uiOffsetPlayerY)
            {
                o.SetActive(true);
            }
            i++;
        }
    }
}
