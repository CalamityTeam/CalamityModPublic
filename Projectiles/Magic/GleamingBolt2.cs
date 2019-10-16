using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class GleamingBolt2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.timeLeft = 120;
            projectile.penetrate = 1;
            projectile.magic = true;
        }

        public override void AI()
        {
            projectile.velocity.X *= 0.985f;
            projectile.velocity.Y *= 0.985f;
            for (int dust = 0; dust < 2; dust++)
            {
                int randomDust = Main.rand.Next(2);
                if (randomDust == 0)
                {
                    randomDust = 64;
                }
                else
                {
                    randomDust = 204;
                }
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                int randomDust = Main.rand.Next(2);
                if (randomDust == 0)
                {
                    randomDust = 64;
                }
                else
                {
                    randomDust = 204;
                }
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
