﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Model
{
    public class Mapset
    {
        public Level[] Levels;
        public TileSet Tiles;
    }

    public class Level
    {
        public BaseEnvironment[,] Environment;
        public BaseCollectible[,] Collectibles;
        public int Index;
        public Direction StartDirection;
        public LevelMechanic Mechanic;
        public int StartX = 1;
        public int StartY = 1;


        public Level(int width, int height)
        {
            Environment = new BaseEnvironment[width, height];
            Collectibles = new BaseCollectible[width, height];
            Index = 0;
            StartDirection = Direction.Up;
        }

        public void StackLevelCollectibles(Level lvl)
        {
            for (int i = 0; i < Collectibles.GetLength(0); i++)
                for (int j = 0; j < Collectibles.GetLength(1); j++)
                    if (this.Collectibles[i, j].Type == CollectibleType.Nothing)
                        this.Collectibles[i, j] = lvl.Collectibles[i, j].Clone();
                
        }
    }

    public enum LevelMechanic
    {
        None,
        TurnRight,
        TurnLeft,
        SlideRight,
        SlideLeft,
        TeleportFwd,
        TeleportBwd,
        StartStop
    }

    public class TileSet
    {
        public Dictionary<int, BaseEnvironment> EnvironmentSet = new Dictionary<int, BaseEnvironment>();
        public Dictionary<int, BaseCollectible> CollectibleSet = new Dictionary<int, BaseCollectible>();
    }
}
