using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class ReaverOrb : ModProjectile
    {
    	public int dust = 3;
    	
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Reaver Orb");
			Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 18000;
            projectile.alpha = 50;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.sentry = true;
        }

        public override void AI()
        {
        	bool flag64 = projectile.type == mod.ProjectileType("ReaverOrb");
			Player player = Main.player[projectile.owner];
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (!modPlayer.reaverOrb)
        	{
        		projectile.active = false;
        		return;
        	}
			if (flag64)
			{
				if (player.dead)
				{
					modPlayer.rOrb = false;
				}
				if (modPlayer.rOrb)
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
					int num503 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 157, 0f, 0f, 0, default(Color), 1f);
					Main.dust[num503].velocity *= 2f;
					Main.dust[num503].scale *= 1.15f;
				}
        	}
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 1f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
			projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
			projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.height / 2) + Main.player[projectile.owner].gfxOffY - 60f;
			if (Main.player[projectile.owner].gravDir == -1f)
			{
				projectile.position.Y = projectile.position.Y + 120f;
				projectile.rotation = 3.14f;
			}
			else
			{
				projectile.rotation = 0f;
			}
			projectile.position.X = (float)((int)projectile.position.X);
			projectile.position.Y = (float)((int)projectile.position.Y);
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
				float num508 = 600f;
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
					int num251 = Main.rand.Next(4, 9);
					for (int num252 = 0; num252 < num251; num252++)
					{
						Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
						while (value15.X == 0f && value15.Y == 0f)
						{
							value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
						}
						value15.Normalize();
						value15 *= (float)Main.rand.Next(90, 121) * 0.1f;
						int spore = Projectile.NewProjectile(projectile.Center.X - 4f, projectile.Center.Y, value15.X, value15.Y, 569 + Main.rand.Next(3), (int)(80f * player.minionDamage), 1.5f, projectile.owner, 0f, 0f);
						Main.projectile[spore].minion = true;
						Main.projectile[spore].minionSlots = 0f;
					}
					Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 77);
					projectile.ai[0] = 50f;
					return;
				}
			}
        }
    }
}