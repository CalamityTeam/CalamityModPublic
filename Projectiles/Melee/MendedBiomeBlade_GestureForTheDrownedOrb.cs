using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.DataStructures;
using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class GestureForTheDrownedOrb : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_GestureForTheDrowned";
        public Player Owner => Main.player[projectile.owner];
        public ref float HasLanded => ref projectile.ai[0]; //Is the orb in the air or is it on the ground?
        public ref float TimeSinceLanding => ref projectile.ai[1]; //How long has it been on the ground for
        public float Timer => MaxTime - projectile.timeLeft;
        public const float MaxTime = 160f;
        public float groundOffset;
        public bool BounceX => projectile.oldPosition.X == projectile.position.X;

        public bool WaterMode = false;

        public float scaleX => 1 + 0.2f * (1f + -(float)Math.Sin((TimeSinceLanding - 10f) * 0.1f)) * 0.5f;
        public float scaleY => (1 + 0.4f * (1f + (float)Math.Sin((TimeSinceLanding - 10f) * 0.1f)) * 0.5f) * MathHelper.Clamp((TimeSinceLanding - 10) / 30f, 0f, 1f) * MathHelper.Clamp(projectile.timeLeft / 30f, 0f, 1f);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gesture for the Drowned");

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 26;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = (int)MaxTime;
            projectile.melee = true;
            projectile.alpha = 90;
            projectile.ignoreWater = true;
            projectile.hide = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (WaterMode)
            {
                return true;
            }
            HasLanded = 1f;
            projectile.velocity.Y = -4f;
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (TimeSinceLanding < 10 && !WaterMode)
                return base.Colliding(projHitbox, targetHitbox);
            float collisionPoint = 0f;
            if (!WaterMode)
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Bottom, projectile.Bottom - (Vector2.UnitY * scaleY * 110), scaleX * 40, ref collisionPoint);

            float halfLenght = 66f * projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center + Utils.SafeNormalize(projectile.velocity.RotatedBy(MathHelper.PiOver2), Vector2.Zero) * halfLenght, projectile.Center - Utils.SafeNormalize(projectile.velocity.RotatedBy(MathHelper.PiOver2), Vector2.Zero) * halfLenght, 40f, ref collisionPoint);
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {
            if (Collision.WetCollision(projectile.position, projectile.width, projectile.height))
            {
                WaterMode = true;
                projectile.velocity.Y *= 0.33f;
            }
            else if (WaterMode)
            {
                projectile.Kill();
            }

            if (WaterMode)
            {
                projectile.velocity *= 1.02f;
                projectile.scale *= 1.01f;
                projectile.timeLeft--;

                for (int i = 0; i <= 4; i++)
                {
                    Dust waterTrail = Main.dust[Dust.NewDust(projectile.Center + Main.rand.NextVector2Circular(10f, 10f), 23, 0, 33, -3.7f * Math.Sign(projectile.velocity.X), -6f, 0, new Color(255, 255, 255), Main.rand.NextFloat(1f, 3f))];
                    waterTrail.fadeIn = 3f;
                }

                Dust foamDust = Main.dust[Dust.NewDust(projectile.Center + Main.rand.NextVector2Circular(10f, 10f), 9, 0, 16, 2.7f * Math.Sign(projectile.velocity.X), -3f, 0, new Color(255, 255, 255), 1.4f)];
                foamDust.noGravity = true;

                if (Main.rand.NextBool())
                {
                    float angle = Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                    float distance = 10f + 50 * Math.Abs(angle) / MathHelper.PiOver2 * projectile.scale;
                    Vector2 sparkSpeed = -projectile.velocity;
                    Particle Spark = new CritSpark(projectile.Center + Vector2.Normalize(projectile.velocity.RotatedBy(angle)) * distance, sparkSpeed, Color.SkyBlue, Color.CornflowerBlue, 1f + Main.rand.NextFloat(0, 1f), 30, 0.4f, 0.6f);
                    GeneralParticleHandler.SpawnParticle(Spark);
                }
                return;
            }

            if (HasLanded == 1f)
            {
                TimeSinceLanding++;

                groundOffset = -1;
                //Check 7 tiles under the projectile for solid ground
                for (float i = 0; i < 7; i += 0.5f)
                {
                    Vector2 positionToCheck = projectile.position;
                    if (Main.tile[(int)(positionToCheck.X / 16), (int)((positionToCheck.Y / 16) + 1 * i)].IsTileSolidGround() && groundOffset == -1)
                    {
                        groundOffset = i * 16;
                        break;
                    }
                }
                //If no ground has been found, go back to the airborne state
                if (groundOffset == -1)
                {
                    HasLanded = 0f;
                    TimeSinceLanding = 0f;
                }

                if (TimeSinceLanding > 10)
                {
                    //Trailing water!! Dust spam!!!
                    Dust waterTrail = Main.dust[Dust.NewDust(projectile.Top + Vector2.UnitY * groundOffset, 23, 0, 33, -3.7f * Math.Sign(projectile.velocity.X), -6f, 0, new Color(255, 255, 255), Main.rand.NextFloat(1f, 3f))];
                    waterTrail.fadeIn = 3f;
                    waterTrail = Main.dust[Dust.NewDust(projectile.TopRight + Vector2.UnitY * groundOffset + Main.rand.NextVector2Circular(16f, 16f) + Vector2.UnitX * projectile.velocity.X * 4f, 23, 0, 33, -3.7f * Math.Sign(projectile.velocity.X), -6f, 0, new Color(255, 255, 255), Main.rand.NextFloat(1f, 3f))];
                    waterTrail.fadeIn = 3f;
                    waterTrail = Main.dust[Dust.NewDust(projectile.TopLeft + Vector2.UnitY * groundOffset + Main.rand.NextVector2Circular(16f, 16f) + Vector2.UnitX * projectile.velocity.X * 4f, 23, 0, 33, -3.7f * Math.Sign(projectile.velocity.X), -6f, 0, new Color(255, 255, 255), Main.rand.NextFloat(1f, 3f))];
                    waterTrail.fadeIn = 3f;


                    Dust foamDust = Main.dust[Dust.NewDust(projectile.Top + Vector2.UnitY * groundOffset + Vector2.UnitX * 16 * Math.Sign(projectile.velocity.X), 9, 0, 16, 2.7f * Math.Sign(projectile.velocity.X), -3f, 0, new Color(255, 255, 255), 1.4f)];
                    foamDust.noGravity = true;

                    //Foam
                    float height = scaleY * 100f;
                    Vector2 origin = projectile.Top + Vector2.UnitY * groundOffset + Main.rand.NextVector2Circular(projectile.width / 4f, projectile.height / 4f) - (Vector2.UnitY * height);
                    Particle foamSmall = new FakeGlowDust(origin, projectile.velocity / 2f, Color.LightSlateGray, Main.rand.NextFloat(1f, 2f), 25, 0.1f, bigSize: true, gravity: (-projectile.velocity + Vector2.UnitY) * 0.1f);
                    GeneralParticleHandler.SpawnParticle(foamSmall);

                    if (Main.rand.Next(5) == 0)
                    {
                        Particle foamLarge = new MediumMistParticle(origin, projectile.velocity / 2f, Color.LightSlateGray, Color.LightBlue, Main.rand.NextFloat(1f, 2f), 245 - Main.rand.Next(50), 0.1f);
                        GeneralParticleHandler.SpawnParticle(foamLarge);
                    }
                }
            }

            else
            {
                Particle foamSmall = new FakeGlowDust(projectile.Center + Main.rand.NextVector2Circular(projectile.width / 2f, projectile.height / 2f), projectile.velocity / 2f, Color.LightSlateGray, Main.rand.NextFloat(0.2f, 2f), 25, 0.1f, gravity: (-projectile.velocity + Vector2.UnitY) * 0.1f);
                GeneralParticleHandler.SpawnParticle(foamSmall);

                if (Main.rand.NextBool())
                {
                    Particle blob = new WaterGlobParticle(projectile.Center + Main.rand.NextVector2Circular(projectile.width / 2f, projectile.height / 2f), projectile.velocity / 2f, Main.rand.NextFloat(0.2f, 2f), 0.1f);
                    GeneralParticleHandler.SpawnParticle(blob);
                }

            }

            projectile.rotation += 0.1f * ((projectile.velocity.X > 0) ? 1f : -1f);


            if (projectile.velocity.Y < 15f)
            {
                projectile.velocity.Y += 0.3f;
            }

            if (Math.Abs(projectile.velocity.X) < 8)
            {
                projectile.velocity.X += 0.3f * ((projectile.velocity.X > 0) ? 1f : -1f);
            }
            else
            {
                projectile.velocity.X = Math.Sign(projectile.velocity.X) * 8;
            }

            if (BounceX)
            {
                //Die lmao
                projectile.Kill();
            }

            projectile.frameCounter++;
            if (projectile.frameCounter % 6 == 0)
                projectile.frame = (projectile.frame + 1) % 3;
        }

        public override void Kill(int timeLeft)
        {
            var splash = new LegacySoundStyle(SoundID.Splash, 0).WithPitchVariance(Main.rand.NextFloat());
            Main.PlaySound(splash, projectile.Center);

            for (int i = 0; i < 4; i++)
            {
                Particle foamLarge = new MediumMistParticle(projectile.Center + Main.rand.NextVector2Circular(projectile.width, projectile.height), projectile.oldVelocity / 2f, Color.LightSlateGray, Color.LightBlue, Main.rand.NextFloat(1f, 2f), 245 - Main.rand.Next(50), 0.1f);
                GeneralParticleHandler.SpawnParticle(foamLarge);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (BounceX)
                return false;

            if (WaterMode)
            {
                Texture2D wave = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_GestureForTheDrownedAquaWave");
                float drawRotation = projectile.velocity.ToRotation() + MathHelper.Pi;
                Vector2 drawOrigin = new Vector2(wave.Width / 2f, wave.Height / 2f);
                Vector2 drawOffset = projectile.Center - Main.screenPosition;

                spriteBatch.Draw(wave, drawOffset - projectile.velocity, null, Color.Lerp(lightColor, Color.White, 0.5f) * 0.2f, drawRotation, drawOrigin, projectile.scale - 0.2f, 0f, 0f);
                spriteBatch.Draw(wave, drawOffset, null, Color.Lerp(lightColor, Color.White, 0.5f) * 0.5f, drawRotation, drawOrigin, projectile.scale, 0f, 0f);
                return false;
            }


            if (HasLanded == 1f && TimeSinceLanding > 10f)
            {
                Texture2D wave = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_GestureForTheDrownedWave");
                Rectangle frame = new Rectangle(0, 0 + 110 * projectile.frame, 40, 110);

                float drawRotation = 0f;
                Vector2 drawOrigin = new Vector2(0f, frame.Height);


                Vector2 drawOffset;

                //Wobble
                Vector2 Scale;

                SpriteEffects flip = projectile.velocity.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                for (int i = 2; i >= 0; i--)
                {
                    drawOffset = projectile.position + Vector2.UnitY * groundOffset + Vector2.UnitY / 2f - Main.screenPosition;
                    drawOffset -= Vector2.UnitX * 10f * i * Math.Sign(projectile.velocity.X);
                    float ScaleYAlter = (1 + 0.4f * (1f + (float)Math.Sin((TimeSinceLanding - 10f) * 0.1f + 0.2 * i)) * 0.5f) * MathHelper.Clamp((TimeSinceLanding - 10) / 30f, 0f, 1f) * MathHelper.Clamp(projectile.timeLeft / 30f, 0f, 1f);
                    Scale = new Vector2(scaleX, ScaleYAlter - 0.05f * i);
                    float slightfade = 1f - 0.24f * i;
                    Color darkenedColor = Color.Lerp(lightColor, Color.Black, i * 0.15f);

                    spriteBatch.Draw(wave, drawOffset, frame, darkenedColor * MathHelper.Clamp(TimeSinceLanding / 30f, 0f, 1f) * slightfade, drawRotation, drawOrigin, Scale, flip, 0f);
                }



                return false;
            }

            else
            {
                Texture2D ball = GetTexture("CalamityMod/Projectiles/Melee/MendedBiomeBlade_GestureForTheDrowned");
                float drawRotation = projectile.rotation;
                Vector2 drawOrigin = projectile.Size / 2f;
                Vector2 drawOffset = projectile.Center - Main.screenPosition;

                spriteBatch.Draw(ball, drawOffset, null, lightColor * MathHelper.Clamp(1 - TimeSinceLanding / 10f, 0f, 1f), drawRotation, drawOrigin, projectile.scale, 0f, 0f);

                return false;
            }

        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(WaterMode);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            WaterMode = reader.ReadBoolean();
        }

    }
}