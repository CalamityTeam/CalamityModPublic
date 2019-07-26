using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class RedDevil : ModProjectile
    {
    	public int dust = 3;

    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Red Devil");
			Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
		}

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
        }

        public override void AI()
        {
        	bool flag64 = projectile.type == mod.ProjectileType("RedDevil");
        	Player player = Main.player[projectile.owner];
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
        	if (!modPlayer.redDevil)
        	{
        		projectile.active = false;
        		return;
        	}
			if (flag64)
			{
				if (player.dead)
				{
					modPlayer.rDevil = false;
				}
				if (modPlayer.rDevil)
				{
					projectile.timeLeft = 2;
				}
			}
        	dust--;
        	if (dust >= 0)
        	{
        		int num501 = 50;
				for (int num502 = 0; num502 < num501; num502++)
				{
					int num503 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 235, 0f, 0f, 0, default(Color), 1f);
					Main.dust[num503].velocity *= 2f;
					Main.dust[num503].scale *= 1.15f;
				}
        	}
        	projectile.frameCounter++;
			if (projectile.frameCounter > 8)
			{
			    projectile.frame++;
			    projectile.frameCounter = 0;
			}
			if (projectile.frame > 4)
			{
			   projectile.frame = 0;
			}
			float num = (float)Main.rand.Next(90, 111) * 0.01f;
        	num *= Main.essScale;
			Lighting.AddLight(projectile.Center, 1f * num, 0f * num, 0.15f * num);
			projectile.rotation = projectile.velocity.X * 0.04f;
			if ((double)Math.Abs(projectile.velocity.X) > 0.2)
			{
				projectile.spriteDirection = -projectile.direction;
        	}
			float num633 = 700f;
			float num634 = 2000f;
			float num635 = 3000f;
			float num636 = 150f;
			float num637 = 0.05f;
			for (int num638 = 0; num638 < 1000; num638++)
			{
				bool flag23 = (Main.projectile[num638].type == mod.ProjectileType("RedDevil"));
				if (num638 != projectile.whoAmI && Main.projectile[num638].active && Main.projectile[num638].owner == projectile.owner && flag23 && Math.Abs(projectile.position.X - Main.projectile[num638].position.X) + Math.Abs(projectile.position.Y - Main.projectile[num638].position.Y) < (float)projectile.width)
				{
					if (projectile.position.X < Main.projectile[num638].position.X)
					{
						projectile.velocity.X = projectile.velocity.X - num637;
					}
					else
					{
						projectile.velocity.X = projectile.velocity.X + num637;
					}
					if (projectile.position.Y < Main.projectile[num638].position.Y)
					{
						projectile.velocity.Y = projectile.velocity.Y - num637;
					}
					else
					{
						projectile.velocity.Y = projectile.velocity.Y + num637;
					}
				}
			}
			bool flag24 = false;
			if (projectile.ai[0] == 2f)
			{
				projectile.ai[1] += 1f;
				projectile.extraUpdates = 1;
				if (projectile.ai[1] > 40f)
				{
					projectile.ai[1] = 1f;
					projectile.ai[0] = 0f;
					projectile.extraUpdates = 0;
					projectile.numUpdates = 0;
					projectile.netUpdate = true;
				}
				else
				{
					flag24 = true;
				}
			}
			if (flag24)
			{
				return;
			}
			Vector2 vector46 = projectile.position;
			bool flag25 = false;
			if (projectile.ai[0] != 1f)
			{
				projectile.tileCollide = false;
			}
			if (projectile.tileCollide && WorldGen.SolidTile(Framing.GetTileSafely((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16)))
			{
				projectile.tileCollide = false;
			}
			if (player.HasMinionAttackTargetNPC)
			{
				NPC npc = Main.npc[player.MinionAttackTargetNPC];
				if (npc.CanBeChasedBy(projectile, false))
				{
					float num646 = Vector2.Distance(npc.Center, projectile.Center);
					if ((Vector2.Distance(projectile.Center, vector46) > num646 && num646 < num633) || !flag25)
					{
						num633 = num646;
						vector46 = npc.Center;
						flag25 = true;
					}
				}
			}
			else
			{
				for (int num645 = 0; num645 < 200; num645++)
				{
					NPC nPC2 = Main.npc[num645];
					if (nPC2.CanBeChasedBy(projectile, false))
					{
						float num646 = Vector2.Distance(nPC2.Center, projectile.Center);
						if (((Vector2.Distance(projectile.Center, vector46) > num646 && num646 < num633) || !flag25) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, nPC2.position, nPC2.width, nPC2.height))
						{
							num633 = num646;
							vector46 = nPC2.Center;
							flag25 = true;
						}
					}
				}
			}
			float num647 = num634;
			if (flag25)
			{
				num647 = num635;
			}
			if (Vector2.Distance(player.Center, projectile.Center) > num647)
			{
				projectile.ai[0] = 1f;
				projectile.tileCollide = false;
				projectile.netUpdate = true;
			}
			if (flag25 && projectile.ai[0] == 0f)
			{
				Vector2 vector47 = vector46 - projectile.Center;
				float num648 = vector47.Length();
				vector47.Normalize();
				if (num648 > 200f)
				{
					float scaleFactor2 = 16f; //8
					vector47 *= scaleFactor2;
					projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
				}
				else
				{
					float num649 = 4f;
					vector47 *= -num649;
					projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
				}
			}
			else
			{
				bool flag26 = false;
				if (!flag26)
				{
					flag26 = (projectile.ai[0] == 1f);
				}
				float num650 = 5f; //6
				if (flag26)
				{
					num650 = 12f; //15
				}
				Vector2 center2 = projectile.Center;
				Vector2 vector48 = player.Center - center2 + new Vector2(0f, -30f); //-60
				float num651 = vector48.Length();
				if (num651 > 200f && num650 < 6.5f) //200 and 8
				{
					num650 = 6.5f; //8
				}
				if (num651 < num636 && flag26 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
				{
					projectile.ai[0] = 0f;
					projectile.netUpdate = true;
				}
				if (num651 > 2000f)
				{
					projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
					projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.height / 2);
					projectile.netUpdate = true;
				}
				if (num651 > 70f)
				{
					vector48.Normalize();
					vector48 *= num650;
					projectile.velocity = (projectile.velocity * 40f + vector48) / 41f;
				}
				else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
				{
					projectile.velocity.X = -0.2f;
					projectile.velocity.Y = -0.1f;
				}
			}
			if (projectile.ai[1] > 0f)
			{
				projectile.ai[1] += (float)Main.rand.Next(1, 4);
			}
			if (projectile.ai[1] > 80f)
			{
				projectile.ai[1] = 0f;
				projectile.netUpdate = true;
			}
			if (projectile.ai[0] == 0f)
			{
				float scaleFactor3 = 24f;
				int num658 = 114;
				if (flag25 && projectile.ai[1] == 0f)
				{
					projectile.ai[1] += 1f;
					if (Main.myPlayer == projectile.owner && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, vector46, 0, 0))
					{
						Vector2 value19 = vector46 - projectile.Center;
						value19.Normalize();
						value19 *= scaleFactor3;
						int num659 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value19.X, value19.Y, num658, projectile.damage, 0f, Main.myPlayer, 0f, 0f);
						Main.projectile[num659].timeLeft = 300;
						projectile.netUpdate = true;
					}
				}
			}
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = (projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			Texture2D texture2D13 = Main.projectileTexture[projectile.type];
			int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
			int y6 = num214 * projectile.frame;
			Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, spriteEffects, 0f);
			return false;
		}

		public override bool CanDamage()
		{
			return false;
		}
	}
}
