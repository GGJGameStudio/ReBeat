using System;
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
        public LevelStartDirection StartDirection;
        public LevelMechanic Mechanic;
        public int StartX = 1;
        public int StartY = 1;


        public Level(int width, int height)
        {
            Environment = new BaseEnvironment[width, height];
            Collectibles = new BaseCollectible[width, height];
            Index = 0;
            StartDirection = LevelStartDirection.Up;
        }

        public void StackLevelCollectibles(Level lvl)
        {
            for (int i = 0; i < Collectibles.GetLength(0); i++)
                for (int j = 0; j < Collectibles.GetLength(1); i++)
                    if (this.Collectibles[i, j] != null && this.Collectibles[i, j].Type ==CollectibleType.Nothing)
                        this.Collectibles[i, j] = lvl.Collectibles[i, j];
                
        }
    }

    public enum LevelStartDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum LevelMechanic
    {
        None,
        TurnRight,
        TurnLeft,
        SlideRight,
        SlideLeft,
        TeleportFwd,
        TeleportBwd
    }

    public class TileSet
    {
        public Dictionary<int, BaseEnvironment> EnvironmentSet = new Dictionary<int, BaseEnvironment>();
        public Dictionary<int, BaseCollectible> CollectibleSet = new Dictionary<int, BaseCollectible>();
        
    }
}
