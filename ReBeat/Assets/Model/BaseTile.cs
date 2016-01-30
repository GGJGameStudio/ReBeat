using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Model
{
    public class BaseTile
    {
        public TileType Type;
        public String UnityResource;
        public int Id;
    }

    public enum TileType
    {
        Blank,
        Wall
    }
}
