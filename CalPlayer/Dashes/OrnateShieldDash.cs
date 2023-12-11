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
            //nothing left, could add a sound or other effect if we want later
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            for (int d = 0; d < 3; d++)
            {
                Dust iceDashDust = Dust.NewDustPerfect(player.Center + new Vector2 (Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) - (player.velocity * 1.7f), Main.rand.NextBool(8) ? 223 : 180, -player.velocity.RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(0.6f, 0.8f));
                iceDashDust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
                iceDashDust.noGravity = true;
                iceDashDust.fadeIn = 0.5f;
                if (iceDashDust.type == 180)
                    iceDashDust.scale = Main.rand.NextFloat(1.6f, 2.2f);
            }

            // Dash at a faster speed than the default value.
            dashSpeed = 12.5f;
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            // Define hit context variables.
            int hitDirection = player.direction;
            if (player.velocity.X != 0f)
                hitDirection = Math.Sign(player.velocity.X);
            hitContext.HitDirection = hitDirection;
            hitContext.PlayerImmunityFrames = OrnateShield.ShieldSlamIFrames;

            // Define damage parameters.
            int dashDamage = OrnateShield.ShieldSlamDamage;
            hitContext.damageClass = DamageClass.Melee;
            hitContext.BaseDamage = player.ApplyArmorAccDamageBonusesTo(dashDamage);
            hitContext.BaseKnockback = OrnateShield.ShieldSlamKnockback;

            npc.AddBuff(BuffID.Frostburn2, 180); //Great, Frostbite is ACTUALLY called Frostburn 2
        }
    }
}
