using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Net.Mime;

namespace SurgeBreaker
{
    public enum GameState
    {
        Title,
        Level01,
        Level02,
        Level03,
        Instructions,
        GameOver
    }

    internal static class Global
    {
        public static int brokenBricks = 0;

        public static float chargeGained = 0;

        public static GameState gameState = GameState.Title;

        public static Random RNG = new Random();

        public static Point windowSize = new Point(1600, 900);

        public static float elapsedTime;

        public static SpriteBatch spriteBatch;

        public static SoundEffect energyFX;
        public static SoundEffect beepFX;

        public static SoundEffectInstance energyFXInstance;

        public static void Update(GameTime gameTime)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
