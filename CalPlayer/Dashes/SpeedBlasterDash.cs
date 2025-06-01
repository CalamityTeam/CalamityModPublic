using CalamityMod.Enums;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace CalamityMod.CalPlayer.Dashes
{
    public class SpeedBlasterDash : PlayerDashEffect
    {
        public static new string ID => "Speed Blaster";
        public override DashCollisionType CollisionType => DashCollisionType.NoCollision;

        public override bool IsOmnidirectional => true;

        public override float CalculateDashSpeed(Player player) => 30f;

        public override void OnDashEffects(Player player)
        {
            SoundEngine.PlaySound(SpeedBlaster.Dash, player.Center);
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            Dust trail = Dust.NewDustPerfect(player.Center + Main.rand.NextVector2Unit() * 12f, DustID.RainbowTorch, -player.velocity * 0.2f, 150, Color.Aqua, 1.2f);
            trail.noGravity = true;

            Vector2 sparkVel = player.velocity.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(-3f, -6f);
            Color sparkColor = SpeedBlasterShot.GetColor(Main.rand.Next(5));
            float scale = Main.rand.NextFloat(1f, 1.6f); // Bloom effect is double the spark's size
            Particle spark = new CritSpark(player.Center + Main.rand.NextVector2Unit() * 12f, sparkVel, Color.White, sparkColor, scale, 15, 0.5f, scale * 2f);
            GeneralParticleHandler.SpawnParticle(spark);

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
