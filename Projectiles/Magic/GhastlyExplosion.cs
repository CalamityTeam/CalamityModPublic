using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class GhastlyExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghastly Explosion");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.magic = true;
        }

        public override void AI()
        {
            int num3;
            int num328 = (int)projectile.ai[0];
            for (int num329 = 0; num329 < 3; num329 = num3 + 1)
            {
                int num330 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, projectile.velocity.X, projectile.velocity.Y, num328, default, 1.2f);
                Main.dust[num330].position = (Main.dust[num330].position + projectile.Center) / 2f;
                Main.dust[num330].noGravity = true;
                Dust dust = Main.dust[num330];
                dust.velocity *= 0.5f;
                num3 = num329;
            }
            for (int num331 = 0; num331 < 2; num331 = num3 + 1)
            {
                int num330 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 60, projectile.velocity.X, projectile.velocity.Y, num328, default, 0.4f);
                if (num331 == 0)
                {
                    Main.dust[num330].position = (Main.dust[num330].position + projectile.Center * 5f) / 6f;
                }
                else if (num331 == 1)
                {
                    Main.dust[num330].position = (Main.dust[num330].position + (projectile.Center + projectile.velocity / 2f) * 5f) / 6f;
                }
                Dust dust = Main.dust[num330];
                dust.velocity *= 0.1f;
                Main.dust[num330].noGravity = true;
                Main.dust[num330].fadeIn = 1f;
                num3 = num331;
            }
            return;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item50, projectile.position);
            int num3;
            for (int num116 = 0; num116 < 20; num116 = num3 + 1)
            {
                int num117 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)projectile.ai[0], projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 0, default, 0.5f);
                Dust dust;
                Main.dust[num117].scale = 1.2f + (float)Main.rand.Next(-10, 11) * 0.01f;
                Main.dust[num117].noGravity = true;
                dust = Main.dust[num117];
                dust.velocity *= 2.5f;
                dust = Main.dust[num117];
                dust.velocity -= projectile.oldVelocity / 10f;
                num3 = num116;
            }
            if (Main.myPlayer == projectile.owner)
            {
                int num118 = 3;
                for (int num119 = 0; num119 < num118; num119 = num3 + 1)
                {
                    Vector2 vector8 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (vector8.X == 0f && vector8.Y == 0f)
                    {
                        vector8 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    vector8.Normalize();
                    vector8 *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), vector8.X, vector8.Y, ModContent.ProjectileType<GhastlyExplosionShard>(), (int)((double)projectile.damage * 0.8), projectile.knockBack * 0.8f, projectile.owner, projectile.ai[0], 0f);
                    num3 = num119;
                }
            }
        }
    }
}
