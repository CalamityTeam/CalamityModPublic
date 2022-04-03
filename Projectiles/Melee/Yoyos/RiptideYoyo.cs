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
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 7.5f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 220f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 12.5f;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = 99;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 150;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f) //200 blocks
                Projectile.Kill();
            if (Main.rand.NextBool(90))
            {
                switch (Main.rand.Next(1, 9))
                {
                    case 1:
                        Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, 0, -10, ModContent.ProjectileType<AquaStream>(), 4, 0.0f, Projectile.owner, 1.2f/*X Increment*/, 0.2f/*Y Increment*/);
                        break;
                    case 2:
                        Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, 5, -5, ModContent.ProjectileType<AquaStream>(), 4, 0.0f, Projectile.owner, 0.7f/*X Increment*/, 0.7f/*Y Increment*/);
                        break;
                    case 3:
                        Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, 10, 0, ModContent.ProjectileType<AquaStream>(), 4, 0.0f, Projectile.owner, 0.2f/*X Increment*/, 1.2f/*Y Increment*/);
                        break;
                    case 4:
                        Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, 5, 5, ModContent.ProjectileType<AquaStream>(), 4, 0.0f, Projectile.owner, -0.7f/*X Increment*/, 0.7f/*Y Increment*/);
                        break;
                    case 5:
                        Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, -0, 10, ModContent.ProjectileType<AquaStream>(), 4, 0.0f, Projectile.owner, -1.2f/*X Increment*/, -0.2f/*Y Increment*/);
                        break;
                    case 6:
                        Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, -5, 5, ModContent.ProjectileType<AquaStream>(), 4, 0.0f, Projectile.owner, -0.7f/*X Increment*/, -0.7f/*Y Increment*/);
                        break;
                    case 7:
                        Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, -10, -0, ModContent.ProjectileType<AquaStream>(), 4, 0.0f, Projectile.owner, -0.2f/*X Increment*/, -1.2f/*Y Increment*/);
                        break;
                    case 8:
                        Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, -10, -10, ModContent.ProjectileType<AquaStream>(), 4, 0.0f, Projectile.owner, 0.7f/*X Increment*/, -0.7f/*Y Increment*/);
                        break;
                }
            }
        }
    }
}
