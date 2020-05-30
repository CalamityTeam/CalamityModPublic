using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class LightBead : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light Bead");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.alpha = 50;
            projectile.scale = 1.2f;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.magic = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.5f, 0.5f, 0.5f);
            projectile.rotation += projectile.velocity.X * 0.2f;
            projectile.ai[1] += 1f;
            if (Main.rand.NextBool(3))
            {
                int num300 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 0, default, 1f);
                Main.dust[num300].noGravity = true;
                Main.dust[num300].velocity *= 0.5f;
                Main.dust[num300].scale *= 0.9f;
            }
            if (projectile.ai[1] > 300f)
            {
                projectile.scale -= 0.05f;
                if ((double)projectile.scale <= 0.2)
                {
                    projectile.scale = 0.2f;
                    projectile.Kill();
                    return;
                }
            }
			CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 500f, 18f, 10f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 200, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 6; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            int num251 = Main.rand.Next(2, 3);
            if (projectile.owner == Main.myPlayer)
            {
                for (int num252 = 0; num252 < num251; num252++)
                {
                    Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (value15.X == 0f && value15.Y == 0f)
                    {
                        value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    value15.Normalize();
                    value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), value15.X, value15.Y, ModContent.ProjectileType<LightBeadSplit>(), (int)((double)projectile.damage * 0.5), 0f, projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
