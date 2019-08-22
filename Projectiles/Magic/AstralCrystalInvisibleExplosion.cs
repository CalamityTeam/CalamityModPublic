using Terraria;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Magic
{
    public class AstralCrystalInvisibleExplosion : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.magic = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(mod.BuffType("AstralInfectionDebuff"), 300);
		}

		public override void AI()
        {
            //KILL VELOCITY
            projectile.ai[0]++;
            if (projectile.ai[0] > 10)
            {
                projectile.Kill();
            }
        }
    }
}
