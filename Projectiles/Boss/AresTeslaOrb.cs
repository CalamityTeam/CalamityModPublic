using CalamityMod.Events;
using CalamityMod.Graphics.Primitives;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class AresTeslaOrb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public ref float Identity => ref Projectile.ai[0];
        private const int timeLeft = 480;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.Opacity = 0f;
            CooldownSlot = ImmunityCooldownID.Bosses;
            Projectile.timeLeft = timeLeft;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            bool deathrayPhase = false;
            if (CalamityGlobalNPC.draedonExoMechPrime != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
                    deathrayPhase = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Calamity().newAI[0] == (float)AresBody.Phase.Deathrays;
            }

            if (Projectile.velocity.Length() < (deathrayPhase ? 10.4f : 20.8f))
                Projectile.velocity *= 1.01f;

            int fadeOutTime = 15;
            int fadeInTime = 3;
            if (Projectile.timeLeft < fadeOutTime)
                Projectile.Opacity = MathHelper.Clamp(Projectile.timeLeft / (float)fadeOutTime, 0f, 1f);
            else
                Projectile.Opacity = MathHelper.Clamp(1f - ((Projectile.timeLeft - (timeLeft - fadeInTime)) / (float)fadeInTime), 0f, 1f);

            Lighting.AddLight(Projectile.Center, 0.1f * Projectile.Opacity, 0.25f * Projectile.Opacity, 0.25f * Projectile.Opacity);

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;

                float speed1 = 1.8f;
                float speed2 = 2.8f;
                float angleRandom = 0.35f;

                for (int i = 0; i < 40; i++)
                {
                    float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2f * angleRandom);
                    int randomDustType = Main.rand.Next(2) == 0 ? 206 : 229;
                    float scale = randomDustType == 206 ? 1.5f : 1f;

                    int teslaDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 200, default, 2.5f * scale);
                    Main.dust[teslaDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                    Main.dust[teslaDust].noGravity = true;

                    Dust dust = Main.dust[teslaDust];
                    dust.velocity *= 3f;

                    teslaDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 100, default, 1.5f * scale);
                    Main.dust[teslaDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;

                    dust = Main.dust[teslaDust];
                    dust.velocity *= 2f;

                    Main.dust[teslaDust].noGravity = true;
                    Main.dust[teslaDust].fadeIn = 1f;
                    Main.dust[teslaDust].color = Color.Cyan * 0.5f;
                }
                for (int j = 0; j < 20; j++)
                {
                    float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                    Vector2 dustVel = new Vector2(dustSpeed, 0f).RotatedBy(Projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2f * angleRandom);
                    int randomDustType = Main.rand.Next(2) == 0 ? 206 : 229;
                    float scale = randomDustType == 206 ? 1.5f : 1f;

                    int teslaDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 0, default, 3f * scale);
                    Main.dust[teslaDust2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 3f;
                    Main.dust[teslaDust2].noGravity = true;

                    Dust dust = Main.dust[teslaDust2];
                    dust.velocity *= 0.5f;
                }
            }
        }

        public override bool CanHitPlayer(Player target) => Projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Projectile.Opacity != 1f)
                return;

            target.AddBuff(BuffID.Electrified, 240);
        }

        public Projectile GetOrbToAttachTo()
        {
            if (CalamityGlobalNPC.draedonExoMechPrime < 0 || !Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
                return null;

            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            float detachDistance = bossRush ? 1600f : death ? 1360f : revenge ? 1280f : expertMode ? 1200f : 960f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type != Projectile.type || Main.projectile[i].ai[0] != Identity + 1f || !Main.projectile[i].active || Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Calamity().newAI[0] == (float)AresBody.Phase.Deathrays)
                    continue;

                if (Vector2.Distance(Projectile.Center, Main.projectile[i].Center) > detachDistance)
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
                float offsetMuffleFactor = Utils.GetLerpValue(0.12f, 0.25f, completionRatio, true) * Utils.GetLerpValue(0.88f, 0.75f, completionRatio, true);

                // Give a sense of time for the noise on the vertical axis. This is achieved via a 0-1 constricted sinusoid.
                float noiseY = (float)Math.Cos(completionRatio * 17.2f + Main.GlobalTimeWrappedHourly * 10.7f) * 0.5f + 0.5f;

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
            return MathHelper.Lerp(0.75f, 1.85f, (float)Math.Sin(MathHelper.Pi * completionRatio)) * Projectile.scale;
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeToWhite = MathHelper.Lerp(0f, 0.65f, (float)Math.Sin(MathHelper.TwoPi * completionRatio + Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f);
            Color baseColor = Color.Lerp(Color.Cyan, Color.White, fadeToWhite);
            return Color.Lerp(baseColor, Color.LightBlue, ((float)Math.Sin(MathHelper.Pi * completionRatio + Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f) * 0.8f) * Projectile.Opacity;
        }

        internal float BackgroundWidthFunction(float completionRatio) => WidthFunction(completionRatio) * 4f;

        internal Color BackgroundColorFunction(float completionRatio)
        {
            Color color = Color.CornflowerBlue * Projectile.Opacity * 0.4f;
            return color;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile orbToAttachTo = GetOrbToAttachTo();
            if (orbToAttachTo != null)
            {
                List<Vector2> arcPoints = DetermineElectricArcPoints(Projectile.Center, orbToAttachTo.Center, 117);
                PrimitiveSet.Prepare(arcPoints, new(BackgroundWidthFunction, BackgroundColorFunction, smoothen: false), 90);
                PrimitiveSet.Prepare(arcPoints, new(WidthFunction, ColorFunction, smoothen: false), 90);
            }

            lightColor.R = (byte)(255 * Projectile.Opacity);
            lightColor.G = (byte)(255 * Projectile.Opacity);
            lightColor.B = (byte)(255 * Projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;

            float _ = 0f;
            Projectile orbToAttachTo = GetOrbToAttachTo();
            if (orbToAttachTo != null && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, orbToAttachTo.Center, 8f, ref _))
                return true;

            return false;
        }
    }
}
