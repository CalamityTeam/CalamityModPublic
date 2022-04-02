using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class LaserFountain : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fountain");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 60;
            projectile.melee = true;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            float SpeedX = (float)Main.rand.Next(-15, 15);
            float SpeedY = (float)Main.rand.Next(-20, -10);
            if (projectile.localAI[0] >= 12f)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    int projectile1 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, SpeedX, SpeedY, ModContent.ProjectileType<NebulaShot>(), (int)(350f * Main.player[projectile.owner].MeleeDamage()), projectile.knockBack, projectile.owner, 0f, 0f);
                    if (projectile1.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[projectile1].Calamity().forceMelee = true;
                        Main.projectile[projectile1].aiStyle = 1;
                    }
                }
                projectile.localAI[0] = 0f;
            }
        }
    }
}
