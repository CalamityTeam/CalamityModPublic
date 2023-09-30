using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class GestureForTheDrownedOrb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_GestureForTheDrowned";
        public Player Owner => Main.player[Projectile.owner];
        public ref float HasLanded => ref Projectile.ai[0]; //Is the orb in the air or is it on the ground?
        public ref float TimeSinceLanding => ref Projectile.ai[1]; //How long has it been on the ground for
        public float Timer => MaxTime - Projectile.timeLeft;
        public const float MaxTime = 160f;
        public float groundOffset;
        public bool BounceX => Projectile.oldPosition.X == Projectile.position.X;

        public bool WaterMode = false;

        public float scaleX => 1 + 0.2f * (1f + -(float)Math.Sin((TimeSinceLanding - 10f) * 0.1f)) * 0.5f;
        public float scaleY => (1 + 0.4f * (1f + (float)Math.Sin((TimeSinceLanding - 10f) * 0.1f)) * 0.5f) * MathHelper.Clamp((TimeSinceLanding - 10) / 30f, 0f, 1f) * MathHelper.Clamp(Projectile.timeLeft / 30f, 0f, 1f);

        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = (int)MaxTime;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 90;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (WaterMode)
            {
                return true;
            }
            HasLanded = 1f;
            Projectile.velocity.Y = -4f;
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (TimeSinceLanding < 10 && !WaterMode)
                return base.Colliding(projHitbox, targetHitbox);
            float collisionPoint = 0f;
            if (!WaterMode)
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Bottom, Projectile.Bottom - (Vector2.UnitY * scaleY * 110), scaleX * 40, ref collisionPoint);

            float halfLength = 66f * Projectile.scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + Utils.SafeNormalize(Projectile.velocity.RotatedBy(MathHelper.PiOver2), Vector2.Zero) * halfLength, Projectile.Center - Utils.SafeNormalize(Projectile.velocity.RotatedBy(MathHelper.PiOver2), Vector2.Zero) * halfLength, 40f, ref collisionPoint);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {
            if (Collision.WetCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                WaterMode = true;
                Projectile.velocity.Y *= 0.33f;
            }
            else if (WaterMode)
            {
                Projectile.Kill();
            }

            if (WaterMode)
            {
                Projectile.velocity *= 1.02f;
                Projectile.scale *= 1.01f;
                Projectile.timeLeft--;

                for (int i = 0; i <= 4; i++)
                {
                    Dust waterTrail = Main.dust[Dust.NewDust(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), 23, 0, 33, -3.7f * Math.Sign(Projectile.velocity.X), -6f, 0, new Color(255, 255, 255), Main.rand.NextFloat(1f, 3f))];
                    waterTrail.fadeIn = 3f;
                }

                Dust foamDust = Main.dust[Dust.NewDust(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), 9, 0, 16, 2.7f * Math.Sign(Projectile.velocity.X), -3f, 0, new Color(255, 255, 255), 1.4f)];
                foamDust.noGravity = true;

                if (Main.rand.NextBool())
                {
                    float angle = Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                    float distance = 10f + 50 * Math.Abs(angle) / MathHelper.PiOver2 * Projectile.scale;
                    Vector2 sparkSpeed = -Projectile.velocity;
                    Particle Spark = new CritSpark(Projectile.Center + Vector2.Normalize(Projectile.velocity.RotatedBy(angle)) * distance, sparkSpeed, Color.SkyBlue, Color.CornflowerBlue, 1f + Main.rand.NextFloat(0, 1f), 30, 0.4f, 0.6f);
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
                    Vector2 positionToCheck = Projectile.position;
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
                    Dust waterTrail = Main.dust[Dust.NewDust(Projectile.Top + Vector2.UnitY * groundOffset, 23, 0, 33, -3.7f * Math.Sign(Projectile.velocity.X), -6f, 0, new Color(255, 255, 255), Main.rand.NextFloat(1f, 3f))];
                    waterTrail.fadeIn = 3f;
                    waterTrail = Main.dust[Dust.NewDust(Projectile.TopRight + Vector2.UnitY * groundOffset + Main.rand.NextVector2Circular(16f, 16f) + Vector2.UnitX * Projectile.velocity.X * 4f, 23, 0, 33, -3.7f * Math.Sign(Projectile.velocity.X), -6f, 0, new Color(255, 255, 255), Main.rand.NextFloat(1f, 3f))];
                    waterTrail.fadeIn = 3f;
                    waterTrail = Main.dust[Dust.NewDust(Projectile.TopLeft + Vector2.UnitY * groundOffset + Main.rand.NextVector2Circular(16f, 16f) + Vector2.UnitX * Projectile.velocity.X * 4f, 23, 0, 33, -3.7f * Math.Sign(Projectile.velocity.X), -6f, 0, new Color(255, 255, 255), Main.rand.NextFloat(1f, 3f))];
                    waterTrail.fadeIn = 3f;


                    Dust foamDust = Main.dust[Dust.NewDust(Projectile.Top + Vector2.UnitY * groundOffset + Vector2.UnitX * 16 * Math.Sign(Projectile.velocity.X), 9, 0, 16, 2.7f * Math.Sign(Projectile.velocity.X), -3f, 0, new Color(255, 255, 255), 1.4f)];
                    foamDust.noGravity = true;

                    //Foam
                    float height = scaleY * 100f;
                    Vector2 origin = Projectile.Top + Vector2.UnitY * groundOffset + Main.rand.NextVector2Circular(Projectile.width / 4f, Projectile.height / 4f) - (Vector2.UnitY * height);
                    Particle foamSmall = new FakeGlowDust(origin, Projectile.velocity / 2f, Color.LightSlateGray, Main.rand.NextFloat(1f, 2f), 25, 0.1f, bigSize: true, gravity: (-Projectile.velocity + Vector2.UnitY) * 0.1f);
                    GeneralParticleHandler.SpawnParticle(foamSmall);

                    if (Main.rand.Next(5) == 0)
                    {
                        Particle foamLarge = new MediumMistParticle(origin, Projectile.velocity / 2f, Color.LightSlateGray, Color.LightBlue, Main.rand.NextFloat(1f, 2f), 245 - Main.rand.Next(50), 0.1f);
                        GeneralParticleHandler.SpawnParticle(foamLarge);
                    }
                }
            }

            else
            {
                Particle foamSmall = new FakeGlowDust(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2f, Projectile.height / 2f), Projectile.velocity / 2f, Color.LightSlateGray, Main.rand.NextFloat(0.2f, 2f), 25, 0.1f, gravity: (-Projectile.velocity + Vector2.UnitY) * 0.1f);
                GeneralParticleHandler.SpawnParticle(foamSmall);

                if (Main.rand.NextBool())
                {
                    Particle blob = new WaterGlobParticle(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2f, Projectile.height / 2f), Projectile.velocity / 2f, Main.rand.NextFloat(0.2f, 2f), 0.1f);
                    GeneralParticleHandler.SpawnParticle(blob);
                }

            }

            Projectile.rotation += 0.1f * ((Projectile.velocity.X > 0) ? 1f : -1f);


            if (Projectile.velocity.Y < 15f)
            {
                Projectile.velocity.Y += 0.3f;
            }

            if (Math.Abs(Projectile.velocity.X) < 8)
            {
                Projectile.velocity.X += 0.3f * ((Projectile.velocity.X > 0) ? 1f : -1f);
            }
            else
            {
                Projectile.velocity.X = Math.Sign(Projectile.velocity.X) * 8;
            }

            if (BounceX)
            {
                //Die lmao
                Projectile.Kill();
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter % 6 == 0)
                Projectile.frame = (Projectile.frame + 1) % 3;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Splash with { PitchVariance = 2f}, Projectile.Center);

            for (int i = 0; i < 4; i++)
            {
                Particle foamLarge = new MediumMistParticle(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), Projectile.oldVelocity / 2f, Color.LightSlateGray, Color.LightBlue, Main.rand.NextFloat(1f, 2f), 245 - Main.rand.Next(50), 0.1f);
                GeneralParticleHandler.SpawnParticle(foamLarge);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (WaterMode)
                modifiers.SourceDamage *= TrueBiomeBlade.MarineAttunement_InWaterDamageMultiplier;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (BounceX)
                return false;

            if (WaterMode)
            {
                Texture2D wave = Request<Texture2D>("CalamityMod/Projectiles/Melee/MendedBiomeBlade_GestureForTheDrownedAquaWave").Value;
                float drawRotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
                Vector2 drawOrigin = new Vector2(wave.Width / 2f, wave.Height / 2f);
                Vector2 drawOffset = Projectile.Center - Main.screenPosition;

                Main.EntitySpriteDraw(wave, drawOffset - Projectile.velocity, null, Color.Lerp(lightColor, Color.White, 0.5f) * 0.2f, drawRotation, drawOrigin, Projectile.scale - 0.2f, 0f, 0);
                Main.EntitySpriteDraw(wave, drawOffset, null, Color.Lerp(lightColor, Color.White, 0.5f) * 0.5f, drawRotation, drawOrigin, Projectile.scale, 0f, 0);
                return false;
            }


            if (HasLanded == 1f && TimeSinceLanding > 10f)
            {
                Texture2D wave = Request<Texture2D>("CalamityMod/Projectiles/Melee/MendedBiomeBlade_GestureForTheDrownedWave").Value;
                Rectangle frame = new Rectangle(0, 0 + 110 * Projectile.frame, 40, 110);

                float drawRotation = 0f;
                Vector2 drawOrigin = new Vector2(0f, frame.Height);


                Vector2 drawOffset;

                //Wobble
                Vector2 Scale;

                SpriteEffects flip = Projectile.velocity.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                for (int i = 2; i >= 0; i--)
                {
                    drawOffset = Projectile.position + Vector2.UnitY * groundOffset + Vector2.UnitY / 2f - Main.screenPosition;
                    drawOffset -= Vector2.UnitX * 10f * i * Math.Sign(Projectile.velocity.X);
                    float ScaleYAlter = (1 + 0.4f * (1f + (float)Math.Sin((TimeSinceLanding - 10f) * 0.1f + 0.2 * i)) * 0.5f) * MathHelper.Clamp((TimeSinceLanding - 10) / 30f, 0f, 1f) * MathHelper.Clamp(Projectile.timeLeft / 30f, 0f, 1f);
                    Scale = new Vector2(scaleX, ScaleYAlter - 0.05f * i);
                    float slightfade = 1f - 0.24f * i;
                    Color darkenedColor = Color.Lerp(lightColor, Color.Black, i * 0.15f);

                    Main.EntitySpriteDraw(wave, drawOffset, frame, darkenedColor * MathHelper.Clamp(TimeSinceLanding / 30f, 0f, 1f) * slightfade, drawRotation, drawOrigin, Scale, flip, 0);
                }



                return false;
            }

            else
            {
                Texture2D ball = Request<Texture2D>("CalamityMod/Projectiles/Melee/MendedBiomeBlade_GestureForTheDrowned").Value;
                float drawRotation = Projectile.rotation;
                Vector2 drawOrigin = Projectile.Size / 2f;
                Vector2 drawOffset = Projectile.Center - Main.screenPosition;

                Main.EntitySpriteDraw(ball, drawOffset, null, lightColor * MathHelper.Clamp(1 - TimeSinceLanding / 10f, 0f, 1f), drawRotation, drawOrigin, Projectile.scale, 0f, 0);

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
