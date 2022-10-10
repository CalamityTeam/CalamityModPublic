using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace CalamityMod.Skies
{
    public class AstralSky : CustomSky
    {
        private bool skyActive;
        private float opacity;

        public override void Deactivate(params object[] args)
        {
            skyActive = Main.LocalPlayer.Calamity().ZoneAstral;
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
            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                spriteBatch.Draw(CalamityMod.AstralSky, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Main.ColorOfTheSkies * opacity);

                //Terraria's conditions.
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < Main.star.Length; i++)
                    {
                        Star star = Main.star[i];
                        if (star == null)
                            continue;

                        Texture2D t2D = TextureAssets.Star[star.type].Value;
                        Vector2 origin = new Vector2(t2D.Width * 0.5f, t2D.Height * 0.5f);

                        int bgTop = (int)((-Main.screenPosition.Y) / (Main.worldSurface * 16.0 - 600.0) * 200.0);
                        float posX = star.position.X * (Main.screenWidth / 800f);
                        float posY = star.position.Y * (Main.screenHeight / 600f);

                        Vector2 position = new Vector2(posX + origin.X, posY + origin.Y + bgTop);

                        spriteBatch.Draw(t2D, position, new Rectangle(0, 0, t2D.Width, t2D.Height), Color.White * star.twinkle * 0.952f * opacity, star.rotation, origin, star.scale * star.twinkle, SpriteEffects.None, 0f);
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Main.LocalPlayer.Calamity().ZoneAstral)
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

        public override float GetCloudAlpha()
        {
            return (1f - opacity) * 0.97f + 0.03f;
        }
    }
}
