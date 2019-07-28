using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class ChaosFlame : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chaos Flame");
		}

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.penetrate = 1;
            projectile.minionSlots = 0f;
            projectile.minion = true;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.5f) / 255f, ((255 - projectile.alpha) * 0.25f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
			if (projectile.localAI[0] == 0f)
			{
				Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
				projectile.localAI[0] += 1f;
			}
			for (int num457 = 0; num457 < 5; num457++)
			{
				int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 127, 0f, 0f, 100, default(Color), 1.2f);
				Main.dust[num458].noGravity = true;
				Main.dust[num458].velocity *= 0.5f;
				Main.dust[num458].velocity += projectile.velocity * 0.1f;
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(BuffID.OnFire, 180);
        }
    }
}
