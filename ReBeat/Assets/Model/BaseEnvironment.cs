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
                case "floor00":
                case "floor02":
                case "floor03":
                case "floor04":
                case "floor05":
                case "floor06":
                case "floor07":
                case "floor08":
                case "floor09":
                case "floor10":
                case "floor11":
                case "floor12":
                case "floor13":
                case "floor14":
                    Type = TileType.Wall;
                    break;
                default:
                    Type = TileType.Blank;
                    break;
            }
        }


        public BaseEnvironment Clone()
        {
            BaseEnvironment clone = new BaseEnvironment()
            {
                Id = this.Id,
                Type = this.Type,
                UnityResource = this.UnityResource
            };
            return clone;
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
