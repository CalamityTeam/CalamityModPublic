using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class ProfanedEnergy : ModProjectile
    {
    	public float count = 0;
    	
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Profaned Energy");
            Main.projFrames[projectile.type] = 4;
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.sentry = true;
            projectile.timeLeft = Projectile.SentryLifeTime;
            projectile.penetrate = -1;
        }
        
        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
            if (count == 0f)
        	{
        		Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
				for (int num621 = 0; num621 < 5; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 10; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, 246, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, 246, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
				count += 1f;
        	}
        	if (projectile.owner == Main.myPlayer)
			{
				if (projectile.ai[0] != 0f)
				{
					projectile.ai[0] -= 1f;
					return;
				}
				bool flag18 = false;
				float num506 = projectile.Center.X;
				float num507 = projectile.Center.Y;
				float num508 = 1000f;
				NPC ownerMinionAttackTargetNPC = projectile.OwnerMinionAttackTargetNPC;
				if (ownerMinionAttackTargetNPC != null && ownerMinionAttackTargetNPC.CanBeChasedBy(projectile, false)) 
				{
					float num509 = ownerMinionAttackTargetNPC.position.X + (float)(ownerMinionAttackTargetNPC.width / 2);
					float num510 = ownerMinionAttackTargetNPC.position.Y + (float)(ownerMinionAttackTargetNPC.height / 2);
					float num511 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num509) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num510);
					if (num511 < num508 && Collision.CanHit(projectile.position, projectile.width, projectile.height, ownerMinionAttackTargetNPC.position, ownerMinionAttackTargetNPC.width, ownerMinionAttackTargetNPC.height)) 
					{
						num508 = num511;
						num506 = num509;
						num507 = num510;
						flag18 = true;
					}
				}
				if (!flag18) 
				{
					for (int num512 = 0; num512 < 200; num512++) 
					{
						if (Main.npc[num512].CanBeChasedBy(projectile, false)) 
						{
							float num513 = Main.npc[num512].position.X + (float)(Main.npc[num512].width / 2);
							float num514 = Main.npc[num512].position.Y + (float)(Main.npc[num512].height / 2);
							float num515 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num513) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num514);
							if (num515 < num508 && Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[num512].position, Main.npc[num512].width, Main.npc[num512].height)) 
							{
								num508 = num515;
								num506 = num513;
								num507 = num514;
								flag18 = true;
							}
						}
					}
				}
				if (flag18)
				{
					float num516 = num506;
					float num517 = num507;
					num506 -= projectile.Center.X;
					num507 -= projectile.Center.Y;
                    if (num506 < 0f)
                    {
                        projectile.spriteDirection = 1;
                    }
                    else
                    {
                        projectile.spriteDirection = -1;
                    }
                    int projectileType = Main.rand.Next(2);
					if (projectileType == 0)
					{
						projectileType = mod.ProjectileType("FlameBlast");
					}
					else
					{
						projectileType = mod.ProjectileType("FlameBurst");
					}
					float num403 = Main.rand.Next(20, 30); //modify the speed the projectile are shot.  Lower number = slower projectile.
					Vector2 vector29 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
					float num404 = num516 - vector29.X;
					float num405 = num517 - vector29.Y;
					float num406 = (float)Math.Sqrt((double)(num404 * num404 + num405 * num405));
					num406 = num403 / num406;
					num404 *= num406;
					num405 *= num406;
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num404, num405, projectileType, 170, projectile.knockBack, projectile.owner, 0f, 0f);
					projectile.ai[0] = 8f;
					return;
				}
        	}
        }
    }
}