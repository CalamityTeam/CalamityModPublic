using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class AMR : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("AMR");
		}

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.light = 0.5f;
            projectile.alpha = 255;
			projectile.extraUpdates = 10;
			projectile.scale = 1.18f;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            aiType = 242;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
            {
                for (int x = 0; x < 8; x++)
                {
                    float xPos = projectile.ai[0] > 0 ? projectile.position.X + 500 : projectile.position.X - 500;
                    Vector2 vector2 = new Vector2(xPos, projectile.position.Y + Main.rand.Next(-500, 501));
                    float num80 = xPos;
                    float speedX = (float)target.position.X - vector2.X;
                    float speedY = (float)target.position.Y - vector2.Y;
                    float dir = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
                    dir = 10 / num80;
                    speedX *= dir * 150;
                    speedY *= dir * 150;
                    if (projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(vector2.X, vector2.Y, speedX, speedY, mod.ProjectileType("AMR2"), (int)((double)projectile.damage * 0.1), 1f, projectile.owner);
                    }
                }
            }
            target.AddBuff(mod.BuffType("MarkedforDeath"), 600);
            if (target.defense > 50)
            {
                target.defense -= 50;
            }
        }
    }
}
