using CalamityMod.Enums;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace CalamityMod.CalPlayer.Dashes
{
    public class SuperradiantSawDash : PlayerDashEffect
    {
        public static new string ID => "Superradiant Slaughterer";
        public override DashCollisionType CollisionType => DashCollisionType.NoCollision;

        public override bool IsOmnidirectional => true;

        public override float CalculateDashSpeed(Player player) => 36f;

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            Dust trail = Dust.NewDustPerfect(player.Center + Main.rand.NextVector2Unit() * 12f, DustID.RainbowTorch, -player.velocity * 0.2f, 150, Color.Lime, 1.2f);
            trail.noGravity = true;

            Vector2 sparkVel = player.velocity.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(-3f, -6f);
            Color sparkColor = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.25f);
            float scale = Main.rand.NextFloat(1.2f, 2f); // Bloom effect is double the spark's size
            Particle spark = new CritSpark(player.Center + Main.rand.NextVector2Unit() * 12f, sparkVel, Color.White, sparkColor, scale, 24, 0.5f, scale * 2f);
            GeneralParticleHandler.SpawnParticle(spark);

            // Fall way, way, faster than usual.
            player.maxFallSpeed = 50f;

            // Dash at a much, much faster speed than the default value.
            dashSpeed = 24f;
            runSpeedDecelerationFactor = 0.8f;

            // Cooldown for dash.
            player.Calamity().SpeedBlasterDashStarted = false;
            player.Calamity().sBlasterDashActivated = false;
        }
    }
}
