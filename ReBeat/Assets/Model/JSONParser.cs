using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Model
{
    class JSONParser
    {

        public static Mapset Parse(String mapsetJsonStr)
        {
            Mapset mapset = new Mapset();

            var mapsetJson = JSON.Parse(mapsetJsonStr);

            //Build TileSet
            TileSet tileSet = new TileSet();
            tileSet.CollectibleSet.Add(0, new BaseCollectible() { Id = 0, Type = CollectibleType.Nothing });
            tileSet.EnvironmentSet.Add(0, new BaseEnvironment() { Id = 0, Type = TileType.Blank });
            #region tileset Build
            JSONArray tilesetJsonArray = mapsetJson["tilesets"].AsArray;
            foreach(JSONNode tilesetJson in tilesetJsonArray)
            {
                
                int baseGuid = tilesetJson["firstgid"].AsInt;
                String tilesetName = tilesetJson["name"].Value;

                JSONClass properties = tilesetJson["tileproperties"].AsObject;

                foreach (String key in properties.GetKeys())
                {
                    int guid = baseGuid + Int32.Parse(key);
                    String unityResource = properties[key]["UnityResource"].Value;
                    BaseTile tile;
                    if (tilesetName == "Env")
                    {
                        tile = new BaseEnvironment();
                        tileSet.EnvironmentSet.Add(guid,(BaseEnvironment)tile);
                    }
                    else
                    {
                        tile = new BaseCollectible();
                        tileSet.CollectibleSet.Add(guid,(BaseCollectible)tile);
                    }

                    tile.Id = guid;
                    tile.UnityResource = unityResource;
                    tile.initTileType();
                }
            }
            #endregion
            mapset.Tiles = tileSet;



            //Build Sets and Levels
            JSONArray levelsArray = mapsetJson["layers"].AsArray;

            int width = mapsetJson["width"].AsInt;
            int height = mapsetJson["height"].AsInt;

            Dictionary<int, Level> levels = new Dictionary<int, Level>();
            foreach (JSONNode node in levelsArray)
            {
                String name = node["name"].Value;
                String[] metaData = name.Split('_');
                int index = Int32.Parse(metaData[0]);
                String type = metaData[1];

                Level lvl;
                if (levels.ContainsKey(index))
                {
                    lvl = levels[index];
                }
                else
                {
                    lvl = new Level(width, height);
                    lvl.Index = index;
                    levels.Add(index, lvl);
                }

                if (type == "E")
                {
                    String mechanic = node["properties"]["Mechanic"].Value;
                    LevelMechanic mech;
                    switch (mechanic)
                    {
                        case "TurnR": mech = LevelMechanic.TurnRight; break;
                        case "TurnL": mech = LevelMechanic.TurnLeft; break;
                        case "SlideR": mech = LevelMechanic.SlideRight; break;
                        case "SlideL": mech = LevelMechanic.SlideLeft; break;
                        case "TeleF": mech = LevelMechanic.TeleportFwd; break;
                        case "TeleB": mech = LevelMechanic.TeleportBwd; break;
                        default: mech = LevelMechanic.None; break;
                    }
                    lvl.Mechanic = mech;
                }
                else if (type == "C")
                {
                    String direction = node["properties"]["StartDirection"].Value;
                    Direction dir;
                    switch (direction)
                    {
                        case "U": dir = Direction.Up; break;
                        case "D": dir = Direction.Down; break;
                        case "L": dir = Direction.Left; break;
                        case "R": dir = Direction.Right; break;
                        default: dir = Direction.Up; break;
                    }
                    lvl.StartDirection = dir;
                }

                JSONArray data = node["data"].AsArray;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        int dataIndex = i + (height - j - 1) * height;
                        int tileKey = data[dataIndex].AsInt;

                        if (type == "C" && tileSet.CollectibleSet.ContainsKey(tileKey))
                        {
                            lvl.Collectibles[i, j] = tileSet.CollectibleSet[tileKey];

                            if (lvl.Collectibles[i, j].UnityResource == "START")
                            {
                                lvl.StartX = i;
                                lvl.StartY = j;
                            }
                        }

                        if (type == "E" && tileSet.EnvironmentSet.ContainsKey(tileKey))
                        {
                            lvl.Environment[i, j] = tileSet.EnvironmentSet[tileKey];
                        }
                    }
                }
            }

            List<Level> levelList = levels.Values.ToList<Level>();
            levelList.Sort((x, y) => x.Index.CompareTo(y.Index));
            mapset.Levels = levelList.ToArray();

            return mapset;
        }


    }
}
