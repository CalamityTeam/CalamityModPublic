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

        private const float Scale = 2f;
        private const float ScreenParralaxMultiplier = 0.4f;

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
            // Small worlds, default draw height.
            int AstralBiomeHeight = (World.AstralBiome.YStart + (int)Main.worldSurface) / 2;

            // Medium worlds.
            if (Main.maxTilesX >= 6400 && Main.maxTilesX < 8400)
                AstralBiomeHeight = (World.AstralBiome.YStart + (int)Main.worldSurface) / 4;

            // Large worlds (and anything bigger).
            if (Main.maxTilesX >= 8400)
                AstralBiomeHeight = (World.AstralBiome.YStart + (int)Main.worldSurface) / 140;

            // Background from here starting from the back layer to the front layer.
            float width = Main.screenWidth / 2f;
            float height = Main.screenHeight / 2f;
            Color textureColor = new Color(63, 51, 90, 255) * opacity;
            Vector2 origin = new Vector2(0f, AstralBiomeHeight);
            if (maxDepth >= 9f && minDepth < 9f)
            {
                Texture2D texture = CalamityMod.AstralSurfaceHorizon;
                int x = (int)(Main.screenPosition.X * 0.4f * ScreenParralaxMultiplier);
                x %= (int)(texture.Width * Scale);
                int y = (int)(Main.screenPosition.Y * 0.4f * ScreenParralaxMultiplier);
                y -= 1380; // 1000
                Vector2 position = texture.Size() / 2f * Scale;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + texture.Width * k * Scale, height - y);
                    spriteBatch.Draw(texture, pos - position, null, textureColor, 0f, origin, Scale, SpriteEffects.None, 0f);
                }
            }

            if (maxDepth >= 8f && minDepth < 8f)
            {
                Texture2D texture = CalamityMod.AstralSurfaceFar;
                int x = (int)(Main.screenPosition.X * 0.5f * ScreenParralaxMultiplier);
                x %= (int)(texture.Width * Scale);
                int y = (int)(Main.screenPosition.Y * 0.45f * ScreenParralaxMultiplier);
                y -= 1520; // 1000
                Vector2 position = texture.Size() / 2f * Scale;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + texture.Width * k * Scale, height - y);
                    spriteBatch.Draw(texture, pos - position, null, textureColor, 0f, origin, Scale, SpriteEffects.None, 0f);
                }
            }

            if (maxDepth >= 7f && minDepth < 7f)
            {
                Texture2D texture = CalamityMod.AstralSurfaceMiddle;
                int x = (int)(Main.screenPosition.X * 0.8f * ScreenParralaxMultiplier);
                x %= (int)(texture.Width * Scale);
                int y = (int)(Main.screenPosition.Y * 0.5f * ScreenParralaxMultiplier);
                y -= 1900; // 1000
                Vector2 position = texture.Size() / 2f * Scale;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + texture.Width * k * Scale, height - y);
                    spriteBatch.Draw(texture, pos - position, null, textureColor, 0f, origin, Scale, SpriteEffects.None, 0f);
                }

                Texture2D textureglow = CalamityMod.AstralSurfaceMiddleGlow;
                Color textureGlowColor = Color.White * 0.5f * opacity;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + texture.Width * k * Scale, height - y);
                    spriteBatch.Draw(textureglow, pos - position, null, textureGlowColor, 0f, origin, Scale, SpriteEffects.None, 0f);
                }
            }

            if (maxDepth >= 6f && minDepth < 6f)
            {
                Texture2D texture = CalamityMod.AstralSurfaceClose;
                int x = (int)(Main.screenPosition.X * 0.9f * ScreenParralaxMultiplier);
                x %= (int)(texture.Width * Scale);
                int y = (int)(Main.screenPosition.Y * 0.55f * ScreenParralaxMultiplier);
                y -= 1880; // 1000
                Vector2 position = texture.Size() / 2f * Scale;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + texture.Width * k * Scale, height - y);
                    spriteBatch.Draw(texture, pos - position, null, textureColor, 0f, origin, Scale, SpriteEffects.None, 0f);
                }

                Texture2D textureglow = CalamityMod.AstralSurfaceCloseGlow;
                Color textureGlowColor = Color.White * 0.7f * opacity;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + texture.Width * k * Scale, height - y);
                    spriteBatch.Draw(textureglow, pos - position, null, textureGlowColor, 0f, origin, Scale, SpriteEffects.None, 0f);
                }
            }

            if (maxDepth >= 5f && minDepth < 5f)
            {
                Texture2D texture = CalamityMod.AstralSurfaceFront;
                int x = (int)(Main.screenPosition.X * 1.1f * ScreenParralaxMultiplier);
                x %= (int)(texture.Width * Scale);
                int y = (int)(Main.screenPosition.Y * 0.6f * ScreenParralaxMultiplier);
                y -= 2100; // 1000
                Vector2 position = texture.Size() / 2f * Scale;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + texture.Width * k * Scale, height - y);
                    spriteBatch.Draw(texture, pos - position, null, textureColor, 0f, origin, Scale, SpriteEffects.None, 0f);
                }

                Texture2D textureglow = CalamityMod.AstralSurfaceFrontGlow;
                Color textureGlowColor = Color.White * 0.9f * opacity;
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + texture.Width * k * Scale, height - y);
                    spriteBatch.Draw(textureglow, pos - position, null, textureGlowColor, 0f, origin, Scale, SpriteEffects.None, 0f);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Main.LocalPlayer.Calamity().ZoneAstral || Main.gameMenu)
                skyActive = false;

            if (skyActive && opacity < 1f)
                opacity += 0.02f;
            else if (!skyActive && opacity > 0f)
                opacity -= 0.02f;
        }

        public override float GetCloudAlpha()
        {
            return (1f - opacity) * 0.97f + 0.03f;
        }
    }
}
