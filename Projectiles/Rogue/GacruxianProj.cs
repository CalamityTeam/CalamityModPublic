using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class GacruxianProj : ModProjectile
    {
        private int sparkTrailTimer = 10;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mollusk");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 113;
            projectile.timeLeft = 600;
            aiType = 598;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.ai[1] == 1f && projectile.owner == Main.myPlayer)
            {
                sparkTrailTimer--;
                if (Main.rand.NextBool(4))
                {
                    Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, mod.DustType("AstralOrange"), projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
                }
                if (sparkTrailTimer == 0)
                {
                    if (projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0f, projectile.velocity.Y * 0f, mod.ProjectileType("GacruxianHome"), (int)((double)projectile.damage * 0.75), projectile.knockBack, projectile.owner, 0f, 0f);
                    }
                    sparkTrailTimer = 10;
                }
            }
            else
            {
                sparkTrailTimer--;
                if (Main.rand.NextBool(4))
                {
                    Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, mod.DustType("AstralOrange"), projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
                }
                if (sparkTrailTimer == 0)
                {
                    if (projectile.owner == Main.myPlayer)
                    {
                        int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0f, projectile.velocity.Y * 0f, mod.ProjectileType("UltimusCleaverDust"), (int)((double)projectile.damage * 0.75), projectile.knockBack, projectile.owner, 0f, 0f);
                        Main.projectile[proj].Calamity().forceRogue = true;
                    }
                    sparkTrailTimer = 10;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 10; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, mod.DustType("AstralOrange"), projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
