using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class LuxorsGiftMagic : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gift");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 2;
            projectile.alpha = 255;
            projectile.timeLeft = 240;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            if (projectile.localAI[0] == 0f)
            {
                projectile.scale -= 0.01f;
                projectile.alpha += 15;
                if (projectile.alpha >= 250)
                {
                    projectile.alpha = 255;
                    projectile.localAI[0] = 1f;
                }
            }
            else if (projectile.localAI[0] == 1f)
            {
                projectile.scale += 0.01f;
                projectile.alpha -= 15;
                if (projectile.alpha <= 0)
                {
                    projectile.alpha = 0;
                    projectile.localAI[0] = 0f;
                }
            }
            projectile.localAI[1] += 1f;
            if (projectile.localAI[1] == 3f)
            {
                for (int l = 0; l < 12; l++)
                {
                    Vector2 vector3 = Vector2.UnitX * (float)-(float)projectile.width / 2f;
                    vector3 += -Vector2.UnitY.RotatedBy((double)((float)l * 3.14159274f / 6f), default) * new Vector2(8f, 16f);
                    vector3 = vector3.RotatedBy((double)(projectile.rotation - 1.57079637f), default);
                    int num9 = Dust.NewDust(projectile.Center, 0, 0, 244, 0f, 0f, 160, default, 1f);
                    Main.dust[num9].scale = 1.1f;
                    Main.dust[num9].noGravity = true;
                    Main.dust[num9].position = projectile.Center + vector3;
                    Main.dust[num9].velocity = projectile.velocity * 0.1f;
                    Main.dust[num9].velocity = Vector2.Normalize(projectile.Center - projectile.velocity * 3f - Main.dust[num9].position) * 1.25f;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.velocity *= 0.25f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }
    }
}
