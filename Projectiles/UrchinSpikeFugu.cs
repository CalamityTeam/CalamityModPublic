using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class UrchinSpikeFugu : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Urchin Spike");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 90;
            projectile.noEnchantments = true;
        }
        
        public override void AI()
        {
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            if (projectile.ai[0] == 0f)
            {
                float num695 = 100f;
                int num696 = -1;
                int num3;
                for (int num697 = 0; num697 < 200; num697 = num3 + 1)
                {
                    NPC nPC5 = Main.npc[num697];
                    if (nPC5.CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.position, projectile.width, projectile.height, nPC5.position, nPC5.width, nPC5.height))
                    {
                        float num698 = (nPC5.Center - projectile.Center).Length();
                        if (num698 < num695)
                        {
                            num696 = num697;
                            num695 = num698;
                        }
                    }
                    num3 = num697;
                }
                projectile.ai[0] = (float)(num696 + 1);
                if (projectile.ai[0] == 0f)
                {
                    projectile.ai[0] = -15f;
                }
                if (projectile.ai[0] > 0f)
                {
                    float scaleFactor5 = (float)Main.rand.Next(35, 75) / 30f;
                    projectile.velocity = (projectile.velocity * 20f + Vector2.Normalize(Main.npc[(int)projectile.ai[0] - 1].Center - projectile.Center + new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101))) * scaleFactor5) / 21f;
                    projectile.netUpdate = true;
                }
            }
            else if (projectile.ai[0] > 0f)
            {
                Vector2 value16 = Vector2.Normalize(Main.npc[(int)projectile.ai[0] - 1].Center - projectile.Center);
                projectile.velocity = (projectile.velocity * 40f + value16 * 12f) / 41f;
            }
            else
            {
                float[] var_2_1E1A4_cp_0 = projectile.ai;
                int var_2_1E1A4_cp_1 = 0;
                float num73 = var_2_1E1A4_cp_0[var_2_1E1A4_cp_1];
                var_2_1E1A4_cp_0[var_2_1E1A4_cp_1] = num73 + 1f;
                projectile.alpha -= 25;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
                projectile.velocity.Y = projectile.velocity.Y + 0.015f;
            }
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(BuffID.Venom, 180);
        }
    }
}