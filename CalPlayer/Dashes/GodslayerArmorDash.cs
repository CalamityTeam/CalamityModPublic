using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Enums;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.GodSlayer;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.Dashes
{
    public class GodslayerArmorDash : PlayerDashEffect
    {
        public static new string ID => "Godslayer Armor";

        public static int GodslayerCooldown = 45;

        public SlotId GSDashSlot;

        public static readonly SoundStyle Impact = new("CalamityMod/Sounds/NPCKilled/DevourerDeathImpact") { Volume = 0.5f };

        public override DashCollisionType CollisionType => DashCollisionType.ShieldSlam;

        public override bool IsOmnidirectional => true;
        
        public int Time = 0;
        public float Size = 2.2f;
        public bool SoundOnce = true;

        public override float CalculateDashSpeed(Player player) => 80f;

        public override void OnDashEffects(Player player)
        {
            Time = 0;
            Size = 2.2f;
            GSDashSlot = SoundEngine.PlaySound(DevourerofGodsHead.DeathAnimationSound, player.Center);
            SoundOnce = true;

            Particle pulse = new DirectionalPulseRing(player.Center, Vector2.Zero, Color.Orchid, new Vector2(2f, 2f), Main.rand.NextFloat(12f, 25f), 0.1f, 12f, 18);
            GeneralParticleHandler.SpawnParticle(pulse);

            for (int i = 0; i <= 15; i++)
            {
                Dust dust = Dust.NewDustPerfect(player.position, 181, -player.velocity.RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.3f, 0.9f), 0, default, Main.rand.NextFloat(3.1f, 3.9f));
                dust.noGravity = false;
            }
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            if (SoundEngine.TryGetActiveSound(GSDashSlot, out var Dashsound) && Dashsound.IsPlaying)
                Dashsound.Position = player.Center;

            Time++;
            Size -= 0.04f;

            // Fall way, way, faster than usual.
            player.maxFallSpeed = 50f;
            if (Time < 20)
            {
                Particle jaws = new Jaws(player.Center + player.velocity * 0.5f, player.velocity, Color.Fuchsia, new Vector2(0.8f, 1f), player.velocity.ToRotation() + MathHelper.PiOver2, Size, Size, 2);
                GeneralParticleHandler.SpawnParticle(jaws);
                Particle jaws2 = new Jaws(player.Center + player.velocity * 0.45f, player.velocity, Color.Aqua, new Vector2(0.8f, 1f), player.velocity.ToRotation() + MathHelper.PiOver2, Size - 0.3f, Size - 0.3f, 2);
                GeneralParticleHandler.SpawnParticle(jaws2);
            }

            float radiusFactor = MathHelper.Lerp(0f, 1f, Utils.GetLerpValue(2f, 2.5f, Time, true));
            for (int i = 0; i < 9; i++)
            {
                float offsetRotationAngle = player.velocity.ToRotation() + Time / 20f;
                float radius = (30f + (float)Math.Cos(Time / 3f) * 24f) * radiusFactor;
                Vector2 dustPosition = player.Center + player.velocity * 0.8f;
                dustPosition += offsetRotationAngle.ToRotationVector2().RotatedBy(i / 5f * MathHelper.TwoPi) * radius;
                Dust dust = Dust.NewDustPerfect(dustPosition, Main.rand.NextBool(5) ? 181 : 295);
                dust.noGravity = true;
                dust.velocity = player.velocity * 0.5f;
                dust.scale = Main.rand.NextFloat(2.7f, 3.0f);
                Dust dust2 = Dust.NewDustPerfect(player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) + (player.velocity * 0.5f), Main.rand.NextBool(14) ? 180 : 295, -player.velocity.RotatedByRandom(MathHelper.ToRadians(30f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(2.7f, 3.9f));
                dust2.noGravity = true;
            }

            float sparkscale = Size * 1.3f;
            Vector2 SparkVelocity1 = player.velocity.RotatedBy(player.direction * -4, default) * 0.08f - player.velocity / 2f;
            SparkParticle spark = new SparkParticle(player.Center + player.velocity.RotatedBy(2f * player.direction) * 1.2f, SparkVelocity1, false, Main.rand.Next(11, 13), sparkscale, Main.rand.NextBool(3) ? Color.Aqua : Color.Fuchsia);
            GeneralParticleHandler.SpawnParticle(spark);
            Vector2 SparkVelocity2 = player.velocity.RotatedBy(player.direction * 4, default) * 0.08f - player.velocity / 2f;
            SparkParticle spark2 = new SparkParticle(player.Center + player.velocity.RotatedBy(-2f * player.direction) * 1.2f, SparkVelocity2, false, Main.rand.Next(11, 13), sparkscale, Main.rand.NextBool(3) ? Color.Aqua : Color.Fuchsia);
            GeneralParticleHandler.SpawnParticle(spark2);

            if (Time > 20 && Time < 100)
            {
                Particle pulse = new DirectionalPulseRing(player.Center - player.velocity * 0.52f, player.velocity / 1.5f, Color.Fuchsia, new Vector2(1f, 2f), player.velocity.ToRotation(), 0.82f, 0.32f, 60);
                GeneralParticleHandler.SpawnParticle(pulse);
                Particle pulse2 = new DirectionalPulseRing(player.Center - player.velocity * 0.40f, player.velocity / 1.5f * 0.9f, Color.Aqua, new Vector2(0.8f, 1.5f), player.velocity.ToRotation(), 0.58f, 0.28f, 50);
                GeneralParticleHandler.SpawnParticle(pulse2);
                Time = 111;
            }

            // Dash at a much, much faster speed than the default value.
            dashSpeed = 40f;
            runSpeedDecelerationFactor = 0.8f;

            // Cooldown for God Slayer Armor dash.
            player.AddCooldown(Cooldowns.GodSlayerDash.ID, CalamityUtils.SecondsToFrames(GodslayerCooldown));
            player.Calamity().godSlayerDashHotKeyPressed = false;
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            if (SoundOnce)
            {
                SoundEngine.PlaySound(DevourerofGodsHead.DeathExplosionSound, player.Center);
                SoundOnce = false;
            }

            for (int i = 0; i <= 25; i++)
            {
                Dust dust = Dust.NewDustPerfect(player.position, Main.rand.NextBool(3) ? 226 : 272, player.velocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.1f, 0.5f), 0, default, Main.rand.NextFloat(2.1f, 2.9f));
                dust.noGravity = false;
            }

            // Define hit context variables.
            int hitDirection = player.direction;
            if (player.velocity.X != 0f)
                hitDirection = Math.Sign(player.velocity.X);
            hitContext.HitDirection = hitDirection;
            hitContext.PlayerImmunityFrames = AsgardianAegis.ShieldSlamIFrames;

            // Define damage parameters.
            int dashDamage = 3000;
            hitContext.damageClass = player.GetBestClass();
            hitContext.BaseDamage = player.ApplyArmorAccDamageBonusesTo(dashDamage);
            hitContext.BaseKnockback = 15f;

            // God Slayer Dash intentionally does not use the vanilla function for collision attack iframes.
            // This is because its immunity is meant to be completely consistent and not subject to vanilla anticheese.
            hitContext.PlayerImmunityFrames = GodSlayerChestplate.DashIFrames;

            npc.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
        }
    }
}
