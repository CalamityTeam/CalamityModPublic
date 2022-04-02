using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ShadecrystalProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.scale = 1.5f;
            projectile.friendly = true;
            projectile.alpha = 50;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.magic = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.15f, 0f, 0.15f);

            projectile.rotation += projectile.velocity.X * 0.2f;

            if (Main.rand.NextBool(4))
            {
                int num300 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 70, 0f, 0f, 0, default, 1f);
                Main.dust[num300].noGravity = true;
                Main.dust[num300].velocity *= 0.2f;
                Main.dust[num300].scale *= 0.8f;
            }

            projectile.velocity *= 0.99f;

            projectile.ai[1] += 1f;
            if (projectile.ai[1] > 90f)
            {
                projectile.scale -= 0.05f;
                if (projectile.scale <= 0.2)
                {
                    projectile.scale = 0.2f;
                    projectile.Kill();
                }
                projectile.width = (int)(6f * projectile.scale);
                projectile.height = (int)(6f * projectile.scale);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X;

            if (projectile.velocity.Y != oldVelocity.Y)
                projectile.velocity.Y = -oldVelocity.Y;

            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 70, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0);
        }
    }
}
