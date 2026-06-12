using System;
using CalamityMod.Dusts;
using CalamityMod.Enums;
using CalamityMod.Graphics.Renderers;
using CalamityMod.Items.Accessories;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Items;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.Dashes
{
    public class V8EngineDash : PlayerDashEffect
    {
        public static new string ID { get; private set; }

        public override DashCollisionType CollisionType => DashCollisionType.NoCollision;

        public override bool IsOmnidirectional => false;
        public int Time = 0;

        public override void Load()
        {
            ID = DashID;
        }

        public override float CalculateDashSpeed(Player player) => 6.5f;

        public override void OnDashEffects(Player player)
        {
            Time = 0;

            // Burst of evenly spaced but random velocity dusts
            for (int m = 0; m < 6; m++)
            {
                float fxFade = Utils.GetLerpValue(5, 15, Math.Abs(player.velocity.X), true);

                Vector2 trailPos = player.Center - (player.velocity * 2) + new Vector2(0, (18 - (m * 6)));
                float trailScale = 1 * fxFade;
                Color trailColor = Main.rand.NextBool(3) ? Color.Firebrick : Color.OrangeRed;

                int dustStyle = ModContent.DustType<SquashDust>();
                Dust dust2 = Dust.NewDustPerfect(trailPos, dustStyle, -player.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(5, 16));
                dust2.scale = Main.rand.NextFloat(0.8f, 1.3f);
                dust2.color = trailColor;
                dust2.noGravity = true;
                dust2.fadeIn = 0.5f;
            }

            // Thruster prim
            int beamCount = 3;
            float maxAngle = MathHelper.ToRadians(10f);

            Vector2 baseDir = -player.velocity.SafeNormalize(Vector2.UnitX);

            for (int i = -1; i < beamCount; i++)
            {
                float t = i / (float)(beamCount - 1);

                float centered = t * 2f - 1f;

                float angleOffset = centered * maxAngle;

                Vector2 dir = baseDir.RotatedBy(angleOffset);

                float sizeFactor = 1f - Math.Abs(centered) * 0.7f;

                ThrusterParticle particle = new ThrusterParticle(player, dir, 12, 100f * sizeFactor, 0); 
                GeneralParticleHandler.SpawnParticle(particle, false, GeneralDrawLayer.BeforeProjectiles);
            }

            SoundStyle sound = V8Engine.DashSound;
            SoundEngine.PlaySound(sound with { Volume = 0.5f, Pitch = Main.rand.NextFloat(-0.4f, -0.3f) }, player.Center);
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            player.velocity.X *= 0.925f;
            Dust hFlameDust = Dust.NewDustPerfect(player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) - (player.velocity * 5f), DustID.HeatRay, -player.velocity.RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, Color.Firebrick, Main.rand.NextFloat(1.8f, 2.8f));

            // Angled, consistently placed sparks
            Vector2 dustVel = -player.velocity.RotatedBy(0.05f + MathHelper.Clamp(Time * 0.03f, 0, 0.55f)) * 0.75f;
            Vector2 dustVel2 = -player.velocity.RotatedBy(-0.05f - MathHelper.Clamp(Time * 0.03f, 0, 0.55f)) * 0.75f;

            PointParticle spark = new PointParticle(player.Center + new Vector2(0, -15 * player.direction) + dustVel, dustVel, false, 8, 1.25f, (Main.rand.NextBool() ? Color.Firebrick : Color.OrangeRed) * 0.66f);
            GeneralParticleHandler.SpawnParticle(spark);
            PointParticle spark2 = new PointParticle(player.Center + new Vector2(0, 15 * player.direction) + dustVel2, dustVel2, false, 8, 1.25f, (Main.rand.NextBool() ? Color.Firebrick : Color.OrangeRed) * 0.66f);
            GeneralParticleHandler.SpawnParticle(spark2);

            // Glow beam
            float fadeInLerp = Utils.GetLerpValue(0, 12, Time, true);
            Color primaryColor = Color.Lerp(Color.Firebrick, Color.OrangeRed, fadeInLerp);
            Particle beamBody = new CustomSpark(player.Center, -player.velocity * 0.15f, "CalamityMod/Particles/BloomCircle", false, 7, 0.65f - 0.3f * fadeInLerp, primaryColor * 0.8f, new Vector2(1.1f - 0.4f * fadeInLerp, 0.8f + 0.6f * fadeInLerp), true, false, shrinkSpeed: 0.6f);
            GeneralParticleHandler.SpawnParticle(beamBody);
            Particle beamCore = new CustomSpark(player.Center, -player.velocity * 0.15f, "CalamityMod/Particles/BloomCircle", false, 5, 0.35f - 0.15f * fadeInLerp, Color.Lerp(primaryColor, Color.White, 0.6f), new Vector2(0.5f, 1.2f), true, false, shrinkSpeed: 0.6f);
            GeneralParticleHandler.SpawnParticle(beamCore);


            for (int i = 0; i < 2; i++)
            {
                // Fruit loop particles, also used by Elysian Aegis
                Dust dust = Dust.NewDustPerfect(player.Center + Main.rand.NextVector2Circular(6, 6) - player.velocity * 2.2f, DustID.GoldFlame);
                dust.velocity = -player.velocity * Main.rand.NextFloat(0.66f, 1.8f);
                dust.scale = Main.rand.NextFloat(0.4f, 1.3f);
                dust.noGravity = true;
                Particle sparkler = new CustomSpark(player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) - (player.velocity * 1.2f), -player.velocity.RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.1f, 0.8f), "CalamityMod/Particles/ProvidenceMarkParticle", false, 17, Main.rand.NextFloat(1.15f, 1.25f), Main.rand.NextBool(4) ? Color.Khaki : Color.Orange, new Vector2(1.3f, 0.5f), true, false, 0, false, false, Main.rand.NextFloat(0.4f, 0.5f));
                GeneralParticleHandler.SpawnParticle(sparkler);
            }
        }
    }
}
