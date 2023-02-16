using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace CalamityMod.Skies
{
    public class SulphurSeaSky : CustomSky
    {
        private bool skyActive;
        private float opacity;

        public override void Deactivate(params object[] args)
        {
            skyActive = Main.LocalPlayer.Calamity().ZoneSulphur;
        }

        public override void Reset()
        {
            skyActive = false;
        }

        public override bool IsActive()
        {
            return skyActive || opacity > 0f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            skyActive = true;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 4f && minDepth < 4f)
            {
                spriteBatch.Draw(CalamityMod.SulphurSeaSky, new Rectangle(0, (int)(-Main.screenPosition.Y / 6f) + 1200, Main.screenWidth, Main.screenHeight), Color.Lerp(Main.ColorOfTheSkies, Color.White, 0.33f) * 0.4f * opacity);
            }
            if (maxDepth >= 1f && minDepth < 1f)
            {
                spriteBatch.Draw(CalamityMod.SulphurSeaSkyFront, new Rectangle(0, (int)(-Main.screenPosition.Y / 6f) + 1300, Main.screenWidth, Main.screenHeight), Color.Lerp(Main.ColorOfTheSkies, Color.White, 0.33f) * 1.5f * opacity);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Main.LocalPlayer.Calamity().ZoneSulphur || Main.gameMenu)
                skyActive = false;

            if (skyActive && opacity < 1f)
            {
                opacity += 0.02f;
            }
            else if (!skyActive && opacity > 0f)
            {
                opacity -= 0.02f;
            }
        }

        //public override float GetCloudAlpha()
        //{
        //    return (1f - opacity) * 0.97f + 0.03f;
        //}
    }
}
