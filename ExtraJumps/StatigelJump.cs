using Microsoft.Xna.Framework;
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

            // Vanilla dust code
            int offset = player.height;
            if (player.gravDir == -1f)
                offset = 0;
            for (int d = 0; d < 30; ++d)
            {
                int goo = Dust.NewDust(new Vector2(player.position.X, player.position.Y + offset), player.width, 12, 4, player.velocity.X * 0.3f, player.velocity.Y * 0.3f, 100, new Color(0, 80, 255, 100), 1.5f);
                if (d % 2 == 0)
                    Main.dust[goo].velocity.X += (float)Main.rand.Next(30, 71) * 0.1f;
                else
                    Main.dust[goo].velocity.X -= (float)Main.rand.Next(30, 71) * 0.1f;
                Main.dust[goo].velocity.Y += (float)Main.rand.Next(-10, 31) * 0.1f;
                Main.dust[goo].noGravity = true;
                Main.dust[goo].scale += (float)Main.rand.Next(-10, 41) * 0.01f;
                Main.dust[goo].velocity *= Main.dust[goo].scale * 0.7f;
            }
        }
    }
}
