using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class ShaderainHostile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Rain");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 40;
            projectile.hostile = true;
            projectile.extraUpdates = 1;
            projectile.penetrate = -1;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            projectile.alpha = 50;
        }

        public override void Kill(int timeLeft)
        {
            int num310 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + (float)projectile.height - 2f), 2, 2, 14, 0f, 0f, 0, default, 1f);
            Dust expr_A0A0_cp_0 = Main.dust[num310];
            expr_A0A0_cp_0.position.X -= 2f;
            Main.dust[num310].alpha = 38;
            Main.dust[num310].velocity *= 0.1f;
            Main.dust[num310].velocity += -projectile.oldVelocity * 0.25f;
            Main.dust[num310].scale = 0.95f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(102, 255, 102, projectile.alpha);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 90);
        }
    }
}
