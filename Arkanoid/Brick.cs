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
        private int thickness;
        private int hits;

        private int score;

        public int Score { get => score; }

        internal bool isAlive = true;

        PictureBox pictureBox;

        public Brick(PictureBox picBox,int thickness = 1)
        {
            this.thickness = thickness;
            score = 10 * thickness;
            hits = 0;
            pictureBox = picBox;
        }

        public void hit()
        {
            hits += 1;
            if(hits >= thickness)
            {
                isAlive = false;
                pictureBox.Visible = false;
            }
            else
            {
                object obj = GameForm.resourceManager.GetObject("crack" + (thickness-hits).ToString());
                pictureBox.Image = (System.Drawing.Bitmap)obj;
            }
        }
    }
}
