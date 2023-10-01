using Microsoft.Xna.Framework;
using CalamityMod.Particles;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.ExtraJumps
{
    public class StatigelJump : ExtraJump
    {
        public override Position GetDefaultPosition() => BeforeBottleJumps;
        public override float GetDurationMultiplier(Player player) => 1.25f;

        // Values equivalent to the Fart in a Jar
        public override void UpdateHorizontalSpeeds(Player player)
        {
            player.runAcceleration *= 3f;
            player.maxRunSpeed *= 1.75f;
        }

        public override void OnStarted(Player player, ref bool playSound)
        {
            playSound = true;

            int offset = player.height;
            if (player.gravDir == -1f)
                offset = 0;
            for (int i = 0; i < 35; ++i)
            {
                Dust dust = Dust.NewDustPerfect(new Vector2(player.Center.X, player.Center.Y + offset), Main.rand.NextBool() ? 243 : 56, new Vector2(-player.velocity.X, 15).RotatedByRandom(MathHelper.ToRadians(50f)) * Main.rand.NextFloat(0.1f, 0.8f), 100, default, Main.rand.NextFloat(1.2f, 1.9f));
                dust.noGravity = true;
            }
            for (int i = 0; i < 20; ++i)
            {
                Dust dust = Dust.NewDustPerfect(new Vector2(player.Center.X, player.Center.Y + offset), Main.rand.NextBool() ? 242 : 135, new Vector2(-player.velocity.X, 15).RotatedByRandom(MathHelper.ToRadians(50f)) * Main.rand.NextFloat(0.1f, 0.8f), 100, default, Main.rand.NextFloat(1.2f, 1.9f));
                dust.noGravity = true;
            }
        }
        public override void ShowVisuals(Player player)
        {
            for (int i = 0; i < 3; ++i)
            {
                //debuff spot is used here to spawn the visuals on the player correctly
                Vector2 pulsePosition = player.Calamity().RandomDebuffVisualSpot + new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3));
                Vector2 pulseVelocity = new Vector2(Main.rand.NextFloat(-1f, 1f) - player.velocity.X * 0.5f, Main.rand.NextFloat(4f, 7f)) * Main.rand.NextFloat(0.2f, 1f);
                Particle orb = new GenericBloom(pulsePosition, pulseVelocity, Main.rand.NextBool() ? Color.DarkTurquoise : Color.Orchid, 0.055f, 8);
                GeneralParticleHandler.SpawnParticle(orb);
                Dust dust = Dust.NewDustPerfect(pulsePosition, Main.rand.NextBool() ? 243 : 56, pulseVelocity, 100, default, Main.rand.NextFloat(0.6f, 0.9f));
                dust.noGravity = false;
                dust.alpha = 190;
                Dust dust2 = Dust.NewDustPerfect(pulsePosition, Main.rand.NextBool() ? 242 : 135, pulseVelocity * 0.9f , 100, default, Main.rand.NextFloat(1.2f, 1.9f));
                dust2.noGravity = true;
            }
        }
    }
}
