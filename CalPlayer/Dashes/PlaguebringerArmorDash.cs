using System;
using CalamityMod.Enums;
using CalamityMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace CalamityMod.CalPlayer.Dashes
{
    public class PlaguebringerArmorDash : PlayerDashEffect
    {
        public static new string ID => "Plaguebringer Armor";

        public override DashCollisionType CollisionType => DashCollisionType.NoCollision;

        public override bool IsOmnidirectional => false;

        public override float CalculateDashSpeed(Player player) => 19f;

        public override void OnDashEffects(Player player)
        {
            // Spawn plague dust around the player's body.
            for (int d = 0; d < 60; d++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, 89, 0f, 0f, 100, default, 1.25f);
                dust.position.X += Main.rand.NextFloat(-5f, 5f);
                dust.position.Y += Main.rand.NextFloat(-5f, 5f);
                dust.velocity *= 0.2f;
                dust.scale *= Main.rand.NextFloat(1f, 1.2f);
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                dust.noGravity = true;
                dust.fadeIn = 0.5f;
            }
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            // Spawn plague dust around the player's body.
            for (int m = 0; m < 24; m++)
            {
                Dust plagueDashDust = Dust.NewDustDirect(new Vector2(player.position.X, player.position.Y + 4f), player.width, player.height - 8, 89, 0f, 0f, 100, default, 1f);
                plagueDashDust.velocity *= 0.1f;
                plagueDashDust.scale *= Main.rand.NextFloat(1f, 1.2f);
                plagueDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
                plagueDashDust.noGravity = true;
                if (Main.rand.NextBool())
                    plagueDashDust.fadeIn = 0.5f;
            }

            // Dash at a faster speed than the default value.
            dashSpeed = 12.5f;
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            float kbFactor = 3f;
            bool crit = false;
            if (player.kbGlove)
                kbFactor *= 2f;
            if (player.kbBuff)
                kbFactor *= 1.5f;

            int hitDirection = player.direction;
            if (player.velocity.X != 0f)
                hitDirection = Math.Sign(player.velocity.X);

            // Define hit context variables.
            hitContext.CriticalHit = crit;
            hitContext.HitDirection = hitDirection;
            hitContext.KnockbackFactor = kbFactor;
            hitContext.PlayerImmunityFrames = AsgardsValor.ShieldSlamIFrames;
            hitContext.Damage = (int)player.GetBestClassDamage().ApplyTo(50f);
        }
    }
}
