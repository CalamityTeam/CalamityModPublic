using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
namespace CalamityMod.Projectiles.Summon
{
    public class PlateProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plate");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.aiStyle = 1;
        }

        public override void AI()
        {
            projectile.velocity.X *= 0.9995f;
            projectile.velocity.Y = projectile.velocity.Y + 0.01f;
            projectile.rotation -= MathHelper.ToRadians(90) * projectile.direction;
            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 5)
            {
                projectile.frame = 0;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Shatter, (int)projectile.Center.X, (int)projectile.Center.Y);
            if (projectile.owner == Main.myPlayer)
            {
                for (int index = 0; index < 3; ++index)
                {
                    float SpeedX = -projectile.velocity.X * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    float SpeedY = -projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    int shard = Projectile.NewProjectile(projectile.Center.X + SpeedX, projectile.Center.Y + SpeedY, SpeedX, SpeedY, ProjectileID.CrystalShard, projectile.damage / 2, 0f, projectile.owner);
                    if (shard.WithinBounds(Main.maxProjectiles))
                        Main.projectile[shard].Calamity().forceMinion = true;
                }
            }
        }
    }
}
