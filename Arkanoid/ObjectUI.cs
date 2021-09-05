using System;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace Arkanoid
{
    public abstract class ObjectUI
    {
        internal PictureBox pictureBox;

        void updatePosition() { }
    }

    public class BallUI : ObjectUI
    {
        internal Ball ball;

        public BallUI(Panel panel, Ball ball)
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

    public class PaddleUI : ObjectUI
    {
        Paddle paddle;

        public PaddleUI(Panel panel, Paddle paddle)
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

    public class PowerUpUI : ObjectUI
    {
        PowerUp powerUp;

        private int animationState = 0;
        private int animationSteps = 30;

        public PowerUpUI(Panel panel, PowerUp powerUp)
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

    public class BrickMapUI
    {
        internal List<List<Brick>> brickMap;
        internal List<List<PictureBox>> bricksPicBoxes;
        private ResourceManager resourceManager;
        Coords[] recentlyHitBricks;
        Panel panel;

        public BrickMapUI(Panel panel, ResourceManager resourceManager, Game game)
        {
            this.resourceManager = resourceManager;
            this.recentlyHitBricks = new Coords[game.maxBallsCount];
            this.recentlyHitBricks = game.recentlyHitBricks;
            this.panel = panel;
            brickMap = game.levelBrickMap;
            bricksPicBoxes = new List<List<PictureBox>>();
            updatePicBoxes(game);
        }

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
                    brickPictureBox.BackgroundImage = (System.Drawing.Bitmap)obj;
                    brickLine.Add(brickPictureBox);
                }
                bricksPicBoxes.Add(brickLine);
            }
        }

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
