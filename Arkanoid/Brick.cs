using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    public class Brick
    {
        int thickness;
        int hits;

        PictureBox pictureBox;

        public Brick(PictureBox picBox, int thickness = 1)
        {
            this.thickness = thickness;
            hits = 0;
            pictureBox = picBox;
        }
    }
}
