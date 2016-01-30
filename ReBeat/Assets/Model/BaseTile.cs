using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Model
{
    public abstract class BaseTile
    {
        public GameObject GameObject;
        public String UnityResource;
        public int Id;

        public abstract void initTileType();
        
    }
}
