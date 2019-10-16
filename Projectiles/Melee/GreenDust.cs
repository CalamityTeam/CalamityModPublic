using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class GreenDust : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dust");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.melee = true;
            projectile.timeLeft = 3600;
        }

        public override void AI()
        {
            projectile.rotation += projectile.velocity.X * 0.02f;
            if (projectile.velocity.X < 0f)
            {
                projectile.rotation -= Math.Abs(projectile.velocity.Y) * 0.02f;
            }
            else
            {
                projectile.rotation += Math.Abs(projectile.velocity.Y) * 0.02f;
            }
            projectile.velocity *= 0.98f;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 60f)
            {
                if (projectile.timeLeft > 85)
                    projectile.timeLeft = 85;
                if (projectile.alpha < 255)
                {
                    projectile.alpha += 2;
                    if (projectile.alpha > 255)
                    {
                        projectile.alpha = 255;
                    }
                }
                else if (projectile.owner == Main.myPlayer)
                {
                    projectile.Kill();
                }
            }
            else if (projectile.alpha > 80)
            {
                projectile.alpha -= 30;
                if (projectile.alpha < 80)
                {
                    projectile.alpha = 80;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }
    }
}
