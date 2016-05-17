using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Basket__n_Ball
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteFont font;
        SpriteBatch spriteBatch;
        Texture2D imgPlayer1, imgPlayer2, imgBall, imgBasket, bgi;
        SoundEffect fail, success;
        Rectangle basketRec1, basketRec2, player1rec, player2rec, ballRec, intersectRec, intersectRec2, bgiRec;   // basic rectangles for each pictures
        //Rectangle player1SSwidth, player2SSwidth;               // I only use width since both sprite sheets have only one row
        int player1curCol=0, player2curCol=0;                       // Stores each player's current column numbers which is always 0
        int player1SSwidth, player2SSwidth, player1SSheight, player2SSheight;                       // Stores each player's current row numbers which vary.
        int basketX1, basketX2;                                            // Random number for basket position will be stored here.
        Random rndmBasket;                                      // Rondom number for position of the basket
        Random rndmAngle, rndmPow;    // Random number for splash screen
        int splash;
        int angleR1, powR1, angleR2, powR2;
        bool rdmPA1, rdmPA2;
        Song bgm;

        int power1, angle1, power2, angle2;                                       // stores power and angle (will be used when player throws)
        double angleR;
        int player1timer, keyboardDelay, player2timer;
        string gameState;
        float g = 9.8f;                                         // gravity of Earth
        float time, time2;                                             // time took for parabola

        bool player1Move = false;
        bool player2Move = false;
        bool ballVisible = false;                               // if this is false, you cannot see ball. starts from false because you can't see it from the beginning.
        bool basketRandom1 = false, basketRandom2 = false;                              // if this is true, random number for position of basket is generated.
        bool change = true;                                     // if this is true, player can change power1 and angle

        double ballRecX,ballRecY;

        KeyboardState oldKey;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            gameState = "title";
            power1 = 50;
            angle1 = 0;
            power2 = 50;
            angle2 = 0;
            splash = 1;
            player1rec = new Rectangle(30,GraphicsDevice.Viewport.Height-100, 70, 100);
            player2rec = new Rectangle(GraphicsDevice.Viewport.Width - 130, GraphicsDevice.Viewport.Height - 100, 130, 100);
            basketRec1 = new Rectangle(basketX1, GraphicsDevice.Viewport.Height - 50, 50, 50);
            basketRec2 = new Rectangle(basketX2, GraphicsDevice.Viewport.Height - 50, 50, 50);
            intersectRec = new Rectangle(basketX1, basketRec1.Y + basketRec1.Height / 2, basketRec1.Width, 1);
            intersectRec2 = new Rectangle(basketX2, basketRec2.Y + basketRec2.Height / 2, basketRec2.Width, 1);
            bgiRec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            rndmBasket =new Random();
            rndmAngle = new Random();
            rndmPow = new Random();
            //basketRandom1 = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            imgPlayer1 = Content.Load<Texture2D>("player1ss");
            imgPlayer2 = Content.Load<Texture2D>("player2ss");
            imgBall = Content.Load<Texture2D>("ball");
            imgBasket = Content.Load<Texture2D>("basket");
            bgi = Content.Load<Texture2D>("bgi");

            font = Content.Load<SpriteFont>("font");

            fail = Content.Load<SoundEffect>("fail");
            success = Content.Load<SoundEffect>("success");

            bgm = Content.Load<Song>("bgm");

            player1SSwidth = imgPlayer1.Width / 5;      // Get width of each section
            player2SSwidth = imgPlayer2.Width / 16;      // Get width of each section
            player1SSheight = imgPlayer1.Height;
            player2SSheight = imgPlayer2.Height;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keys=Keyboard.GetState();
            keyboardDelay += 1;             // This is for key detection. if it doesn't have this, number will go up so quickly so that players can't set to values they want.
            keyboardDelay = keyboardDelay % 6;
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed||keys.IsKeyDown(Keys.Escape))
                this.Exit();
            // Automatic spash screen movement
            if (gameState == "title")
            {
                if (keys.IsKeyDown(Keys.Space)||oldKey.IsKeyUp(Keys.Space))
                {
                    rdmPA1 = true;
                    gameState = "splash";
                }
            }
            if (gameState == "splash")
            {
                if (splash == 1)    // function for player 1 auto play
                {
                    if (player1curCol != 4 /*&& player1timer == 2*/)        // if focus is not on the last picture, 5th picture, then move right by one
                        player1curCol++;
                    else if (player1curCol == 4 /*&& player1timer == 2*/)    // if the last picture is shown, go back to the first picture // player1timer makes interval between pictures.
                    {
                        player1curCol = 0;
                    }
                    if (player1curCol == 3)
                        ballVisible = true;
                    if (ballVisible)
                    {
                        time += 0.1f;
                        angleR = Math.PI * angleR1 / 180.0;                                                         // Convert degrees to radians so that the number can be used when computer calculates sine and cosine.
                        // ball  movement

                        // Generate X coordinate
                        ballRecX = powR1 * Math.Cos(angleR) * time + 41;                                          // x coordinate of ball
                        // added 41 to adjust atarting point of Y coordinate
                        // Generate Y coordinate
                        ballRecY = -powR1 * Math.Sin(angleR) * time + Math.Pow(time, 2) * g / 2 + 390;              // y coordinate of ball (subtract gravity) I multiplied by minus because otherwise, it will draw U-like shape.
                        // added 390 to adjust starting point of Y coordinate

                        // Check if the ball got into basket
                        if (ballRec.Intersects(intersectRec))
                        {
                            
                            ballVisible = false;
                            success.Play();         // play sound effect
                        }
                        if (ballRecX > GraphicsDevice.Viewport.Width || ballRecY > GraphicsDevice.Viewport.Height)
                        {
                            ballVisible = false;    // make it false so that 
                            basketRandom2 = true;
                            rdmPA2 = true;
                            time = 0;           // if this isn't here, when turn comes back to player 1, the ball will not start from its original place.
                            splash = 2;  // Change turn
                            fail.Play();
                            ballRecX = 10;          // Need to be set as numbers that is not on/out of the edge of the screen. if not, soundeffect etc won't work properly
                            ballRecY = 10;
                        }
                    }
                }
                else if (splash == 2)       // function for player 2 auto play
                {
                    if (player2curCol != 15 /*&& player2timer == 1*/)        // if focus is not on the last picture, 16th picture, then move right by one
                        player2curCol++;
                    else if (player2curCol == 15 /*&& player2timer == 1*/)    // if the last picture is shown, go back to the first picture
                    {
                        player2curCol = 0;
                    }
                    if (player2curCol == 12)
                        ballVisible = true;
                    if (ballVisible)    // Work only when ballVIsible is set true
                    {
                        time2 += 0.1f;
                        angleR = Math.PI * angle2 / 180.0;                                                         // Convert degrees to radians so that the number can be used when computer calculates sine and cosine.
                        // ball  movement

                        // Generate X coordinate
                        ballRecX = -1 * (powR2 * Math.Cos(angleR) * time2) + GraphicsDevice.Viewport.Width - 70;                                          // x coordinate of ball
                        // added length of display minus 70 to adjust atarting point of Y coordinate
                        // Generate Y coordinate
                        ballRecY = -powR2 * Math.Sin(angleR) * time2 + Math.Pow(time2, 2) * g / 2 + 395;              // y coordinate of ball (subtract gravity) I multiplied minus 1 because otherwise, it will draw U-like shape.
                        // added 395 to adjust starting point of Y coordinate

                        // Check if the ball got into basket
                        if (ballRec.Intersects(intersectRec2))
                        {
                            ballVisible = false;
                            success.Play();
                        }
                        // If ball hits edge of screen, set ballvisible to false so that it won't move anymore.
                        if (ballRecX < 0 || ballRecY > GraphicsDevice.Viewport.Height)
                        {
                            ballVisible = false;
                            basketRandom1 = true;
                            rdmPA1 = true;
                            time2 = 0;          // if this isn't here, when turn comes back to player 2, the ball will not start from its original place.
                            splash = 2;
                            fail.Play();
                            ballRecX = 10;      // Need to be set as numbers that is not on/out of the edge of the screen. if not, soundeffect etc won't work properly
                            ballRecY = 10;
                            change = true;
                        }
                    }
                }
                if (keys.IsKeyDown(Keys.C))
                {
                    gameState = "intro";
                    time = 0;
                    time2 = 0;
                }
            }

            if (gameState == "intro" && keys.IsKeyDown(Keys.Enter) && oldKey.IsKeyUp(Keys.Enter))
            {
                gameState = "player1";
                basketRandom1 = true;
                if (MediaPlayer.State != MediaState.Playing)
                {
                    MediaPlayer.Play(bgm);                           //if bgm is not playing, then play bgm.
                    MediaPlayer.Volume = 0.3f;
                    MediaPlayer.IsRepeating = true;                 // repeat bgm until the player wins or quit the game
                }
            }
            if (gameState == "player1")
            {
                player1timer += 1;      // start a timer for changes of spritesheet (it works without this, but the character will move so fast)
                player1timer = player1timer % 7;
                // <player 1 function>
                if(keys.IsKeyDown(Keys.R)&&change)
                {
                    angle1 = 0;
                    power1 = 50;
                }
                if (keys.IsKeyDown(Keys.Up) && angle1 != 90 && keyboardDelay == 0 && change)
                    angle1 += 1;
                if (keys.IsKeyDown(Keys.Down) && angle1 != 0 && keyboardDelay == 0 && change)
                    angle1 -= 1;
                if (keys.IsKeyDown(Keys.Right) && keyboardDelay == 0 && change)
                    power1 += 1;
                if (keys.IsKeyDown(Keys.Left) && power1 != 0 && keyboardDelay == 0 && change)
                    power1 -= 1;

                if (keys.IsKeyDown(Keys.Space) && oldKey.IsKeyUp(Keys.Space))
                {
                    if (player1Move == false)
                        player1Move = true;
                    change = false;     // if I didn't set this variable, players are able to change power and angle even after ball is launched.
                }
                if (player1Move)
                {
                    if (player1curCol != 4 && player1timer == 2)        // if focus is not on the last picture, 5th picture, then move right by one
                        player1curCol++;
                    else if (player1curCol == 4 && player1timer == 2)    // if the last picture is shown, go back to the first picture // player1timer makes interval between pictures.
                    {
                        player1curCol = 0;
                        player1Move = false;
                    }
                }
                // </player 1 funciton>
                if (player1curCol == 3)
                {
                    ballVisible = true;
                }
                if (ballVisible)
                {
                    time += 0.1f;
                    angleR = Math.PI * angle1 / 180.0;                                                         // Convert degrees to radians so that the number can be used when computer calculates sine and cosine.
                    // ball  movement

                    // Generate X coordinate
                    ballRecX = power1 * Math.Cos(angleR) * time + 41;                                          // x coordinate of ball
                                    // added 41 to adjust atarting point of Y coordinate
                    // Generate Y coordinate
                    ballRecY = -power1 * Math.Sin(angleR) * time + Math.Pow(time, 2) * g / 2 + 390;              // y coordinate of ball (subtract gravity) I multiplied by minus because otherwise, it will draw U-like shape.
                                    // added 390 to adjust starting point of Y coordinate

                    // Check if the ball got into basket
                    if (ballRec.Intersects(intersectRec))
                    {
                        gameState = "win1";
                        ballVisible = false;    // stop showing ball
                        MediaPlayer.Pause();
                        success.Play();         // play sound effect
                    }
                    if (ballRecX > GraphicsDevice.Viewport.Width || ballRecY > GraphicsDevice.Viewport.Height)
                    {
                        ballVisible = false;    // make it false so that 
                        basketRandom2 = true;
                        time = 0;           // if this isn't here, when turn comes back to player 1, the ball will not start from its original place.
                        gameState = "player2";  // Change turn
                        fail.Play();
                        ballRecX = 10;          // Need to be set as numbers that is not on/out of the edge of the screen. if not, soundeffect etc won't work properly
                        ballRecY = 10;
                        change = true;
                    }
                }
            }

                // <player2 function>
            else if (gameState == "player2")
            {
                player2timer += 1;      // start a timer for changes of spritesheet (it works without this, but the character will move so fast)
                player2timer = player2timer % 5;
                if (keys.IsKeyDown(Keys.R) && change)
                {
                    angle2 = 0;
                    power2 = 50;
                }
                if (keys.IsKeyDown(Keys.Up) && angle2 != 90 && keyboardDelay == 0 && change)
                    angle2 += 1;
                if (keys.IsKeyDown(Keys.Down) && angle2 != 0 && keyboardDelay == 0 && change)
                    angle2 -= 1;
                if (keys.IsKeyDown(Keys.Right) && keyboardDelay == 0 && change)
                    power2 += 1;
                if (keys.IsKeyDown(Keys.Left) && power2 != 0 && keyboardDelay == 0 && change)
                    power2 -= 1;
                if (keys.IsKeyDown(Keys.Space) && oldKey.IsKeyUp(Keys.Space))
                {
                    if (player2Move == false)
                        player2Move = true;
                    change = false;     // if I didn't set this variable, players are able to change power and angle even after ball is launched.
                }
                if (player2Move)
                {
                    if (player2curCol != 15 && player2timer == 1)        // if focus is not on the last picture, 16th picture, then move right by one
                        player2curCol++;
                    else if (player2curCol == 15 && player2timer == 1)    // if the last picture is shown, go back to the first picture
                    {
                        player2curCol = 0;
                        player2Move = false;
                    }
                }
                // </player2 function>
                if (player2curCol == 12)
                    ballVisible = true;
                if (ballVisible)    // Work only when ballVIsible is set true
                {
                    time2 += 0.1f;
                    angleR = Math.PI * angle2 / 180.0;                                                         // Convert degrees to radians so that the number can be used when computer calculates sine and cosine.
                    // ball  movement

                    // Generate X coordinate
                    ballRecX = -1*(power2 * Math.Cos(angleR) * time2) + GraphicsDevice.Viewport.Width-70;                                          // x coordinate of ball
                    // added length of display minus 70 to adjust atarting point of Y coordinate
                    // Generate Y coordinate
                    ballRecY = -power2 * Math.Sin(angleR) * time2 + Math.Pow(time2, 2) * g / 2 + 395;              // y coordinate of ball (subtract gravity) I multiplied minus 1 because otherwise, it will draw U-like shape.
                    // added 395 to adjust starting point of Y coordinate

                    // Check if the ball got into basket
                    if (ballRec.Intersects(intersectRec2))
                    {
                        gameState = "win2";
                        ballVisible = false;
                        MediaPlayer.Pause();
                        success.Play();
                    }
                    // If ball hits edge of screen, set ballvisible to false so that it won't move anymore.
                    if (ballRecX < 0 || ballRecY > GraphicsDevice.Viewport.Height)
                    {
                        ballVisible = false;
                        basketRandom1 = true;
                        time2 = 0;          // if this isn't here, when turn comes back to player 2, the ball will not start from its original place.
                        gameState = "player1";
                        fail.Play();
                        ballRecX = 10;      // Need to be set as numbers that is not on/out of the edge of the screen. if not, soundeffect etc won't work properly
                        ballRecY = 10;
                        change = true;
                    }
                }
            }
                


                if (ballRec.X > GraphicsDevice.Viewport.Width || ballRec.Y > GraphicsDevice.Viewport.Height)
                    ballVisible = false;
                // Random number for position of basket
                if (basketRandom1)
                {
                    basketX1 = rndmBasket.Next(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Width - basketRec1.Width);  // Generates random number between middle of screen and right edge of screen
                    basketRandom1 = false;   // stop generating random number
                    basketRec1.X = basketX1;
                    intersectRec.X = basketX1;
                }
                if (basketRandom2)
                {
                    basketX2 = rndmBasket.Next(0, GraphicsDevice.Viewport.Width / 2);  // Generates random number between middle of screen and left edge of screen
                    basketRandom2 = false;   // stop generating random number
                    basketRec2.X = basketX2;
                    intersectRec2.X = basketX2;
                }
                
                // randome numbers for splash screen
                if (rdmPA1)
                {
                    angleR1 = rndmAngle.Next(0, 90);
                    powR1 = rndmPow.Next(50, 200);
                    rdmPA1 = false;
                }
                if (rdmPA2)
                {
                    angleR2 = rndmAngle.Next(0, 90);
                    powR2 = rndmPow.Next(50, 200);
                    rdmPA2 = false;
                }
            oldKey = keys;
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        void DrawSpritePlayer1(Rectangle windowLocation,
                            int rowNum,
                            int colNum)
        {
            Rectangle player1SSrect = new Rectangle(colNum * player1SSwidth, 0, player1SSwidth, player1SSheight);
            spriteBatch.Draw(imgPlayer1, windowLocation, player1SSrect, Color.White);
        }
        void DrawSpritePlayer2(Rectangle windowLocation2,
                                    int colNum2)
        {
            Rectangle player2SSrect = new Rectangle(colNum2 * player2SSwidth, 0, player2SSwidth, player2SSheight);
            spriteBatch.Draw(imgPlayer2, windowLocation2, player2SSrect, Color.White);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            // variables for draw
            Vector2 powerVector = new Vector2(100, 350);
            Vector2 angleVector=new Vector2(400,350);
            Vector2 introVector=new Vector2(100,150);
            Vector2 winVector =new Vector2(GraphicsDevice.Viewport.Width/2-100,GraphicsDevice.Viewport.Height/2);
            Vector2 playerVector = new Vector2(GraphicsDevice.Viewport.Width / 2 - 100, 100);
            string introTxt="Use Up or Down arrow key to change angle\nUse Right or Left arrow key to change power\nPress Space to shoot\nHit R to reset values\n\n                          Press Enter to start";
            string winTxt1 = "Player 1's win!";
            string winTxt2 = "Player 2's win!";
            ballRec = new Rectangle((int)ballRecX, (int)ballRecY, 10, 10);
            string angleText = "Angle: " + angle1.ToString();
            string powerText = "Power: " + power1.ToString();
            string angleText2 = "Angle: " + angle2.ToString();
            string powerText2 = "Power: " + power2.ToString();
            spriteBatch.Begin();

            if (gameState == "title")
            {
                spriteBatch.DrawString(font, "Basket 'n Ball", playerVector, Color.Red);
            }
            if (gameState=="intro")
            {
                spriteBatch.DrawString(font, introTxt, introVector, Color.Black);
            }
            if (gameState == "player1")
            {
                spriteBatch.Draw(bgi, bgiRec, Color.White);
                spriteBatch.Draw(imgBasket, basketRec1, Color.White);
                spriteBatch.DrawString(font, "Player 1", playerVector, Color.Black);
                if (ballVisible)            // Only draw ball when boolean ballVisible is true
                {
                    spriteBatch.Draw(imgBall, ballRec, Color.White);
                }
                spriteBatch.DrawString(font, powerText, angleVector, Color.Black);
                spriteBatch.DrawString(font, angleText, powerVector, Color.Black);
                DrawSpritePlayer1(player1rec, 0, player1curCol);
            }
            if (gameState == "player2")
            {
                spriteBatch.Draw(bgi, bgiRec, Color.White);
                spriteBatch.Draw(imgBasket, basketRec2, Color.White);
                spriteBatch.DrawString(font, "Player 2", playerVector, Color.Black);
                if (ballVisible)    // Only draw ball when boolean ballVisible is true
                {
                    spriteBatch.Draw(imgBall, ballRec, Color.White);
                }
                spriteBatch.DrawString(font, angleText2, angleVector, Color.Black);
                spriteBatch.DrawString(font, powerText2, powerVector, Color.Black);
                DrawSpritePlayer2(player2rec, player2curCol);
            }
            if (gameState == "win1")
            {
                spriteBatch.DrawString(font, winTxt1, winVector, Color.Black);
            }
            if (gameState == "win2")
            {
                spriteBatch.DrawString(font, winTxt2, winVector, Color.Black);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
