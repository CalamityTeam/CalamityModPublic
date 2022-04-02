using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class HellwingPillar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pillar");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 90;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.25f / 255f, (255 - projectile.alpha) * 0.05f / 255f, (255 - projectile.alpha) * 0.05f / 255f);
            if (projectile.ai[0] > 7f)
            {
                float scalar = 1f;
                if (projectile.ai[0] == 8f)
                {
                    scalar = 0.25f;
                }
                else if (projectile.ai[0] == 9f)
                {
                    scalar = 0.5f;
                }
                else if (projectile.ai[0] == 10f)
                {
                    scalar = 0.75f;
                }
                projectile.ai[0] += 1f;
                int num297 = 127;
                if (Main.rand.NextBool(3))
                {
                    int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                    Dust dust = Main.dust[num299];
                    if (Main.rand.NextBool(3))
                    {
                        dust.noGravity = true;
                        dust.scale *= 2f;
                        dust.velocity.X *= 2f;
                        dust.velocity.Y *= 2f;
                    }
                    else
                    {
                        dust.scale *= 1.5f;
                    }
                    dust.velocity.X *= 1.2f;
                    dust.velocity.Y *= 1.2f;
                    dust.scale *= scalar;
                }
            }
            else
            {
                projectile.ai[0] += 1f;
            }
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) - MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 240);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
