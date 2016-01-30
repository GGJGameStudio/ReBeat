using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Model
{
    public class BaseCollectible : BaseTile
    {
        public CollectibleType Type;

        public override void initTileType()
        {
            switch (UnityResource)
            {
                case "coin":
                    Type = CollectibleType.Coin;
                    break;
                case "bigcoin":
                    Type = CollectibleType.BigCoin;
                    break;
                case "malus":
                    Type = CollectibleType.Malus;
                    break;
                default:
                    Type = CollectibleType.Nothing;
                    break;
            }
        }

        public virtual void ApplyEffect()
        {
            //this method is separated from collectibles because it will interfere on something else
        }
    }

    public enum CollectibleType
    {
        Nothing,
        Coin,
        BigCoin,
        Malus
    }
}
