using System;
using CalamityMod.Enums;
using CalamityMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.Dashes
{
    public class OrnateShieldDash : PlayerDashEffect
    {
        public static new string ID => "Ornate Shield";

        public override DashCollisionType CollisionType => DashCollisionType.ShieldSlam;

        public override bool IsOmnidirectional => false;

        public override float CalculateDashSpeed(Player player) => 16.9f;

        public override void OnDashEffects(Player player)
        {
            for (int d = 0; d < 60; d++)
            {
                Dust iceDashDust = Dust.NewDustDirect(player.position, player.width, player.height, 67, 0f, 0f, 100, default, 1.25f);
                iceDashDust.position += Main.rand.NextVector2Square(-5f, 5f);
                iceDashDust.velocity *= 0.2f;
                iceDashDust.scale *= Main.rand.NextFloat(1f, 1.2f);
                iceDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
                iceDashDust.noGravity = true;
                iceDashDust.fadeIn = 0.5f;
            }
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            for (int m = 0; m < 24; m++)
            {
                Dust iceDashDust = Dust.NewDustDirect(player.position + Vector2.UnitY * 4f, player.width, player.height - 8, 67, 0f, 0f, 100, default, 1.25f);
                iceDashDust.position += Main.rand.NextVector2Square(-5f, 5f);
                iceDashDust.velocity *= 0.2f;
                iceDashDust.scale *= Main.rand.NextFloat(1f, 1.2f);
                iceDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
                iceDashDust.noGravity = true;
                if (Main.rand.NextBool(2))
                    iceDashDust.fadeIn = 0.5f;
            }

            // Dash at a faster speed than the default value.
            dashSpeed = 12.5f;
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            float kbFactor = 3f;
            if (player.kbGlove)
                kbFactor *= 2f;
            if (player.kbBuff)
                kbFactor *= 1.5f;

            int hitDirection = player.direction;
            if (player.velocity.X != 0f)
                hitDirection = Math.Sign(player.velocity.X);

            // Define hit context variables.
            hitContext.CriticalHit = false;
            hitContext.HitDirection = hitDirection;
            hitContext.KnockbackFactor = kbFactor;
            hitContext.PlayerImmunityFrames = OrnateShield.ShieldSlamIFrames;
            hitContext.Damage = (int)player.GetTotalDamage<MeleeDamageClass>().ApplyTo(50f);

            npc.AddBuff(BuffID.Frostburn, 300);
        }
    }
}
