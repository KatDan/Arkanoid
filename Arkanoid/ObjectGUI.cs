using System;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace Arkanoid
{
    /// <summary>
    /// An abstract class for the interfaces between the GUI and the objects in a game.
    /// </summary>
    public abstract class ObjectGUI
    {
        /// <summary>
        /// A System.Windows.Forms.PictureBox instance that visualizes the object from the game.
        /// </summary>
        internal PictureBox pictureBox;

        /// <summary>
        /// Updates the position of pictureBox when the position of represented object has changed.
        /// </summary>
        void updatePosition() { }
    }

    /// <summary>
    /// A class that visualizes Arkanoid.Ball in the GUI.
    /// </summary>
    public class BallGUI : ObjectGUI
    {
        /// <summary>
        /// Arkanoid.Ball object that is represented by pictureBox in the GUI.
        /// </summary>
        internal Ball ball;

        /// <summary>
        /// A constructor that sets pictureBox properties before adding it to Controls.
        /// </summary>
        /// <param name="panel"> System.Windows.Forms.Panel that is used as the GUI of the game.</param>
        /// <param name="ball">Arkanoid.Ball object needed to be represented in the GUI.</param>
        public BallGUI(Panel panel, Ball ball)
        {
            this.ball = ball;
            this.pictureBox = new PictureBox
            {
                Size = new Size(2*ball.radius, 2*ball.radius),
                Location = new Point((int)(ball.x - ball.radius), (int)(ball.y - ball.radius)),
                Image = Properties.Resources.ball,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Parent = panel
            };
        }

        public void updatePosition()
        {
            if(2*ball.radius != pictureBox.Width)
            {
                pictureBox.Size = new Size(2*ball.radius, 2 * ball.radius);
            }
            pictureBox.Location = new Point((int)ball.x - ball.radius,
                (int)ball.y - ball.radius);
            pictureBox.Visible = true;
        }
    }

    /// <summary>
    /// A class that visualizes Arkanoid.Paddle in the GUI.
    /// </summary>
    public class PaddleGUI : ObjectGUI
    {
        /// <summary>
        /// Arkanoid.Paddle object that is represented by pictureBox in the GUI.
        /// </summary>
        Paddle paddle;

        /// <summary>
        /// A constructor that sets pictureBox properties before adding it to Controls.
        /// </summary>
        /// <param name="panel"> System.Windows.Forms.Panel that is used as the GUI of the game.</param>
        /// <param name="paddle">Arkanoid.Paddle object needed to be represented in the GUI.</param>
        public PaddleGUI(Panel panel, Paddle paddle)
        {
            this.paddle = paddle;
            this.pictureBox = new PictureBox
            {
                Parent = panel,
                Size = new Size(paddle.Width, paddle.Height),
                Location = new Point(paddle.x - paddle.Width / 2, paddle.y - paddle.Height/2),
                Image = Properties.Resources.paddle,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
        }

        public void updatePosition()
        {
            if(paddle.Width != pictureBox.Width || paddle.Height != pictureBox.Height)
            {
                pictureBox.Size = new Size(paddle.Width, paddle.Height);
            }
            pictureBox.Location = new Point(paddle.x - paddle.Width / 2, paddle.y - paddle.Height / 2);
        }
    }

    /// <summary>
    /// A class that visualizes Arkanoid.Ball in the GUI.
    /// </summary>
    public class PowerUpGUI : ObjectGUI
    {
        /// <summary>
        /// Arkanoid.PowerUp object that is represented by pictureBox in the GUI.
        /// </summary>
        PowerUp powerUp;

        /// <summary>
        /// A state of the twinkling animation of power up.
        /// </summary>
        private int animationState = 0;
        /// <summary>
        /// Number of states of the twinkling animation of power up.
        /// </summary>
        private int animationSteps = 30;

        /// <summary>
        /// A constructor that sets pictureBox properties before adding it to Controls.
        /// </summary>
        /// <param name="panel"> System.Windows.Forms.Panel that is used as the GUI of the game.</param>
        /// <param name="powerUp">Arkanoid.PowerUp object needed to be represented in the GUI.</param>
        public PowerUpGUI(Panel panel, PowerUp powerUp)
        {
            this.powerUp = powerUp;
            pictureBox = new PictureBox
            {
                Size = new Size(powerUp.radius*2, powerUp.radius*2),
                Location = new Point(powerUp.x - powerUp.radius, powerUp.y - powerUp.radius),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Parent = panel
            };
            switch (powerUp.type)
            {
                case PowerUpType.SLOWBALL:
                    pictureBox.Image = Properties.Resources.slowball;
                    break;
                case PowerUpType.FASTBALL:
                    pictureBox.Image = Properties.Resources.fastball;
                    break;
                case PowerUpType.SUPERBALL:
                    pictureBox.Image = Properties.Resources.superball;
                    break;
                case PowerUpType.TRIPLEBALL:
                    pictureBox.Image = Properties.Resources.tripleball;
                    break;
            }
            pictureBox.Visible = true;
        }

        public void updatePosition()
        {
            if(powerUp == null)
            {
                pictureBox.Visible = false;
                return;
            }
            if(animationState < animationSteps/2 && animationState % 3 == 0)
            {
                pictureBox.Size = new Size(pictureBox.Width + 2, pictureBox.Height + 2);
                pictureBox.Location = new Point(powerUp.x - powerUp.radius - 1, powerUp.y - powerUp.radius - 1);
            }
            else if(animationState >= animationSteps / 2 && animationState % 3 == 1)
            {
                pictureBox.Size = new Size(pictureBox.Width - 2, pictureBox.Height - 2);
                pictureBox.Location = new Point(powerUp.x - powerUp.radius + 1, powerUp.y - powerUp.radius + 1);
            }
            pictureBox.Refresh();
            animationState++;
            animationState = animationState % animationSteps;
        }
    }

    /// <summary>
    /// A class that visualizes levels' brick map in the GUI.
    /// </summary>
    public class BrickMapGUI
    {
        /// <summary>
        /// 2D list of bricks that are set in the current level.
        /// </summary>
        internal List<List<Brick>> brickMap;
        /// <summary>
        /// 2D list of picture boxes that visualize the bricks from brickMap
        /// </summary>
        internal List<List<PictureBox>> bricksPicBoxes;
        /// <summary>
        /// System.Resources.ResourceManager that manages resources for this project.
        /// It is used when the brick is hit and its front image needs to be changed 
        /// into one of the crack types from resources.
        /// </summary>
        private ResourceManager resourceManager;
        /// <summary>
        /// Array of bricks that were recently hit by the existing balls.
        /// </summary>
        Coords[] recentlyHitBricks;
        /// <summary>
        /// Panel used as the GUI.
        /// </summary>
        Panel panel;

        /// <summary>
        /// A constructor that initializes 2D list of picture boxes representing
        /// individual bricks of brickMap.
        /// </summary>
        /// <param name="panel">System.Windows.Forms.Panel used as the GUI.</param>
        /// <param name="resourceManager">System.Resources.ResourceManager that manages resources for this project.</param>
        /// <param name="game">Arkanoid.Game that needs to be visualized in the GUI.</param>
        public BrickMapGUI(Panel panel, ResourceManager resourceManager, Game game)
        {
            this.resourceManager = resourceManager;
            this.recentlyHitBricks = new Coords[game.maxBallsCount];
            this.recentlyHitBricks = game.recentlyHitBricks;
            this.panel = panel;
            brickMap = game.levelBrickMap;
            bricksPicBoxes = new List<List<PictureBox>>();
            updatePicBoxes(game);
        }

        /// <summary>
        /// Sets the new PictureBox array when level of the game is changed, 
        /// so it represents current brick map of the level.
        /// </summary>
        /// <param name="game">Arkanoid.Game that is visualized in the GUI.</param>
        public void updatePicBoxes(Game game)
        {
            brickMap = game.levelBrickMap;
            bricksPicBoxes.Clear();
            for (int line = 0; line < brickMap.Count; line++)
            {
                List<PictureBox> brickLine = new List<PictureBox>();
                for (int column = 0; column < brickMap[line].Count; column++)
                {
                    Brick brick = brickMap[line][column];
                    if(brick == null){
                        brickLine.Add(null);
                        continue;
                    }
                    PictureBox brickPictureBox = new PictureBox
                    {
                        Parent = panel,
                        Size = new Size(game.brickWidth, game.brickHeight),
                        Location = new Point(column * game.brickWidth, line * game.brickHeight),
                        Image = null,
                        BackgroundImageLayout = ImageLayout.Stretch,
                        SizeMode = PictureBoxSizeMode.StretchImage
                    };
                    object obj = resourceManager.GetObject("brick" + brick.thickness.ToString());
                    brickPictureBox.BackgroundImage = (Bitmap)obj;
                    brickLine.Add(brickPictureBox);
                }
                bricksPicBoxes.Add(brickLine);
            }
        }

        /// <summary>
        /// Updates pictureBoxes of particular bricks that were listed in recentlyHitBricks as hit the most recently.
        /// </summary>
        public void update()
        {
            for(int index = 0; index < recentlyHitBricks.Length; index++)
            {
                if (recentlyHitBricks[index] == null || recentlyHitBricks[index].row == -1 || recentlyHitBricks[index].column == -1) continue;
                else
                {
                    PictureBox recentBrick = bricksPicBoxes[recentlyHitBricks[index].row][recentlyHitBricks[index].column];

                    Brick brick = brickMap[recentlyHitBricks[index].row][recentlyHitBricks[index].column];
                    if (!brick.isAlive)
                    {
                        recentBrick.Visible = false;
                        return;
                    }
                    object obj = resourceManager.GetObject("crack" + (brick.thickness - brick.Hits).ToString());
                    recentBrick.Image = (System.Drawing.Bitmap)obj;
                }
            }
        }
    }

}
