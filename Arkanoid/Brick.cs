using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid
{
    public class Brick
    {
        int thickness;
        int hits;

        public Brick(int thickness = 1)
        {
            this.thickness = thickness;
            this.hits = 0;
        }
    }
}
