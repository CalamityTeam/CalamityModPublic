using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace CalamityMod.Skies
{
    public class AstralSkySurface : CustomSky
    {
        private bool skyActive;
        private float opacity;

        public override void Deactivate(params object[] args)
        {
            skyActive = Main.LocalPlayer.Calamity().ZoneAstral && !Main.LocalPlayer.ZoneDungeon && BiomeTileCounterSystem.AstralTiles > 950 && !Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneSnow;
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
        public override Color OnTileColor(Color inColor)
        {
            return Color.Lerp(inColor, new Color(63, 51, 90, inColor.A), opacity);
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            //Background from here starting from the back layer to the front layer
            if (maxDepth >= 9f && minDepth < 9f)
            {
                float screenParralaxMultiplier = 0.4f;
                Texture2D texture = CalamityMod.AstralSurfaceHorizon;
                float scale = 2.0f;
                int x = (int)(Main.screenPosition.X * 0.4f * screenParralaxMultiplier);
                x %= (int)(texture.Width * scale);
                int y = (int)(Main.screenPosition.Y * 0.4f * screenParralaxMultiplier);
                y -= 1380; //1000
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(Main.screenWidth / 2f - x + texture.Width * k * scale, Main.screenHeight / 2f - y);
                    spriteBatch.Draw(texture, pos - texture.Size() / 2f * scale, null, new Color(63, 51, 90, 255) * opacity, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0f);
                }
            }
            if (maxDepth >= 8f && minDepth < 8f)
            {
                float screenParralaxMultiplier = 0.4f;
                Texture2D texture = CalamityMod.AstralSurfaceFar;
                float scale = 2.0f;
                int x = (int)(Main.screenPosition.X * 0.5f * screenParralaxMultiplier);
                x %= (int)(texture.Width * scale);
                int y = (int)(Main.screenPosition.Y * 0.45f * screenParralaxMultiplier);
                y -= 1520; //1000
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(Main.screenWidth / 2f - x + texture.Width * k * scale, Main.screenHeight / 2f - y);
                    spriteBatch.Draw(texture, pos - texture.Size() / 2f * scale, null, new Color(63, 51, 90, 255) * opacity, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0f);
                }
            }
            if (maxDepth >= 7f && minDepth < 7f)
            {
                float screenParralaxMultiplier = 0.4f;
                Texture2D texture = CalamityMod.AstralSurfaceMiddle;
                float scale = 2.0f;
                int x = (int)(Main.screenPosition.X * 0.8f * screenParralaxMultiplier);
                x %= (int)(texture.Width * scale);
                int y = (int)(Main.screenPosition.Y * 0.5f * screenParralaxMultiplier);
                y -= 1900; //1000
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(Main.screenWidth / 2f - x + texture.Width * k * scale, Main.screenHeight / 2f - y);
                    spriteBatch.Draw(texture, pos - texture.Size() / 2f * scale, null, new Color(63, 51, 90, 255) * opacity, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0f);
                }

                Texture2D textureglow = CalamityMod.AstralSurfaceMiddleGlow;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(Main.screenWidth / 2f - x + texture.Width * k * scale, Main.screenHeight / 2f - y);
                    spriteBatch.Draw(textureglow, pos - texture.Size() / 2f * scale, null, Color.White * 0.5f * opacity, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0f);
                }
            }
            if (maxDepth >= 6f && minDepth < 6f)
            {
                float screenParralaxMultiplier = 0.4f;
                Texture2D texture = CalamityMod.AstralSurfaceClose;
                float scale = 2.0f;
                int x = (int)(Main.screenPosition.X * 0.9f * screenParralaxMultiplier);
                x %= (int)(texture.Width * scale);
                int y = (int)(Main.screenPosition.Y * 0.55f * screenParralaxMultiplier);
                y -= 1880; //1000
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(Main.screenWidth / 2f - x + texture.Width * k * scale, Main.screenHeight / 2f - y);
                    spriteBatch.Draw(texture, pos - texture.Size() / 2f * scale, null, new Color(63, 51, 90, 255) * opacity, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0f);
                }

                Texture2D textureglow = CalamityMod.AstralSurfaceCloseGlow;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(Main.screenWidth / 2f - x + texture.Width * k * scale, Main.screenHeight / 2f - y);
                    spriteBatch.Draw(textureglow, pos - texture.Size() / 2f * scale, null, Color.White * 0.7f * opacity, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0f);
                }
            }
            if (maxDepth >= 5f && minDepth < 5f)
            {
                float screenParralaxMultiplier = 0.4f;
                Texture2D texture = CalamityMod.AstralSurfaceFront;
                float scale = 2.0f;
                int x = (int)(Main.screenPosition.X * 1.1f * screenParralaxMultiplier);
                x %= (int)(texture.Width * scale);
                int y = (int)(Main.screenPosition.Y * 0.6f * screenParralaxMultiplier);
                y -= 2100; //1000
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(Main.screenWidth / 2f - x + texture.Width * k * scale, Main.screenHeight / 2f - y);
                    spriteBatch.Draw(texture, pos - texture.Size() / 2f * scale, null, new Color(63, 51, 90, 255) * opacity, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0f);
                }

                Texture2D textureglow = CalamityMod.AstralSurfaceFrontGlow;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(Main.screenWidth / 2f - x + texture.Width * k * scale, Main.screenHeight / 2f - y);
                    spriteBatch.Draw(textureglow, pos - texture.Size() / 2f * scale, null, Color.White * 0.9f * opacity, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0f);
                }
            }

        }

        public override void Update(GameTime gameTime)
        {
            if (!Main.LocalPlayer.Calamity().ZoneAstral || Main.gameMenu)
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
