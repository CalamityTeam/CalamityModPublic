using CalamityMod.World;
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

        private const float ScreenParralaxMultiplier = 0.4f;

        // Vanila scales backgrounds to 250% size. Depending on what you might want to do you can change this if you wanted to make a non scaled bg.
        private const float Scale = 2.5f;

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
                spriteBatch.Draw(CalamityMod.SulphurSeaSky, new Rectangle(0, (int)(-Main.screenPosition.Y / 6f) + 1300, Main.screenWidth, Main.screenHeight), Color.Lerp(Main.ColorOfTheSkies, Color.LightSeaGreen, 0.33f) * 0.2f * opacity);
            
            // Small worlds, default draw height
            int sulphurSeaHeight = (World.SulphurousSea.YStart + (int)Main.worldSurface) / 2;

            // Medium worlds
            if (Main.maxTilesX >= 6400 && Main.maxTilesX < 8400)
                sulphurSeaHeight = (World.SulphurousSea.YStart + (int)Main.worldSurface) / 5;

            // Large worlds (and anything bigger)
            if (Main.maxTilesX >= 8400)
                sulphurSeaHeight = (World.SulphurousSea.YStart + (int)Main.worldSurface) / 140;

            // Explantion on how to use this BG code for skies.
            // This changes the speed of the parralax, the closer the layer to the player the faster it should be.
            if (maxDepth >= 1f && minDepth < 1f)
            {
                Texture2D texture = CalamityMod.SulphurSeaSkyFront;

                // Keep in mind that y paralex should always be half of x's or it will feel odd compared to how terraria does it.
                // Keep in mind when you change screen parralax it affects the y offset for the bg in the world.
                int x = (int)(Main.screenPosition.X * ScreenParralaxMultiplier);
                x %= (int)(texture.Width * Scale);
                int y = (int)(Main.screenPosition.Y * 0.5f * ScreenParralaxMultiplier);

                // Y offset to align with whatever position you want it in the world (is affected by screenParralaxMultiplier as stated before).
                y -= 1800;

                float screenWidth = Main.screenWidth / 2f;
                float screenHeight = Main.screenHeight / 2f;
                Vector2 position = texture.Size() / 2f * Scale;
                Color color = Color.LightSeaGreen * 0.5f * opacity;

                // This loops the BG horizontally.
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(screenWidth - x + texture.Width * k * Scale, screenHeight - y);
                    spriteBatch.Draw(texture, pos - position, null, color, 0f, new Vector2(0f, (float)sulphurSeaHeight), Scale, SpriteEffects.None, 0f);
                }
            }

            if (maxDepth >= 3f && minDepth < 3f)
            {
                Texture2D texture = CalamityMod.SulphurSeaSurface;

                int x = (int)(Main.screenPosition.X * ScreenParralaxMultiplier);
                x %= (int)(texture.Width * Scale);
                int y = (int)(Main.screenPosition.Y * 0.5f * ScreenParralaxMultiplier);
                y -= 1800; // 1000

                float screenWidth = Main.screenWidth / 2f;
                float screenHeight = Main.screenHeight / 2f;
                Vector2 position = texture.Size() / 2f * Scale;
                Color color = Main.ColorOfTheSkies * opacity;

                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(screenWidth - x + texture.Width * k * Scale, screenHeight - y);
                    spriteBatch.Draw(texture, pos - position, null, color, 0f, new Vector2(0f, (float)sulphurSeaHeight), Scale, SpriteEffects.None, 0f);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Main.LocalPlayer.Calamity().ZoneSulphur || Main.gameMenu)
                skyActive = false;

            if (skyActive && opacity < 1f)
                opacity += 0.02f;
            else if (!skyActive && opacity > 0f)
                opacity -= 0.02f;
        }
    }
}
