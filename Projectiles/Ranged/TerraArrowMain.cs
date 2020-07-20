using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class TerraArrowMain : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arrow");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.arrow = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.aiStyle = 1;
        }

        public override void AI()
        {
            projectile.velocity *= 1.005f;
            if (projectile.velocity.Length() >= 20f)
            {
                projectile.Kill();
            }
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 32;
            projectile.position.X -= (float)(projectile.width / 2);
            projectile.position.Y -= (float)(projectile.height / 2);
            Main.PlaySound(SoundID.Item60, projectile.position);
            for (int d = 0; d < 3; d++)
            {
                int terra = Dust.NewDust(projectile.position, projectile.width, projectile.height, 107, 0f, 0f, 100, default, 2f);
                Main.dust[terra].velocity *= 1.2f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[terra].scale = 0.5f;
                    Main.dust[terra].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            if (projectile.owner == Main.myPlayer)
            {
                for (int a = 0; a < 2; a++)
                {
					Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<TerraArrowSplit>(), (int)(projectile.damage * 0.25), 0f, projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
