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
    public class MonolithSky : CustomSky
    {
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
            if (Main.LocalPlayer.Calamity().monolithAccursedShader < 1 || Main.gameMenu)
                skyActive = false;

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
            if (skyActive)
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
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (Main.LocalPlayer.Calamity().monolithAccursedShader < 1)
                return;

            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue && Main.LocalPlayer.Calamity().monolithAccursedShader > 21)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/MainMenu/MenuBackground").Value;
                int offset = Main.BackgroundEnabled ? 200 : 0;
                spriteBatch.Draw(texture, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16 - Main.screenPosition.Y - texture.Height * 2) * 0.1f)) - offset, Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * opacity));
            }

            // Draw cinders.
            Texture2D cinderTexture = ModContent.Request<Texture2D>("CalamityMod/Skies/CalamitasCinder").Value;
            Color offsetDrawColor = Color.Red * 0.56f;
            offsetDrawColor.A = 0;
            Vector2 origin = cinderTexture.Size() * 0.5f;
            for (int i = 0; i < Cinders.Count; i++)
            {
                Vector2 drawPosition = Cinders[i].Center - Main.screenPosition;
                for (int j = 0; j < 3; j++)
                {
                    Vector2 offsetDrawPosition = drawPosition + (MathHelper.TwoPi * j / 3f).ToRotationVector2() * 1.4f;
                    spriteBatch.Draw(cinderTexture, offsetDrawPosition, null, offsetDrawColor, 0f, origin, Cinders[i].Scale * 1.5f, SpriteEffects.None, 0f);
                }

                spriteBatch.Draw(cinderTexture, drawPosition, null, Cinders[i].DrawColor, 0f, origin, Cinders[i].Scale, SpriteEffects.None, 0f);
            }
        }

        public override Color OnTileColor(Color color) => Color.Lerp(color, new Color(205, 100, 100), opacity);

        public override float GetCloudAlpha() => (1f - opacity) * 0.3f + 0.7f;
    }
}
