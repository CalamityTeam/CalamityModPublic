using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SirensSong : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Song");
		}

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
        	projectile.velocity.X *= 0.985f;
        	projectile.velocity.Y *= 0.985f;
        	if (projectile.localAI[0] == 0f)
			{
				projectile.scale += 0.02f;
				if (projectile.scale >= 1.25f)
				{
					projectile.localAI[0] = 1f;
				}
			}
			else if (projectile.localAI[0] == 1f)
			{
				projectile.scale -= 0.02f;
				if (projectile.scale <= 0.75f)
				{
					projectile.localAI[0] = 0f;
				}
			}
			if (projectile.ai[1] == 0f)
			{
				projectile.ai[1] = 1f;
				Main.harpNote = projectile.ai[0];
				Main.PlaySound(SoundID.Item26, projectile.position);
			}
			Lighting.AddLight(projectile.Center, 0f, 1.2f, 0f);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 0);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 7;
            target.AddBuff(BuffID.Confused, 300);
        }
    }
}
