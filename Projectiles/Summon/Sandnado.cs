using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Summon
{
    public class Sandnado : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sandnado");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
		}

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 40;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (projectile.localAI[1] == 0f)
            {
				projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionDamageValue = Main.player[projectile.owner].minionDamage;
				projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionProjectileDamageValue = projectile.damage;
				int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 85, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 1.4f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].noLight = true;
                    Main.dust[num228].velocity = vector7;
                }
                projectile.localAI[1] += 1f;
            }
			if (Main.player[projectile.owner].minionDamage != projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionDamageValue)
			{
				int damage2 = (int)(((float)projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionProjectileDamageValue /
					projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionDamageValue) *
					Main.player[projectile.owner].minionDamage);
				projectile.damage = damage2;
			}
			bool flag64 = projectile.type == mod.ProjectileType("Sandnado");
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.GetCalamityPlayer();
            player.AddBuff(mod.BuffType("Sandnado"), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.sandnado = false;
                }
                if (modPlayer.sandnado)
                {
                    projectile.timeLeft = 2;
                }
            }
            float num8 = 0.1f;
            float num9 = ((float)projectile.width * 2f);
            for (int j = 0; j < 1000; j++)
            {
                if (j != projectile.whoAmI && Main.projectile[j].active && Main.projectile[j].owner == projectile.owner && Main.projectile[j].type == projectile.type &&
                    Math.Abs(projectile.position.X - Main.projectile[j].position.X) + Math.Abs(projectile.position.Y - Main.projectile[j].position.Y) < num9)
                {
                    if (projectile.position.X < Main.projectile[j].position.X)
                    {
                        projectile.velocity.X = projectile.velocity.X - num8;
                    }
                    else
                    {
                        projectile.velocity.X = projectile.velocity.X + num8;
                    }
                    if (projectile.position.Y < Main.projectile[j].position.Y)
                    {
                        projectile.velocity.Y = projectile.velocity.Y - num8;
                    }
                    else
                    {
                        projectile.velocity.Y = projectile.velocity.Y + num8;
                    }
                }
            }
            Vector2 vector = projectile.position;
            float num10 = 400f;
            bool flag = false;
            int num11 = -1;
            if (Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.alpha += 20;
                if (projectile.alpha > 150)
                {
                    projectile.alpha = 150;
                }
            }
            else
            {
                projectile.alpha -= 50;
                if (projectile.alpha < 60)
                {
                    projectile.alpha = 60;
                }
            }
            Vector2 center = Main.player[projectile.owner].Center;
            Vector2 value = new Vector2(0.5f);
			if (player.HasMinionAttackTargetNPC)
			{
				NPC npc = Main.npc[player.MinionAttackTargetNPC];
				if (npc.CanBeChasedBy(projectile, false))
				{
					Vector2 vector2 = npc.position + npc.Size * value;
					float num12 = Vector2.Distance(vector2, center);
					if (((Vector2.Distance(center, vector) > num12 && num12 < num10) || !flag) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
					{
						num10 = num12;
						vector = vector2;
						flag = true;
						num11 = npc.whoAmI;
					}
				}
			}
			else
			{
				for (int k = 0; k < 200; k++)
                {
                    NPC nPC = Main.npc[k];
                    if (nPC.CanBeChasedBy(projectile, false))
                    {
                        Vector2 vector3 = nPC.position + nPC.Size * value;
                        float num13 = Vector2.Distance(vector3, center);
                        if (((Vector2.Distance(center, vector) > num13 && num13 < num10) || !flag) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, nPC.position, nPC.width, nPC.height))
                        {
                            num10 = num13;
                            vector = vector3;
                            flag = true;
                            num11 = k;
                        }
                    }
                }
            }
            int num16 = 750;
            if (flag)
            {
                num16 = 2000;
            }
            if (Vector2.Distance(player.Center, projectile.Center) > (float)num16)
            {
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }
            if (flag && projectile.ai[0] == 0f)
            {
                Vector2 vector4 = vector - projectile.Center;
                float num17 = vector4.Length();
                vector4.Normalize();
                if (num17 > 400f)
                {
                    float scaleFactor = 6f;
                    vector4 *= scaleFactor;
                    projectile.velocity = (projectile.velocity * 20f + vector4) / 21f;
                }
                else
                {
                    projectile.velocity *= 0.96f;
                }
                if (num17 > 200f)
                {
                    float scaleFactor2 = 12f;
                    vector4 *= scaleFactor2;
                    projectile.velocity.X = (projectile.velocity.X * 40f + vector4.X) / 41f;
                    projectile.velocity.Y = (projectile.velocity.Y * 40f + vector4.Y) / 41f;
                }
                if (projectile.velocity.Y > -1f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - 0.1f;
                }
            }
            else
            {
                if (!Collision.CanHitLine(projectile.Center, 1, 1, Main.player[projectile.owner].Center, 1, 1))
                {
                    projectile.ai[0] = 1f;
                }
                float num21 = 12f;
                Vector2 center2 = projectile.Center;
                Vector2 vector6 = (player.Center - center2 + new Vector2(0f, -60f)) + new Vector2(0f, 40f);
                float num23 = vector6.Length();
                if (num23 > 200f && num21 < 12f)
                {
                    num21 = 12f;
                }
                if (num23 < 100f && projectile.ai[0] == 1f && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                if (num23 > 2000f)
                {
                    projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.width / 2);
                }
                if (Math.Abs(vector6.X) > 40f || Math.Abs(vector6.Y) > 10f)
                {
                    vector6.Normalize();
                    vector6 *= num21;
                    vector6 *= new Vector2(1.25f, 0.65f);
                    projectile.velocity = (projectile.velocity * 20f + vector6) / 21f;
                }
                else
                {
                    if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                    {
                        projectile.velocity.X = -0.15f;
                        projectile.velocity.Y = -0.05f;
                    }
                    projectile.velocity *= 1.01f;
                }
            }
            projectile.rotation = projectile.velocity.X * 0.05f;
            projectile.frameCounter++;
            int num25 = 2;
            if (projectile.frameCounter >= 6 * num25)
            {
                projectile.frameCounter = 0;
            }
            projectile.frame = projectile.frameCounter / num25;
            if (Main.rand.NextBool(5))
            {
                int num26 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 85, 0f, 0f, 100, default, 2f);
                Main.dust[num26].velocity *= 0.3f;
                Main.dust[num26].noGravity = true;
                Main.dust[num26].noLight = true;
            }
            if (projectile.velocity.X > 0f)
            {
                projectile.spriteDirection = (projectile.direction = -1);
            }
            else if (projectile.velocity.X < 0f)
            {
                projectile.spriteDirection = (projectile.direction = 1);
            }
            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += 1f;
                if (Main.rand.Next(3) != 0)
                {
                    projectile.ai[1] += 1f;
                }
            }
            if (projectile.ai[1] > 60f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            if (projectile.ai[0] == 0f)
            {
                float scaleFactor4 = 14f;
                int num28 = mod.ProjectileType("MiniSandShark");
                if (flag)
                {
                    if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    {
                        if (projectile.ai[1] == 0f)
                        {
                            projectile.ai[1] += 1f;
                            if (Main.myPlayer == projectile.owner)
                            {
                                Vector2 vector9 = vector - projectile.Center;
                                vector9.Normalize();
                                vector9 *= scaleFactor4;
                                int num32 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector9.X, vector9.Y, num28, projectile.damage, 0f, Main.myPlayer, 0f, 0f);
                                Main.projectile[num32].timeLeft = 300;
                                Main.projectile[num32].netUpdate = true;
                                projectile.netUpdate = true;
                            }
                        }
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

		public override bool CanDamage()
		{
			return false;
		}
	}
}
