using System;
using System.Collections.Generic;

namespace GameOfRevenge.Common.Helper
{
    public static class MathHelper
    {
        //public static string DicToString(Dictionary<byte, object> d)
        //{
        //    //string message = string.Empty;
        //    //foreach (var item in d)
        //    //{
        //    //    message += ("  KEY: " + item.Key.ToString()) + "  " + (" VALUE: " + JsonConvert.SerializeObject(item.Value));
        //    //}
        //    //return message;
        //}

        public static List<int[]> GetStraightLinePoints(int x, int y, int x2, int y2)
        {
            var cordinates = new List<int[]>();
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                cordinates.Add(new int[]
                {
                   x,y
                });
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
            return cordinates;
        }
        public static List<int[]> BresenhamLine(int x0, int y0, int x1, int y1)
        {
            var cordinates = new List<int[]>();
            var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0); // Check the growth of the segment on the X axis and the Y-axis
                                                               // Reflectible diagonal line, if the angle is too big
            if (steep)
            {
                Swap(ref x0, ref y0); // Shuffling the origin is moved to a separate function
                Swap(ref x1, ref y1);
            }
            // If the line does not grow from left to right, then change the start and end of the segment places
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2; // It uses optimization multiplied by dx, to get rid of excess fractions
            int ystep = y0 < y1 ? 1 : -1; // Choose the direction of growth of the Y coordinate
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                cordinates.Add(new int[] { steep ? y : x, steep ? x : y });
                //DrawPoint(steep ? y : x, steep ? x : y); // Do not forget to return the coordinates back
                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
            return cordinates;
        }
        public static List<int[]> WuLine(int x0, int y0, int x1, int y1)
        {
            var cordinates = new List<int[]>();
            var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }
            //DrawPoint(steep, x0, y0, 1); // This function changes the coordinates of the machine, depending on the variable steep
            //DrawPoint(steep, x1, y1, 1); // The last argument - the intensity expressed as a decimal
            float dx = x1 - x0;
            float dy = y1 - y0;
            float gradient = dy / dx;
            float y = y0 + gradient;
            for (var x = x0 + 1; x <= x1 - 1; x++)
            {
                cordinates.Add(new int[] { x, (int)y });
                cordinates.Add(new int[] { x, (int)y + 1 });
                //  DrawPoint(steep, x, (int)y, 1 - (y - (int)y));
                //   DrawPoint(steep, x, (int)y + 1, y - (int)y);
                y += gradient;
            }
            return cordinates;
        }
        private static void Swap(ref int x, ref int y)
        {
            int temp = x;
            x = y;
            y = temp;
        }
        public static List<int[]> MidPointCircleDraw(int x_centre, int y_centre, int r)
        {
            int x = r, y = 0;
            var cordinates = new List<int[]>
            {
                new int[] { x + x_centre, y + y_centre }
            };
            if (r > 0)
            {
                cordinates.Add(new int[] { x + x_centre, -y + y_centre });
                cordinates.Add(new int[] { y + x_centre, x + y_centre });
                cordinates.Add(new int[] { -y + x_centre, x + y_centre });
            }
            int P = 1 - r;
            while (x > y)
            {
                y++;
                if (P <= 0)
                    P = P + 2 * y + 1;
                else
                {
                    x--;
                    P = P + 2 * y - 2 * x + 1;
                }
                if (x < y)
                    break;

                cordinates.Add(new int[] { x + x_centre, y + y_centre });
                cordinates.Add(new int[] { -x + x_centre, y + y_centre });
                cordinates.Add(new int[] { x + x_centre, -y + y_centre });
                cordinates.Add(new int[] { -x + x_centre, -y + y_centre });
                if (x != y)
                {

                    cordinates.Add(new int[] { y + x_centre, x + y_centre });
                    cordinates.Add(new int[] { -y + x_centre, x + y_centre });
                    cordinates.Add(new int[] { y + x_centre, -x + y_centre });
                    cordinates.Add(new int[] { -y + x_centre, -x + y_centre });
                }
            }
            return cordinates;
        }

    }
}
