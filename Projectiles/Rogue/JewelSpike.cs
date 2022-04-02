using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class JewelSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spike");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 44;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 40;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            projectile.velocity *= 0f;

            if (Main.rand.NextBool(5) && projectile.frame < 3)
            {
                int crystalDust = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 87, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
                Main.dust[crystalDust].noGravity = true;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter > 4 && projectile.frame > 0)
            {
                projectile.frame--;
                projectile.frameCounter = 0;
            }
            if (projectile.frame < 0)
                projectile.frame = 0;
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
            {
                for (int index = 0; index < 3; ++index)
                {
                    float SpeedX = -projectile.velocity.X * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    float SpeedY = -projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    Projectile.NewProjectile(projectile.Center.X + SpeedX, projectile.Center.Y + SpeedY, SpeedX, SpeedY, ProjectileID.CrystalShard, projectile.damage / 4, 0f, projectile.owner);
                }
            }
        }
    }
}
