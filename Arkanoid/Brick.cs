using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    public class Brick
    {
        internal int thickness;
        private int hits;
        public int Hits { get => hits; }

        private int score;

        public int Score { get => score; }

        internal bool isAlive = true;

        public Brick(int thickness = 1)
        {
            this.thickness = thickness;
            score = 10 * thickness;
            hits = 0;
        }

        public void hit()
        {
            hits += 1;
            if(hits >= thickness)
            {
                isAlive = false;
            }
        }
    }
}
