using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PlagueBeeSmall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Bee");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 420;
            projectile.ignoreWater = true;
            projectile.minion = true;
        }

        public override void AI()
        {
            if (projectile.velocity.X > 0f)
            {
                projectile.spriteDirection = projectile.direction = 1;
            }
            else if (projectile.velocity.X < 0f)
            {
                projectile.spriteDirection = projectile.direction = -1;
            }
            projectile.rotation = projectile.velocity.X * 0.05f;
            projectile.frameCounter++;
            if (projectile.frameCounter >= 3)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 3)
            {
                projectile.frame = 0;
            }
            float num373 = projectile.position.X;
            float num374 = projectile.position.Y;
            float num375 = 1000f;
            bool flag10 = false;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 30f)
            {
                projectile.ai[0] = 30f;
				Player player = Main.player[projectile.owner];
				if (player.HasMinionAttackTargetNPC)
				{
					NPC npc = Main.npc[player.MinionAttackTargetNPC];
					if (npc.CanBeChasedBy(projectile, false) && (!npc.wet || projectile.type == 307))
					{
						float num377 = npc.position.X + (float)(npc.width / 2);
						float num378 = npc.position.Y + (float)(npc.height / 2);
						float num379 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num377) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num378);
						if (num379 < 800f && num379 < num375 && Collision.CanHit(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
						{
							num375 = num379;
							num373 = num377;
							num374 = num378;
							flag10 = true;
						}
					}
				}
				else
				{
					for (int num376 = 0; num376 < Main.maxNPCs; num376++)
					{
						if (Main.npc[num376].CanBeChasedBy(projectile, false) && (!Main.npc[num376].wet || projectile.type == 307))
						{
							float num377 = Main.npc[num376].position.X + (float)(Main.npc[num376].width / 2);
							float num378 = Main.npc[num376].position.Y + (float)(Main.npc[num376].height / 2);
							float num379 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num377) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num378);
							if (num379 < 800f && num379 < num375 && Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[num376].position, Main.npc[num376].width, Main.npc[num376].height))
							{
								num375 = num379;
								num373 = num377;
								num374 = num378;
								flag10 = true;
							}
						}
					}
				}
            }
            if (!flag10)
            {
                num373 = projectile.position.X + (float)(projectile.width / 2) + projectile.velocity.X * 100f;
                num374 = projectile.position.Y + (float)(projectile.height / 2) + projectile.velocity.Y * 100f;
            }
            float num380 = 10f;
            float num381 = 0.14f;
            Vector2 vector30 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
            float num382 = num373 - vector30.X;
            float num383 = num374 - vector30.Y;
            float num384 = (float)Math.Sqrt((double)(num382 * num382 + num383 * num383));
            num384 = num380 / num384;
            num382 *= num384;
            num383 *= num384;
            if (projectile.velocity.X < num382)
            {
                projectile.velocity.X = projectile.velocity.X + num381;
                if (projectile.velocity.X < 0f && num382 > 0f)
                {
                    projectile.velocity.X = projectile.velocity.X + num381 * 2f;
                }
            }
            else if (projectile.velocity.X > num382)
            {
                projectile.velocity.X = projectile.velocity.X - num381;
                if (projectile.velocity.X > 0f && num382 < 0f)
                {
                    projectile.velocity.X = projectile.velocity.X - num381 * 2f;
                }
            }
            if (projectile.velocity.Y < num383)
            {
                projectile.velocity.Y = projectile.velocity.Y + num381;
                if (projectile.velocity.Y < 0f && num383 > 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y + num381 * 2f;
                }
            }
            else if (projectile.velocity.Y > num383)
            {
                projectile.velocity.Y = projectile.velocity.Y - num381;
                if (projectile.velocity.Y > 0f && num383 < 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - num381 * 2f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            int num3;
            for (int num418 = 0; num418 < 3; num418 = num3 + 1)
            {
                int num419 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 89, projectile.velocity.X, projectile.velocity.Y, 50, default, 1f);
                Main.dust[num419].noGravity = true;
                Main.dust[num419].scale = 1f;
                num3 = num418;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 60);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 60);
        }
    }
}
