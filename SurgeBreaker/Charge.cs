using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SurgeBreaker
{
    internal class Charge
    {
        public float _barWidth;

        float _dechargeTimer = 0f;
        float _maxDechargeTimer = 120f;

        Texture2D _backGround;
        Texture2D _chargeBar;

        public Charge(Texture2D backGround, Texture2D chargeBar) 
        {
            _backGround = backGround;
            _chargeBar = chargeBar;

            _barWidth = 800;

            _dechargeTimer = _maxDechargeTimer;
        }

        public void UpdateCharge()
        {
            //A check to stop the bar going wider than the screen.
            if(_barWidth >= 1600)
            {
                _barWidth = 1600;
            }

            //A timer to slowly decrease charge.
            if(_dechargeTimer <= 0f)
            {
                _barWidth -= 20f;
                _dechargeTimer = _maxDechargeTimer;
            }
            else
            {
                _dechargeTimer -= 1f;
            }
        }

        public void DrawChargeBar()
        {
            Global.spriteBatch.Draw(_backGround,
                new Rectangle(0, Global.windowSize.Y - 65, 1600, Global.windowSize.Y),
                new Rectangle(0, 100, 601, 65),
                Color.White);

            Global.spriteBatch.Draw(_chargeBar,
                new Rectangle(10, Global.windowSize.Y - 50, (int)_barWidth, 40),
                new Rectangle(0, 0, 1, 1),
            Color.Yellow);
        }
    }
}
