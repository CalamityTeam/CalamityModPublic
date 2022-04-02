using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class MiniSharkron : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mini Sharkron");
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = 1;
            aiType = ProjectileID.MiniSharkron;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void Kill(int timeLeft)
        {
            for (int d = 0; d < 15; ++d)
            {
              int idx = Dust.NewDust(projectile.Center - Vector2.One * 10f, 50, 50, DustID.Blood, 0f, -2f, 0, default, 1f);
              Dust dust = Main.dust[idx];
              dust.velocity /= 2f;
            }
            int tail = Gore.NewGore(projectile.Center, projectile.velocity * 0.8f, 584, 1f);
            Main.gore[tail].timeLeft /= 10;
            int body = Gore.NewGore(projectile.Center, projectile.velocity * 0.9f, 585, 1f);
            Main.gore[body].timeLeft /= 10;
            int head = Gore.NewGore(projectile.Center, projectile.velocity * 1f, 586, 1f);
            Main.gore[head].timeLeft /= 10;
        }
    }
}
