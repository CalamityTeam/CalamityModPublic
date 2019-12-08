using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace CalamityMod.Projectiles.Rogue
{
    public class SphereSpiked : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiked Sphere");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 300;
            aiType = 52;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 1f, 0f, 0f);
			if (projectile.timeLeft == 300)
				projectile.damage *= (int)1.2f;
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 229, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 100);
            }
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(3, (int)projectile.Center.X, (int)projectile.Center.Y, 34);
			if (Main.rand.NextBool(10)) //idk if this even works
			{
				if (!crit)
					crit = true;
			}
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
