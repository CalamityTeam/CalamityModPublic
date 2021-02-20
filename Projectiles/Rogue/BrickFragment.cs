using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class BrickFragment : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brick Fragment");
            Main.projFrames[projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 13;
            projectile.height = 13;
            projectile.Calamity().rogue = true;
        }
        public override void AI()
        {
            //Choose the sprite of the projectile
            if (projectile.ai[0] != 1)
            {
                projectile.frame = Main.rand.Next(2);
                projectile.ai[0] += 1;
            }
            //Rotation and gravity
            projectile.rotation += 0.6f * projectile.direction;
            projectile.velocity.Y = projectile.velocity.Y + 0.27f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }

        }
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y);
            //Dust effect
            int splash = 0;
            while (splash < 4)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 9, -projectile.velocity.X * 0.15f, -projectile.velocity.Y * 0.10f, 150, default, 0.9f);
                splash += 1;
            }
        }
    }
}
