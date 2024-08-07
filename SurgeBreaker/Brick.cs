using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SurgeBreaker
{
    internal class Brick
    {
        public int _hitsLeft;

        public string _brickType;

        Color _tint;

        public Rectangle _colRect;
        Rectangle _brickSource;

        public Brick(Rectangle colRect, int hitsLeft, string brickType, int brickColour) 
        {
            _colRect = colRect;
            _hitsLeft = hitsLeft;

            _brickType = brickType;

            _brickSource = new Rectangle(256 - (_hitsLeft - 1) * 128, 0, 128, 64);

            switch(brickColour)
            {
                case 1:
                    _tint = Color.Red;
                    break;
                case 2: 
                    _tint = Color.Blue;
                    break;
                case 3:
                    _tint = Color.Green;
                    break;
                case 4:
                    _tint = Color.Purple;
                    break;
                case 9:
                    _tint = Color.LightSkyBlue; 
                    break;
                case 10:
                    _tint = Color.DarkSlateGray; 
                    break;
            }
        }

        public void UpdateStaticBricks()
        {
            //Stops static brick from disappearing
            if(_brickType == "Static")
            {
                _hitsLeft = 3;
            }
        }

        public void DrawBricks(Texture2D brickTexture, List<Brick> brickList, Texture2D debugPixel)
        {
            if (IsDead())
            {
                return;
            }
            if (_brickType == "Normal")
            {
                Global.spriteBatch.Draw(brickTexture, new Vector2(_colRect.X, _colRect.Y), _brickSource, _tint);
            }
            else if(_brickType == "Static")
            {
                _brickSource = new Rectangle(0, 0, 128, 64);
                Global.spriteBatch.Draw(brickTexture, new Vector2(_colRect.X, _colRect.Y), _brickSource, _tint);
            }
            else if (_brickType == "Glass")
            {
                _brickSource = new Rectangle(0, 0, 128, 64);
                Global.spriteBatch.Draw(brickTexture, new Vector2(_colRect.X, _colRect.Y), _brickSource, _tint);
            }
        }

        //Method that takes in an int to be subtracted from brick hits left.
        public void TakeHit(int damage)
        {
            if (_hitsLeft <= 0)
            {
                return;
            }

            if(_brickType != "Static")
            {
                _hitsLeft--;
                _brickSource.X = 256 - (_hitsLeft - 1) * 128;
            }
        }

        public bool IsDead()
        {
            return _hitsLeft <= 0;
        }
    }
}
