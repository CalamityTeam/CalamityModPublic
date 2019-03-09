using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class Valediction : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scythe");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 70;
            projectile.height = 70;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 15;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
		}
        
        public override void AI()
        {
        	if (projectile.soundDelay == 0)
			{
				projectile.soundDelay = 8;
				Main.PlaySound(SoundID.Item7, projectile.position);
			}
        	if (projectile.ai[0] == 0f)
			{
				projectile.ai[1] += 1f;
                if (projectile.ai[1] >= 60f)
				{
					projectile.ai[0] = 1f;
					projectile.ai[1] = 0f;
					projectile.netUpdate = true;
				}
                else
                {
                    float num472 = projectile.Center.X;
                    float num473 = projectile.Center.Y;
                    float num474 = 400f;
                    bool flag17 = false;
                    for (int num475 = 0; num475 < 200; num475++)
                    {
                        if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1))
                        {
                            float num476 = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
                            float num477 = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
                            float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
                            if (num478 < num474)
                            {
                                num474 = num478;
                                num472 = num476;
                                num473 = num477;
                                flag17 = true;
                            }
                        }
                    }
                    if (flag17)
                    {
                        float num483 = 20f;
                        Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                        float num484 = num472 - vector35.X;
                        float num485 = num473 - vector35.Y;
                        float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
                        num486 = num483 / num486;
                        num484 *= num486;
                        num485 *= num486;
                        projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
                        projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
                        return;
                    }
                }
        	}
        	else
			{
				float num42 = 30f;
				float num43 = 5f;
				Vector2 vector2 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num44 = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector2.X;
				float num45 = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - vector2.Y;
				float num46 = (float)Math.Sqrt((double)(num44 * num44 + num45 * num45));
				if (num46 > 3000f)
				{
					projectile.Kill();
				}
				num46 = num42 / num46;
				num44 *= num46;
				num45 *= num46;
				if (projectile.velocity.X < num44)
				{
					projectile.velocity.X = projectile.velocity.X + num43;
					if (projectile.velocity.X < 0f && num44 > 0f)
					{
						projectile.velocity.X = projectile.velocity.X + num43;
					}
				}
				else if (projectile.velocity.X > num44)
				{
					projectile.velocity.X = projectile.velocity.X - num43;
					if (projectile.velocity.X > 0f && num44 < 0f)
					{
						projectile.velocity.X = projectile.velocity.X - num43;
					}
				}
				if (projectile.velocity.Y < num45)
				{
					projectile.velocity.Y = projectile.velocity.Y + num43;
					if (projectile.velocity.Y < 0f && num45 > 0f)
					{
						projectile.velocity.Y = projectile.velocity.Y + num43;
					}
				}
				else if (projectile.velocity.Y > num45)
				{
					projectile.velocity.Y = projectile.velocity.Y - num43;
					if (projectile.velocity.Y > 0f && num45 < 0f)
					{
						projectile.velocity.Y = projectile.velocity.Y - num43;
					}
				}
				if (Main.myPlayer == projectile.owner)
				{
					Rectangle rectangle = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
					Rectangle value2 = new Rectangle((int)Main.player[projectile.owner].position.X, (int)Main.player[projectile.owner].position.Y, Main.player[projectile.owner].width, Main.player[projectile.owner].height);
					if (rectangle.Intersects(value2))
					{
						projectile.Kill();
					}
				}
        	}
        	projectile.rotation += 0.5f;
			return;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType("CrushDepth"), 600);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 21);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 100;
            projectile.height = 100;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 5; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default(Color), 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 8; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default(Color), 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 33, 0f, 0f, 100, default(Color), 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }
    }
}