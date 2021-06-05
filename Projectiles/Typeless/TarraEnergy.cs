using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class TarraEnergy : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Round");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 120;
            projectile.extraUpdates = 1;
            aiType = ProjectileID.Bullet;
        }

		// Reduce damage of projectiles if more than the cap are active
		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			int projectileCount = Main.player[projectile.owner].ownedProjectileCounts[projectile.type];
			int cap = 5;
			int oldDamage = damage;
			if (projectileCount > cap)
			{
				damage -= (int)(oldDamage * ((projectileCount - cap) * 0.05));
				if (damage < 1)
					damage = 1;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }

        public override bool PreAI()
        {
            for (int num136 = 0; num136 < 2; num136++)
            {
                float x2 = projectile.position.X - projectile.velocity.X / 10f * (float)num136;
                float y2 = projectile.position.Y - projectile.velocity.Y / 10f * (float)num136;
                Vector2 dspeed = projectile.velocity * Main.rand.NextFloat(0.7f, 0.4f);
                int num137 = Dust.NewDust(new Vector2(x2, y2), 1, 1, 107, 0f, 0f, 0, default, 1f);
                Main.dust[num137].alpha = projectile.alpha;
                Main.dust[num137].position.X = x2;
                Main.dust[num137].position.Y = y2;
                Main.dust[num137].velocity = dspeed;
                Main.dust[num137].noGravity = true;
                Main.dust[num137].noLight = true;
            }
            float num138 = (float)Math.Sqrt((double)(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y));
            float num139 = projectile.localAI[0];
            if (num139 == 0f)
            {
                projectile.localAI[0] = num138;
            }
            return false;
        }
    }
}
