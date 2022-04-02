using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class ReaverOrb : ModProjectile
    {
        public int dust = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 50;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 18000;
            projectile.alpha = 50;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
        }

        public override void AI()
        {
            bool correctMinion = projectile.type == ModContent.ProjectileType<ReaverOrb>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.reaverExplore)
            {
                projectile.active = false;
                return;
            }
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.rOrb = false;
                }
                if (modPlayer.rOrb)
                {
                    projectile.timeLeft = 2;
                }
            }
            dust--;
            if (dust >= 0)
            {
                int dustAmt = 50;
                for (int d = 0; d < dustAmt; d++)
                {
                    int idx = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 157, 0f, 0f, 0, default, 1f);
                    Main.dust[idx].velocity *= 2f;
                    Main.dust[idx].scale *= 1.15f;
                }
            }

            Lighting.AddLight(projectile.Center, 0.5f, 2f, 0.5f);

            projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY - 60f);
            if (player.gravDir == -1f)
            {
                projectile.position.Y += 120f;
                projectile.rotation = MathHelper.Pi;
            }
            else
            {
                projectile.rotation = 0f;
            }
            projectile.position.X = (int)projectile.position.X;
            projectile.position.Y = (int)projectile.position.Y;
        }

        public override bool CanDamage() => false;
    }
}