using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Model
{
    public class BaseEnvironment : BaseTile
    {

        public TileType Type;
        public override void initTileType()
        {
            switch (UnityResource)
            {
                case "wall":
                    Type = TileType.Wall;
                    break;
                default:
                    Type = TileType.Blank;
                    break;
            }
        }

        public virtual void ApplyEffect()
        {
            //this method is separated from collectibles because it will interfere on something else

        }

    }

    public enum TileType
    {
        Blank,
        Wall
    }
}
