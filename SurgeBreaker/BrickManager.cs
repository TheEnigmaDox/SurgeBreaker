using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SurgeBreaker
{
    internal class BrickManager
    {
        int _brickSpacing = 5;
        int _brickXSize = 128;
        int _brickYSize = 64;

        int[] _offsetInts = new int[4]
        {
            64,
            0,
            0,
            64
        };

        int _level = 0;

        public int _breakableBricks = 0;

        public bool _levelOneComplete = false;
        public bool _levelTwoComplete = false;
        public bool _levelThreeComplete = false;

        string _brickType;

        Texture2D _brickTexture;

        Rectangle _colRect;

        public List<Brick> _brickList = new List<Brick>();

        public int[,] _levelToSpawn;

        //The level Layouts.
        public int[,] _levelZero = new int[,]
        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 },
            { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 0 },
        };

        public int[,] _levelOne = new int[,] 
        {
            { 10, 9, 10, 9, 10, 9, 10, 9, 10, 9, 10 },        
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },        
            { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 },
            { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
        };

        public int[,] _levelTwo = new int[,]
        {
            { 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 0 },
            { 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 0 },
            { 1, 1, 1, 2, 2, 3, 3, 4, 4, 4, 0 },
            { 1, 1, 1, 1, 2, 3, 4, 4, 4, 4, 0 },
        };

        public int[,] _levelThree = new int[,]
        {
            { 4, 9, 4, 9, 4, 9, 4, 9, 4, 9, 4 },
            { 9, 3, 9, 3, 9, 3, 9, 3, 9, 3, 9 },
            { 2, 9, 2, 9, 2, 9, 2, 9, 2, 9, 2 },
            { 10, 1, 10, 1, 10, 1, 10, 1, 10, 1, 10 },
        };

        public BrickManager(Texture2D texture) 
        {
            _brickTexture = texture;
        }

        public void SpawnLevel(int level) 
        {
            //A check to see which level should be spawned.
            switch (level)
            {
                case 0:
                    _levelToSpawn = _levelZero;
                    break;
                case 1:
                    _levelToSpawn = _levelOne;
                    break;
                case 2:
                    _levelToSpawn = _levelTwo;
                    break;
                case 3:
                    _levelToSpawn = _levelThree;
                    break;
            }

            //Find the centre of the screen.
            Vector2 centrePos = Global.windowSize.ToVector2() / 2;

            //Calculate where the first brick should be placed.
            Vector2 brickDimension = new Vector2(_brickXSize + _brickSpacing, _brickYSize + _brickSpacing);
            //Calculate the offset to push first brick to the left.
            Vector2 gridOffset = new Vector2(brickDimension.X * _levelToSpawn.GetLength(1) / 2,
                brickDimension.Y * _levelToSpawn.GetLength(0) * 1.25f);
            //Store the top left position.
            Vector2 gridTopLeft = centrePos - gridOffset;

            //Variable to store brick offset.
            int brickOffset;

            //Loop through the first part of the array.
            for(int i = 0; i < _levelToSpawn.GetLength(1); i++)
            {
                //Loop through the second part of the array.
                for(int j = 0; j < _levelToSpawn.GetLength(0); j++)
                {
                    //If position [j, i] is equal to 1...
                    if (_levelToSpawn[j, i] == 1 || _levelToSpawn[j, i] == 2 || _levelToSpawn[j, i] == 3 || _levelToSpawn[j, i] == 4)
                    {
                        //Set the brick type.
                        _brickType = "Normal";

                        //Create a collision rectangle for the brick.
                        _colRect = new Rectangle((gridTopLeft + new Vector2((brickDimension.X * i), brickDimension.Y * j)).ToPoint(),
                            new Point(_brickXSize, _brickYSize));

                        if (j % 2 != 0)
                        {
                            //Add any offset to the x element of the collision rectangle.
                            _colRect.X += _offsetInts[level]; 
                        }

                        //Add the brick to the list of bricks.
                        _brickList.Add(new Brick(_colRect, 3, _brickType, _levelToSpawn[j, i]));

                        _breakableBricks++;
                    }
                    else if (_levelToSpawn[j, i] == 10)
                    {
                        //Set the brick type.
                        _brickType = "Static";

                        //Calculate if there is a brick offset.
                        if (j % 2 == 0)
                        {
                            //Set brick offset to zero.
                            brickOffset = 0;
                        }
                        else
                        {
                            //Set brick offset to hlaf the bricks width.
                            brickOffset = 64;
                        }

                        //Create a collision rectangle for the brick.
                        _colRect = new Rectangle((gridTopLeft + new Vector2((brickDimension.X * i), brickDimension.Y * j)).ToPoint(),
                            new Point(_brickXSize, _brickYSize));

                        if (brickOffset != 0)
                        {
                            //Add any offset to the x element of the collision rectangle.
                            _colRect.X += brickOffset; 
                        }

                        //Add the brick to the list of bricks.
                        _brickList.Add(new Brick(_colRect, 3, _brickType, _levelToSpawn[j, i]));
                    }
                    else if (_levelToSpawn[j, i] == 9)
                    {
                        //Set the brick type.
                        _brickType = "Glass";

                        //Calculate if there is a brick offset.
                        if (j % 2 == 0)
                        {
                            //Set brick offset to zero.
                            brickOffset = 0;
                        }
                        else
                        {
                            //Set brick offset to hlaf the bricks width.
                            brickOffset = 64;
                        }

                        //Create a collision rectangle for the brick.
                        _colRect = new Rectangle((gridTopLeft + new Vector2((brickDimension.X * i),
                            brickDimension.Y * j)).ToPoint(),
                            new Point(_brickXSize, _brickYSize));

                        //Add any offset to the x element of the collision rectangle.
                        _colRect.X += brickOffset;

                        //Add the brick to the list of bricks.
                        _brickList.Add(new Brick(_colRect, 1, _brickType, _levelToSpawn[j, i]));
                        _breakableBricks++;
                    }
                }
            }

            //Debug.WriteLine("Breakable Bricks: " + _breakableBricks.ToString());
            //Debug.WriteLine("BrickList count: " + _brickList.Count.ToString());
        }

        public void Update(Ball ball, Player bat, Charge chargeBar)
        {
            //Loop through the brick list
            for(int i = 0; i < _brickList.Count; i++)
            {
                //If the brick is dead.
                if (_brickList[i].IsDead())
                {
                    //Play sound effect.
                    Global.energyFX.Play();
                    //Remove the brick from the list.
                    _brickList.RemoveAt(i);
                    //Decrement the breakable bricks integer.
                    _breakableBricks--;
                    //Increment the broken bricks integer.
                    Global.brokenBricks++;
                    //Add charge to the charge bar.
                    chargeBar._barWidth += 100f;
                    //Add charge to the charge gained float.
                    Global.chargeGained += 100f;
                    //Increment the ball speed.
                    ball._maxSpeed++;
                }
                //Otherwise
                else
                {
                    //If the ball and player rectangles intersect
                    if (ball._colRect.Intersects(_brickList[i]._colRect) && _brickList[i]._brickType != "Glass")
                    {
                        //Play this sound effect.
                        Global.beepFX.Play();

                        //Create and store the rectangle created by the overlap
                        Rectangle overlap = Rectangle.Intersect(ball._colRect, _brickList[i]._colRect);

                        //If overlap width is equal to height.
                        if(overlap.Width == overlap.Height)
                        {
                            //Invert balll velocity.
                            ball._ballVelocity *= -1f;
                        }
                        //Otherwise if the width is greater than the height
                        else if (overlap.Width > overlap.Height)
                        {
                            //Invert the y part of the ball velocity.
                            ball._ballVelocity.Y *= -1f;
                        }
                        //Otherwise
                        else
                        {
                            //Onvert the x part of the ball velocity.
                            ball._ballVelocity.X *= -1f;
                        }
                        //Decrement the brick hits left.
                        _brickList[i].TakeHit(1);
                        //Move the ball back to the previous position
                        ball.BackToOldPos();
                    }
                    //Otherwise if the ball and bat collide and the brick type is glass.
                    else if(ball._colRect.Intersects(_brickList[i]._colRect) && _brickList[i]._brickType == "Glass")
                    {
                        //Decrement brick hits.
                        _brickList[i].TakeHit(1);
                    }
                }

                //On the collision of the last brick on the level 
                if(_breakableBricks == 1 && _brickList[0]._hitsLeft == 1 && ball._colRect.Intersects(_brickList[0]._colRect))
                {
                    //Reset the balls position.
                    ball.ResetBall(bat);
                }
            }
        }

        public void DrawBricks(Texture2D debugPixel)
        {
            if (_brickList.Count > 0)
            {
                //Loop through the brick list and draw each brick.
                for (int i = 0; i < _brickList.Count; i++)
                {
                    _brickList[i].DrawBricks(_brickTexture, _brickList, debugPixel);
                } 
            }
        }

        public void ResetBrickList()
        {
            _brickList = new List<Brick>();
        }
    }
}
