using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Model
{
    public class BaseCollectible
    {
        public CollectibleType Type;
        public String UnityResource;
        public int Id;
    }

    public enum CollectibleType
    {
        Coin,
        BigCoin,
        Malus
    }
}
