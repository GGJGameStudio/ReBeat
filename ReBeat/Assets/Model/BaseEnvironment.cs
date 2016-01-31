﻿using System;
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
                case "floor01":
                    Type = TileType.Blank;
                    break;
                default:
                    Type = TileType.Wall;
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
