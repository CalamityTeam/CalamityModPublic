using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class MiniSharkron : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.MiniSharkron;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void OnKill(int timeLeft)
        {
            for (int d = 0; d < 15; ++d)
            {
              int idx = Dust.NewDust(Projectile.Center - Vector2.One * 10f, 50, 50, DustID.Blood, 0f, -2f, 0, default, 1f);
              Dust dust = Main.dust[idx];
              dust.velocity /= 2f;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                int tail = Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity * 0.8f, 584, 1f);
                Main.gore[tail].timeLeft /= 10;
                int body = Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity * 0.9f, 585, 1f);
                Main.gore[body].timeLeft /= 10;
                int head = Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity * 1f, 586, 1f);
                Main.gore[head].timeLeft /= 10;
            }
        }
    }
}
