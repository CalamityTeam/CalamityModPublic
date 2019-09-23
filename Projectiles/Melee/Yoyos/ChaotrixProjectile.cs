using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class ChaotrixProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chaotrix");
		}

        public override void SetDefaults()
        {
        	projectile.CloneDefaults(ProjectileID.Yelets);
            projectile.width = 16;
            projectile.scale = 1.15f;
            projectile.height = 16;
            projectile.penetrate = 10;
            projectile.melee = true;
            aiType = 549;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 127, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(BuffID.OnFire, 300);
			if (projectile.owner == Main.myPlayer)
			{
				int boom = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("FuckYou"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
				Main.projectile[boom].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceMelee = true;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D tex = Main.projectileTexture[projectile.type];
			spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
	}
}
