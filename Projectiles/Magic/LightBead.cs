using Microsoft.Xna.Framework;
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
            if (Main.rand.NextBool(5))
            {
                Dust whiteMagic = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 0, default, 1f);
                whiteMagic.noGravity = true;
                whiteMagic.velocity *= 0.5f;
                whiteMagic.scale *= 0.9f;
            }

            if (projectile.ai[1] > 300f)
            {
                projectile.scale -= 0.05f;
                if (projectile.scale <= 0.2f)
                {
                    projectile.scale = 0.2f;
                    projectile.Kill();
                    return;
                }
            }
            CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 200f, 15f, 15f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 200, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 4; k++)
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);

            int beadAmt = Main.rand.Next(2, 3);
            if (projectile.owner == Main.myPlayer)
            {
                for (int b = 0; b < beadAmt; b++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<LightBeadSplit>(), (int)(projectile.damage * 0.5), 0f, projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
