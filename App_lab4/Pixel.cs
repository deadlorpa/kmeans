using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_lab4
{
    public class Pixel
    {
        public int r { set; get; }
        public int g { set; get; }
        public int b { set; get; }
        public int x { set; get; }
        public int y { set; get; }
        public int cluster { set; get; }
        public int i { set; get; }
        public Pixel(int r, int g, int b, int x, int y, int i)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.x = x;
            this.y = y;
            this.i = i;
            cluster = -1;
        }

        public long distance(Pixel other)
        {
            return 30 * (r - other.r) * (r - other.r) + 59 * (g - other.g) * (g - other.g) + 11 * (b - other.b) * (b - other.b);
        }
    }
}
