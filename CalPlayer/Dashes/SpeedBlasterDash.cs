using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Enums;
using CalamityMod.Items.Armor.GodSlayer;
using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using CalamityMod.Projectiles.Ranged;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.Dashes
{
    public class SpeedBlasterDash : PlayerDashEffect
    {
        public static new string ID => "Speed Blaster";
        public override DashCollisionType CollisionType => DashCollisionType.NoCollision;

        public override bool IsOmnidirectional => true;

        public bool dustOnce = true;

        public override float CalculateDashSpeed(Player player) => 30f;

        public override void OnDashEffects(Player player)
        {
            SoundEngine.PlaySound(SpeedBlaster.Dash, player.Center);
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            // Fall way, way, faster than usual.
            player.maxFallSpeed = 50f;

            // Dash at a much, much faster speed than the default value.
            dashSpeed = 20f;
            runSpeedDecelerationFactor = 0.8f;

            // Cooldown for dash.
            player.Calamity().SpeedBlasterDashStarted = false;
            player.Calamity().sBlasterDashActivated = false;
        }
    }
}
