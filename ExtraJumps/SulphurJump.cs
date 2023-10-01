using CalamityMod.CalPlayer;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.ExtraJumps
{
    public class SulphurJump : ExtraJump
    {
        public override Position GetDefaultPosition() => BeforeBottleJumps;
        public override float GetDurationMultiplier(Player player) => 1.5f;

        // Values equivalent to the Tsunami in a Bottle
        public override void UpdateHorizontalSpeeds(Player player)
        {
            player.runAcceleration *= 1.5f;
            player.maxRunSpeed *= 1.25f;
        }

        public override void OnStarted(Player player, ref bool playSound)
        {
            CalamityPlayer modPlayer = player.Calamity();
            playSound = true;

            int offset = player.height;
            if (player.gravDir == -1f)
                offset = 0;
            for (int i = 0; i < 25; ++i)
            {
                Dust dust = Dust.NewDustPerfect(new Vector2(player.Center.X, player.Center.Y + offset), Main.rand.NextBool(3) ? 75 : 161, new Vector2(-player.velocity.X, 6).RotatedByRandom(MathHelper.ToRadians(50f)) * Main.rand.NextFloat(0.1f, 0.8f), 100, default, Main.rand.NextFloat(1.7f, 2.2f));
                dust.noGravity = true;
                if (dust.type == 161)
                {
                    dust.scale = 1.5f;
                    dust.velocity = new Vector2(Main.rand.NextFloat(-4, 4) + -player.velocity.X * 0.3f, Main.rand.NextFloat(2, 4));
                    dust.noGravity = false;
                    dust.alpha = 190;
                }
            }

            // Spawn a sulphur bubble projectile on a short cooldown when using this jump.
            if (modPlayer.sulphurBubbleCooldown <= 0)
            {
                var source = player.GetSource_Misc("0");
                int damage = (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(20);
                int bubble = Projectile.NewProjectile(source, new Vector2(player.position.X, player.position.Y + (player.gravDir == -1f ? 20 : -20)), Vector2.Zero, ModContent.ProjectileType<SulphuricAcidBubbleFriendly>(), damage, 0f, player.whoAmI, 1f, 0f);
                if (bubble.WithinBounds(Main.maxProjectiles))
                    Main.projectile[bubble].DamageType = DamageClass.Generic;
                modPlayer.sulphurBubbleCooldown = 20;
            }
        }
        public override void ShowVisuals(Player player)
        {
            for (int i = 0; i < 2; ++i)
            {
                //debuff spot is used here to spawn the visuals on the player correctly
                Vector2 pulsePosition = player.Calamity().RandomDebuffVisualSpot + new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3)) - player.velocity * 2;
                Vector2 pulseVelocity = new Vector2(Main.rand.NextFloat(-1f, 1f) - player.velocity.X * 0.5f, Main.rand.NextFloat(4f, 7f));
                DirectionalPulseRing pulse = new DirectionalPulseRing(pulsePosition, pulseVelocity * Main.rand.NextFloat(0.2f, 1f), Main.rand.NextBool() ? Color.OliveDrab : Color.GreenYellow, new Vector2(0.8f, 1), 0, 0.1f, 0f, 60);
                GeneralParticleHandler.SpawnParticle(pulse);
            }
        }
    }
}
