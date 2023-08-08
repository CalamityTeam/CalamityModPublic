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
    public class AstralSkyDesert : CustomSky
    {
        private bool skyActive;
        private float opacity;

        private const float Scale = 2f;

        public override void Deactivate(params object[] args)
        {
            skyActive = Main.LocalPlayer.Calamity().ZoneAstral && !Main.LocalPlayer.ZoneDungeon && BiomeTileCounterSystem.AstralTiles > 950 && Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneSnow;
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
            Color color = new Color(63, 51, 90, 255) * opacity;
            if (maxDepth >= 9f && minDepth < 9f)
            {
                float screenParralaxMultiplier = 0.16f;
                Texture2D texture = CalamityMod.AstralSurfaceHorizon;
                int x = (int)(Main.screenPosition.X * screenParralaxMultiplier);
                x %= (int)(texture.Width * Scale);
                int y = (int)(Main.screenPosition.Y * screenParralaxMultiplier);
                y -= 1380; // 1000
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + texture.Width * k * Scale, height - y);
                    spriteBatch.Draw(texture, pos - texture.Size() / 2f * Scale, null, color, 0f, new Vector2(0f, (float)AstralBiomeHeight), Scale, SpriteEffects.None, 0f);
                }
            }

            if (maxDepth >= 8f && minDepth < 8f)
            {
                float screenParralaxMultiplier = 0.4f;
                Texture2D texture = CalamityMod.AstralDesertSurfaceFar;
                int x = (int)(Main.screenPosition.X * 0.5f * screenParralaxMultiplier);
                x %= (int)(texture.Width * Scale);
                int y = (int)(Main.screenPosition.Y * 0.45f * screenParralaxMultiplier);
                y -= 1520; // 1000
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + texture.Width * k * Scale, height - y);
                    spriteBatch.Draw(texture, pos - texture.Size() / 2f * Scale, null, color, 0f, new Vector2(0f, (float)AstralBiomeHeight), Scale, SpriteEffects.None, 0f);
                }
            }

            if (maxDepth >= 7f && minDepth < 7f)
            {
                float screenParralaxMultiplier = 0.4f;
                Texture2D texture = CalamityMod.AstralDesertSurfaceMiddle;
                int x = (int)(Main.screenPosition.X * 0.8f * screenParralaxMultiplier);
                x %= (int)(texture.Width * Scale);
                int y = (int)(Main.screenPosition.Y * 0.5f * screenParralaxMultiplier);
                y -= 1900; // 1000
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + texture.Width * k * Scale, height - y);
                    spriteBatch.Draw(texture, pos - texture.Size() / 2f * Scale, null, color, 0f, new Vector2(0f, (float)AstralBiomeHeight), Scale, SpriteEffects.None, 0f);
                }
            }

            if (maxDepth >= 6f && minDepth < 6f)
            {
                float screenParralaxMultiplier = 0.4f;
                Texture2D texture = CalamityMod.AstralDesertSurfaceClose;
                int x = (int)(Main.screenPosition.X * 1f * screenParralaxMultiplier);
                x %= (int)(texture.Width * Scale);
                int y = (int)(Main.screenPosition.Y * 0.55f * screenParralaxMultiplier);
                y -= 2050; // 1000
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + texture.Width * k * Scale, height - y);
                    spriteBatch.Draw(texture, pos - texture.Size() / 2f * Scale, null, color, 0f, new Vector2(0f, (float)AstralBiomeHeight), Scale, SpriteEffects.None, 0f);
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
