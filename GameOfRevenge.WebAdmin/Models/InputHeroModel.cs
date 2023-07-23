using System;
using System.Collections.Generic;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.WebAdmin.Models
{
    public class InputHeroModel
    {
        public int PlayerId { get; set; }
        public string HeroType { get; set; }
        public string HeroValues { get; set; }

        public UserHeroDetails Hero { get; set; }
        public int Level
        {
            get
            {
                var lvl = 0;
                if (Hero != null)
                {
                    lvl = (int)Math.Floor((double)Hero.Points / UserHeroDetails.UNLOCK_POINTS);
                }
                return lvl;
            }
        }

        public InputHeroModel()
        {
        }
    }
}
