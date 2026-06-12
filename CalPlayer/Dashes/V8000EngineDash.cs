using System;
using CalamityMod.Dusts;
using CalamityMod.Enums;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.Dashes
{
    public class V8000EngineDash : PlayerDashEffect
    {
        public static new string ID { get; private set; }

        public override DashCollisionType CollisionType => DashCollisionType.ShieldSlam;

        public override bool IsOmnidirectional => false;
        public int Time = 0;

        public override void Load()
        {
            ID = DashID;
        }

        public override float CalculateDashSpeed(Player player) => 16f;

        public override void DashStartupEffects(Player player)
        {
            player.velocity *= 0.9f;
        }

        public override void OnDashEffects(Player player)
        {
            Time = 0;

            // Burst of evenly spaced but random velocity dusts
            for (int m = 0; m < 6; m++)
            {
                float fxFade = Utils.GetLerpValue(5, 15, Math.Abs(player.velocity.X), true);

                Vector2 trailPos = player.Center - (player.velocity * 2) + new Vector2(0, (18 - (m * 6)));
                float trailScale = 1 * fxFade;
                Color trailColor = Main.rand.NextBool(3) ? Color.DodgerBlue : Color.LightSkyBlue;

                int dustStyle = ModContent.DustType<SquashDust>();
                Dust dust2 = Dust.NewDustPerfect(trailPos, dustStyle, -player.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(6, 21));
                dust2.scale = Main.rand.NextFloat(0.8f, 1.4f);
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

                ThrusterParticle particle = new ThrusterParticle(player, dir, 16, 120f * sizeFactor, 1);
                GeneralParticleHandler.SpawnParticle(particle, false, GeneralDrawLayer.BeforeProjectiles);
            }

            SoundStyle sound = V8000Engine.DashSound;
            SoundEngine.PlaySound(sound with { Volume = 0.5f, Pitch = Main.rand.NextFloat(-0.4f, -0.3f) }, player.Center);
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            Time++; // For VFX

            if (Time % 2 == 0 && Time < 10)
            {
                // Angled, consistently placed sparks
                Vector2 dustVel = -player.velocity.RotatedBy(0.05f + MathHelper.Clamp(Time * 0.03f, 0, 0.55f)) * 0.75f;
                Vector2 dustVel2 = -player.velocity.RotatedBy(-0.05f - MathHelper.Clamp(Time * 0.03f, 0, 0.55f)) * 0.75f;

                PointParticle spark = new PointParticle(player.Center + new Vector2(0, -15 * player.direction) + dustVel, dustVel, false, 8, 1.25f, (Main.rand.NextBool() ? Color.DodgerBlue : Color.LightSkyBlue) * 0.66f);
                GeneralParticleHandler.SpawnParticle(spark);
                PointParticle spark2 = new PointParticle(player.Center + new Vector2(0, 15 * player.direction) + dustVel2, dustVel2, false, 8, 1.25f, (Main.rand.NextBool() ? Color.DodgerBlue : Color.LightSkyBlue) * 0.66f);
                GeneralParticleHandler.SpawnParticle(spark2);

                // Glow beam
                float fadeInLerp = Utils.GetLerpValue(0, 12, Time, true);
                Color primaryColor = Color.Lerp(Color.DodgerBlue, Color.LightSkyBlue, fadeInLerp);
                Particle beamBody = new CustomSpark(player.Center, -player.velocity * 0.15f, "CalamityMod/Particles/BloomCircle", false, 7, 0.65f - 0.3f * fadeInLerp, primaryColor * 0.8f, new Vector2(1.1f - 0.4f * fadeInLerp, 0.8f + 0.6f * fadeInLerp), true, true, shrinkSpeed: 0.6f);
                GeneralParticleHandler.SpawnParticle(beamBody);
                Particle beamCore = new CustomSpark(player.Center, -player.velocity * 0.15f, "CalamityMod/Particles/BloomCircle", false, 5, 0.35f - 0.15f * fadeInLerp, Color.Lerp(primaryColor, Color.White, 0.6f), new Vector2(0.5f, 1.4f), true, true, shrinkSpeed: 0.6f);
                GeneralParticleHandler.SpawnParticle(beamCore);

                // Horizontal dust
                float fxFade = Utils.GetLerpValue(5, 15, Math.Abs(player.velocity.X), true);
                Vector2 trailPos = player.Center - (player.velocity * 2) + Main.rand.NextVector2Circular(10, 20);
                float trailScale = 1 * fxFade;
                Color trailColor = Main.rand.NextBool(3) ? Color.DodgerBlue : Color.LightSkyBlue;

                int dustStyle = ModContent.DustType<SquashDust>();
                Dust dust2 = Dust.NewDustPerfect(trailPos, dustStyle, -player.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(6, 21));
                dust2.scale = Main.rand.NextFloat(0.8f, 1.4f);
                dust2.color = trailColor;
                dust2.noGravity = true;
                dust2.fadeIn = 0.5f;
            }

            // SHPS-esque souls
            if (Time % 4 == 0)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Vector2 spawnPos = player.Center + Main.rand.NextVector2Circular(10, 10);
                    Vector2 trailVel = -player.velocity * 0.6f + Main.rand.NextVector2Circular(3f, 3f);

                    Projectile.NewProjectile(player.GetSource_FromThis(), spawnPos, trailVel, ModContent.ProjectileType<V8000SoulVisual>(), 0, 0, player.whoAmI);
                }
            }

            dashSpeed = 14f;
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            SoundStyle hit = new("CalamityMod/Sounds/Item/DoomsdayDeviceImpact");
            SoundEngine.PlaySound(hit with { Pitch = -0.2f, Volume = 0.4f }, player.Center);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Volume = 1f, Pitch = -0.2f }, player.Center);
            SoundEngine.PlaySound(SoundID.Item74 with { Volume = 0.7f, Pitch = 1f }, player.Center);

            for (int i = 0; i <= 6; i++)
            {
                Dust dust = Dust.NewDustPerfect(player.Center, Main.rand.NextBool() ? 278 : 132, player.velocity.RotatedByRandom(0.7f) * Main.rand.NextFloat(0.5f, 1f) + new Vector2(0, -3f));
                if (dust.type == 278)
                {
                    dust.scale = 1.2f;
                    dust.color = Main.rand.NextBool() ? Color.DodgerBlue : Color.LightSkyBlue;
                }
                else
                {
                    dust.scale = 0.9f;
                }
                dust.noGravity = false;
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
            }

            Particle pulse = new DirectionalPulseRing(npc.Center, Vector2.Zero, Color.DodgerBlue * 0.8f, new Vector2(2f, 2f), 0, 0.05f, 1f, 24);
            GeneralParticleHandler.SpawnParticle(pulse);

            for (int i = 0; i < 14; i++)
            {
                Vector2 velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3f, 7f);
                Color dustColor = Color.Lerp(Color.DodgerBlue, Color.White, Main.rand.NextFloat());

                float scale = Main.rand.NextFloat(0.25f, .6f);
                int lifetime = Main.rand.Next(30, 50);

                SquishyLightParticle plasma = new(player.Center, velocity, scale, dustColor, lifetime);
                GeneralParticleHandler.SpawnParticle(plasma);
            }

            // Define hit context variables.
            int hitDirection = player.direction;
            if (player.velocity.X != 0f)
                hitDirection = Math.Sign(player.velocity.X);
            hitContext.HitDirection = hitDirection;
            hitContext.PlayerImmunityFrames = V8000Engine.ShieldSlamIFrames;

            // Define damage parameters.
            hitContext.damageClass = DamageClass.Melee;
            hitContext.BaseDamage = V8000Engine.ShieldSlamDamage;
            hitContext.BaseKnockback = V8000Engine.ShieldSlamKnockback;
        }
    }
}
