using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Model
{
    public class Mapset
    {

        public Level[] Levels;

    }

    public class Level
    {
        BaseTile[,] Environment;
        BaseCollectible[,] Collectibles;
    }
}
