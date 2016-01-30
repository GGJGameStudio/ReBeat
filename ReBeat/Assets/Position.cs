using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public Position (int x, int y)
    {
        X = x;
        Y = y;
    }

    public Position(Position p)
    {
        X = p.X;
        Y = p.Y;
    }

    public Position Add(Position p)
    {
        return new Position(this.X + p.X, this.Y + p.Y);
    }

    public Position Multiply(int i)
    {
        return new Position(this.X * i, this.Y * i);
    }

    public Vector3 ToWorldPos(int tilesize, int mapsize)
    {
        return new Vector3(-mapsize / 2 + X, -mapsize / 2 + Y, 0) * tilesize / 100;
    }

    public static Position DirToPos(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return new Position(0, 1);
            case Direction.Right:
                return new Position(1, 0);
            case Direction.Down:
                return new Position(0, -1);
            case Direction.Left:
                return new Position(-1, 0);
            case Direction.None:
            default:
                return new Position(0, 0);
        }
    }

    public static Direction turnRight(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return Direction.Right;
            case Direction.Right:
                return Direction.Down;
            case Direction.Down:
                return Direction.Left;
            case Direction.Left:
                return Direction.Up;
            case Direction.None:
            default:
                return Direction.None;
        }
    }

    public static Direction turnLeft(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return Direction.Left;
            case Direction.Right:
                return Direction.Up;
            case Direction.Down:
                return Direction.Right;
            case Direction.Left:
                return Direction.Down;
            case Direction.None:
            default:
                return Direction.None;
        }
    }

    public static Position TeleportLeft(Position initialPosition, Direction direction, int tileCount)
    {
        return DirToPos(turnLeft(direction)).Multiply(tileCount).Add(initialPosition);
    }

    public static Position TeleportRight(Position initialPosition, Direction direction, int tileCount)
    {
        return DirToPos(turnRight(direction)).Multiply(tileCount).Add(initialPosition);
    }

    public static Position TeleportFwd(Position initialPosition, Direction direction, int tileCount)
    {
        return DirToPos(direction).Multiply(tileCount).Add(initialPosition);
    }

    public static Position TeleportBwd(Position initialPosition, Direction direction, int tileCount)
    {
        return DirToPos(direction).Multiply(-tileCount).Add(initialPosition);
    }

    public static Quaternion DirToRot(Direction dir)
    {
        Quaternion rotation = Quaternion.identity;
        switch (dir)
        {
            case Direction.Up:
                rotation.eulerAngles = new Vector3(0, 0, 90);
                break;
            case Direction.Right:
                break;
            case Direction.Down:
                rotation.eulerAngles = new Vector3(0, 0, -90);
                return rotation;
            case Direction.Left:
                rotation.eulerAngles = new Vector3(0, 0, -180);
                return rotation;
            case Direction.None:
            default:
                return Quaternion.identity;
        }

        return rotation;
    }





}
