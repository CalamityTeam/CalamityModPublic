using CalamityMod.Enums;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.Dashes
{
    public class StatisVoidSashDash : PlayerDashEffect
    {
        public static new string ID => "Statis' Void Sash";

        public override DashCollisionType CollisionType => DashCollisionType.NoCollision;

        public override bool IsOmnidirectional => false;

        public override float CalculateDashSpeed(Player player) => 25.4f;

        public override void OnDashEffects(Player player)
        {
            for (int d = 0; d < 60; d++)
            {
                Dust iceDashDust = Dust.NewDustDirect(player.position, player.width, player.height, 33, 0f, 0f, 100, default, 1.25f);
                iceDashDust.position += Main.rand.NextVector2Square(-5f, 5f);
                iceDashDust.velocity *= 0.2f;
                iceDashDust.scale *= Main.rand.NextFloat(1f, 1.2f);
                iceDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
                iceDashDust.noGravity = true;
                iceDashDust.fadeIn = 0.5f;
            }
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            // Spawn dust at the player's feet if they are moving vertically. Otherwise, spawn it around their body.
            int dustSpawnHeight = 8;
            float dustSpawnTop = player.Bottom.Y - 4f;
            if (player.velocity.Y != 0f)
            {
                dustSpawnHeight = 16;
                dustSpawnTop = player.Center.Y - 8f;
            }
            Vector2 dustSpawnPosition = new Vector2(player.position.X, dustSpawnTop);

            Dust purpleDashDust = Dust.NewDustDirect(dustSpawnPosition, player.width, dustSpawnHeight, 70, 0f, 0f, 100, default, 1.4f);
            purpleDashDust.velocity *= 0.1f;
            purpleDashDust.scale *= Main.rand.NextFloat(1f, 1.2f);
            purpleDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);

            // Periodically release scythes.
            player.Calamity().statisTimer++;
            if (Main.myPlayer == player.whoAmI && player.Calamity().statisTimer % 5 == 0)
            {
                int scytheDamage = (int)player.GetBestClassDamage().ApplyTo(250);
                int scythe = Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<CosmicScythe>(), scytheDamage, 5f, player.whoAmI);
                if (scythe.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[scythe].DamageType = DamageClass.Generic;
                    Main.projectile[scythe].usesIDStaticNPCImmunity = true;
                    Main.projectile[scythe].idStaticNPCHitCooldown = 10;
                }
            }

            // Dash at a faster speed than the default value.
            dashSpeed = 14f;
        }
    }
}
