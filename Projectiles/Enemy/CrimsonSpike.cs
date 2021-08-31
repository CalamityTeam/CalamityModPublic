using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class CrimsonSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spike");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.aiStyle = 1;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (projectile.alpha == 0 && Main.rand.NextBool(3))
            {
                int num67 = Dust.NewDust(projectile.position - projectile.velocity * 3f, projectile.width, projectile.height, 260, 0f, 0f, 50, new Color(255, 136, 78, 150), 1.2f);
                Main.dust[num67].velocity *= 0.3f;
                Main.dust[num67].velocity += projectile.velocity * 0.3f;
                Main.dust[num67].noGravity = true;
            }
            projectile.alpha -= 50;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item17, (int)projectile.position.X, (int)projectile.position.Y);
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 5f)
            {
                projectile.ai[0] = 5f;
                projectile.velocity.Y = projectile.velocity.Y + 0.15f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Darkness, 90);
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 260, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
