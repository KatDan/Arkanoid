using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arkanoid
{
    public partial class GameForm : Form
    {
        int bricksPerLine = 8;
        
        int brickWidth
        {
            get { return (int)((Width-18) / bricksPerLine); }
        }
        int brickHeight
        {
            get { return (int)(brickWidth / 4); }
        }

        Paddle paddle;
        List<Ball> balls;

        int score = 0;
        int level = 1;

        List<List<Brick>> levelBrickMap;


        public GameForm()
        {
            InitializeComponent();
            initializeGame();
        }

        private void initializeGame()
        {
            this.Label.Text = "level: " + level.ToString();
            this.scoreLabel.Text = "score: " + score.ToString();
            List<List<int>> brickMapData = loadLevelBrickInfo(1);
            levelBrickMap = initializeLevel(brickMapData);
        }

        private List<List<int>> loadLevelBrickInfo(int level)
        {
            List<List<int>> result = new List<List<int>>();
            string filename = "level" + level.ToString() + ".csv";
            using(var reader = new StreamReader(@filename))
            {
                while (!reader.EndOfStream)
                {
                    List<int> brickLine = new List<int>();
                    var line = reader.ReadLine().Split(';');
                    if(line.Length != bricksPerLine)
                    {
                        throw new LevelFormatException();
                    }
                    foreach (string item in line)
                    {
                        int thickness = 0;
                        if (item != "")
                        {
                            if (!int.TryParse(item, out thickness))
                            {
                                throw new LevelFormatException();
                            }
                            brickLine.Add(thickness);
                        }
                    }
                    result.Add(brickLine);
                }
            }
            return result;
        }

        private List<List<Brick>> initializeLevel(List<List<int>> brickMap)
        {
            List<List<Brick>> result = new List<List<Brick>>();
            for(int line = 0; line < brickMap.Count; line++)
            {
                List<Brick> brickLine = new List<Brick>();
                for(int column = 0; column < brickMap[line].Count; column++)
                {
                    int thickness = brickMap[line][column];
                    if(thickness != 0)
                    {
                        PictureBox brickPictureBox = new PictureBox
                        {
                            Size = new Size(brickWidth, brickHeight),
                            Location = new Point(column * brickWidth, 50 + line * brickHeight),
                            Image = Properties.Resources.cihla,
                            SizeMode = PictureBoxSizeMode.Normal
                        };
                        Controls.Add(brickPictureBox);
                        brickLine.Add(new Brick(brickPictureBox, thickness));
                    }
                    else
                    {
                        brickLine.Add(null);
                    }
                }
                result.Add(brickLine);
            }
            return result;
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
