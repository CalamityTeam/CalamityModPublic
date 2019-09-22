using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class RealmRavagerBullet : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Explosive Bullet");
		}

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
			projectile.light = 0.5f;
			projectile.alpha = 255;
			projectile.ranged = true;
			projectile.friendly = true;
            projectile.ignoreWater = true;
			projectile.aiStyle = 1;
			aiType = 242;
			projectile.penetrate = 1;
			projectile.timeLeft = 90;
		}

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.Center.X, (int)projectile.Center.Y, 14);
            projectile.position = projectile.Center;
            projectile.width = (projectile.height = 32);
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num193 = 0; num193 < 2; num193++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 100, default, 1.5f);
            }
            for (int num194 = 0; num194 < 20; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 0, default, 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
			projectile.maxPenetrate = -1;
			projectile.penetrate = -1;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
			projectile.Damage();
        }
    }
}
