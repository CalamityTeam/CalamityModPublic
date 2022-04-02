using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class AegisBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aegis Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 120;
            projectile.alpha = 255;
            projectile.melee = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.25f, 0.25f, 0f);
            projectile.rotation += 1f;
            projectile.alpha -= 25;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.localAI[0] == 0f)
            {
                Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 73);
                projectile.localAI[0] += 1f;
            }
            int num458 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 246, 0f, 0f, 100, new Color(255, Main.DiscoG, 53), 0.8f);
            Main.dust[num458].noGravity = true;
            Main.dust[num458].velocity *= 0.5f;
            Main.dust[num458].velocity += projectile.velocity * 0.1f;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 64;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.Item20, projectile.Center);
            for (int d = 0; d <= 30; d++)
            {
                float num463 = (float)Main.rand.Next(-10, 11);
                float num464 = (float)Main.rand.Next(-10, 11);
                float speed = (float)Main.rand.Next(3, 9);
                float num466 = (float)Math.Sqrt((double)(num463 * num463 + num464 * num464));
                num466 = speed / num466;
                num463 *= num466;
                num464 *= num466;
                int num467 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 246, 0f, 0f, 100, new Color(255, Main.DiscoG, 53), 1.2f);
                Dust dust = Main.dust[num467];
                dust.noGravity = true;
                dust.position.X = projectile.Center.X;
                dust.position.Y = projectile.Center.Y;
                dust.position.X += (float)Main.rand.Next(-10, 11);
                dust.position.Y += (float)Main.rand.Next(-10, 11);
                dust.velocity.X = num463;
                dust.velocity.Y = num464;
            }
            int flameAmt = Main.rand.Next(2, 4);
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < flameAmt; i++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<AegisFlame>(), (int)(projectile.damage * 0.75), 0f, projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
