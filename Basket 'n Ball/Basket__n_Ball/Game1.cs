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
        SpriteBatch spriteBatch;
        Texture2D imgPlayer1, imgPlayer2, imgBall, imgBasket;
        SoundEffect fail, success;
        Rectangle basketRec, player1rec, player2rec, ballRec;   // basic rectangles for each pictures
        //Rectangle player1SSwidth, player2SSwidth;               // I only use width since both sprite sheets have only one row
        int player1curCol=0, player2curCol=0;                       // Stores each player's current column numbers which is always 0
        int player1SSwidth, player2SSwidth, player1SSheight, player2SSheight;                       // Stores each player's current row numbers which vary.
        Random rndmBasket;                                      // Rondom number for position of the basket
        Song bgm;

        int power, angle;                                       // stores power and angle (will be used when player throws)
        int player1timer;
        string gameState;

        bool player1Move = false;
        bool ballVisible = false;                               // if this is false, you cannot see ball. starts from false because you can't see it from the beginning.

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
            gameState = "player1";
            power = 50;
            angle = 0;
            player1rec = new Rectangle(30,GraphicsDevice.Viewport.Height-100, 70, 100);
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
            player1timer += 1;
            player1timer=player1timer%7;
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed||keys.IsKeyDown(Keys.Escape))
                this.Exit();


            // <player 1 function>
            if (keys.IsKeyDown(Keys.Up))
                angle += 1;
            if(keys.IsKeyDown(Keys.Down))
                angle -= 1;
            if(keys.IsKeyDown(Keys.Right))
                power+=1;
            if(keys.IsKeyDown(Keys.Left))
                power-=1;

            if (keys.IsKeyDown(Keys.Space) && oldKey.IsKeyUp(Keys.Space))
            {
                if(player1Move==false)
                    player1Move = true;
            }
            if (player1Move)
            {
                if (player1curCol != 4 && player1timer==2)        // if focus is not on the last picture, 5th picture, then move right by one
                    player1curCol++;
                else if (player1curCol == 4 && player1timer==2)    // if the last picture is shown, go back to the first picture
                {
                    player1curCol = 0;
                    player1Move = false;
                }
            }
            // </player 1 funciton>
            oldKey = keys;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        void DrawSprite(Rectangle windowLocation,
                            int rowNum,
                            int colNum)
        {
            Rectangle player1SSrect = new Rectangle(colNum * player1SSwidth, 0, player1SSwidth, player1SSheight);
            spriteBatch.Draw(imgPlayer1, windowLocation, player1SSrect, Color.White);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Vector2 powerVector = new Vector2(0, 0);
            spriteBatch.Begin();
            DrawSprite(player1rec, 0, player1curCol);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
