using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Enums;
using CalamityMod.Items.Accessories;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.Dashes
{
    public class AsgardianAegisDash : PlayerDashEffect
    {
        public static new string ID => "Asgardian Aegis";

        public override DashCollisionType CollisionType => DashCollisionType.ShieldSlam;
        public override bool IsOmnidirectional => false;
        public int Time = 0;
        public bool PostHit = false;

        public override float CalculateDashSpeed(Player player) => 23.3f;

        public override void OnDashEffects(Player player)
        {
            Time = 0;
            PostHit = false;
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            Time += 2;
            float radiusFactor = MathHelper.Lerp(0f, 1f, Utils.GetLerpValue(2f, 2.5f, Time, true));
            for (int i = 0; i < 3; i++)
            {
                float offsetRotationAngle = player.velocity.ToRotation() + Time / 5f;
                float radius = (15f + (float)Math.Cos(Time / 3f) * 12f) * radiusFactor;
                Vector2 dustPosition = player.Center - player.velocity * 2;
                dustPosition += offsetRotationAngle.ToRotationVector2().RotatedBy(i / 5f * MathHelper.TwoPi) * radius;
                Dust dust = Dust.NewDustPerfect(dustPosition, Main.rand.NextBool(5) ? 181 : 295);
                dust.alpha = 220;
                dust.noGravity = true;
                dust.velocity = player.velocity * 0.8f;
                dust.scale = Main.rand.NextFloat(1.7f, 2.0f);
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
                Dust dust2 = Dust.NewDustPerfect(player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) + (player.velocity * 1.5f), Main.rand.NextBool(8) ? 180 : 295, -player.velocity.RotatedByRandom(MathHelper.ToRadians(30f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(1.7f, 1.9f));
                dust2.alpha = 170;
                dust2.noGravity = true;
                dust2.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
            }

            // Dash at a faster speed than the default value.
            dashSpeed = 16f;
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            if (!PostHit)
            {
                player.Calamity().GeneralScreenShakePower = 5f;
                PostHit = true;
            }

            Particle pulse = new DirectionalPulseRing(npc.Center, Vector2.Zero, Color.Aqua, new Vector2(2f, 2f), 0, 0.1f, 0.85f, 36);
            GeneralParticleHandler.SpawnParticle(pulse);

            Particle explosion2 = new DetailedExplosion(npc.Center, Vector2.Zero, Color.Magenta, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, 0.65f, 26);
            GeneralParticleHandler.SpawnParticle(explosion2);

            // Define hit context variables.
            int hitDirection = player.direction;
            if (player.velocity.X != 0f)
                hitDirection = Math.Sign(player.velocity.X);
            hitContext.HitDirection = hitDirection;
            hitContext.PlayerImmunityFrames = AsgardianAegis.ShieldSlamIFrames;

            // Define damage parameters.
            int dashDamage = AsgardianAegis.ShieldSlamDamage;
            hitContext.damageClass = DamageClass.Melee;
            hitContext.BaseDamage = player.ApplyArmorAccDamageBonusesTo(dashDamage);
            hitContext.BaseKnockback = AsgardianAegis.ShieldSlamKnockback;

            // On-hit Cosmic Dash Explosion
            int explosionDamage = (int)player.GetBestClassDamage().ApplyTo(AsgardianAegis.RamExplosionDamage);
            explosionDamage = player.ApplyArmorAccDamageBonusesTo(explosionDamage);
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<CosmicDashExplosion>(), explosionDamage, AsgardianAegis.RamExplosionKnockback, Main.myPlayer, 3f, 0f);
            npc.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
        }
    }
}
