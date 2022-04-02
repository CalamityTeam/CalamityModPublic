using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class RiptideYoyo : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Riptide");
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 7.5f;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 220f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 12.5f;
        }

        public override void SetDefaults()
        {
            projectile.aiStyle = 99;
            projectile.width = 16;
            projectile.height = 16;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.alpha = 150;
            projectile.penetrate = -1;
            projectile.MaxUpdates = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if ((projectile.position - Main.player[projectile.owner].position).Length() > 3200f) //200 blocks
                projectile.Kill();
            if (Main.rand.NextBool(90))
            {
                switch (Main.rand.Next(1, 9))
                {
                    case 1:
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, -10, ModContent.ProjectileType<AquaStream>(), 4, 0.0f, projectile.owner, 1.2f/*X Increment*/, 0.2f/*Y Increment*/);
                        break;
                    case 2:
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 5, -5, ModContent.ProjectileType<AquaStream>(), 4, 0.0f, projectile.owner, 0.7f/*X Increment*/, 0.7f/*Y Increment*/);
                        break;
                    case 3:
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 10, 0, ModContent.ProjectileType<AquaStream>(), 4, 0.0f, projectile.owner, 0.2f/*X Increment*/, 1.2f/*Y Increment*/);
                        break;
                    case 4:
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 5, 5, ModContent.ProjectileType<AquaStream>(), 4, 0.0f, projectile.owner, -0.7f/*X Increment*/, 0.7f/*Y Increment*/);
                        break;
                    case 5:
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -0, 10, ModContent.ProjectileType<AquaStream>(), 4, 0.0f, projectile.owner, -1.2f/*X Increment*/, -0.2f/*Y Increment*/);
                        break;
                    case 6:
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -5, 5, ModContent.ProjectileType<AquaStream>(), 4, 0.0f, projectile.owner, -0.7f/*X Increment*/, -0.7f/*Y Increment*/);
                        break;
                    case 7:
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -10, -0, ModContent.ProjectileType<AquaStream>(), 4, 0.0f, projectile.owner, -0.2f/*X Increment*/, -1.2f/*Y Increment*/);
                        break;
                    case 8:
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -10, -10, ModContent.ProjectileType<AquaStream>(), 4, 0.0f, projectile.owner, 0.7f/*X Increment*/, -0.7f/*Y Increment*/);
                        break;
                }
            }
        }
    }
}
