using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Model
{
    public abstract class BaseTile
    {
        public String UnityResource;
        public int Id;

        public abstract void initTileType();
    }
}
