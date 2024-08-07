using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SurgeBreaker
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;

        bool spawnTitle = false;

        bool isEnterPressed = false;

        float colourTimer = 0f;
        float maxColourTimer = 1f;

        string gameOverBricks = "";
        string gameOverCharge = "";

        Texture2D textureSheet;
        Texture2D debugPixel;
        Texture2D buttonTexture;
        Texture2D chargeBackground;

        Texture2D windowBackground;
        Texture2D titleBackground;
        Texture2D gameOver;

        SpriteFont debugFont;
        SpriteFont titleFont;
        SpriteFont titleButtonFont;
        SpriteFont gameOverMainFont;
        SpriteFont gameOverSecFont;
        SpriteFont instructionFont;

        BrickManager brickManager;
        Player player;
        Ball ball;
        Charge chargeBar;

        Color instructionBrick;
        Color[] brickColours = new Color[4];

        MouseState mouseState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //Set the window size.
            _graphics.PreferredBackBufferWidth = Global.windowSize.X;
            _graphics.PreferredBackBufferHeight = Global.windowSize.Y;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Global.spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            textureSheet = Content.Load<Texture2D>("Textures/Bricks02");
            debugPixel = Content.Load<Texture2D>("Textures/Pixel");
            buttonTexture = Content.Load<Texture2D>("Textures/ButtonBackground");
            chargeBackground = Content.Load<Texture2D>("Textures/Bricks02");

            windowBackground = Content.Load<Texture2D>("Textures/WindowBackground");
            titleBackground = Content.Load<Texture2D>("Textures/TitleBackground");

            debugFont = Content.Load<SpriteFont>("Fonts/DebugFont");
            titleFont = Content.Load <SpriteFont>("Fonts/TitleFont");
            titleButtonFont = Content.Load <SpriteFont>("Fonts/TitleButtonFont");
            gameOverMainFont = Content.Load<SpriteFont>("Fonts/GameOverMain");
            gameOverSecFont = Content.Load<SpriteFont>("Fonts/GameOverSec");
            instructionFont = Content.Load<SpriteFont>("FOnts/InstructionsFont");

            Global.energyFX = Content.Load<SoundEffect>("Sounds/EnergyWAV");
            Global.beepFX = Content.Load<SoundEffect>("Sounds/Retro4");

            brickManager = new BrickManager(textureSheet);

            player = new Player(textureSheet, debugPixel);

            ball = new Ball(textureSheet, player);

            chargeBar = new Charge(chargeBackground, debugPixel);

            //Populate the brick colours array.
            brickColours[0] = Color.Red;
            brickColours[1] = Color.Blue;
            brickColours[2] = Color.Green;
            brickColours[3] = Color.Purple;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            // TODO: Add your update logic here

            Global.Update(gameTime);

            //Switch to check which gamestate is running and call the appropriate update method.
            switch (Global.gameState)
            {
                case GameState.Title:
                    UpdateTitleMenu(keyboardState);
                    if(spawnTitle == false)
                    {
                        brickManager.SpawnLevel(0);
                        spawnTitle = true;
                    }
                    break;
                case GameState.Level01:
                    UpdateLevel01(keyboardState);
                    break;
                case GameState.Level02:
                    UpdateLevel02(keyboardState);
                    break;
                case GameState.Level03:
                    UpdateLevel03(keyboardState);
                    break;
                case GameState.Instructions:
                    UpdateInstructions(gameTime, keyboardState);
                    break;
                case GameState.GameOver:
                    UpdateGameOver(keyboardState);
                    break;
            }

            gameOverBricks = Global.brokenBricks.ToString();
            gameOverCharge = Global.chargeGained.ToString();

            //Debug.WriteLine(brickManager._brickList.Count + " brick list count");
            //Debug.WriteLine(brickManager._breakableBricks + " breakable bricks");
            //Debug.WriteLine(Global.gameState + " GameState");

            base.Update(gameTime);
        }

        void UpdateTitleMenu(KeyboardState keyboardState)
        {
            brickManager.Update(ball, player, chargeBar);

            player.UpdateBat(keyboardState, ball);

            ball.UpdateBall(player, keyboardState);

            if (keyboardState.IsKeyDown(Keys.Enter) && isEnterPressed == false)
            {
                //Reset ball collision rectangle.
                ball._colRect = new Rectangle((int)player._batPos.X / 2 - 16,
                    (int)player._batPos.Y - 48,
                    ball._ballSource.Width,
                    ball._ballSource.Height);
                //Reset the ball to above the player bat.
                ball.ResetBall(player);
                //Set enter is pressed bool to true.
                isEnterPressed = true;
                //Clear the brick list ready for a new list of bricks.
                brickManager.ResetBrickList();
                //Reset the charge bar to half way.
                chargeBar._barWidth = 800;
                //Reset the breakable bricks variable to 0.
                brickManager._breakableBricks = 0;
                //change the game state to level 01
                Global.gameState = GameState.Level01;
                //Spawn level 01 bricks.
                brickManager.SpawnLevel(1);
            }
            else if(!keyboardState.IsKeyDown(Keys.Enter) && isEnterPressed == true)
            {
                isEnterPressed = false;
            }

            if (keyboardState.IsKeyDown(Keys.I))
            {
                Global.gameState = GameState.Instructions;
            }
        }

        void UpdateLevel01(KeyboardState keyboardState)
        {
            brickManager.Update(ball, player, chargeBar);

            player.UpdateBat(keyboardState, ball);

            ball.UpdateBall(player, keyboardState);

            chargeBar.UpdateCharge();

            //Is breakable bricks are equal to 0, load the next level.
            if (brickManager._breakableBricks <= 0)
            {
                ball.ResetBall(player);
                brickManager._brickList.Clear();
                chargeBar._barWidth = 800;
                Global.gameState = GameState.Level02;
                brickManager.SpawnLevel(2);
            }

            //If charge bar is less than or equal to 0 or player loses 3 lifes, load game over.
            if (chargeBar._barWidth <= 0 || player._playerLives <= 0)
            {
                Global.gameState = GameState.GameOver;
            }
        }

        void UpdateLevel02(KeyboardState keyboardState)
        {
            brickManager.Update(ball, player, chargeBar);

            player.UpdateBat(keyboardState, ball);

            ball.UpdateBall(player, keyboardState);

            chargeBar.UpdateCharge();

            
            if (brickManager._breakableBricks <= 0)
            {
                ball.ResetBall(player);
                brickManager._brickList.Clear();
                chargeBar._barWidth = 800;
                Global.gameState = GameState.Level03;
                brickManager.SpawnLevel(3);
            }
            
            
            if (chargeBar._barWidth <= 0 || player._playerLives <= 0)
            {
                Global.gameState = GameState.GameOver;
            }
        }

        void UpdateLevel03(KeyboardState keyboardState)
        {
            brickManager.Update(ball, player, chargeBar);

            player.UpdateBat(keyboardState, ball);

            ball.UpdateBall(player, keyboardState);

            chargeBar.UpdateCharge();

            if (brickManager._breakableBricks <= 0)
            {
                ball.ResetBall(player);
                brickManager._brickList.Clear();
                Global.gameState = GameState.GameOver;
            }
            
            if (chargeBar._barWidth <= 0 || player._playerLives <= 0)
            {
                Global.gameState = GameState.GameOver;
            }
        }
        void UpdateInstructions(GameTime gameTime, KeyboardState keyboardState)
        {
            //A two second timer to switch through the colours of the bricks.
            if(colourTimer <= 0)
            {
                instructionBrick = brickColours[Global.RNG.Next(0, brickColours.Length)];
                colourTimer = maxColourTimer;
            }
            else 
            { 
                colourTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (keyboardState.IsKeyDown(Keys.Back))
            {
                Global.gameState = GameState.Title;
            }
        }

        void UpdateGameOver(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Enter) && isEnterPressed == false)
            {
                isEnterPressed = true;
                Global.gameState = GameState.Title;
            }
            else if (!keyboardState.IsKeyDown(Keys.Enter) && isEnterPressed == true)
            {
                isEnterPressed = false;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            Global.spriteBatch.Begin();

            switch (Global.gameState)
            {
                case GameState.Title:
                    DrawTitleMenu();
                    break;
                case GameState.Instructions:
                    DrawInstructions();
                    break;
                case GameState.Level01:
                    DrawLevel01();
                    break;
                case GameState.Level02:
                    DrawLevel02();
                    break;
                case GameState.Level03:
                    DrawLevel03();
                    break;
                case GameState.GameOver:
                    DrawGameOver();
                    break;
            }

            Global.spriteBatch.End();

            base.Draw(gameTime);
        }

        void DrawTitleMenu()
        {
            Global.spriteBatch.Draw(windowBackground,
                Vector2.Zero,
                Color.White);

            Global.spriteBatch.Draw(titleBackground,
                new Vector2(100, 500),
                Color.White);

            Global.spriteBatch.Draw(buttonTexture,
                new Vector2(775, 560),
                Color.White);

            Global.spriteBatch.Draw(buttonTexture,
                new Vector2(775, 660),
                Color.White);

            Global.spriteBatch.DrawString(titleFont,
                "Surge\n    Breaker",
                new Vector2(125, 507),
                new Color(255, 255, 51));

            Global.spriteBatch.DrawString(titleButtonFont,
                "Press Enter to Start Game!",
                new Vector2(800, 570),
                new Color(255, 255, 51));

            Global.spriteBatch.DrawString(titleButtonFont,
                "Press I For Instructions!",
                new Vector2(830, 670),
                new Color(255, 255, 51));

            //Global.spriteBatch.Draw(testWindow,
            //    Vector2.Zero,
            //    Color.White);

            brickManager.DrawBricks(debugPixel);

            player.DrawBat();

            ball.DrawBall();
        }

        void DrawInstructions()
        {
            Global.spriteBatch.Draw(windowBackground,
                new Vector2(0, 0),
                Color.White);

            Global.spriteBatch.Draw(titleBackground,
                new Vector2(Global.windowSize.X / 2 - titleBackground.Width / 2, 100),
                Color.White);

            Global.spriteBatch.DrawString(instructionFont,
                "Use A / D to move the fuse (bat)",
                new Vector2(Global.windowSize.X / 2 - instructionFont.MeasureString("Use A / D to move the fuse/bat").X / 2, 120),
                new Color(255, 255, 51));

            Global.spriteBatch.DrawString(instructionFont,
                "Break bricks to fill your charge meter\n        at the bottom of the screen",
                new Vector2(Global.windowSize.X / 2 - instructionFont.MeasureString("Break bricks to fill your charge meter\n        at the bottom of the screen").X / 2, 175),
                new Color(255, 255, 51));

            Global.spriteBatch.DrawString(instructionFont,
                "If the charge meters reaches 0,\n             game over",
                new Vector2(Global.windowSize.X / 2 - instructionFont.MeasureString("If the charge meters reaches 0,\n             game over").X / 2, 260),
                new Color(255, 255, 51));

            Global.spriteBatch.Draw(textureSheet,
                new Vector2(Global.windowSize.X / 8 - 64, Global.windowSize.Y / 2 - 32),
                new Rectangle(0, 0, 128, 64),
                instructionBrick);

            Global.spriteBatch.DrawString(instructionFont,
                "Normal bricks can be broken.",
                new Vector2(Global.windowSize.X / 8 + 128, Global.windowSize.Y / 2 - 16),
                new Color(255, 255, 51));

            Global.spriteBatch.Draw(textureSheet,
                new Vector2(Global.windowSize.X / 8 - 64, Global.windowSize.Y / 2 + 48),
                new Rectangle(0, 0, 128, 64),
                Color.LightSkyBlue);

            Global.spriteBatch.DrawString(instructionFont,
                "Glass bricks can be broken. Will not affect the balls direction.",
                new Vector2(Global.windowSize.X / 8 + 128, Global.windowSize.Y / 2 + 64),
                new Color(255, 255, 51));

            Global.spriteBatch.Draw(textureSheet,
                new Vector2(Global.windowSize.X / 8 - 64, Global.windowSize.Y / 2 + 128),
                new Rectangle(0, 0, 128, 64),
                Color.DarkSlateGray);

            Global.spriteBatch.DrawString(instructionFont,
                "Static bricks cannot be broken.",
                new Vector2(Global.windowSize.X / 8 + 128, Global.windowSize.Y / 2 + 145),
                new Color(255, 255, 51));

            Global.spriteBatch.DrawString(titleButtonFont,
                "Press backspace to return to title.",
                new Vector2(Global.windowSize.X / 2 - titleButtonFont.MeasureString("Press backspace to return to title.").X / 2, Global.windowSize.Y - 100),
                new Color(255, 255, 51));
        }

        void DrawLevel01()
        {
            Global.spriteBatch.Draw(windowBackground,
                Vector2.Zero,
                Color.White);

            brickManager.DrawBricks(debugPixel);

            player.DrawBat();

            ball.DrawBall();

            chargeBar.DrawChargeBar();
        }

        void DrawLevel02()
        {
            Global.spriteBatch.Draw(windowBackground,
                Vector2.Zero,
                Color.White);

            brickManager.DrawBricks(debugPixel);

            player.DrawBat();

            ball.DrawBall();

            chargeBar.DrawChargeBar();
        }

        void DrawLevel03()
        {
            Global.spriteBatch.Draw(windowBackground,
                Vector2.Zero,
                Color.White);

            brickManager.DrawBricks(debugPixel);

            player.DrawBat();

            ball.DrawBall();

            chargeBar.DrawChargeBar();
        }

        void DrawGameOver()
        {
            Global.spriteBatch.Draw(windowBackground,
                Vector2.Zero,
                Color.White);

            Global.spriteBatch.Draw(titleBackground,
                new Vector2(Global.windowSize.X / 2 - titleBackground.Width / 2, 100),
                Color.White);

            Global.spriteBatch.DrawString(gameOverMainFont,
                "Game \n   Over!",
                new Vector2(Global.windowSize.X / 2 - titleBackground.Width / 3 + 15, 107),
                new Color(255, 255, 51));

            Global.spriteBatch.Draw(titleBackground,
                new Vector2(Global.windowSize.X / 2 - titleBackground.Width / 2, 459),
                Color.White);

            Global.spriteBatch.DrawString(gameOverSecFont,
                "Brick broken : " + gameOverBricks,
                new Vector2(Global.windowSize.X / 2 - gameOverSecFont.MeasureString("Brick broken : " + gameOverBricks).X / 2, 480),
                new Color(255, 255, 51));

            Global.spriteBatch.DrawString(gameOverSecFont,
                "Charge Gained : " + gameOverCharge,
                new Vector2(Global.windowSize.X / 2 - gameOverSecFont.MeasureString("Charge Gained : " + gameOverCharge).X / 2, 600),
                new Color(255, 255, 51));

            Global.spriteBatch.DrawString(gameOverSecFont,
                "Press Enter to return to title screen",
                new Vector2(Global.windowSize.X / 2 - gameOverSecFont.MeasureString("Press Enter to return to title screen").X / 2, 725),
                new Color(255, 255, 51));

            Global.spriteBatch.DrawString(gameOverSecFont,
                "Thanks for playing!",
                new Vector2(Global.windowSize.X / 2 - gameOverSecFont.MeasureString("Thanks for playing!").X / 2, 800),
                new Color(255, 255, 51));
        }
    }
}
