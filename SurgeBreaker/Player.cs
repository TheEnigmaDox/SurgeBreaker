using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SurgeBreaker
{
    internal class Player
    {
        public int _playerLives = 3;

        bool _autoPlay = false;

        public float _xMoveSpeed;

        public Vector2 _batPos;

        Texture2D _batTexture;
        Texture2D _debugPixel;

        public Rectangle _batSource;
        public Rectangle _colRect;

        public Player(Texture2D batTexture, Texture2D debugPixel) 
        {
            _batTexture = batTexture;
            _debugPixel = debugPixel;

            _batPos = new Vector2(Global.windowSize.X / 2 - 84, Global.windowSize.Y - 125);

            _batSource = new Rectangle(0, 64, 168, 24);

            _colRect = new Rectangle((int)_batPos.X, (int)_batPos.Y, _batSource.Width, _batSource.Height);

            _xMoveSpeed = 5f;
        }

        public void UpdateBat(KeyboardState keyboardState, Ball ball)
        {
            UpdateBatCollision();

            //Clamp bat position to screen width
            _batPos.X = MathHelper.Clamp(_batPos.X, 0, Global.windowSize.X - _batSource.Width);

            if (keyboardState.IsKeyDown(Keys.S))
            {
                _autoPlay = !_autoPlay;
            }

            //If the title screen is running.
            if (Global.gameState == GameState.Title)
            {
                //Bat autoplays.
                _batPos = new Vector2(ball._ballPos.X - _batSource.Width / 2, _batPos.Y);
            }
            //otherwise if the gamestate is equal to one of the level states.
            else if (Global.gameState == GameState.Level01 || 
                Global.gameState == GameState.Level02 || 
                Global.gameState == GameState.Level03)
            {
                //Reset bat position
                _batPos = new Vector2(_batPos.X , Global.windowSize.Y - 125);

                //If the A key is pressed
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    //Minus the movespeed from bat position
                    _batPos.X -= _xMoveSpeed;
                }
                //Otherwise if D key is pressed.
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    //Add the movespeed to the bat position
                    _batPos.X += _xMoveSpeed;
                } 

                //is autoplay is true
                if(_autoPlay)
                {
                    //Match bats x position to the balls x position.
                    _batPos = new Vector2(ball._ballPos.X - _batSource.Width / 2, _batPos.Y);
                }
            }
        }

        public void DrawBat() 
        {
            Global.spriteBatch.Draw(_batTexture,
                _batPos,
                _batSource,
                Color.White);

            //Global.spriteBatch.Draw(_debugPixel,
            //    _colRect,
            //    Color.Red);
        }

        void UpdateBatCollision()
        {
            //Updates the collision rectangle around the player.
            _colRect = new Rectangle((int)_batPos.X, (int)_batPos.Y, _batSource.Width, _batSource.Height);
        }
    }
}
