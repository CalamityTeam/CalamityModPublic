using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class TitaniumRailgunShell : Particle
    {
        public override string Texture => "CalamityMod/Particles/TitaniumRailgunShell";
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;

        public float Opacity;
        public Point TileAttachement;
        public Color GlowColor;
        public bool isStuckToTile
        {
            get
            {
                if (Main.tile[TileAttachement].HasTile && Main.tile[TileAttachement].IsTileSolid())
                    return true;

                else
                {
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (Main.tile[TileAttachement + new Point(i, j)].HasTile && Main.tile[TileAttachement + new Point(i, j)].IsTileSolid())
                                return true;
                        }
                    }

                    return false;

                }
            }
        }

        public TitaniumRailgunShell(Vector2 position, Point tileCoords, float rotation, Color glowColor, int lifetime = 80)
        {
            Position = position;
            Scale = 1f;
            Color = Color.White;
            GlowColor = glowColor;
            Opacity = 1f;
            Velocity = Vector2.Zero;
            Rotation = rotation;
            Lifetime = lifetime;
            TileAttachement = tileCoords;

            if (!isStuckToTile)
                Opacity = 0;
        }

        public override void Update()
        {
            if (LifetimeCompletion > 0.7f)
                Opacity = MathHelper.Lerp(1f, 0f, (LifetimeCompletion - 0.7f) / 0.3f);

            Color = Lighting.GetColor(TileAttachement);
            Velocity *= 0.9f;

            if (!isStuckToTile)
                Opacity = 0f;

            if (Opacity <= 0.01f)
                GeneralParticleHandler.RemoveParticle(this);

            if (LifetimeCompletion < 0.2f && Main.rand.NextBool())
            {
                Vector2 dustPosition = Position + Main.rand.NextVector2Circular(8f, 8f);

                Dust dust = Dust.NewDustPerfect(dustPosition, 6, Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(-1f, -5f), 0, Color.White, 1.2f);
                dust.noGravity = true;
            }
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GeneralParticleHandler.GetTexture(Type);
            Texture2D glowTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/TitaniumRailgunShellGlow").Value;

            Vector2 minorDisplacementToMakeItLookLikeItActuallyWasMovingButFast = Rotation.ToRotationVector2() * -13f * (float)Math.Pow(MathHelper.Clamp(1 - LifetimeCompletion * 3f, 0f, 1f), 3f);

            spriteBatch.Draw(texture, Position + minorDisplacementToMakeItLookLikeItActuallyWasMovingButFast - Main.screenPosition, null, Color * Opacity, Rotation + MathHelper.PiOver2, texture.Size() * 0.5f, Scale, SpriteEffects.None, 0f);

            GlowColor.A = 4;
            float glowOpacity = 1 - Math.Clamp(LifetimeCompletion * 2f, 0f, 1f);

            spriteBatch.Draw(glowTexture, Position + minorDisplacementToMakeItLookLikeItActuallyWasMovingButFast - Main.screenPosition, null, GlowColor * glowOpacity, Rotation + MathHelper.PiOver2, glowTexture.Size() * 0.5f, Scale, SpriteEffects.None, 0f);
        }
    }
}
