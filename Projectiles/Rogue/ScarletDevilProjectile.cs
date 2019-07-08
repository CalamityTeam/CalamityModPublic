using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.GameContent.Achievements;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Rogue
{
    public class ScarletDevilProjectile : ModProjectile
    {
		bool lifesteal = false;
		
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spear the Gungnir");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 95;
			projectile.height = 95;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
			projectile.extraUpdates = 1;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
		}

        public override void AI()
        {
			CalamityPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>(mod);
			Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.55f) / 255f, ((100 - projectile.alpha) * 0.25f) / 255f, ((100 - projectile.alpha) * 0.01f) / 255f);
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
			Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 130, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 0, new Color(255, 255, 255), 0.85f);
			projectile.ai[0] += 1f;
			if(projectile.ai[0] == 1f && modPlayer.rogueStealth >= modPlayer.rogueStealthMax && modPlayer.rogueStealthMax > 0f)
				lifesteal = true;
			if ((projectile.ai[0] %= 5f) == 0f)
			{
				int numProj = 2;
				float rotation = MathHelper.ToRadians(15);
				if (projectile.owner == Main.myPlayer)
				{
					for (int i = 0; i < numProj; i++)
					{
						Vector2 perturbedSpeed = new Vector2(-projectile.velocity.X / 3, -projectile.velocity.Y / 3).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
						for(int j = 0; j < 2; j++)
						{
							Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, mod.ProjectileType("ScarletDevilBullet"), (int)((double)projectile.damage * 0.03), 0f, projectile.owner, 0f, 0f);
							perturbedSpeed *= 1.05f;
						}
					}
				}
			}
        }
		
		public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250);
        }

        public override void Kill(int timeLeft)
        {
			Main.PlaySound(SoundID.Item122, projectile.position);
        }
		
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			projectile.position = projectile.Center;
            projectile.width = (projectile.height = 150);
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("ScarletBlast"), (int)((double)projectile.damage * 0.0075), 0f, projectile.owner, 0f, 0f);
			if (target.type == NPCID.TargetDummy || !lifesteal)
			{
				return;
			}
        	Main.player[Main.myPlayer].statLife += 120;
			Main.player[Main.myPlayer].HealEffect(120);
        }
		
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                Texture2D trail = mod.GetTexture("Projectiles/Rogue/ScarletDevilProjectile");
                lightColor = new Color(100, 100, 100);
                Vector2 drawPos = projectile.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - i) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(trail, drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}