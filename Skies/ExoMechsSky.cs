using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Effects;

namespace CalamityMod.Skies
{
    public class ExoMechsSky : CustomSky
    {
        public class Lightning
        {
            public int Lifetime;
            public float Depth;
            public Vector2 Position;
        }

        public float BackgroundIntensity;
        public float LightningIntensity;
        public List<Lightning> LightningBolts = new List<Lightning>();
        public bool CanSkyBeActive
        {
			get
			{
                int draedon = CalamityGlobalNPC.draedon;
                if (draedon == -1 || !Main.npc[draedon].active)
                    return false;

                if (Main.npc[draedon].ModNPC<Draedon>().DefeatTimer <= 0 && !Draedon.ExoMechIsPresent)
                    return false;

                return true;
            }
		}

        public float CurrentIntensity
		{
			get
			{
                float combinedLifeRatio = 0f;
                if (CalamityGlobalNPC.draedonExoMechPrime != -1 && Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
                    combinedLifeRatio += Main.npc[CalamityGlobalNPC.draedonExoMechPrime].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechPrime].lifeMax;

                if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1 && Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
                    combinedLifeRatio += Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].lifeMax;

                if (CalamityGlobalNPC.draedonExoMechWorm != -1 && Main.npc[CalamityGlobalNPC.draedonExoMechWorm].active)
                    combinedLifeRatio += Main.npc[CalamityGlobalNPC.draedonExoMechWorm].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechWorm].lifeMax;

                return (float)Math.Pow(1f - combinedLifeRatio / 3f, 2D);
            }
		}

        public static readonly Color DrawColor = new Color(0.16f, 0.16f, 0.16f);

        public override void Update(GameTime gameTime)
		{
            if (!CanSkyBeActive)
			{
                LightningIntensity = 0f;
                BackgroundIntensity = MathHelper.Clamp(BackgroundIntensity - 0.05f, 0f, 1f);
                LightningBolts.Clear();
                return;
            }

            LightningIntensity = MathHelper.Clamp(LightningIntensity * 0.95f - 0.025f, 0f, 1f);
            BackgroundIntensity = MathHelper.Clamp(BackgroundIntensity + 0.1f, 0f, 1f);

            for (int i = 0; i < LightningBolts.Count; i++)
			{
                LightningBolts[i].Lifetime--;
			}

            for (int i = 0; i < 8; i++)
            {
                if (Main.rand.NextBool((int)MathHelper.Lerp(20f, 125f, CurrentIntensity)))
                {
                    Lightning lightning = new Lightning()
                    {
                        Lifetime = 30,
                        Depth = Main.rand.NextFloat(1.5f, 10f),
                        Position = new Vector2(Main.rand.NextFloat(2000f, Main.maxTilesX * 16f - 2000f), Main.rand.NextFloat(4850f))
                    };
                    LightningBolts.Add(lightning);
                }
            }

            if (Main.rand.NextBool((int)MathHelper.Lerp(780f, 300f, CurrentIntensity)))
                LightningIntensity = 0.33f;
        }

        public override Color OnTileColor(Color inColor) => new Color(Vector4.Lerp(DrawColor.ToVector4() * 0.7f, inColor.ToVector4(), 1f - BackgroundIntensity));

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= float.MaxValue)
            {
                // Draw lightning in the background based on Main.magicPixel.
                // It is a long, white vertical strip that exists for some reason.
                // This lightning effect is achieved by expanding this to fit the entire background and then drawing it as a distinct element.
                Vector2 scale = new Vector2(Main.screenWidth * 1.1f / Main.magicPixel.Width, Main.screenHeight * 1.1f / Main.magicPixel.Height);
                Vector2 screenArea = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
                Color drawColor = Color.White * MathHelper.Lerp(0f, 0.38f, LightningIntensity) * BackgroundIntensity;

                // Draw a grey background as base.
                spriteBatch.Draw(Main.magicPixel, screenArea, null, OnTileColor(Color.Transparent), 0f, Main.magicPixel.Size() * 0.5f, scale, SpriteEffects.None, 0f);

                for (int i = 0; i < 2; i++)
                    spriteBatch.Draw(Main.magicPixel, screenArea, null, drawColor, 0f, Main.magicPixel.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            }

            Texture2D flashTexture = TextureManager.Load("Images/Misc/VortexSky/Flash");
            Texture2D boltTexture = TextureManager.Load("Images/Misc/VortexSky/Bolt");

            // Draw lightning bolts.
            float spaceFade = Math.Min(1f, (Main.screenPosition.Y - 600f) / 600f);
            Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.5f);
            Rectangle rectangle = new Rectangle(-1000, -1000, 4000, 4000);

            LightningBolts.RemoveAll(l => l.Lifetime <= 0);

            for (int i = 0; i < LightningBolts.Count; i++)
            {
                if (LightningBolts[i].Depth > minDepth && LightningBolts[i].Depth < maxDepth)
                {
                    Vector2 boltScale = new Vector2(1f / LightningBolts[i].Depth, 0.9f / LightningBolts[i].Depth);
                    Vector2 position = (LightningBolts[i].Position - screenCenter) * boltScale + screenCenter - Main.screenPosition;
                    if (rectangle.Contains((int)position.X, (int)position.Y))
                    {
                        Texture2D texture = boltTexture;
                        int life = LightningBolts[i].Lifetime;
                        if (life > 24 && life % 2 == 0)
                            texture = flashTexture;

                        float opacity = life * spaceFade / 20f;
                        spriteBatch.Draw(texture, position, null, Color.White * opacity, 0f, Vector2.Zero, boltScale.X * 5f, SpriteEffects.None, 0f);
                    }
                }
            }
        }

        public override void Reset() { }

        public override void Activate(Vector2 position, params object[] args) { }

        public override void Deactivate(params object[] args) { }

        public override bool IsActive() => CanSkyBeActive && !Main.gameMenu;
    }
}
