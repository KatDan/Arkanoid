using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    public partial class GameForm : Form
    {
        Paddle paddle;
        List<Ball> balls;

        int score = 0;
        int level = 1;

        Brick[][] levelBrickMap;


        public GameForm()
        {
            InitializeComponent();
        }

        private void initializeGame()
        {
            this.Label.Text = "level: " + level.ToString();
            this.scoreLabel.Text = "score: " + score.ToString();
        }

        private void tick(object sender, EventArgs e)
        {

        }

        private void keyIsDown(object sender, KeyEventArgs e)
        {

        }

        private void keyIsUp(object sender, KeyEventArgs e)
        {

        }
    }
}
