using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class AetherBeam : ModProjectile
    {
		private bool split = true;

    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Beam");
		}

        public override void SetDefaults()
        {
            projectile.width = 5;
            projectile.height = 5;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 5;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 1f)
            {
				projectile.magic = false;
				projectile.ranged = true;
			}
			if (projectile.ai[1] == 1f)
			{
				split = false;
				projectile.tileCollide = false;
				projectile.ai[1] = 0f;
			}
            projectile.damage += 5;
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 25;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 1f, 0f, 0.7f);
            float num55 = 100f;
            float num56 = 2f; //3
            if (projectile.ai[1] == 0f)
            {
                projectile.localAI[0] += num56;
                if (projectile.localAI[0] > num55)
                {
                    projectile.localAI[0] = num55;
                }
            }
            else
            {
                projectile.localAI[0] -= num56;
                if (projectile.localAI[0] <= 0f)
                {
                    projectile.Kill();
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 50, 200, 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Microsoft.Xna.Framework.Color color25 = Lighting.GetColor((int)((double)projectile.position.X + (double)projectile.width * 0.5) / 16, (int)(((double)projectile.position.Y + (double)projectile.height * 0.5) / 16.0));
            int num147 = 0;
            int num148 = 0;
            float num149 = (float)(Main.projectileTexture[projectile.type].Width - projectile.width) * 0.5f + (float)projectile.width * 0.5f;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Microsoft.Xna.Framework.Rectangle value6 = new Microsoft.Xna.Framework.Rectangle((int)Main.screenPosition.X - 500, (int)Main.screenPosition.Y - 500, Main.screenWidth + 1000, Main.screenHeight + 1000);
            if (projectile.getRect().Intersects(value6))
            {
                Vector2 value7 = new Vector2(projectile.position.X - Main.screenPosition.X + num149 + (float)num148, projectile.position.Y - Main.screenPosition.Y + (float)(projectile.height / 2) + projectile.gfxOffY);
                float num162 = 100f;
                float scaleFactor = 2f;
                if (projectile.ai[1] == 1f)
                {
                    num162 = (float)((int)projectile.localAI[0]);
                }
                for (int num163 = 1; num163 <= (int)projectile.localAI[0]; num163++)
                {
                    Vector2 value8 = Vector2.Normalize(projectile.velocity) * (float)num163 * scaleFactor;
                    Microsoft.Xna.Framework.Color color29 = projectile.GetAlpha(color25);
                    color29 *= (num162 - (float)num163) / num162;
                    color29.A = 0;
                    Main.spriteBatch.Draw(Main.projectileTexture[projectile.type], value7 - value8, null, color29, projectile.rotation, new Vector2(num149, (float)(projectile.height / 2 + num147)), projectile.scale, spriteEffects, 0f);
                }
            }
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			if (split)
			{
				float random = Main.rand.Next(30, 90);
				float spread = random * 0.0174f;
				double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
				double deltaAngle = spread / 8f;
				double offsetAngle;
				int i;
				if (projectile.owner == Main.myPlayer)
				{
					for (i = 0; i < 4; i++)
					{
						offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
						int proj1 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), mod.ProjectileType("AetherBeam"), projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0], 1f);
						int proj2 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), mod.ProjectileType("AetherBeam"), projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0], 1f);
					}
				}
			}
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(BuffID.ShadowFlame, 60);
			target.AddBuff(BuffID.Ichor, 120);
			target.AddBuff(BuffID.Frostburn, 180);
			target.AddBuff(BuffID.OnFire, 240);
			target.AddBuff(BuffID.Poisoned, 300);
		}
    }
}
