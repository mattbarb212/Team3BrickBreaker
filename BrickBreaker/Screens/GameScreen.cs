﻿/*  Created by: Team 3 - Taiyo, Charlie, Manny, Miguel, Matthew, Isaac
 *  Project: Brick Breaker
 *  Date: 
 */ 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace BrickBreaker
{
    public partial class GameScreen : UserControl
    {
        #region global values

        //player1 button control keys - DO NOT CHANGE
        Boolean leftArrowDown, rightArrowDown;

        // Game values
        int lives;
        int score;

        // Paddle and Ball objects
        Paddle paddle;
        Ball ball;

        // list of all blocks for current level
        List<Block> blocks = new List<Block>();
        string[] powerUps = new string[5];

        // Brushes
        SolidBrush paddleBrush = new SolidBrush(Color.White);
        SolidBrush ballBrush = new SolidBrush(Color.White);
        SolidBrush textBrush = new SolidBrush(Color.White);
        SolidBrush blockBrush = new SolidBrush(Color.Red);

        //font for text
        Font textFont = new Font("Arial", 16);

        public static Random randGen = new Random();
        public static string powerImage;
        #endregion

        public GameScreen()
        {
            InitializeComponent();
            OnStart();
        }


        public void OnStart()
        {
            string[] powerUps = { "BrickBreaker.Properties.Resources.Fire_flower", "BrickBreaker.Properties.Resources.Super_Star", "BrickBreaker.Properties.Resources.Double_Cherry", "BrickBreaker.Properties.Resources.Super_Mushroom", "BrickBreaker.Properties.Resources.Mini_Mushroom" };
            //set life counter
            lives = 3;

            //set all button presses to false.
            leftArrowDown = rightArrowDown = false;

            // setup starting paddle values and create paddle object
            int paddleWidth = 80;
            int paddleHeight = 20;
            int paddleX = ((this.Width / 2) - (paddleWidth / 2));
            int paddleY = (this.Height - paddleHeight) - 60;
            int paddleSpeed = 8;
            paddle = new Paddle(paddleX, paddleY, paddleWidth, paddleHeight, paddleSpeed, Color.White);

            // setup starting ball values
            int ballX = this.Width / 2 - 10;
            int ballY = this.Height - paddle.height - 80;

            // Creates a new ball
            int xSpeed = 6;
            int ySpeed = 6;
            int ballSize = 20;
            ball = new Ball(ballX, ballY, xSpeed, ySpeed, ballSize);

            #region Creates blocks for generic level. Need to replace with code that loads levels.
            
            //TODO - replace all the code in this region eventually with code that loads levels from xml files
            
            blocks.Clear();
            int x = 10;

            while (blocks.Count < 12)
            {
                x += 57;
                Block b1 = new Block(x, 10, 1, Color.White, 1);
                blocks.Add(b1);
            }

            #endregion

            // start the game engine loop
            gameTimer.Enabled = true;
        }

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //player 1 button presses
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = true;
                    break;
                case Keys.Right:
                    rightArrowDown = true;
                    break;
                default:
                    break;
            }
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            //player 1 button releases
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = false;
                    break;
                case Keys.Right:
                    rightArrowDown = false;
                    break;
                default:
                    break;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            // Move the paddle
            if (leftArrowDown && paddle.x > 0)
            {
                paddle.Move("left");
            }
            if (rightArrowDown && paddle.x < (this.Width - paddle.width))
            {
                paddle.Move("right");
            }

            // Move ball
            ball.Move();

            // Check for collision with top and side walls
            ball.WallCollision(this);

            // Check for ball hitting bottom of screen
            if (ball.BottomCollision(this))
            {
                lives--;

                // Moves the ball back to origin
                ball.x = ((paddle.x - (ball.size / 2)) + (paddle.width / 2));
                ball.y = (this.Height - paddle.height) - 85;

                if (lives == 0)
                {
                    gameTimer.Enabled = false;
                    OnEnd();
                }
            }

            // Check for collision of ball with paddle, (incl. paddle movement)
            ball.PaddleCollision(paddle);

            // Check if ball has collided with any blocks
            foreach (Block b in blocks)
            {
                if (ball.BlockCollision(b))
                {
                    blocks.Remove(b);

                    if (blocks.Count == 0)
                    {
                        gameTimer.Enabled = false;
                        OnEnd();
                    }

                    break;
                }
            }

            //redraw the screen
            Refresh();
        }

        public void OnEnd()
        {
            // Goes to the game over screen
            Form form = this.FindForm();
            GameOverScreen gos = new GameOverScreen();

            gos.Location = new Point((form.Width - gos.Width) / 2, (form.Height - gos.Height) / 2);

            form.Controls.Add(gos);
            form.Controls.Remove(this);
        }

        public void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            // Draws paddle
            paddleBrush.Color = paddle.colour;
            e.Graphics.FillRectangle(paddleBrush, paddle.x, paddle.y, paddle.width, paddle.height);

            // Draws blocks
            foreach (Block b in blocks)
            {
                e.Graphics.FillRectangle(blockBrush, b.x, b.y, b.width, b.height);
                //e.Graphics.DrawImage(powerImage[0], 20, 20);
                //e.Graphics.DrawImage(BrickBreaker.Properties.Resources.Fire_flower, 20, 20);
                //e.Graphics.DrawImage(BrickBreaker.Properties.Resources.Super_Star, 20, 20);
                //e.Graphics.DrawImage(BrickBreaker.Properties.Resources.Double_Cherry, 20, 20);
                //e.Graphics.DrawImage(BrickBreaker.Properties.Resources.Super_Mushroom, 20, 20);
                //e.Graphics.DrawImage(BrickBreaker.Properties.Resources.Mini_Mushroom, 20, 20);
            }

            // Draws ball
            e.Graphics.FillRectangle(ballBrush, ball.x, ball.y, ball.size, ball.size);

            e.Graphics.DrawString($"Lives left: {lives}", textFont, textBrush, 370, 500);
        }
       

    }
}
