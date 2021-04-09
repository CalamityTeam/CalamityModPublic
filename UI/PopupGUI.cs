using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityMod.UI
{
    public abstract class PopupGUI
    {
        public int FadeTime;
        public bool Active;
        public virtual int FadeTimeMax { get; set; } = 30;
        public virtual void Update()
        {
            if (Active)
            {
                if (FadeTime < FadeTimeMax)
                    FadeTime++;
            }
            else if (FadeTime > 0)
                FadeTime--;
            if (Main.mouseLeft && Main.mouseLeftRelease && !Main.blockMouse && FadeTime >= 30)
            {
                Main.mouseLeftRelease = false;
                Main.mouseLeft = false;
                Active = false;
            }
        }
        public float GetYTop() => MathHelper.Lerp(Main.screenHeight * 2, Main.screenHeight * 0.25f, FadeTime / (float)FadeTimeMax);
        public Vector2 GetScreenAdjustedScale(float textureScale, Texture2D drawTexture)
        {
            float xScale = MathHelper.Lerp(0.004f, 1f, FadeTime / (float)FadeTimeMax);
            Vector2 scale = new Vector2(xScale, 1f) * new Vector2(Main.screenWidth, Main.screenHeight) / drawTexture.Size();
            scale *= 0.5f;
            scale *= textureScale;
            return scale;
        }
        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}
