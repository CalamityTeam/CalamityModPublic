using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class XerocBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 5;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.35f) / 255f, ((255 - projectile.alpha) * 0.05f) / 255f, ((255 - projectile.alpha) * 0.25f) / 255f);
            bool flag15 = false;
            bool flag16 = false;
            if (projectile.velocity.X < 0f && projectile.position.X < projectile.ai[0])
            {
                flag15 = true;
            }
            if (projectile.velocity.X > 0f && projectile.position.X > projectile.ai[0])
            {
                flag15 = true;
            }
            if (projectile.velocity.Y < 0f && projectile.position.Y < projectile.ai[1])
            {
                flag16 = true;
            }
            if (projectile.velocity.Y > 0f && projectile.position.Y > projectile.ai[1])
            {
                flag16 = true;
            }
            if (flag15 && flag16)
            {
                projectile.Kill();
            }
            float num461 = 25f;
            if (projectile.ai[0] > 180f)
            {
                num461 -= (projectile.ai[0] - 180f) / 2f;
            }
            if (num461 <= 0f)
            {
                num461 = 0f;
                projectile.Kill();
            }
            num461 *= 0.7f;
            projectile.ai[0] += 4f;
            int num462 = 0;
            while ((float)num462 < num461)
            {
                float num463 = (float)Main.rand.Next(-10, 11);
                float num464 = (float)Main.rand.Next(-10, 11);
                float num465 = (float)Main.rand.Next(3, 9);
                float num466 = (float)Math.Sqrt((double)(num463 * num463 + num464 * num464));
                num466 = num465 / num466;
                num463 *= num466;
                num464 *= num466;
                int num467 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 58, 0f, 0f, 100, default, 1f);
                Main.dust[num467].noGravity = true;
                Main.dust[num467].position.X = projectile.Center.X;
                Main.dust[num467].position.Y = projectile.Center.Y;
                Dust expr_149DF_cp_0 = Main.dust[num467];
                expr_149DF_cp_0.position.X += (float)Main.rand.Next(-10, 11);
                Dust expr_14A09_cp_0 = Main.dust[num467];
                expr_14A09_cp_0.position.Y += (float)Main.rand.Next(-10, 11);
                Main.dust[num467].velocity.X = num463;
                Main.dust[num467].velocity.Y = num464;
                num462++;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
            target.AddBuff(BuffID.OnFire, 120);
        }
    }
}
