using System;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.WebAdmin.Models
{
    public class UserTable
    {
        public PlayerInfo[] Users { get; set; }
        public int[] Pages { get; set; }
        public int CurrentPage { get; set; }
        public int LastPage { get; set; }
        public int OffsetPage { get; set; }

        public int PageFromOffset(int shift)
        {
            var page = OffsetPage + shift;
            if (page < 0) page = 0;
            if (page > LastPage) page = LastPage;
            return page;
        }

        public int PageFromCurrent(int shift)
        {
            if (shift < 0)
            {
                return Math.Max(CurrentPage + shift, 0);
            }
            else
            {
                return Math.Min(CurrentPage + shift, LastPage);
            }
        }
    }
}
