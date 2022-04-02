using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class AresTeslaOrb : ModProjectile
    {
        public ref float Identity => ref projectile.ai[0];
        public PrimitiveTrail LightningDrawer;
        public PrimitiveTrail LightningBackgroundDrawer;
        private const int timeLeft = 480;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tesla Sphere");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.Opacity = 0f;
            cooldownSlot = 1;
            projectile.timeLeft = timeLeft;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            bool deathrayPhase = false;
            if (CalamityGlobalNPC.draedonExoMechPrime != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
                    deathrayPhase = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Calamity().newAI[0] == (float)AresBody.Phase.Deathrays;
            }

            if (projectile.velocity.Length() < (deathrayPhase ? 10.4f : 20.8f))
                projectile.velocity *= 1.01f;

            int fadeOutTime = 15;
            int fadeInTime = 3;
            if (projectile.timeLeft < fadeOutTime)
                projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / (float)fadeOutTime, 0f, 1f);
            else
                projectile.Opacity = MathHelper.Clamp(1f - ((projectile.timeLeft - (timeLeft - fadeInTime)) / (float)fadeInTime), 0f, 1f);

            Lighting.AddLight(projectile.Center, 0.1f * projectile.Opacity, 0.25f * projectile.Opacity, 0.25f * projectile.Opacity);

            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;

                float speed1 = 1.8f;
                float speed2 = 2.8f;
                float angleRandom = 0.35f;

                for (int num53 = 0; num53 < 40; num53++)
                {
                    float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2f * angleRandom);
                    int randomDustType = Main.rand.Next(2) == 0 ? 206 : 229;
                    float scale = randomDustType == 206 ? 1.5f : 1f;

                    int num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 200, default, 2.5f * scale);
                    Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;
                    Main.dust[num54].noGravity = true;

                    Dust dust = Main.dust[num54];
                    dust.velocity *= 3f;

                    num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 100, default, 1.5f * scale);
                    Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;

                    dust = Main.dust[num54];
                    dust.velocity *= 2f;

                    Main.dust[num54].noGravity = true;
                    Main.dust[num54].fadeIn = 1f;
                    Main.dust[num54].color = Color.Cyan * 0.5f;
                }
                for (int num55 = 0; num55 < 20; num55++)
                {
                    float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                    Vector2 dustVel = new Vector2(dustSpeed, 0f).RotatedBy(projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2f * angleRandom);
                    int randomDustType = Main.rand.Next(2) == 0 ? 206 : 229;
                    float scale = randomDustType == 206 ? 1.5f : 1f;

                    int num56 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 0, default, 3f * scale);
                    Main.dust[num56].position = projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(projectile.velocity.ToRotation()) * projectile.width / 3f;
                    Main.dust[num56].noGravity = true;

                    Dust dust = Main.dust[num56];
                    dust.velocity *= 0.5f;
                }
            }
        }

        public override bool CanHitPlayer(Player target) => projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.Opacity != 1f)
                return;

            target.AddBuff(BuffID.Electrified, 240);
        }

        public Projectile GetOrbToAttachTo()
        {
            if (CalamityGlobalNPC.draedonExoMechPrime < 0 || !Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
                return null;

            // Difficulty modes
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            float detachDistance = malice ? 1600f : death ? 1360f : revenge ? 1280f : expertMode ? 1200f : 960f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type != projectile.type || Main.projectile[i].ai[0] != Identity + 1f || !Main.projectile[i].active || Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Calamity().newAI[0] == (float)AresBody.Phase.Deathrays)
                    continue;

                if (Vector2.Distance(projectile.Center, Main.projectile[i].Center) > detachDistance)
                    continue;

                return Main.projectile[i];
            }

            return null;
        }

        public static List<Vector2> DetermineElectricArcPoints(Vector2 start, Vector2 end, int seed)
        {
            List<Vector2> points = new List<Vector2>();

            // Determine the base points based on a linear path from the start the end end point.
            for (int i = 0; i <= 75; i++)
                points.Add(Vector2.Lerp(start, end, i / 73.5f));

            // Then, add continuous randomness to the positions of various points.
            for (int i = 0; i < points.Count; i++)
            {
                float completionRatio = i / (float)points.Count;

                // Noise offsets should taper off at the ends of the line.
                float offsetMuffleFactor = Utils.InverseLerp(0.12f, 0.25f, completionRatio, true) * Utils.InverseLerp(0.88f, 0.75f, completionRatio, true);

                // Give a sense of time for the noise on the vertical axis. This is achieved via a 0-1 constricted sinusoid.
                float noiseY = (float)Math.Cos(completionRatio * 17.2f + Main.GlobalTime * 10.7f) * 0.5f + 0.5f;

                float noise = CalamityUtils.PerlinNoise2D(completionRatio, noiseY, 2, seed);

                // Now that the noise value has been computed, convert it to a direction by treating the noise as an angle
                // and then converting it into a unit vector.
                Vector2 offsetDirection = (noise * MathHelper.Pi * 0.7f).ToRotationVector2();

                // Then, determine the factor of the offset. This is based on the initial direction (but squashed) and the muffle factor from above.
                Vector2 offset = offsetDirection * (float)Math.Pow(offsetDirection.Y, 2D) * offsetMuffleFactor * 15f;

                points[i] += offset;
            }

            return points;
        }

        internal float WidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(0.75f, 1.85f, (float)Math.Sin(MathHelper.Pi * completionRatio)) * projectile.scale;
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeToWhite = MathHelper.Lerp(0f, 0.65f, (float)Math.Sin(MathHelper.TwoPi * completionRatio + Main.GlobalTime * 4f) * 0.5f + 0.5f);
            Color baseColor = Color.Lerp(Color.Cyan, Color.White, fadeToWhite);
            return Color.Lerp(baseColor, Color.LightBlue, ((float)Math.Sin(MathHelper.Pi * completionRatio + Main.GlobalTime * 4f) * 0.5f + 0.5f) * 0.8f) * projectile.Opacity;
        }

        internal float BackgroundWidthFunction(float completionRatio) => WidthFunction(completionRatio) * 4f;

        internal Color BackgroundColorFunction(float completionRatio)
        {
            Color color = Color.CornflowerBlue * projectile.Opacity * 0.4f;
            return color;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (LightningDrawer is null)
                LightningDrawer = new PrimitiveTrail(WidthFunction, ColorFunction, PrimitiveTrail.RigidPointRetreivalFunction);
            if (LightningBackgroundDrawer is null)
                LightningBackgroundDrawer = new PrimitiveTrail(BackgroundWidthFunction, BackgroundColorFunction, PrimitiveTrail.RigidPointRetreivalFunction);

            Projectile orbToAttachTo = GetOrbToAttachTo();
            if (orbToAttachTo != null)
            {
                List<Vector2> arcPoints = DetermineElectricArcPoints(projectile.Center, orbToAttachTo.Center, 117);
                LightningBackgroundDrawer.Draw(arcPoints, -Main.screenPosition, 90);
                LightningDrawer.Draw(arcPoints, -Main.screenPosition, 90);
            }

            lightColor.R = (byte)(255 * projectile.Opacity);
            lightColor.G = (byte)(255 * projectile.Opacity);
            lightColor.B = (byte)(255 * projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;

            float _ = 0f;
            Projectile orbToAttachTo = GetOrbToAttachTo();
            if (orbToAttachTo != null && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, orbToAttachTo.Center, 8f, ref _))
                return true;

            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
