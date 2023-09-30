using CalamityMod.CalPlayer;
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
            playSound = true;
            CalamityPlayer modPlayer = player.Calamity();

            // Vanilla dust code
            int offset = player.height;
            if (player.gravDir == -1f)
                offset = 0;
            for (int d = 0; d < 30; ++d)
            {
                int sulfur = Dust.NewDust(new Vector2(player.position.X, player.position.Y + offset), player.width, 12, 31, player.velocity.X * 0.3f, player.velocity.Y * 0.3f, 100, default, 1.5f);
                if (d % 2 == 0)
                    Main.dust[sulfur].velocity.X += (float)Main.rand.Next(30, 71) * 0.1f;
                else
                    Main.dust[sulfur].velocity.X -= (float)Main.rand.Next(30, 71) * 0.1f;
                Main.dust[sulfur].velocity.Y += (float)Main.rand.Next(-10, 31) * 0.1f;
                Main.dust[sulfur].noGravity = true;
                Main.dust[sulfur].scale += (float)Main.rand.Next(-10, 41) * 0.01f;
                Main.dust[sulfur].velocity *= Main.dust[sulfur].scale * 0.7f;
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
    }
}
