using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class AstrealArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astreal Arrow");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.arrow = true;
            projectile.extraUpdates = 1;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;

            if (projectile.localAI[0] == 0f)
            {
                projectile.scale -= 0.02f;
                projectile.alpha += 30;
                if (projectile.alpha >= 250)
                {
                    projectile.alpha = 255;
                    projectile.localAI[0] = 1f;
                }
            }
            else if (projectile.localAI[0] == 1f)
            {
                projectile.scale += 0.02f;
                projectile.alpha -= 30;
                if (projectile.alpha <= 0)
                {
                    projectile.alpha = 0;
                    projectile.localAI[0] = 0f;
                }
            }

            if (projectile.velocity.Length() < 16f)
            {
                switch ((int)projectile.ai[0])
                {
                    case 0:
                        projectile.velocity.X *= 1.02f;
                        break;
                    case 1:
                        projectile.velocity.Y *= 1.02f;
                        break;
                    case 2:
                        projectile.velocity.X += 0.1f;
                        projectile.velocity.Y *= 1.01f;
                        break;
                    case 3:
                        projectile.velocity.Y += 0.1f;
                        projectile.velocity.X *= 1.01f;
                        break;
                    default:
                        break;
                }
            }

            if (Main.rand.NextBool(5))
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);

            projectile.ai[1] += Main.rand.Next(2) + 1;
            if (projectile.ai[1] >= 135f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<AstrealFlame>(), (int)(projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 180);
        }
    }
}
