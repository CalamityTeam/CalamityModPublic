using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class ThePlaguebringer : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pandemic");
		}
    	
        public override void SetDefaults()
        {
        	projectile.CloneDefaults(ProjectileID.TheEyeOfCthulhu);
            projectile.width = 16;
            projectile.scale = 1f;
            projectile.height = 16;
            projectile.penetrate = -1;
            projectile.melee = true;
            aiType = 555;
        }
        
        public override void AI()
        {
            int[] array = new int[20];
			int num428 = 0;
			float num429 = 300f;
			bool flag14 = false;
			for (int num430 = 0; num430 < 200; num430++)
			{
				if (Main.npc[num430].CanBeChasedBy(projectile, false))
				{
					float num431 = Main.npc[num430].position.X + (float)(Main.npc[num430].width / 2);
					float num432 = Main.npc[num430].position.Y + (float)(Main.npc[num430].height / 2);
					float num433 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num431) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num432);
					if (num433 < num429 && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num430].Center, 1, 1))
					{
						if (num428 < 20)
						{
							array[num428] = num430;
							num428++;
						}
						flag14 = true;
					}
				}
			}
			if (flag14)
			{
				int num434 = Main.rand.Next(num428);
				num434 = array[num434];
				float num435 = Main.npc[num434].position.X + (float)(Main.npc[num434].width / 2);
				float num436 = Main.npc[num434].position.Y + (float)(Main.npc[num434].height / 2);
				projectile.localAI[0] += 1f;
				if (projectile.localAI[0] > 32f)
				{
					projectile.localAI[0] = 0f;
					float num437 = 6f;
					Vector2 value10 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
					value10 += projectile.velocity * 4f;
					float num438 = num435 - value10.X;
					float num439 = num436 - value10.Y;
					float num440 = (float)Math.Sqrt((double)(num438 * num438 + num439 * num439));
					num440 = num437 / num440;
					num438 *= num440;
					num439 *= num440;
					if (projectile.owner == Main.myPlayer)
					{
						Projectile.NewProjectile(value10.X, value10.Y, num438, num439, mod.ProjectileType("PlagueSeeker"), (int)((double)projectile.damage * 0.5f), projectile.knockBack, projectile.owner, 0f, 0f);
					}
					return;
				}
			}
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("Plague"), 600);
        }
    }
}