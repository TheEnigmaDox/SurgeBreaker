using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SurgeBreaker
{
    internal class Ball
    {
        //Integer variable to hold the number of lives.
        int _livesLeft = 3;

        //Boolean switch to check for the user starting the game.
        public bool _start = false;
        //Boolean switch to check for the ball leaving the main window.
        bool _ballLost = false;

        //Float variable to store the maximum speed.
        public float _maxSpeed = 10f;

        //A vector2 to store the ball position.
        public Vector2 _ballPos;
        //A vector2 to store the balls velocity
        public Vector2 _ballVelocity;
        //A vector2 to store the balls previous position.
        public Vector2 _oldPosition;

        //A texture to store the ball texture.
        Texture2D _ballTexture;

        //A rectangle to store the ball source rectangle.
        public Rectangle _ballSource;
        //A rectangle to store the balls collision rectangle.
        public Rectangle _colRect;
        //A reference to the player bat class.
        Player _bat;

        //The balls constructor
        public Ball(Texture2D textureSheet, Player bat) 
        {
            //Set the balls texture to the texture passed into the constructor.
            _ballTexture = textureSheet;

            //Set the reference to the player.
            _bat = bat;

            //Set the balls source and collision rectangles.
            _ballSource = new Rectangle(176, 64, 32, 32);
            _colRect = new Rectangle((int)_ballPos.X, (int)_ballPos.Y, _ballSource.Width, _ballSource.Height);

            //Store the balls position in the old position variable.
            _oldPosition = _ballPos;
            //Set the start position of the ball.
            _ballPos = new Vector2(_bat._batPos.X / 2 - 16, _bat._batPos.Y - 48);

            if(Global.gameState == GameState.Title)
            {
                _maxSpeed = 2f;
            }
        }   

        //Ball update method.
        public void UpdateBall(Player bat, KeyboardState keyboardState)
        {
            //Firstly set the old position of the ball. 
            _oldPosition = _ballPos;
            //Set up the max speed of the ball. 
            _maxSpeed = bat._xMoveSpeed + 1;

            //Funtion to clamp the position of the ball.
            ClampPosition();
            //Funtion to check if the player has started the game.
            CheckForGameStart(keyboardState, bat);

            //if the balls collider intersects the bats collider...
            if (_colRect.Intersects(bat._colRect))
            {
                //Create a rectangle to store the overlap rectangle created from collision.
                Rectangle overlap = Rectangle.Intersect(_colRect, bat._colRect);

                if(overlap.Width == overlap.Height)
                {
                    _ballVelocity *= -1f;
                }

                //If the overlap rectangle width is greater than its height...
                if(overlap.Width > overlap.Height)
                {
                    //Store the position of the ball hit in x
                    float x = HitFactor(_ballPos, bat, bat._batSource.Width);

                    if (x < 0.5f)
                    {
                        //Create a new vector with x (multiplied to add effect (not sure if this works))
                        Vector2 direction = new Vector2(x * 1.5f, -1);
                        //Normalise the vector.
                        direction.Normalize();
                        //Move the ball back to its old position.
                        BackToOldPos();
                        //Apply the new velocity multiplied by the speed.
                        _ballVelocity = direction * _maxSpeed;
                    }
                    else if (x > 0.5f)
                    {
                        Vector2 direction = new Vector2(-x * 1.5f, -1);
                        direction.Normalize();
                        //Move the ball back to its old position.
                        BackToOldPos();
                        _ballVelocity = direction * _maxSpeed;
                    }
                }
                else
                {
                    //Move the ball back to its old position.
                    BackToOldPos();
                    //Invert the balls velocity on the x-axis.
                    _ballVelocity.X *= -1f;
                }
            }

            //If the start boolean is true.
            if (_start)
            {
                //Add ball velocity to the ball position.
                _ballPos += _ballVelocity;
            }
            
            //If the balls y position is greater than the bottom of the screen.
            if (_ballPos.Y > Global.windowSize.Y)
            {
                //Remove a player life.
                bat._playerLives--;
                //Ball lost boolean switches to true;
                _ballLost = true;
                //Function to reset the game without reseting the level.
                ResetBall(bat);
            }

            //Function to update the balls collision.
            UpdateCollision();
        }

        //Function to draw the ball.
        public void DrawBall()
        {
            //If the ball lost boolean is true.
            if (_ballLost == false)
            {
                //Draw the ball.
                Global.spriteBatch.Draw(_ballTexture,
                        _ballPos,
                        _ballSource,
                        Color.White); 
            }
        }

        //Function to set the velocity of the ball.
        void SetBallVelocity(float _maxSpeed)
        {
            //Set the velocity of the ball to a new random number on each axis.
            _ballVelocity = new Vector2(Global.RNG.Next(-1, 2), -1);
            //Normalize the ball velocity
            _ballVelocity.Normalize();
            //Multiply the velocity by the max speed.
            _ballVelocity.X *= _maxSpeed;

            //If the ball velocity is less than 0.5 and greater than -0.5,
            //ball is travelling extremely vertical and needs re-adjusted.
            if(_ballVelocity.X < 0.5f && _ballVelocity.X > -0.5f)
            {
                //Reset the ball velocity on the x-axis.
                _ballVelocity.X = Global.RNG.Next(-3, 4);
            }
        }

        //Function to update the ball collision rectangle.
        void UpdateCollision()
        {
            //Updates the collision rectangle to move along with the ball.
            _colRect = new Rectangle((int)_ballPos.X, (int)_ballPos.Y, _ballSource.Width, _ballSource.Height);
        }

        //Function to move the ball back to its old position.
        public void BackToOldPos()
        {
            //Move the ball back to its old position.
            _ballPos = _oldPosition;
            //Update ball collision.
            UpdateCollision();
        }

        //Function to clamp ball position
        void ClampPosition()
        {
            //If ball x position is less than 0 OR
            //ball x position plus its texture width is greater than the x size of the window
            if (_ballPos.X < 0 || _ballPos.X > Global.windowSize.X - _ballSource.Width)
            {
                //Invert the ball velocity on the x axis.
                _ballVelocity.X *= -1f;
                //Function to move the ball back to its old position.
                BackToOldPos();
            }

            //If the ball y position is less than 0.
            if (_ballPos.Y < 0)
            {
                //Invert the ball velocity on the y axis.
                _ballVelocity.Y *= -1f;
                //Function to move the ball back its old position.
                BackToOldPos();
            }
        }

        //Funtion to check if the player has started the game.
        void CheckForGameStart(KeyboardState keyboardState, Player bat)
        {
            //If the player has not started the game.
            if (_start == false)
            {
                //Set the ball position to just above the centre of the bat.
                _ballPos = new Vector2(bat._batPos.X + bat._batSource.Width / 2 - _ballSource.Width / 2, bat._batPos.Y - 48);
            }

            //If the game state is equal to the title state and the start boolean is false;
            if (Global.gameState == GameState.Title && _start == false)
            {
                //Function to set the ball velocity.
                SetBallVelocity(_maxSpeed);
                //Set the game start bool to true.
                _start = true;
            }
            //If the player has pressed the space bar to start game.
            else if (Global.gameState == GameState.Level01 && _start == false && keyboardState.IsKeyDown(Keys.Space))
            {
                //Function to set the ball velocity.
                SetBallVelocity(_maxSpeed);
                //Set the game start bool to true.
                _start = true;
            }
            else if(Global.gameState == GameState.Level02 && _start == false && keyboardState.IsKeyDown(Keys.Space))
            {
                //Function to set the ball velocity.
                SetBallVelocity(_maxSpeed);
                //Set the game start bool to true.
                _start = true;
            }
            else if (Global.gameState == GameState.Level03 && _start == false && keyboardState.IsKeyDown(Keys.Space))
            {
                //Function to set the ball velocity.
                SetBallVelocity(_maxSpeed);
                //Set the game start bool to true.
                _start = true;
            }

        }

        //Function to reset the ball.
        public void ResetBall(Player bat)
        {
            //If the player still has lives left.
            if (_livesLeft > 0)
            {
                _colRect = new Rectangle((int)bat._batPos.X / 2 - 16,
                    (int)bat._batPos.Y - 48,
                    _ballSource.Width,
                    _ballSource.Height);
                //Reset the bat position to the centre of screen.
                bat._batPos = new Vector2(Global.windowSize.X / 2 - 84, Global.windowSize.Y - 100);
                //Reset the ball position to just above the centre of the bat.
                _ballPos = new Vector2(bat._batPos.X + bat._batSource.Width / 2 - _ballSource.Width / 2, bat._batPos.Y - 48);
                //Reset ball velocity to zero.
                _ballVelocity = Vector2.Zero;
                //Set the start boolean to false.
                _start = false;
                //Reset the ball lost boolean to false.
                _ballLost = false;
                ////Decrement lives left.
                //_livesLeft--;
            }
            //If the player has no lives left.
            else
            {
                //Change the game state to game over.
                Global.gameState = GameState.GameOver;
            }
        }

        //A method that calculates the postiion of the ball collision with the bat.
        float HitFactor(Vector2 ballPos, Player bat, float batWidth)
        {
            return (ballPos.X - (bat._batPos.X + bat._batSource.Width / 2)) / batWidth;
        }
    }
}
