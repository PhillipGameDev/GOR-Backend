﻿namespace GameOfRevenge.Common.Models.Kingdom.AttackAlertReport
{
    public class MapLocation
    {
        public int X { get; set; }
        public int Y { get; set; }

        public MapLocation()
        {
        }

        public MapLocation(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}