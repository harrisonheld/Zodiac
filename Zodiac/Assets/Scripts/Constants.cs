using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public static class Constants
{
    // some far away point to put stuff
    public static readonly Vector2Int OFFSCREEN = new Vector2Int(-100, -100);

    // how much energy it takes for various actions
    public const int COST_MOVE = 1000;

    // max amount of energy an entity can have
    public const int ENERGY_CAP = 1000;
}