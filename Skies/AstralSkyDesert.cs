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
            if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
            {
                spriteBatch.Draw(CalamityMod.AstralSky, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * opacity);

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
                        float posX = star.position.X * (Main.screenWidth / 500f);
                        float posY = star.position.Y * (Main.screenHeight / 600f);
                        Color astralcyan = new Color(100, 183, 255);

                        Vector2 position = new Vector2(posX + origin.X, posY + origin.Y + bgTop);

                        spriteBatch.Draw(t2D, position, new Rectangle(0, 0, t2D.Width, t2D.Height), astralcyan * star.twinkle * 0.952f * opacity, star.rotation, origin, (star.scale * star.twinkle) - 0.2f, SpriteEffects.None, 0f);
                    }
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < Main.star.Length; i++)
                    {

                        Star star = Main.star[i];
                        if (star == null)
                            continue;

                        Texture2D t2D = TextureAssets.Star[star.type].Value;
                        Vector2 origin = new Vector2(t2D.Width * 0.2f, t2D.Height * 0.2f);

                        int bgTop = (int)((-Main.screenPosition.Y) / (Main.worldSurface * 16.0 - 600.0) * 200.0);
                        float posX = star.position.X * (Main.screenWidth / 600f);
                        float posY = star.position.Y * (Main.screenHeight / 800f);
                        Color purple = new Color(201, 148, 255);

                        Vector2 position = new Vector2(posX + origin.X, posY + origin.Y + bgTop);

                        spriteBatch.Draw(t2D, position, new Rectangle(0, 0, t2D.Width, t2D.Height), purple * star.twinkle * 0.952f * opacity, star.rotation, origin, (star.scale * star.twinkle) + 0.2f, SpriteEffects.None, 0f);
                    }
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < Main.star.Length; i++)
                    {

                        Star star = Main.star[i];
                        if (star == null)
                            continue;

                        Texture2D t2D = TextureAssets.Star[star.type].Value;
                        Vector2 origin = new Vector2(t2D.Width * 0.8f, t2D.Height * 0.8f);

                        int bgTop = (int)((-Main.screenPosition.Y) / (Main.worldSurface * 16.0 - 600.0) * 200.0);
                        float posX = star.position.X * (Main.screenWidth / 200f);
                        float posY = star.position.Y * (Main.screenHeight / 900f);
                        Color yellow = new Color(255, 146, 73);

                        Vector2 position = new Vector2(posX + origin.X, posY + origin.Y + bgTop);

                        spriteBatch.Draw(t2D, position, new Rectangle(0, 0, t2D.Width, t2D.Height), yellow * star.twinkle * 0.952f * opacity, star.rotation, origin, star.scale * star.twinkle, SpriteEffects.None, 0f);
                    }
                }
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
                        float posX = star.position.X * (Main.screenWidth / 1000f);
                        float posY = star.position.Y * (Main.screenHeight / 200f);

                        Vector2 position = new Vector2(posX + origin.X, posY + origin.Y + bgTop);

                        spriteBatch.Draw(t2D, position, new Rectangle(0, 0, t2D.Width, t2D.Height), Color.White * star.twinkle * 0.952f * opacity, star.rotation, origin, star.scale * star.twinkle, SpriteEffects.None, 0f);
                    }
                }
            }
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
                    Texture2D texture = CalamityMod.AstralDesertSurfaceFar;
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
                    Texture2D texture = CalamityMod.AstralDesertSurfaceMiddle;
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
                }
                if (maxDepth >= 6f && minDepth < 6f)
                {
                    float screenParralaxMultiplier = 0.4f;
                    Texture2D texture = CalamityMod.AstralDesertSurfaceClose;
                    float scale = 2.0f;
                    int x = (int)(Main.screenPosition.X * 1.0f * screenParralaxMultiplier);
                    x %= (int)(texture.Width * scale);
                    int y = (int)(Main.screenPosition.Y * 0.55f * screenParralaxMultiplier);
                    y -= 2050; //1000
                    for (int k = -1; k <= 1; k++)
                    {
                        var pos = new Vector2(Main.screenWidth / 2f - x + texture.Width * k * scale, Main.screenHeight / 2f - y);
                        spriteBatch.Draw(texture, pos - texture.Size() / 2f * scale, null, new Color(63, 51, 90, 255) * opacity, 0f, new Vector2(0f, 0f), scale, SpriteEffects.None, 0f);
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
