using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Skies
{
    public class BrimstoneCragSky : CustomSky
    {
        public int skyActiveLeeway = 0;

        public class Cinder
        {
            public int Time;
            public int Lifetime;
            public int IdentityIndex;
            public float Scale;
            public float Depth;
            public Color DrawColor;
            public Vector2 Velocity;
            public Vector2 Center;

            public Cinder(int lifetime, int identity, float depth, Color color, Vector2 startingPosition, Vector2 startingVelocity)
            {
                Lifetime = lifetime;
                IdentityIndex = identity;
                Depth = depth;
                DrawColor = color;
                Center = startingPosition;
                Velocity = startingVelocity;
            }
        }

        private bool skyActive;
        private float opacity;
        public List<Cinder> Cinders = new List<Cinder>();

        public override void Deactivate(params object[] args) => skyActive = false;

        public override void Reset() => skyActive = false;

        public override bool IsActive() => skyActive || opacity > 0f;

        public override void Activate(Vector2 position, params object[] args) => skyActive = true;

        public override void Update(GameTime gameTime)
        {
            if (!Main.LocalPlayer.Calamity().ZoneCalamity || Main.gameMenu)
            {
                skyActive = false;
                if (skyActiveLeeway > 0)
                    skyActiveLeeway--;
            }
            else if (skyActiveLeeway < 60)
                skyActiveLeeway++;

            if (skyActive && opacity < 1f)
                opacity += 0.02f;
            else if (!skyActive && opacity > 0f)
                opacity -= 0.02f;

            static Color selectCinderColor()
            {
                if (Main.rand.NextBool(3))
                    return Color.Lerp(Color.DarkGray, Color.LightGray, Main.rand.NextFloat());

                return Color.Lerp(Color.Red, Color.Yellow, Main.rand.NextFloat(0.9f));
            }

            // Randomly add cinders.
            if (Main.rand.NextBool(12) && skyActive)
            {
                int lifetime = Main.rand.Next(285, 445);
                float depth = Main.rand.NextFloat(1.8f, 5f);
                Vector2 startingPosition = Main.screenPosition + new Vector2(Main.screenWidth * Main.rand.NextFloat(-0.1f, 1.1f), Main.screenHeight * 1.05f);
                Vector2 startingVelocity = -Vector2.UnitY.RotatedByRandom(0.91f);
                Cinders.Add(new Cinder(lifetime, Cinders.Count, depth, selectCinderColor(), startingPosition, startingVelocity));
            }

            // Update all cinders.
            if (skyActive || skyActiveLeeway > 0)
            {
                float cinderSpeed = 5.6f;
                for (int i = 0; i < Cinders.Count; i++)
                {
                    Cinders[i].Scale = Utils.GetLerpValue(Cinders[i].Lifetime, Cinders[i].Lifetime / 3, Cinders[i].Time, true);
                    Cinders[i].Scale *= MathHelper.Lerp(0.6f, 0.9f, Cinders[i].IdentityIndex % 6f / 6f);

                    Vector2 idealVelocity = -Vector2.UnitY.RotatedBy(MathHelper.Lerp(-0.94f, 0.94f, (float)Math.Sin(Cinders[i].Time / 36f + Cinders[i].IdentityIndex) * 0.5f + 0.5f)) * cinderSpeed;
                    float movementInterpolant = MathHelper.Lerp(0.01f, 0.08f, Utils.GetLerpValue(45f, 145f, Cinders[i].Time, true));
                    Cinders[i].Velocity = Vector2.Lerp(Cinders[i].Velocity, idealVelocity, movementInterpolant);
                    Cinders[i].Velocity = Cinders[i].Velocity.SafeNormalize(-Vector2.UnitY) * cinderSpeed;
                    Cinders[i].Time++;

                    Cinders[i].Center += Cinders[i].Velocity;
                }
            }

            // Clear away all dead cinders.
            Cinders.RemoveAll(c => c.Time >= c.Lifetime);

            if (!Main.LocalPlayer.Calamity().ZoneCalamity)
                Filters.Scene["CalamityMod:BrimstoneCrag"].Deactivate(Array.Empty<object>());
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (!Main.LocalPlayer.Calamity().ZoneCalamity && skyActiveLeeway == 0)
                return;

            // Draw cinders.
            Texture2D cinderTexture = ModContent.Request<Texture2D>("CalamityMod/Skies/CalamitasCinder").Value;
            float scaleFade = skyActiveLeeway / 60f;
            Color offsetDrawColor = Color.Red * 0.56f;
            offsetDrawColor.A = 0;
            Vector2 origin = cinderTexture.Size() * 0.5f;
            float cinderScale = 1.5f * scaleFade;
            for (int i = 0; i < Cinders.Count; i++)
            {
                Vector2 drawPosition = Cinders[i].Center - Main.screenPosition;
                for (int j = 0; j < 3; j++)
                {
                    Vector2 offsetDrawPosition = drawPosition + (MathHelper.TwoPi * j / 3f).ToRotationVector2() * 1.4f;
                    spriteBatch.Draw(cinderTexture, offsetDrawPosition, null, offsetDrawColor, 0f, origin, Cinders[i].Scale * cinderScale, SpriteEffects.None, 0f);
                }

                spriteBatch.Draw(cinderTexture, drawPosition, null, Cinders[i].DrawColor, 0f, origin, Cinders[i].Scale * scaleFade, SpriteEffects.None, 0f);
            }
        }
    }
}
