using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class BigBeamofDeath2 : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Big Beam of Death");
		}

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 80;
            projectile.penetrate = -1;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 80;
        }

        public override void AI()
        {
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 9f)
			{
				for (int num447 = 0; num447 < 1; num447++)
				{
					Vector2 vector33 = projectile.position;
					vector33 -= projectile.velocity * ((float)num447 * 0.25f);
					projectile.alpha = 255;
					int num249 = 206;
					int num448 = Dust.NewDust(vector33, 1, 1, num249, 0f, 0f, 0, default(Color), 3f);
					Main.dust[num448].position = vector33;
					Main.dust[num448].velocity *= 0.1f;
				}
				return;
			}
        }
    }
}
