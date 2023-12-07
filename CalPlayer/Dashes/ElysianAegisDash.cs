using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Enums;
using CalamityMod.Items.Accessories;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.Dashes
{
    public class ElysianAegisDash : PlayerDashEffect
    {
        public static new string ID => "Elysian Aegis";

        public override DashCollisionType CollisionType => DashCollisionType.ShieldSlam;

        public override bool IsOmnidirectional => false;

        public override float CalculateDashSpeed(Player player) => 21.5f;

        public override void OnDashEffects(Player player)
        {
            // Spawn fire dust around the player's body.
            for (int d = 0; d < 20; d++)
            {
                Dust holyFireDashDust = Dust.NewDustDirect(player.position, player.width, player.height, 246, 0f, 0f, 100, default, 3f);
                holyFireDashDust.position += Main.rand.NextVector2Square(-5f, 5f);
                holyFireDashDust.velocity *= 0.2f;
                holyFireDashDust.scale *= Main.rand.NextFloat(1f, 1.2f);
                holyFireDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
                holyFireDashDust.noGravity = true;
                holyFireDashDust.fadeIn = 0.5f;
            }
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            // Spawn fire dust around the player's body.
            for (int d = 0; d < 4; d++)
            {
                Dust holyFireDashDust = Dust.NewDustDirect(player.position + Vector2.UnitY * 4f, player.width, player.height - 8, 246, 0f, 0f, 100, default, 2.75f);
                holyFireDashDust.velocity *= 0.1f;
                holyFireDashDust.scale *= Main.rand.NextFloat(1f, 1.2f);
                holyFireDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
                holyFireDashDust.noGravity = true;
                if (Main.rand.NextBool())
                    holyFireDashDust.fadeIn = 0.5f;
            }

            // Dash at a faster speed than the default value.
            dashSpeed = 14f;
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            // Define hit context variables.
            int hitDirection = player.direction;
            if (player.velocity.X != 0f)
                hitDirection = Math.Sign(player.velocity.X);
            hitContext.HitDirection = hitDirection;
            hitContext.PlayerImmunityFrames = ElysianAegis.ShieldSlamIFrames;

            // Define damage parameters.
            int dashDamage = ElysianAegis.ShieldSlamDamage;
            hitContext.damageClass = DamageClass.Melee;
            hitContext.BaseDamage = player.Calamity().oldFashioned ? CalamityUtils.CalcOldFashionedDamage(dashDamage) : dashDamage;
            hitContext.BaseKnockback = ElysianAegis.ShieldSlamKnockback;

            // On-hit Supreme Holy Explosion
            int supremeExplosionDamage = (int)player.GetBestClassDamage().ApplyTo(ElysianAegis.RamExplosionDamage);
            if (player.Calamity().oldFashioned)
                supremeExplosionDamage = CalamityUtils.CalcOldFashionedDamage(supremeExplosionDamage);

            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<HolyExplosionSupreme>(), supremeExplosionDamage, ElysianAegis.RamExplosionKnockback, Main.myPlayer, 1f, 0f);
            npc.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
        }
    }
}
