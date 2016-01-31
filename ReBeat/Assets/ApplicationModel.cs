using Assets.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class ApplicationModel
{
    static public int Level = 1;
    
    static public List<List<int>> Inputs = new List<List<int>>();

    static public Mapset Mapset;

    static public int Score = 0;

    static public int World = 1;

    static public int MapsetNumber = 1;

    static public Dictionary<int, int> WorldList = new Dictionary<int, int>();

    static public bool KonamiCodeActivated = false;
}
