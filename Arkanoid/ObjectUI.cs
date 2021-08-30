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
        Ball ball;

        public BallUI(Panel panel, Ball ball, Paddle paddle)
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
                Size = new Size(paddle.Width, paddle.Height),
                Location = new Point(paddle.x - paddle.Width / 2, paddle.y - paddle.Height/2),
                Image = Properties.Resources.paddle,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Parent = panel
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

        public void updatePosition()
        {

        }
    }

    public class BrickMapUI
    {
        internal List<List<Brick>> brickMap;
        internal List<List<PictureBox>> bricksPicBoxes;
        private ResourceManager resourceManager;
        Coords recentlyHitBrick;
        Panel panel;

        public BrickMapUI(Panel panel, ResourceManager resourceManager, Game game)
        {
            this.resourceManager = resourceManager;
            this.recentlyHitBrick = game.recentlyHitBrick;
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
                    PictureBox brickPictureBox = new PictureBox
                    {
                        Parent = panel,
                        Size = new Size(game.brickWidth, game.brickHeight),
                        Location = new Point(column * game.brickWidth, panel.Location.Y + line * game.brickHeight),
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
            if (recentlyHitBrick == null || recentlyHitBrick.row == -1 || recentlyHitBrick.column == -1) return;
            else
            {
                PictureBox recentBrick = bricksPicBoxes[recentlyHitBrick.row][recentlyHitBrick.column];
                
                Brick brick = brickMap[recentlyHitBrick.row][recentlyHitBrick.column];
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
